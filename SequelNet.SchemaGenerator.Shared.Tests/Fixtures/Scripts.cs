namespace SequelNet.SchemaGenerator.Shared.Tests.Fixtures;

internal static class Scripts
{
    public static string Minimal(string className = "MyTable", string schema = "my_table") => $@"
{className}
{schema}
Id: PRIMARY KEY; INT;
";

    public static string WithColumns(string className = "MyTable", string schema = "my_table") => $@"
{className}
{schema}
Id: PRIMARY KEY; INT;
Name: STRING(50);
";
}