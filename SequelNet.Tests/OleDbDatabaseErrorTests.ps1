<#
Build from the repository root:
  dotnet build SequelNet.Connector.OleDb/SequelNet.Connector.OleDb.csproj -p:SignAssembly=false -p:GeneratePackageOnBuild=false
Run with Windows PowerShell (.NET Framework), using x86 for Jet 4.0:
  C:/Windows/SysWOW64/WindowsPowerShell/v1.0/powershell.exe -NoProfile -File SequelNet.Tests/OleDbDatabaseErrorTests.ps1 -RunJetIntegration

Synthetic tests require no installed database provider. The optional integration
tests create and remove a temporary database under the build output directory.
#>
param([switch] $RunJetIntegration)

$ErrorActionPreference = 'Stop'
$buildDirectory = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../_Debug/net472'))
[Reflection.Assembly]::LoadFrom((Join-Path $buildDirectory 'SequelNet.dll')) | Out-Null
[Reflection.Assembly]::LoadFrom((Join-Path $buildDirectory 'SequelNet.Connector.OleDb.dll')) | Out-Null
$script:assertionCount = 0

function Assert-Category($connector, $exception, [string] $expected) {
    $actual = $connector.GetDatabaseError($exception).ToString()
    if ($actual -ne $expected) {
        throw "Expected $expected, got $actual"
    }
    $script:assertionCount++
}

function New-ErrorRecord([string] $sqlState, [int] $nativeError, [string] $source) {
    $record = [Runtime.Serialization.FormatterServices]::GetUninitializedObject([System.Data.OleDb.OleDbError])
    $values = @{
        message = 'An arbitrary localized error message'
        source = $source
        sqlState = $sqlState
        nativeError = $nativeError
    }
    foreach ($name in $values.Keys) {
        $record.GetType().GetField($name, [Reflection.BindingFlags]'Instance,NonPublic').SetValue($record, $values[$name])
    }
    return $record
}

function New-OleDbException([object[]] $records) {
    $errors = [Runtime.Serialization.FormatterServices]::GetUninitializedObject([System.Data.OleDb.OleDbErrorCollection])
    $items = New-Object System.Collections.ArrayList
    foreach ($record in $records) {
        $items.Add($record) | Out-Null
    }
    $errors.GetType().GetField('items', [Reflection.BindingFlags]'Instance,NonPublic').SetValue($errors, $items)
    $exception = [Runtime.Serialization.FormatterServices]::GetUninitializedObject([System.Data.OleDb.OleDbException])
    $exception.GetType().GetField('oledbErrors', [Reflection.BindingFlags]'Instance,NonPublic').SetValue($exception, $errors)
    return $exception
}

$cases = @(
    @('3022', -105121349, 'UniqueViolation'),
    @('3200', -534971980, 'ForeignKeyViolation'),
    @('3201', -535037517, 'ForeignKeyViolation'),
    @('3191', 0, 'DuplicateColumn'),
    @('3380', -539297252, 'DuplicateColumn'),
    @('3010', -534840599, 'DuplicateTable'),
    @('3284', 0, 'DuplicateIndex'),
    @('3375', -538969467, 'DuplicateIndex'),
    @('3012', 0, 'DuplicateObject'),
    @('3283', 0, 'DuplicateObject'),
    @('3372', -538772860, 'UndefinedObject'),
    @('3381', -539362787, 'UndefinedColumn'),
    @('3376', -539034905, 'UndefinedTable'),
    @('3061', 0, 'Unknown'),
    @('', 0, 'Unknown'),
    @('23000', 3022, 'Unknown')
)
$connector = New-Object SequelNet.Connector.OleDbConnector('')
try {
    foreach ($source in @('Microsoft JET Database Engine', 'Microsoft Access Database Engine')) {
        foreach ($case in $cases) {
            $record = New-ErrorRecord $case[0] $case[1] $source
            Assert-Category $connector (New-OleDbException @($record)) $case[2]
        }
    }
    Assert-Category $connector $null 'Unknown'
    Assert-Category $connector (New-Object System.Exception('3022')) 'Unknown'
    Assert-Category $connector (New-OleDbException @()) 'Unknown'
    $unrelated = New-ErrorRecord '3022' 3022 'Another OLE DB Provider'
    Assert-Category $connector (New-OleDbException @($unrelated)) 'Unknown'
    $generic = New-ErrorRecord '' 0 'Microsoft JET Database Engine'
    $specific = New-ErrorRecord '3010' -534840599 'Microsoft JET Database Engine'
    Assert-Category $connector (New-OleDbException @($generic, $specific)) 'DuplicateTable'
    if ($connector.Connection.State -ne [System.Data.ConnectionState]::Closed) {
        throw 'Classification opened the connection'
    }
}
finally {
    $connector.Dispose()
}

