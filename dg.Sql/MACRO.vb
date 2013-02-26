Imports System
Imports EnvDTE
Imports EnvDTE80
Imports EnvDTE90
Imports EnvDTE90a
Imports EnvDTE100
Imports System.Diagnostics
Imports System.Text

' Define a DAL class in comments like this:
' /* Class name
' *  Table name
' *  Column name:   List of keywords ended with ; A comment
' *  @FOREIGNKEY:   NAME(fk_name); FOREIGNTABLE(other DAL's class name); COLUMNS[Column name, Column name]; FOREIGNCOLUMNS[Column name, Column name]; ONUPDATE(CASCADE/RESTRICT/SETNULL/NOACTION); ONDELETE(CASCADE/RESTRICT/SETNULL/NOACTION);
' *  @INDEX:        NAME(ix_name); [Column name, column name]; UNIQUE/PRIMARYKEY/SPATIAL/FULLTEXT/BTREE/RTREE/HASH/NONCLUSTERED/CLUSTERED
' *  @BEFOREINSERT: Code to execute in beginning of Insert(...) code
' *  @BEFOREUPDATE: Code to execute in beginning of Update(...) code
' *  @AFTERREAD:    Code to execute at the end of Read(...) code
' *  @MYSQLENGINE:  InnoDB/MyISAM/ARCHIVE (Use MyISAM for geographical, FULLTEXT, and general RTREE indexes)
' */

' A column keyword can be:
' PRIMARY KEY; AUTOINCREMENT; DEFAULT value; NULLABLE;
' INT64; INT32; INT16; INT8; UINT64; UINT32; UINT16; UINT8; DOUBLE; FLOAT; FIXEDSTRING(length); STRING(length); TEXT; LONGTEXT(length); MEDIUMTEXT(length); BOOL; GUID; DATETIME; 
' GEOMETRY; GEOMETRYCOLLECTION; POINT; LINESTRING; POLYGON; LINE; CURVE; SURFACE; LINEARRING; MULTIPOINT; MULTILINESTRING; MULTIPOLYGON; MULTICURVE; MULTISURFACE;
' DECIMAL; PRECISION(length); SCALE(length);
' NOPROPERTY - will remove the property code for this specific column. Can be used to write a custom property
' ACTUALDEFAULT - define a different code-based default value for the class's instance member variable
' TODB - Define a format to be used in Insert(...) and Update(...). Can use the {0} specifier for column value in format
' FROMDB - Define a format to be used in Read(...). Can use the {0} specifier for column value in format
' VIRTUAL - Defines the property as virtual
' UNIQUE INDEX - Defines a UNIQUE index on this column
' FOREIGN TableClassName.ColumnName - Defines a FOREIGN key for this column

' You can define an enum for the column, if specified like this:
' /* Column name:            DEFAULT EnumName.None; Comment for column:
' *                          "EnumName"
' *                          - None = 0
' *                          - Option1 = 1
' *                          - Option2 = 2
' */

