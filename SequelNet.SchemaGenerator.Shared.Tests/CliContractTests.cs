using Xunit;

using SequelNet.SchemaGenerator.Cli;

namespace SequelNet.SchemaGenerator.Shared.Tests;

public class CliContractTests
{
    [Fact]
    public void Generate_Returns_Code_And_Warnings_In_A_Structured_Response()
    {
        var response = GeneratorCommand.Generate(new GenerateRequest
        {
            Script = """
                MyTable
                my_table
                Id: PRIMARY KEY; INT;
                @Index: NAME(IX_Missing); [DoesNotExist]
                """,
        });

        Assert.True(response.Success);
        Assert.Contains("public partial class MyTable", response.Code);
        Assert.Equal("MyTable", response.RecordName);
        Assert.Equal(new[] { "Column DoesNotExist not found in index IX_Missing" }, response.Warnings);
        Assert.Empty(response.Errors);
    }

    [Fact]
    public void Generate_Returns_A_Structured_Error_For_An_Invalid_Macro()
    {
        var response = GeneratorCommand.Generate(new GenerateRequest { Script = "OnlyAClassName" });

        Assert.False(response.Success);
        Assert.Null(response.Code);
        Assert.NotEmpty(response.Errors);
    }

    [Fact]
    public void Generate_Ignores_A_Leading_Unicode_Byte_Order_Mark()
    {
        var response = GeneratorCommand.Generate(new GenerateRequest
        {
            Script = "\uFEFF\nMyTable\nmy_table\nId: PRIMARY KEY; INT;",
        });

        Assert.True(response.Success);
        Assert.True(response.Code!.IndexOf('\uFEFF') < 0, $"Unexpected BOM at index {response.Code!.IndexOf('\uFEFF')}.");
    }

    [Fact]
    public void Generate_Uses_The_Canonical_RecordName_When_Comment_Asterisks_Are_Present()
    {
        var response = GeneratorCommand.Generate(new GenerateRequest
        {
            Script = "* Customer\n* customers\n* Id: PRIMARY KEY; INT;",
        });

        Assert.True(response.Success);
        Assert.Equal("Customer", response.RecordName);
    }
}