if ($RunJetIntegration) {
    $databasePath = Join-Path $buildDirectory ('jet-errors-' + [Guid]::NewGuid().ToString('N') + '.mdb')
    $connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=$databasePath"
    $catalog = $null
    $connector = $null
    try {
        $catalog = New-Object -ComObject ADOX.Catalog
        $catalog.Create($connectionString) | Out-Null
        $catalog.ActiveConnection.Close()
        [Runtime.InteropServices.Marshal]::FinalReleaseComObject($catalog) | Out-Null
        $catalog = $null
        $connector = New-Object SequelNet.Connector.OleDbConnector($connectionString)
        $connector.Connection.Open()

        $setup = @(
            'CREATE TABLE Parent (Id LONG CONSTRAINT PK_Parent PRIMARY KEY, Name TEXT(50))',
            'CREATE TABLE Child (Id LONG PRIMARY KEY, ParentId LONG, CONSTRAINT FK_Child FOREIGN KEY (ParentId) REFERENCES Parent (Id))',
            'INSERT INTO Parent (Id, Name) VALUES (1, ''One'')',
            'INSERT INTO Child (Id, ParentId) VALUES (1, 1)',
            'CREATE INDEX ExistingIndex ON Parent (Name)'
        )
        foreach ($sql in $setup) {
            $command = $connector.Connection.CreateCommand()
            try {
                $command.CommandText = $sql
                $command.ExecuteNonQuery() | Out-Null
            }
            finally {
                $command.Dispose()
            }
        }

        $queries = @(
            @('INSERT INTO Parent (Id) VALUES (1)', 'UniqueViolation'),
            @('INSERT INTO Child (Id, ParentId) VALUES (2, 999)', 'ForeignKeyViolation'),
            @('DELETE FROM Parent WHERE Id = 1', 'ForeignKeyViolation'),
            @('ALTER TABLE Parent ADD COLUMN Name TEXT(50)', 'DuplicateColumn'),
            @('CREATE TABLE Parent (Id LONG)', 'DuplicateTable'),
            @('CREATE INDEX ExistingIndex ON Parent (Name)', 'DuplicateIndex'),
            @('DROP INDEX MissingIndex ON Parent', 'UndefinedObject'),
            @('ALTER TABLE Parent DROP COLUMN MissingColumn', 'UndefinedColumn'),
            @('DROP TABLE MissingTable', 'UndefinedTable'),
            @('SELECT MissingColumn FROM Parent', 'Unknown')
        )
        foreach ($query in $queries) {
            $command = $connector.Connection.CreateCommand()
            $failure = $null
            try {
                $command.CommandText = $query[0]
                $command.ExecuteNonQuery() | Out-Null
            }
            catch {
                $failure = $_.Exception
                while ($failure.InnerException) {
                    $failure = $failure.InnerException
                }
            }
            finally {
                $command.Dispose()
            }
            if ($failure -isnot [System.Data.OleDb.OleDbException]) {
                throw "Expected an OleDbException for $($query[0])"
            }
            Assert-Category $connector $failure $query[1]
        }
    }
    finally {
        if ($connector) {
            $connector.Dispose()
        }
        if ($catalog) {
            [Runtime.InteropServices.Marshal]::FinalReleaseComObject($catalog) | Out-Null
        }
        if (Test-Path -LiteralPath $databasePath) {
            Remove-Item -LiteralPath $databasePath
        }
    }
}

Write-Output "Passed $script:assertionCount OleDb classification checks."