Public Module dg

    Sub UpgrateDCGEngineToDgSql()
        Dim oldPatternSyntax = DTE.Find.PatternSyntax
        Dim oldTarget = DTE.Find.Target
        Dim oldMatchCase = DTE.Find.MatchCase
        Dim oldMatchWholeWord = DTE.Find.MatchWholeWord
        Dim oldMatchInHiddenText = DTE.Find.MatchInHiddenText
        Dim oldFindWhat = DTE.Find.FindWhat
        Dim oldReplaceWith = DTE.Find.ReplaceWith
        Dim oldResultsLocation = DTE.Find.ResultsLocation
        Dim oldAction = DTE.Find.Action
        Dim oldSearchPath = DTE.Find.SearchPath
        Dim oldFilesOfType = DTE.Find.FilesOfType

        DTE.ExecuteCommand("Edit.SwitchtoReplaceInFiles")
        DTE.Find.FilesOfType = "*.c;*.cpp;*.cxx;*.cc;*.tli;*.tlh;*.h;*.hpp;*.hxx;*.hh;*.inl;*.rc;*.resx;*.idl;*.asm;*.inc;*.cs;*.aspx;*.html;*.js;*.vcxproj;*.sln;*.m;*.txt;*.rc;*.config;*.master;*.asax"
        DTE.Find.KeepModifiedDocumentsOpen = True
        DTE.Find.Target = vsFindTarget.vsFindTargetSolution
        DTE.Find.SearchPath = "Entire Solution"
        DTE.Find.PatternSyntax = vsFindPatternSyntax.vsFindPatternSyntaxRegExpr
        DTE.Find.MatchCase = True
        DTE.Find.MatchWholeWord = True
        DTE.Find.MatchInHiddenText = True
        DTE.Find.FindWhat = "using DCGEngine\.Sql\;"
        DTE.Find.ReplaceWith = "using dg.Sql;\nusing dg.Sql.Connector;"
        DTE.Find.ResultsLocation = vsFindResultsLocation.vsFindResultsNone
        DTE.Find.Action = vsFindAction.vsFindActionReplaceAll
        Dim result = DTE.Find.Execute()

        DTE.Find.PatternSyntax = vsFindPatternSyntax.vsFindPatternSyntaxLiteral
        DTE.Find.FindWhat = "SqlMgrBase"
        DTE.Find.ReplaceWith = "ConnectorBase"
        result = DTE.Find.Execute()

        DTE.Find.FindWhat = "SqlMgrDataReaderBase"
        DTE.Find.ReplaceWith = "DataReaderBase"
        result = DTE.Find.Execute()

        DTE.Find.FindWhat = "new DCGEngine.SqlMgr()"
        DTE.Find.ReplaceWith = "dg.Sql.Connector.ConnectorBase.NewInstance()"
        result = DTE.Find.Execute()

        DTE.Find.FindWhat = "DCGEngine.SqlMgr"
        DTE.Find.ReplaceWith = "dg.Sql.Connector.ConnectorBase"
        result = DTE.Find.Execute()

        DTE.Find.FindWhat = "DCGEngine.Sql.Phrases"
        DTE.Find.ReplaceWith = "dg.Sql.Phrases"
        result = DTE.Find.Execute()

        DTE.Find.FindWhat = "TYPE == ConnectorBase.SqlServiceType.MYSQL"
        DTE.Find.ReplaceWith = "TYPE == dg.Sql.Connector.ConnectorBase.SqlServiceType.MYSQL"
        result = DTE.Find.Execute()

        DTE.Find.FindWhat = "TYPE == DCGEngine.ConnectorBase.SqlServiceType.MYSQL"
        DTE.Find.ReplaceWith = "TYPE == dg.Sql.Connector.ConnectorBase.SqlServiceType.MYSQL"
        result = DTE.Find.Execute()

        DTE.Find.FindWhat = "connectionStringName=""DCGEngine"""
        DTE.Find.ReplaceWith = "connectionStringName=""dg.Sql"""
        result = DTE.Find.Execute()

        DTE.Find.FindWhat = "name=""DCGEngine"""
        DTE.Find.ReplaceWith = "name=""dg.Sql"""
        DTE.Find.FilesOfType = "db.config"
        result = DTE.Find.Execute()

        DTE.Find.PatternSyntax = oldPatternSyntax
        DTE.Find.Target = oldTarget
        DTE.Find.MatchCase = oldMatchCase
        DTE.Find.MatchWholeWord = oldMatchWholeWord
        DTE.Find.MatchInHiddenText = oldMatchInHiddenText
        DTE.Find.FindWhat = oldFindWhat
        DTE.Find.ReplaceWith = oldReplaceWith
        DTE.Find.ResultsLocation = oldResultsLocation
        DTE.Find.Action = oldAction
        DTE.Find.SearchPath = oldSearchPath
        DTE.Find.FilesOfType = oldFilesOfType
    End Sub
    Enum DalColumnType
        TInt
        TInt8
        TInt16
        TInt32
        TInt64
        TUInt8
        TUInt16
        TUInt32
        TUInt64
        TString
        TFixedString
        TText
        TLongText
        TMediumText
        TBool
        TGuid
        TDateTime
        TDecimal
        TDouble
        TFloat
        TGeometry
        TGeometryCollection
        TPoint
        TLineString
        TPolygon
        TLine
        TCurve
        TSurface
        TLinearRing
        TMultiPoint
        TMultiLineString
        TMultiPolygon
        TMultiCurve
        TMultiSurface
    End Enum
    Enum DalIndexClusterMode
        None
        NonClustered
        Clustered
    End Enum
    Enum DalIndexIndexMode
        None
        Unique
        FullText
        Spatial
        PrimaryKey
    End Enum
    Enum DalIndexIndexType
        None
        BTREE
        HASH
        RTREE
    End Enum
    Enum DalForeignKeyReference
        None
        Restrict
        Cascade
        SetNull
        NoAction
    End Enum
    Class DalColumn
        Public IsPrimaryKey As Boolean
        Public IsNullable As Boolean
        Public AutoIncrement As Boolean
        Public NoProperty As Boolean
        Public Type As DalColumnType
        Public EnumTypeName As String
        Public Name As String
        Public NameX As String
        Public MaxLength As Integer
        Public Precision As Integer
        Public Scale As Integer
        Public DefaultValue As String
        Public ActualDefaultValue As String
        Public Comment As String
        Public ActualType As String
        Public ToDb As String
        Public FromDb As String
        Public Virtual As Boolean
    End Class
    Class DalIndex
        Public IndexName As String
        Public ClusterMode As DalIndexClusterMode
        Public IndexType As DalIndexIndexType
        Public IndexMode As DalIndexIndexMode
        Public Columns As System.Collections.Generic.List(Of String)
        Public Sub New()
            Columns = New System.Collections.Generic.List(Of String)
            IndexName = Nothing
            ClusterMode = DalIndexClusterMode.None
            IndexType = DalIndexIndexType.None
            IndexMode = DalIndexIndexMode.None
        End Sub
    End Class
    Class DalForeignKey
        Public ForeignKeyName As String
        Public Columns As System.Collections.Generic.List(Of String)
        Public ForeignTable As String
        Public ForeignColumns As System.Collections.Generic.List(Of String)
        Public OnDelete As DalForeignKeyReference
        Public OnUpdate As DalForeignKeyReference
        Public Sub New()
            Columns = New System.Collections.Generic.List(Of String)
            ForeignColumns = New System.Collections.Generic.List(Of String)
            ForeignKeyName = Nothing
            ForeignTable = Nothing
            OnDelete = DalForeignKeyReference.None
            OnUpdate = DalForeignKeyReference.None
        End Sub
    End Class
    Class DalEnum
        Public Name As String
        Public Items As Collection
    End Class
    Sub GenerateDalClass()

        Dim objDocument As EnvDTE.Document
        Dim objTextDocument As EnvDTE.TextDocument
        Dim objTextSelection As EnvDTE.TextSelection
        Dim Columns = New System.Collections.Generic.List(Of DalColumn)
        Dim Indexes = New System.Collections.Generic.List(Of DalIndex)
        Dim ForeignKeys = New System.Collections.Generic.List(Of DalForeignKey)
        Dim Enums = New System.Collections.Generic.List(Of DalEnum)
        Dim className As String
        Dim schemaName As String
        Dim databaseOwner As String
        Dim primaryKey As String
        Dim beforeInsert As String
        Dim beforeUpdate As String
        Dim afterRead As String
        Dim MySqlEngine As String = ""

        objDocument = DTE.ActiveDocument
        objTextDocument = CType(objDocument.Object, EnvDTE.TextDocument)
        objTextSelection = objTextDocument.Selection

        Dim splt = objTextSelection.Text.Trim(New Char() {" ", "*", "/", vbTab, vbCr, vbLf}).Split(New Char() {vbCr, vbLf}, StringSplitOptions.RemoveEmptyEntries)
        className = splt(0).Trim(New Char() {" ", "*", vbTab})
        schemaName = splt(1).Trim(New Char() {" ", "*", vbTab})
        If schemaName.Contains(".") Then
            databaseOwner = schemaName.Substring(0, schemaName.IndexOf("."))
            schemaName = schemaName.Substring(schemaName.IndexOf(".") + 1)
        End If

        For j As Integer = 2 To splt.Length - 1
            Dim item = splt(j).Trim(New Char() {" ", "*", vbTab})
            Dim itemUpper = item.ToUpper()

            If itemUpper.StartsWith("@INDEX:", StringComparison.Ordinal) Then
                Dim parts = item.Substring(7).Split(New Char() {";"}, StringSplitOptions.RemoveEmptyEntries)
                Dim IDX = New DalIndex

                For jj As Integer = 0 To parts.Length - 1
                    Dim part = parts(jj).Trim()

                    If part.ToUpper().StartsWith("NAME(", StringComparison.Ordinal) Then
                        IDX.IndexName = part.Substring(5, part.IndexOf(")") - 5)
                    ElseIf part.ToUpper().Equals("UNIQUE", StringComparison.Ordinal) Then
                        IDX.IndexMode = DalIndexIndexMode.Unique
                    ElseIf part.ToUpper().Equals("PRIMARYKEY", StringComparison.Ordinal) Then
                        IDX.IndexMode = DalIndexIndexMode.PrimaryKey
                    ElseIf part.ToUpper().Equals("SPATIAL", StringComparison.Ordinal) Then
                        IDX.IndexMode = DalIndexIndexMode.Spatial
                    ElseIf part.ToUpper().Equals("FULLTEXT", StringComparison.Ordinal) Then
                        IDX.IndexMode = DalIndexIndexMode.FullText
                    ElseIf part.ToUpper().Equals("BTREE", StringComparison.Ordinal) Then
                        IDX.IndexType = DalIndexIndexType.BTREE
                    ElseIf part.ToUpper().Equals("RTREE", StringComparison.Ordinal) Then
                        IDX.IndexType = DalIndexIndexType.RTREE
                    ElseIf part.ToUpper().Equals("HASH", StringComparison.Ordinal) Then
                        IDX.IndexType = DalIndexIndexType.HASH
                    ElseIf part.ToUpper().Equals("NONCLUSTERED", StringComparison.Ordinal) Then
                        IDX.ClusterMode = DalIndexClusterMode.NonClustered
                    ElseIf part.ToUpper().Equals("CLUSTERED", StringComparison.Ordinal) Then
                        IDX.ClusterMode = DalIndexClusterMode.Clustered
                    ElseIf part.ToUpper().StartsWith("[", StringComparison.Ordinal) Then
                        Dim cols = part.Trim(New Char() {" ", "[", "]", vbTab}).Split(New Char() {","}, StringSplitOptions.RemoveEmptyEntries)
                        For Each col In cols
                            IDX.Columns.Add(col)
                        Next
                    End If
                Next

                Indexes.Add(IDX)

            ElseIf itemUpper.StartsWith("@FOREIGNKEY:", StringComparison.Ordinal) Then
                Dim parts = item.Substring(12).Split(New Char() {";"}, StringSplitOptions.RemoveEmptyEntries)
                Dim key = New DalForeignKey

                For jj As Integer = 0 To parts.Length - 1
                    Dim part = parts(jj).Trim()
                    Dim partUpper = part.ToUpper()

                    If partUpper.StartsWith("NAME(", StringComparison.Ordinal) Then
                        key.ForeignKeyName = part.Substring(5, part.IndexOf(")") - 5)
                    ElseIf partUpper.StartsWith("FOREIGNTABLE(", StringComparison.Ordinal) Then
                        key.ForeignTable = part.Substring(13, part.IndexOf(")") - 13)
                    ElseIf partUpper.StartsWith("ONUPDATE(", StringComparison.Ordinal) Then
                        Select Case (part.Substring(9, part.IndexOf(")") - 9)).ToUpper()
                            Case "RESTRICT"
                                key.OnUpdate = DalForeignKeyReference.Restrict
                            Case "CASCADE"
                                key.OnUpdate = DalForeignKeyReference.Cascade
                            Case "SETNULL"
                                key.OnUpdate = DalForeignKeyReference.SetNull
                            Case "NOACTION"
                                key.OnUpdate = DalForeignKeyReference.NoAction
                            Case Else
                                key.OnUpdate = DalForeignKeyReference.None
                        End Select
                    ElseIf partUpper.StartsWith("ONDELETE(", StringComparison.Ordinal) Then
                        Select Case (part.Substring(9, part.IndexOf(")") - 9)).ToUpper()
                            Case "RESTRICT"
                                key.OnDelete = DalForeignKeyReference.Restrict
                            Case "CASCADE"
                                key.OnDelete = DalForeignKeyReference.Cascade
                            Case "SETNULL"
                                key.OnDelete = DalForeignKeyReference.SetNull
                            Case "NOACTION"
                                key.OnDelete = DalForeignKeyReference.NoAction
                            Case Else
                                key.OnDelete = DalForeignKeyReference.None
                        End Select
                    ElseIf partUpper.StartsWith("COLUMNS[", StringComparison.Ordinal) Then
                        Dim cols = part.Substring(7).Trim(New Char() {" ", "[", "]", vbTab}).Split(New Char() {","}, StringSplitOptions.RemoveEmptyEntries)
                        For Each col In cols
                            key.Columns.Add(col)
                        Next
                    ElseIf partUpper.StartsWith("FOREIGNCOLUMNS[", StringComparison.Ordinal) Then
                        Dim cols = part.Substring(14).Trim(New Char() {" ", "[", "]", vbTab}).Split(New Char() {","}, StringSplitOptions.RemoveEmptyEntries)
                        For Each col In cols
                            key.ForeignColumns.Add(col)
                        Next
                    End If
                Next

                ForeignKeys.Add(key)
            ElseIf itemUpper.StartsWith("@BEFOREINSERT:", StringComparison.Ordinal) Then
                beforeInsert = item.Substring(14).Trim()
            ElseIf itemUpper.StartsWith("@BEFOREUPDATE:", StringComparison.Ordinal) Then
                beforeUpdate = item.Substring(14).Trim()
            ElseIf itemUpper.StartsWith("@AFTERREAD:", StringComparison.Ordinal) Then
                afterRead = item.Substring(11).Trim()
            ElseIf itemUpper.StartsWith("@MYSQLENGINE:", StringComparison.Ordinal) Then
                MySqlEngine = item.Substring(13).Trim()
            Else
                Dim idx = item.IndexOf(":")
                Dim col = New DalColumn
                col.Name = item.Substring(0, idx).Trim()
                col.NameX = col.Name
                If (className = col.Name) Then col.Name = col.Name + "X"
                col.IsPrimaryKey = False
                col.IsNullable = False
                col.AutoIncrement = False
                col.Type = DalColumnType.TInt
                col.DefaultValue = "null"
                col.ActualDefaultValue = ""
                col.Comment = ""
                col.EnumTypeName = ""
                item = item.Substring(idx + 1).Trim()

                Dim parts = item.Split(New Char() {";"}, StringSplitOptions.None)
                For jj As Integer = 0 To parts.Length - 1
                    Dim part = parts(jj).Trim()
                    Dim partUpper = part.ToUpper()
                    If jj = parts.Length - 1 Then
                        If part.EndsWith(":") AndAlso splt.Length > j + 2 AndAlso (splt(j + 1).Trim(New Char() {" ", "*", vbTab}).StartsWith("""") AndAlso splt(j + 2).Trim(New Char() {" ", "*", vbTab}).StartsWith("-")) Then
                            col.Comment = part.Remove(part.Length - 1, 1)
                            j = j + 1
                            item = splt(j)
                            Dim en = New DalEnum
                            en.Name = item.Trim(New Char() {" ", "*", """", vbTab})
                            col.EnumTypeName = en.Name
                            en.Items = New Collection
                            While (splt.Length > j + 1 AndAlso splt(j + 1).Trim(New Char() {" ", "*", vbTab}).StartsWith("-"))
                                j = j + 1
                                item = splt(j).Trim(New Char() {" ", "*", "-", vbTab})
                                en.Items.Add(item)
                            End While
                            Enums.Add(en)
                        Else
                            col.Comment = part
                        End If
                    ElseIf partUpper.Equals("PRIMARY KEY", StringComparison.Ordinal) Then
                        col.IsPrimaryKey = True
                        If (primaryKey Is Nothing) Then
                            primaryKey = col.Name
                        Else
                            primaryKey = ""
                        End If
                    ElseIf partUpper.Equals("NULLABLE", StringComparison.Ordinal) Then
                        col.IsNullable = True
                    ElseIf partUpper.Equals("AUTOINCREMENT", StringComparison.Ordinal) Then
                        col.AutoIncrement = True
                    ElseIf partUpper.Equals("NOPROPERTY", StringComparison.Ordinal) Then
                        col.NoProperty = True
                    ElseIf partUpper.StartsWith("PRECISION(", StringComparison.Ordinal) Then
                        Dim val = 0
                        Integer.TryParse(part.Substring(10, part.IndexOf(")") - 10), val)
                        col.Precision = val
                    ElseIf partUpper.StartsWith("SCALE(", StringComparison.Ordinal) Then
                        Dim val = 0
                        Integer.TryParse(part.Substring(6, part.IndexOf(")") - 6), val)
                        col.Scale = val

                    ElseIf partUpper.StartsWith("STRING(", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TString
                        Dim length = 0
                        Integer.TryParse(part.Substring(7, part.IndexOf(")") - 7), length)
                        col.MaxLength = length
                    ElseIf partUpper.StartsWith("FIXEDSTRING(", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TFixedString
                        Dim length = 0
                        Integer.TryParse(part.Substring(12, part.IndexOf(")") - 12), length)
                        col.MaxLength = length
                    ElseIf partUpper.Equals("TEXT", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TText
                    ElseIf partUpper.StartsWith("TEXT(", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TText
                        Dim length = 0
                        Integer.TryParse(part.Substring(5, part.IndexOf(")") - 5), length)
                        col.MaxLength = length
                    ElseIf partUpper.Equals("LONGTEXT", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TLongText
                    ElseIf partUpper.StartsWith("LONGTEXT(", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TLongText
                        Dim length = 0
                        Integer.TryParse(part.Substring(9, part.IndexOf(")") - 9), length)
                        col.MaxLength = length
                    ElseIf partUpper.Equals("MEDIUMTEXT", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TMediumText
                    ElseIf partUpper.StartsWith("MEDIUMTEXT(", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TMediumText
                        Dim length = 0
                        Integer.TryParse(part.Substring(11, part.IndexOf(")") - 1), length)
                        col.MaxLength = length

                    ElseIf partUpper.Equals("BOOL", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TBool
                    ElseIf partUpper.Equals("GUID", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TGuid

                    ElseIf partUpper.Equals("DECIMAL", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TDecimal
                    ElseIf partUpper.StartsWith("DECIMAL", StringComparison.Ordinal) Then
                        Dim scale = "", precision = "", idx1 = -1, idx2 = -1, idx3 = -1
                        idx1 = part.IndexOf("(")
                        idx2 = part.IndexOf(",")
                        idx3 = part.IndexOf(")")
                        If (idx1 > -1 And idx2 > -1) Then
                            scale = part.Substring(idx1 + 1, idx2 - idx1 - 1).Trim()
                            precision = part.Substring(idx2 + 1, idx3 - idx2 - 1).Trim()
                        ElseIf (idx1 > -1) Then
                            scale = part.Substring(idx1 + 1, idx3 - idx1 - 1).Trim()
                        End If
                        If (scale.Length > 0) Then col.Scale = Convert.ToInt32(scale)
                        If (precision.Length > 0) Then col.Precision = Convert.ToInt32(precision)
                        col.Type = DalColumnType.TDecimal
                    ElseIf partUpper.Equals("DOUBLE", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TDouble
                    ElseIf partUpper.Equals("FLOAT", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TFloat
                    ElseIf partUpper.Equals("INT", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TInt
                    ElseIf partUpper.Equals("INTEGER", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TInt
                    ElseIf partUpper.Equals("INT8", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TInt8
                    ElseIf partUpper.Equals("INT16", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TInt16
                    ElseIf partUpper.Equals("INT32", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TInt32
                    ElseIf partUpper.Equals("INT64", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TInt64
                    ElseIf partUpper.Equals("UINT8", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TUInt8
                    ElseIf partUpper.Equals("UINT16", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TUInt16
                    ElseIf partUpper.Equals("UINT32", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TUInt32
                    ElseIf partUpper.Equals("UINT64", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TUInt64

                    ElseIf partUpper.Equals("GEOMETRY", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TGeometry
                    ElseIf partUpper.Equals("GEOMETRYCOLLECTION", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TGeometryCollection
                    ElseIf partUpper.Equals("POINT", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TPoint
                    ElseIf partUpper.Equals("LINESTRING", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TLineString
                    ElseIf partUpper.Equals("POLYGON", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TPolygon
                    ElseIf partUpper.Equals("LINE", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TLine
                    ElseIf partUpper.Equals("CURVE", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TCurve
                    ElseIf partUpper.Equals("SURFACE", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TSurface
                    ElseIf partUpper.Equals("LINEARRING", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TLinearRing
                    ElseIf partUpper.Equals("MULTIPOINT", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TMultiPoint
                    ElseIf partUpper.Equals("MULTILINESTRING", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TMultiLineString
                    ElseIf partUpper.Equals("MULTIPOLYGON", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TMultiPolygon
                    ElseIf partUpper.Equals("MULTICURVE", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TMultiCurve
                    ElseIf partUpper.Equals("MULTISURFACE", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TMultiSurface

                    ElseIf part.Equals("DATETIME", StringComparison.Ordinal) Then
                        col.Type = DalColumnType.TDateTime
                    ElseIf partUpper.StartsWith("DEFAULT ", StringComparison.Ordinal) Then
                        col.DefaultValue = part.Substring(8)
                    ElseIf partUpper.StartsWith("ACTUALDEFAULT ", StringComparison.Ordinal) Then
                        col.ActualDefaultValue = part.Substring(14)
                    ElseIf partUpper.StartsWith("TODB ", StringComparison.Ordinal) Then
                        col.ToDb = part.Substring(5)
                    ElseIf partUpper.Equals("VIRTUAL", StringComparison.Ordinal) Then
                        col.Virtual = True
                    ElseIf partUpper.StartsWith("FROMDB ", StringComparison.Ordinal) Then
                        col.FromDb = part.Substring(7)
                    ElseIf partUpper.StartsWith("ACTUALTYPE ", StringComparison.Ordinal) Then
                        col.ActualType = part.Substring(11)
                    ElseIf partUpper.Equals("UNIQUE INDEX", StringComparison.Ordinal) Then
                        Dim unqIdx = New DalIndex
                        unqIdx.Columns.Add(col.Name)
                        unqIdx.IndexMode = DalIndexIndexMode.Unique
                        Indexes.Add(unqIdx)
                    ElseIf partUpper.StartsWith("FOREIGN ", StringComparison.Ordinal) Then
                        Dim key = New DalForeignKey
                        Dim data = part.Substring(8)
                        key.ForeignTable = data.Substring(0, data.IndexOf("."))
                        key.ForeignColumns.Add(data.Substring(data.IndexOf(".") + 1))
                        key.Columns.Add(col.Name)
                        ForeignKeys.Add(key)
                    End If
                Next

                If (col.IsPrimaryKey And col.Type = DalColumnType.TInt) Then col.Type = DalColumnType.TInt64

                Columns.Add(col)
            End If

        Next

        ' Correct casing
        If (MySqlEngine.Equals("MyISAM", StringComparison.OrdinalIgnoreCase)) Then
            MySqlEngine = "MyISAM"
        ElseIf (MySqlEngine.Equals("InnoDB", StringComparison.OrdinalIgnoreCase)) Then
            MySqlEngine = "InnoDB"
        ElseIf (MySqlEngine.Equals("ARCHIVE", StringComparison.OrdinalIgnoreCase)) Then
            MySqlEngine = "ARCHIVE"
        End If

        Dim sbOutput As New StringBuilder

        sbOutput.AppendFormat("public partial class {1}Collection : AbstractRecordList<{1}, {1}Collection> {{{0}}}{0}{0}", _
            vbCrLf, className)

        For Each EN In Enums
            sbOutput.AppendFormat("public enum {1}{0}{{{0}", vbCrLf, EN.Name)
            For Each IT In EN.Items
                sbOutput.AppendFormat("{1},{0}", vbCrLf, IT)
            Next
            sbOutput.AppendFormat("}}{0}{0}", vbCrLf)
        Next

        sbOutput.AppendFormat("public partial class {1} : AbstractRecord<{1}>{0}{{{0}#region Table Schema{0}private static TableSchema _TableSchema;{0}public struct Columns{0}{{{0}", vbCrLf, className)
        For Each Column As DalColumn In Columns
            sbOutput.AppendFormat("public static string {1} = ""{2}"";", vbCrLf, Column.Name, Column.NameX)
            If Not String.IsNullOrEmpty(Column.Comment) Then
                sbOutput.AppendFormat(" // {1}", vbCrLf, Column.Comment)
            End If
            sbOutput.Append(vbCrLf)
        Next
        sbOutput.AppendFormat("}}{0}", vbCrLf)
        sbOutput.AppendFormat("public override TableSchema GetTableSchema(){0}{{{0}if (null == _TableSchema){0}{{{0}TableSchema schema = new TableSchema();{0}schema.SchemaName = @""{1}"";{0}", vbCrLf, schemaName)
        If (Not databaseOwner Is Nothing AndAlso databaseOwner.Length > 0) Then
            sbOutput.AppendFormat("schema.DatabaseOwner = @""{1}"";{0}", vbCrLf, databaseOwner)
        End If

        For Each Column As DalColumn In Columns
            Dim oldActualType = Column.ActualType

            sbOutput.AppendFormat("schema.AddColumn(Columns.{1}, ", vbCrLf, Column.Name)

            If (Not String.IsNullOrEmpty(Column.EnumTypeName)) Then
                Column.ActualType = Column.EnumTypeName
            ElseIf (Column.Type = DalColumnType.TBool) Then
                Column.ActualType = "bool"
            ElseIf (Column.Type = DalColumnType.TGuid) Then
                Column.ActualType = "Guid"
            ElseIf (Column.Type = DalColumnType.TDateTime) Then
                Column.ActualType = "DateTime"
            ElseIf (Column.Type = DalColumnType.TInt) Then
                Column.ActualType = "int"
            ElseIf (Column.Type = DalColumnType.TInt8) Then
                Column.ActualType = "SByte"
            ElseIf (Column.Type = DalColumnType.TInt16) Then
                Column.ActualType = "Int16"
            ElseIf (Column.Type = DalColumnType.TInt32) Then
                Column.ActualType = "Int32"
            ElseIf (Column.Type = DalColumnType.TInt64) Then
                Column.ActualType = "Int64"
            ElseIf (Column.Type = DalColumnType.TUInt8) Then
                Column.ActualType = "Byte"
            ElseIf (Column.Type = DalColumnType.TUInt16) Then
                Column.ActualType = "UInt16"
            ElseIf (Column.Type = DalColumnType.TUInt32) Then
                Column.ActualType = "UInt32"
            ElseIf (Column.Type = DalColumnType.TUInt64) Then
                Column.ActualType = "UInt64"
            ElseIf (Column.Type = DalColumnType.TString Or _
                    Column.Type = DalColumnType.TText Or _
                    Column.Type = DalColumnType.TLongText Or _
                    Column.Type = DalColumnType.TMediumText Or _
                    Column.Type = DalColumnType.TFixedString) Then
                Column.ActualType = "string"
            ElseIf (Column.Type = DalColumnType.TDecimal) Then
                Column.ActualType = "decimal"
            ElseIf (Column.Type = DalColumnType.TDouble) Then
                Column.ActualType = "double"
            ElseIf (Column.Type = DalColumnType.TFloat) Then
                Column.ActualType = "float"
            ElseIf (Column.Type = DalColumnType.TGeometry) Then
                Column.ActualType = "Geometry"
            ElseIf (Column.Type = DalColumnType.TGeometryCollection) Then
                Column.ActualType = "Geometry.GeometryCollection"
            ElseIf (Column.Type = DalColumnType.TPoint) Then
                Column.ActualType = "Geometry.Point"
            ElseIf (Column.Type = DalColumnType.TLineString) Then
                Column.ActualType = "Geometry.LineString"
            ElseIf (Column.Type = DalColumnType.TPolygon) Then
                Column.ActualType = "Geometry.Polygon"
            ElseIf (Column.Type = DalColumnType.TLine) Then
                Column.ActualType = "Geometry.Line"
            ElseIf (Column.Type = DalColumnType.TCurve) Then
                Column.ActualType = "Geometry"
            ElseIf (Column.Type = DalColumnType.TSurface) Then
                Column.ActualType = "Geometry"
            ElseIf (Column.Type = DalColumnType.TLinearRing) Then
                Column.ActualType = "Geometry"
            ElseIf (Column.Type = DalColumnType.TMultiPoint) Then
                Column.ActualType = "Geometry.MultiPoint"
            ElseIf (Column.Type = DalColumnType.TMultiLineString) Then
                Column.ActualType = "Geometry.MultiLineString"
            ElseIf (Column.Type = DalColumnType.TMultiPolygon) Then
                Column.ActualType = "Geometry.MultiPolygon"
            ElseIf (Column.Type = DalColumnType.TMultiCurve) Then
                Column.ActualType = "Geometry.GeometryCollection"
            ElseIf (Column.Type = DalColumnType.TMultiSurface) Then
                Column.ActualType = "Geometry.GeometryCollection"
            End If

            sbOutput.AppendFormat("typeof({0})", Column.ActualType)
            If (Column.Type = DalColumnType.TText) Then
                sbOutput.Append(", DataType.Text")
            ElseIf (Column.Type = DalColumnType.TLongText) Then
                sbOutput.Append(", DataType.LongText")
            ElseIf (Column.Type = DalColumnType.TMediumText) Then
                sbOutput.Append(", DataType.MediumText")
            ElseIf (Column.Type = DalColumnType.TFixedString) Then
                sbOutput.Append(", DataType.Char")
            ElseIf (Column.Type = DalColumnType.TGeometry) Then
                sbOutput.Append(", DataType.Geometry")
            ElseIf (Column.Type = DalColumnType.TGeometryCollection) Then
                sbOutput.Append(", DataType.GeometryCollection")
            ElseIf (Column.Type = DalColumnType.TPoint) Then
                sbOutput.Append(", DataType.Point")
            ElseIf (Column.Type = DalColumnType.TLineString) Then
                sbOutput.Append(", DataType.LineString")
            ElseIf (Column.Type = DalColumnType.TPolygon) Then
                sbOutput.Append(", DataType.Polygon")
            ElseIf (Column.Type = DalColumnType.TLine) Then
                sbOutput.Append(", DataType.Line")
            ElseIf (Column.Type = DalColumnType.TCurve) Then
                sbOutput.Append(", DataType.Curve")
            ElseIf (Column.Type = DalColumnType.TSurface) Then
                sbOutput.Append(", DataType.Surface")
            ElseIf (Column.Type = DalColumnType.TLinearRing) Then
                sbOutput.Append(", DataType.LinearRing")
            ElseIf (Column.Type = DalColumnType.TMultiPoint) Then
                sbOutput.Append(", DataType.MultiPoint")
            ElseIf (Column.Type = DalColumnType.TMultiLineString) Then
                sbOutput.Append(", DataType.MultiLineString")
            ElseIf (Column.Type = DalColumnType.TMultiPolygon) Then
                sbOutput.Append(", DataType.MultiPolygon")
            ElseIf (Column.Type = DalColumnType.TMultiCurve) Then
                sbOutput.Append(", DataType.MultiCurve")
            ElseIf (Column.Type = DalColumnType.TMultiSurface) Then
                sbOutput.Append(", DataType.MultiSurface")
            ElseIf (Not String.IsNullOrEmpty(Column.EnumTypeName)) Then
                If (Column.Type = DalColumnType.TInt8) Then
                    sbOutput.Append(", DataType.TinyInt")
                ElseIf (Column.Type = DalColumnType.TInt16) Then
                    sbOutput.Append(", DataType.SmallInt")
                ElseIf (Column.Type = DalColumnType.TInt32) Then
                    sbOutput.Append(", DataType.Int")
                ElseIf (Column.Type = DalColumnType.TInt64) Then
                    sbOutput.Append(", DataType.BigInt")
                ElseIf (Column.Type = DalColumnType.TUInt8) Then
                    sbOutput.Append(", DataType.UnsignedTinyInt")
                ElseIf (Column.Type = DalColumnType.TUInt16) Then
                    sbOutput.Append(", DataType.UnsignedSmallInt")
                ElseIf (Column.Type = DalColumnType.TUInt32) Then
                    sbOutput.Append(", DataType.UnsignedInt")
                ElseIf (Column.Type = DalColumnType.TUInt64) Then
                    sbOutput.Append(", DataType.UnsignedBigInt")
                End If
            End If

            If Column.IsNullable AndAlso Column.ActualType <> "string" Then Column.ActualType = Column.ActualType + "?"

            sbOutput.AppendFormat(", {1}, {2}, {3}, {4}, {5}, {6}, {7});{0}", vbCrLf, Column.MaxLength, Column.Precision, Column.Scale, IIf(Column.AutoIncrement, "true", "false"), IIf(Column.IsPrimaryKey, "true", "false"), IIf(Column.IsNullable, "true", "false"), Column.DefaultValue)


            If (Not String.IsNullOrEmpty(oldActualType)) Then Column.ActualType = oldActualType
        Next

        sbOutput.AppendFormat("{0}_TableSchema = schema;{0}", vbCrLf)

        If (Indexes.Count > 0) Then
            sbOutput.AppendFormat("{0}", vbCrLf)
            For Each IDX In Indexes
                sbOutput.AppendFormat("schema.AddIndex({0}, TableSchema.ClusterMode.{1}, TableSchema.IndexMode.{2}, TableSchema.IndexType.{3}", _
                                        IIf(IDX.IndexName Is Nothing, "null", """" + IDX.IndexName + """"), _
                                        IDX.ClusterMode.ToString(), _
                                        IDX.IndexMode.ToString(), _
                                        IDX.IndexType.ToString())
                For Each CL In IDX.Columns
                    Dim CLParts = CL.Split(" ")
                    Dim ColText = CLParts(0)
                    Dim col As DalColumn = Columns.Find(Function(c As DalColumn) c.Name = ColText)
                    If (Not col Is Nothing) Then
                        ColText = String.Format("Columns.{0}", col.Name)
                    Else
                        ColText = String.Format("""{0}""", ColText)
                    End If
                    sbOutput.AppendFormat(", {0}", ColText)
                    If (CLParts.Length = 2) Then sbOutput.AppendFormat(", SortDirection.{0}", CLParts(1))
                Next
                sbOutput.AppendFormat(");{0}", vbCrLf)
            Next
        End If

        If (ForeignKeys.Count > 0) Then
            sbOutput.AppendFormat("{0}", vbCrLf)
            For Each key In ForeignKeys
                sbOutput.AppendFormat("schema.AddForeignKey({0}, ", _
                                        IIf(key.ForeignKeyName Is Nothing, "null", """" + key.ForeignKeyName + """"))
                If (key.Columns.Count > 1) Then
                    sbOutput.Append("new string[] {")
                    For Each CL In key.Columns
                        If (CL <> key.Columns(0)) Then sbOutput.Append(" ,")
                        sbOutput.AppendFormat("{0}.Columns.{1}", className, CL)
                    Next
                    sbOutput.Append("}, ")
                Else
                    sbOutput.AppendFormat("{0}.Columns.{1}, ", className, key.Columns(0))
                End If
                If (key.ForeignTable = className) Then
                    sbOutput.Append("schema.SchemaName, ")
                Else
                    sbOutput.AppendFormat("{0}.TableSchema.SchemaName, ", key.ForeignTable)
                End If
                If (key.ForeignColumns.Count > 1) Then
                    sbOutput.Append("new string[] {")
                    For Each CL In key.ForeignColumns
                        If (CL <> key.ForeignColumns(0)) Then sbOutput.Append(" ,")
                        sbOutput.AppendFormat("{0}.Columns.{1}", key.ForeignTable, CL)
                    Next
                    sbOutput.Append("}, ")
                Else
                    sbOutput.AppendFormat("{0}.Columns.{1}, ", key.ForeignTable, key.ForeignColumns(0))
                End If
                sbOutput.AppendFormat("TableSchema.ForeignKeyReference.{1}, TableSchema.ForeignKeyReference.{2});{0}", vbCrLf, key.OnDelete.ToString(), key.OnUpdate.ToString())
            Next
        End If

        If (MySqlEngine.Length > 0) Then
            sbOutput.AppendFormat("{0}schema.SetMySqlEngine(MySqlEngineType.{1});{0}", vbCrLf, MySqlEngine)
        End If

        sbOutput.AppendFormat("{0}}}{0}{0}return _TableSchema;{0}}}{0}#endregion{0}{0}#region Private Members{0}", vbCrLf)

        For Each Column As DalColumn In Columns
            If (Not Column.NoProperty) Then sbOutput.Append("internal ")

            Dim defval As String
            defval = Column.DefaultValue
            If (String.IsNullOrEmpty(defval) OrElse defval = "null") Then
                If (Not String.IsNullOrEmpty(Column.EnumTypeName)) Then
                    defval = Nothing
                ElseIf (Column.Type = DalColumnType.TBool) Then
                    defval = "false"
                ElseIf (Column.Type = DalColumnType.TGuid) Then
                    defval = "Guid.Empty"
                ElseIf (Column.Type = DalColumnType.TDateTime) Then
                    defval = "DateTime.UtcNow"
                ElseIf (Column.Type = DalColumnType.TInt) Then
                    defval = "0"
                ElseIf (Column.Type = DalColumnType.TInt8) Then
                    defval = "0"
                ElseIf (Column.Type = DalColumnType.TInt16) Then
                    defval = "0"
                ElseIf (Column.Type = DalColumnType.TInt32) Then
                    defval = "0"
                ElseIf (Column.Type = DalColumnType.TInt64) Then
                    defval = "0"
                ElseIf (Column.Type = DalColumnType.TUInt8) Then
                    defval = "0"
                ElseIf (Column.Type = DalColumnType.TUInt16) Then
                    defval = "0"
                ElseIf (Column.Type = DalColumnType.TUInt32) Then
                    defval = "0"
                ElseIf (Column.Type = DalColumnType.TUInt64) Then
                    defval = "0"
                ElseIf (Column.Type = DalColumnType.TString Or _
                        Column.Type = DalColumnType.TText Or _
                        Column.Type = DalColumnType.TLongText Or _
                        Column.Type = DalColumnType.TMediumText Or _
                        Column.Type = DalColumnType.TFixedString) Then
                    defval = "string.Empty"
                ElseIf (Column.Type = DalColumnType.TDecimal) Then
                    defval = "0m"
                ElseIf (Column.Type = DalColumnType.TDouble) Then
                    defval = "0d"
                ElseIf (Column.Type = DalColumnType.TFloat) Then
                    defval = "0f"
                End If
            End If
            If (Column.ActualDefaultValue.Length > 0) Then defval = Column.ActualDefaultValue

            If (Not Column.NoProperty) Then
                sbOutput.Append(Column.ActualType)
                sbOutput.AppendFormat(" _{0}", Column.Name)
                If ((Column.DefaultValue = "null" Or (Column.ActualDefaultValue.Length > 0 And Column.ActualDefaultValue = "null")) AndAlso Column.IsNullable) Then
                    sbOutput.AppendFormat(" = {1};{0}", vbCrLf, IIf(Column.ActualDefaultValue.Length > 0, Column.ActualDefaultValue, Column.DefaultValue))
                ElseIf defval Is Nothing Then
                    sbOutput.AppendFormat(";{0}", vbCrLf)
                Else
                    sbOutput.AppendFormat(" = {1};{0}", vbCrLf, defval)
                End If
            End If
        Next

        sbOutput.AppendFormat("#endregion{0}{0}#region Properties{0}", vbCrLf)

        For Each Column As DalColumn In Columns
            If (Not Column.NoProperty) Then sbOutput.AppendFormat("public {3}{1} {2}{0}{{{0}get{{return _{2};}}{0}set{{_{2}=value;}}{0}}}{0}", vbCrLf, Column.ActualType, Column.Name, IIf(Column.Virtual, "virtual ", ""))
        Next
        sbOutput.AppendFormat("#endregion{0}{0}#region AbstractRecord members{0}", vbCrLf)
        sbOutput.AppendFormat("public override object GetPrimaryKeyValue(){0}{{{0}return {1};{0}}}{0}{0}public override void Insert(ConnectorBase conn){0}{{{0}", vbCrLf, IIf(primaryKey Is Nothing Or primaryKey = "", "null", primaryKey))

        Dim extra = False
        If (Not (Columns.Find(Function(c As DalColumn) c.Name = "CreatedBy") Is Nothing)) Then
            sbOutput.AppendFormat("CreatedBy = base.CurrentSessionUserName;{0}", vbCrLf)
            extra = True
        End If
        If (Not (Columns.Find(Function(c As DalColumn) c.Name = "CreatedOn") Is Nothing)) Then
            sbOutput.AppendFormat("CreatedOn = DateTime.UtcNow;{0}", vbCrLf)
            extra = True
        End If
        If extra Then sbOutput.Append(vbCrLf)

        If (Not String.IsNullOrEmpty(beforeInsert)) Then
            sbOutput.AppendFormat("{1}{0}{0}", vbCrLf, beforeInsert)
        End If

        sbOutput.AppendFormat("Query qry = new Query(TableSchema);{0}", vbCrLf)
        For Each Column As DalColumn In Columns
            If (Not Column.AutoIncrement) Then
                If (Not String.IsNullOrEmpty(Column.ToDb)) Then
                    sbOutput.AppendFormat("qry.Insert(Columns.{1}, {2});{0}", vbCrLf, Column.Name, String.Format(Column.ToDb, Column.Name))
                Else
                    sbOutput.AppendFormat("qry.Insert(Columns.{1}, {1});{0}", vbCrLf, Column.Name)
                End If
            End If
        Next
        sbOutput.AppendFormat("{0}object lastInsert = null;{0}if (qry.Execute(conn, out lastInsert) > 0){0}{{{0}", vbCrLf)
        If (Not primaryKey Is Nothing AndAlso primaryKey <> "") Then
            Dim conversion As String = "{0}"
            Dim primaryKeyCol = Columns.Find(Function(c As DalColumn) c.Name = primaryKey)
            If (primaryKeyCol.Type = DalColumnType.TBool) Then
                conversion = "Convert.ToBoolean({0})"
            ElseIf (primaryKeyCol.Type = DalColumnType.TGuid) Then
                conversion = "new Guid({0}.ToString())"
            ElseIf (primaryKeyCol.Type = DalColumnType.TInt) Then
                conversion = "Convert.ToInt32({0})"
            ElseIf (primaryKeyCol.Type = DalColumnType.TInt8) Then
                conversion = "Convert.ToSByte({0})"
            ElseIf (primaryKeyCol.Type = DalColumnType.TInt16) Then
                conversion = "Convert.ToInt16({0})"
            ElseIf (primaryKeyCol.Type = DalColumnType.TInt32) Then
                conversion = "Convert.ToInt32({0})"
            ElseIf (primaryKeyCol.Type = DalColumnType.TInt64) Then
                conversion = "Convert.ToInt64({0})"
            ElseIf (primaryKeyCol.Type = DalColumnType.TUInt8) Then
                conversion = "Convert.ToByte({0})"
            ElseIf (primaryKeyCol.Type = DalColumnType.TUInt16) Then
                conversion = "Convert.ToUInt16({0})"
            ElseIf (primaryKeyCol.Type = DalColumnType.TUInt32) Then
                conversion = "Convert.ToUInt32({0})"
            ElseIf (primaryKeyCol.Type = DalColumnType.TUInt64) Then
                conversion = "Convert.ToUInt64({0})"
            ElseIf (primaryKeyCol.Type = DalColumnType.TDecimal) Then
                conversion = "Convert.ToDecimal({0})"
            ElseIf (primaryKeyCol.Type = DalColumnType.TDouble) Then
                conversion = "Convert.ToDouble({0})"
            ElseIf (primaryKeyCol.Type = DalColumnType.TFloat) Then
                conversion = "Convert.ToFloat({0})"
            ElseIf (primaryKeyCol.Type = DalColumnType.TDateTime) Then
                conversion = "Convert.ToDateTime({0})"
            ElseIf (primaryKeyCol.Type = DalColumnType.TLongText Or _
                    primaryKeyCol.Type = DalColumnType.TMediumText Or _
                    primaryKeyCol.Type = DalColumnType.TText Or _
                    primaryKeyCol.Type = DalColumnType.TString Or _
                    primaryKeyCol.Type = DalColumnType.TFixedString) Then
                conversion = "(string){0}"
            ElseIf (primaryKeyCol.Type = DalColumnType.TGeometry Or _
                    primaryKeyCol.Type = DalColumnType.TGeometryCollection Or _
                    primaryKeyCol.Type = DalColumnType.TPoint Or _
                    primaryKeyCol.Type = DalColumnType.TLineString Or _
                    primaryKeyCol.Type = DalColumnType.TPolygon Or _
                    primaryKeyCol.Type = DalColumnType.TLine Or _
                    primaryKeyCol.Type = DalColumnType.TCurve Or _
                    primaryKeyCol.Type = DalColumnType.TSurface Or _
                    primaryKeyCol.Type = DalColumnType.TLinearRing Or _
                    primaryKeyCol.Type = DalColumnType.TMultiPoint Or _
                    primaryKeyCol.Type = DalColumnType.TMultiLineString Or _
                    primaryKeyCol.Type = DalColumnType.TMultiPolygon Or _
                    primaryKeyCol.Type = DalColumnType.TMultiCurve Or _
                    primaryKeyCol.Type = DalColumnType.TMultiSurface) Then
                conversion = "conn.ReadGeometry({0}) as " + primaryKeyCol.ActualType
            End If

            sbOutput.AppendFormat("{1} = {2};{0}", vbCrLf, primaryKey, String.Format(conversion, "(lastInsert)"))
        End If
        sbOutput.AppendFormat("MarkOld();{0}}}{0}}}{0}public override void Update(ConnectorBase conn){0}{{{0}", vbCrLf)

        extra = False
        If (Not (Columns.Find(Function(c As DalColumn) c.Name = "ModifiedBy") Is Nothing)) Then
            sbOutput.AppendFormat("ModifiedBy = base.CurrentSessionUserName;{0}", vbCrLf)
            extra = True
        End If
        If (Not (Columns.Find(Function(c As DalColumn) c.Name = "ModifiedOn") Is Nothing)) Then
            sbOutput.AppendFormat("ModifiedOn = DateTime.UtcNow;{0}", vbCrLf)
            extra = True
        End If
        If extra Then sbOutput.Append(vbCrLf)

        If (Not String.IsNullOrEmpty(beforeUpdate)) Then
            sbOutput.AppendFormat("{1}{0}{0}", vbCrLf, beforeUpdate)
        End If

        sbOutput.AppendFormat("Query qry = new Query(TableSchema);{0}", vbCrLf)
        ' Update columns
        For Each Column As DalColumn In Columns
            If (Not Column.AutoIncrement) Then
                If (Not String.IsNullOrEmpty(Column.ToDb)) Then
                    sbOutput.AppendFormat("qry.Update(Columns.{1}, {2});{0}", vbCrLf, Column.Name, String.Format(Column.ToDb, Column.Name))
                Else
                    sbOutput.AppendFormat("qry.Update(Columns.{1}, {1});{0}", vbCrLf, Column.Name)
                End If
            End If
        Next
        ' Update WHERE
        Dim firstPrimaryKey = True
        For Each Column As DalColumn In Columns
            If Not Column.IsPrimaryKey Then Continue For
            sbOutput.AppendFormat("qry.{2}(Columns.{1}, {1});{0}", vbCrLf, Column.Name, IIf(firstPrimaryKey, "Where", "AND"))
            firstPrimaryKey = False
        Next

        sbOutput.AppendFormat("{0}qry.Execute(conn);{0}}}{0}public override void Read(DataReaderBase reader){0}{{{0}", vbCrLf)

        ' Read columns
        For Each Column As DalColumn In Columns

            Dim conversion As String = "{0}", readerFormat As String = "reader[Columns.{0}]"
            If (Column.Type = DalColumnType.TBool) Then
                If (Column.IsNullable) Then
                    If (Column.ActualType.EndsWith("?")) Then
                        conversion = String.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToBoolean({{0}})", Column.ActualType)
                    Else
                        conversion = "IsNull({0}) ? {1} : Convert.ToBoolean({0})"
                    End If
                Else
                    conversion = "Convert.ToBoolean({0})"
                End If
            ElseIf (Column.Type = DalColumnType.TGuid) Then
                If (Column.IsNullable) Then
                    If (Column.ActualType.EndsWith("?")) Then
                        conversion = String.Format("IsNull({{0}}) ? ({0}){{1}} : GuidFromDb({{0}})", Column.ActualType)
                    Else
                        conversion = "IsNull({0}) ? {1} : GuidFromDb({0})"
                    End If
                Else
                    conversion = "GuidFromDb({0})"
                End If
            ElseIf (Column.Type = DalColumnType.TInt Or Column.Type = DalColumnType.TInt32) Then
                If (Column.IsNullable) Then
                    If (Column.DefaultValue = "0") Then
                        conversion = "Int32OrZero({0})"
                    ElseIf (Column.DefaultValue = "null") Then
                        conversion = "Int32OrNullFromDb({0})"
                    Else
                        If (Column.ActualType.EndsWith("?")) Then
                            conversion = String.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToInt32({{0}})", Column.ActualType)
                        Else
                            conversion = "IsNull({0}) ? {1} : Convert.ToInt32({0})"
                        End If
                    End If
                Else
                    conversion = "Convert.ToInt32({0})"
                End If
            ElseIf (Column.Type = DalColumnType.TUInt32) Then
                If (Column.IsNullable) Then
                    If (Column.ActualType.EndsWith("?")) Then
                        conversion = String.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToUInt32({{0}})", Column.ActualType)
                    Else
                        conversion = "IsNull({0}) ? {1} : Convert.ToUInt32({0})"
                    End If
                Else
                    conversion = "Convert.ToUInt32({0})"
                End If
            ElseIf (Column.Type = DalColumnType.TInt8) Then
                If (Column.IsNullable) Then
                    If (Column.ActualType.EndsWith("?")) Then
                        conversion = String.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToSByte({{0}})", Column.ActualType)
                    Else
                        conversion = "IsNull({0}) ? {1} : Convert.ToSByte({0})"
                    End If
                Else
                    conversion = "Convert.ToSByte({0})"
                End If
            ElseIf (Column.Type = DalColumnType.TUInt8) Then
                If (Column.IsNullable) Then
                    If (Column.ActualType.EndsWith("?")) Then
                        conversion = String.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToByte({{0}})", Column.ActualType)
                    Else
                        conversion = "IsNull({0}) ? {1} : Convert.ToByte({0})"
                    End If
                Else
                    conversion = "Convert.ToByte({0})"
                End If
            ElseIf (Column.Type = DalColumnType.TInt16) Then
                If (Column.IsNullable) Then
                    If (Column.ActualType.EndsWith("?")) Then
                        conversion = String.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToInt16({{0}})", Column.ActualType)
                    Else
                        conversion = "IsNull({0}) ? {1} : Convert.ToInt16({0})"
                    End If
                Else
                    conversion = "Convert.ToInt16({0})"
                End If
            ElseIf (Column.Type = DalColumnType.TUInt16) Then
                If (Column.IsNullable) Then
                    If (Column.ActualType.EndsWith("?")) Then
                        conversion = String.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToUInt16({{0}})", Column.ActualType)
                    Else
                        conversion = "IsNull({0}) ? {1} : Convert.ToUInt16({0})"
                    End If
                Else
                    conversion = "Convert.ToUInt16({0})"
                End If
            ElseIf (Column.Type = DalColumnType.TInt64) Then
                If (Column.IsNullable) Then
                    If (Column.ActualType.EndsWith("?")) Then
                        conversion = String.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToInt64({{0}})", Column.ActualType)
                    Else
                        conversion = "IsNull({0}) ? {1} : Convert.ToInt64({0})"
                    End If
                Else
                    conversion = "Convert.ToInt64({0})"
                End If
            ElseIf (Column.Type = DalColumnType.TUInt64) Then
                If (Column.IsNullable) Then
                    If (Column.ActualType.EndsWith("?")) Then
                        conversion = String.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToUInt64({{0}})", Column.ActualType)
                    Else
                        conversion = "IsNull({0}) ? {1} : Convert.ToUInt64({0})"
                    End If
                Else
                    conversion = "Convert.ToUInt64({0})"
                End If
            ElseIf (Column.Type = DalColumnType.TDecimal) Then
                If (Column.IsNullable) Then
                    If (Column.DefaultValue = "0" Or Column.DefaultValue = "0m") Then
                        conversion = "DecimalOrZeroFromDb({0})"
                    ElseIf (Column.DefaultValue = "null") Then
                        conversion = "DecimalOrNullFromDb({0})"
                    Else
                        If (Column.ActualType.EndsWith("?")) Then
                            conversion = String.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToDecimal({{0}})", Column.ActualType)
                        Else
                            conversion = "IsNull({0}) ? {1} : Convert.ToDecimal({0})"
                        End If
                    End If
                Else
                    conversion = "Convert.ToDecimal({0})"
                End If
            ElseIf (Column.Type = DalColumnType.TDouble) Then
                If (Column.IsNullable) Then
                    If (Column.ActualType.EndsWith("?")) Then
                        conversion = String.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToDouble({{0}})", Column.ActualType)
                    Else
                        conversion = "IsNull({0}) ? {1} : Convert.ToDouble({0})"
                    End If
                Else
                    conversion = "Convert.ToDouble({0})"
                End If
            ElseIf (Column.Type = DalColumnType.TFloat) Then
                If (Column.IsNullable) Then
                    If (Column.ActualType.EndsWith("?")) Then
                        conversion = String.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToFloat({{0}})", Column.ActualType)
                    Else
                        conversion = "IsNull({0}) ? {1} : Convert.ToFloat({0})"
                    End If
                Else
                    conversion = "Convert.ToFloat({0})"
                End If
            ElseIf (Column.Type = DalColumnType.TDateTime) Then
                If (Column.IsNullable) Then
                    If (Column.DefaultValue = "DateTime.UtcNow") Then
                        conversion = "DateTimeOrNow({0})"
                    ElseIf (Column.DefaultValue = "null") Then
                        conversion = "DateTimeOrNullFromDb({0})"
                    Else
                        If (Column.ActualType.EndsWith("?")) Then
                            conversion = String.Format("IsNull({{0}}) ? ({0}){{1}} : Convert.ToDateTime({{0}})", Column.ActualType)
                        Else
                            conversion = "IsNull({0}) ? {1} : Convert.ToDateTime({0})"
                        End If
                    End If
                Else
                    conversion = "Convert.ToDateTime({0})"
                End If
            ElseIf (Column.Type = DalColumnType.TLongText Or _
                    Column.Type = DalColumnType.TMediumText Or _
                    Column.Type = DalColumnType.TText Or _
                    Column.Type = DalColumnType.TString Or _
                    Column.Type = DalColumnType.TFixedString) Then
                If (Column.IsNullable) Then
                    conversion = "StringOrNullFromDb({0})"
                Else
                    conversion = "(string){0}"
                End If
            ElseIf (Column.Type = DalColumnType.TGeometry Or _
                    Column.Type = DalColumnType.TGeometryCollection Or _
                    Column.Type = DalColumnType.TPoint Or _
                    Column.Type = DalColumnType.TLineString Or _
                    Column.Type = DalColumnType.TPolygon Or _
                    Column.Type = DalColumnType.TLine Or _
                    Column.Type = DalColumnType.TCurve Or _
                    Column.Type = DalColumnType.TSurface Or _
                    Column.Type = DalColumnType.TLinearRing Or _
                    Column.Type = DalColumnType.TMultiPoint Or _
                    Column.Type = DalColumnType.TMultiLineString Or _
                    Column.Type = DalColumnType.TMultiPolygon Or _
                    Column.Type = DalColumnType.TMultiCurve Or _
                    Column.Type = DalColumnType.TMultiSurface) Then
                readerFormat = "reader.GetGeometry(Columns.{0}) as " + Column.ActualType
            End If

            If (Not String.IsNullOrEmpty(Column.EnumTypeName)) Then
                conversion = "(" + Column.EnumTypeName + ")" + conversion
            End If

            If (Not String.IsNullOrEmpty(Column.FromDb)) Then
                conversion = Column.FromDb
            End If

            sbOutput.AppendFormat("{1} = {2};{0}", vbCrLf, Column.Name, String.Format(conversion, String.Format(readerFormat, Column.Name), Column.DefaultValue))
        Next

        If (Not String.IsNullOrEmpty(afterRead)) Then
            sbOutput.AppendFormat("{0}{1}{0}", vbCrLf, afterRead)
        End If

        sbOutput.AppendFormat("{0}IsThisANewRecord = false;}}{0}#endregion{0}", vbCrLf)

        sbOutput.AppendFormat("{0}#region Helpers{0}", vbCrLf)

        Dim PrimaryKeyColumns = New System.Collections.Generic.List(Of DalColumn)
        For Each col In Columns
            If col.IsPrimaryKey Then
                PrimaryKeyColumns.Add(col)
            End If
        Next
        For Each index In Indexes
            If index.IndexMode = DalIndexIndexMode.PrimaryKey Then
                For Each ColText In index.Columns
                    Dim col As DalColumn = Columns.Find(Function(c As DalColumn) c.Name = ColText)
                    If (Not col Is Nothing) Then
                        PrimaryKeyColumns.Add(col)
                    End If
                Next
            End If
        Next

        If (PrimaryKeyColumns.Count > 0) Then

            sbOutput.AppendFormat("public static {1} FetchByID(", vbCrLf, className)
            Dim first = True
            For Each col In PrimaryKeyColumns
                If first Then first = False Else sbOutput.Append(", ")
                sbOutput.AppendFormat("{0} {1}", col.ActualType, col.Name)
            Next
            sbOutput.AppendFormat("){0}{{Query qry = new Query(TableSchema){0}", vbCrLf)
            first = True
            For Each col In PrimaryKeyColumns
                If first Then
                    sbOutput.AppendFormat(".Where(Columns.{0}, {0})", col.Name)
                    first = False
                Else
                    sbOutput.AppendFormat("{0}.AND(Columns.{1}, {1})", vbCrLf, col.Name)
                End If
            Next
            sbOutput.AppendFormat(";{0}using (DataReaderBase reader = qry.ExecuteReader()){0}{{{0}if (reader.Read()){0}{{{0}{1} item = new {1}();{0}item.Read(reader);{0}return item;{0}}}{0}}}{0}return null;{0}}}{0}{0}", vbCrLf, className)

            sbOutput.AppendFormat("public static int Delete(", vbCrLf)
            first = True
            For Each col In PrimaryKeyColumns
                If first Then first = False Else sbOutput.Append(", ")
                sbOutput.AppendFormat("{0} {1}", col.ActualType, col.Name)
            Next
            sbOutput.AppendFormat("){0}{{Query qry = new Query(TableSchema){0}", vbCrLf)
            first = True
            For Each col In PrimaryKeyColumns
                If first Then
                    sbOutput.AppendFormat(".Delete().Where(Columns.{0}, {0})", col.Name)
                    first = False
                Else
                    sbOutput.AppendFormat("{0}.AND(Columns.{1}, {1})", vbCrLf, col.Name)
                End If
            Next
            sbOutput.AppendFormat(";{0}return qry.Execute();{0}}}{0}", vbCrLf)

            sbOutput.AppendFormat("public static {1} FetchByID(ConnectorBase conn, ", vbCrLf, className)
            first = True
            For Each col In PrimaryKeyColumns
                If first Then first = False Else sbOutput.Append(", ")
                sbOutput.AppendFormat("{0} {1}", col.ActualType, col.Name)
            Next
            sbOutput.AppendFormat("){0}{{Query qry = new Query(TableSchema){0}", vbCrLf)
            first = True
            For Each col In PrimaryKeyColumns
                If first Then
                    sbOutput.AppendFormat(".Where(Columns.{0}, {0})", col.Name)
                    first = False
                Else
                    sbOutput.AppendFormat("{0}.AND(Columns.{1}, {1})", vbCrLf, col.Name)
                End If
            Next
            sbOutput.AppendFormat(";{0}using (DataReaderBase reader = qry.ExecuteReader(conn)){0}{{{0}if (reader.Read()){0}{{{0}{1} item = new {1}();{0}item.Read(reader);{0}return item;{0}}}{0}}}{0}return null;{0}}}{0}{0}", vbCrLf, className)

            sbOutput.AppendFormat("public static int Delete(ConnectorBase conn, ", vbCrLf)
            first = True
            For Each col In PrimaryKeyColumns
                If first Then first = False Else sbOutput.Append(", ")
                sbOutput.AppendFormat("{0} {1}", col.ActualType, col.Name)
            Next
            sbOutput.AppendFormat("){0}{{Query qry = new Query(TableSchema){0}", vbCrLf)
            first = True
            For Each col In PrimaryKeyColumns
                If first Then
                    sbOutput.AppendFormat(".Delete().Where(Columns.{0}, {0})", col.Name)
                    first = False
                Else
                    sbOutput.AppendFormat("{0}.AND(Columns.{1}, {1})", vbCrLf, col.Name)
                End If
            Next
            sbOutput.AppendFormat(";{0}return qry.Execute(conn);{0}}}{0}", vbCrLf)

        End If

        sbOutput.AppendFormat("#endregion{0}", vbCrLf)

        sbOutput.Append("}")

        SetClipboard(sbOutput.ToString())

    End Sub
    Dim clipboardData As String
    Public Sub SetClipboard(ByVal text As String)
        clipboardData = text
        Dim ClipBoardThread As Threading.Thread = New Threading.Thread(AddressOf _SetClipboard)
        With ClipBoardThread
            .ApartmentState = Threading.ApartmentState.STA
            .IsBackground = True
            .Start()
            .Join()
        End With
        ClipBoardThread = Nothing
    End Sub
    Private Sub _SetClipboard()
        System.Windows.Forms.Clipboard.SetDataObject(clipboardData, True)
    End Sub
End Module
