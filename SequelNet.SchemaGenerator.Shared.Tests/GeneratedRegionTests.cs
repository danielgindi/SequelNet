using Xunit;

namespace SequelNet.SchemaGenerator.Shared.Tests;

public class GeneratedRegionTests
{
    [Fact]
    public void CreateOrUpdateAfterMacro_Inserts_A_Marked_Region_And_Preserves_The_Macro()
    {
        const string macro = "/*\nMyRecord\nmy_record\nId: PRIMARY KEY; INT;\n*/";
        var change = GeneratedRegion.CreateOrUpdateAfterMacro(macro, macro.Length, "public partial class MyRecord {}\n");

        Assert.Equal(macro.Length, change.Start);
        Assert.Equal(0, change.Length);
        Assert.Equal("\n\n// <sequelnet-generated>\npublic partial class MyRecord {}\n// </sequelnet-generated>", change.Text);
    }

    [Fact]
    public void CreateOrUpdateAfterMacro_Replaces_Only_The_Existing_Region()
    {
        const string macro = "/* macro */";
        var document = macro + "\n// <sequelnet-generated>\nold\n// </sequelnet-generated>\npublic partial class Custom {}";
        var change = GeneratedRegion.CreateOrUpdateAfterMacro(document, macro.Length, "new");

        var updated = document.Substring(0, change.Start) + change.Text + document.Substring(change.Start + change.Length);

        Assert.Equal(macro + "\n// <sequelnet-generated>\nnew\n// </sequelnet-generated>\npublic partial class Custom {}", updated);
    }

    [Fact]
    public void CreateOrUpdateAfterMacro_Does_Not_Claim_A_Region_After_Other_Source_Text()
    {
        const string macro = "/* macro */";
        var document = macro + "\npublic partial class Custom {}\n// <sequelnet-generated>\nother\n// </sequelnet-generated>";
        var change = GeneratedRegion.CreateOrUpdateAfterMacro(document, macro.Length, "new");

        Assert.Equal(macro.Length, change.Start);
        Assert.Equal(0, change.Length);
    }

    [Fact]
    public void CreateOrUpdateAfterMacro_Replaces_A_Nested_Duplicate_Region_As_One_Region()
    {
        const string macro = "/* macro */";
        var document = macro + "\n// <sequelnet-generated>\nfirst\n// <sequelnet-generated>\nsecond\n// </sequelnet-generated>\nthird\n// </sequelnet-generated>\npublic partial class Custom {}";
        var change = GeneratedRegion.CreateOrUpdateAfterMacro(document, macro.Length, "new");

        var updated = document.Substring(0, change.Start) + change.Text + document.Substring(change.Start + change.Length);

        Assert.Equal(macro + "\n// <sequelnet-generated>\nnew\n// </sequelnet-generated>\npublic partial class Custom {}", updated);
    }

    [Fact]
    public void CreateOrUpdateAfterMacro_Rejects_An_Unterminated_Adjacent_Region()
    {
        const string macro = "/* macro */";
        var document = macro + "\n// <sequelnet-generated>\nold";

        Assert.Throws<InvalidOperationException>(() => GeneratedRegion.CreateOrUpdateAfterMacro(document, macro.Length, "new"));
    }

    [Fact]
    public void CreateOrUpdateAfterMacro_Replaces_A_Region_After_Documentation_And_NonMacro_Comments()
    {
        const string macro = "/* macro */";
        var document = macro + "\n/// Customer record documentation\n/* Keep this note. */\n// <sequelnet-generated>\nold\n// </sequelnet-generated>";
        var change = GeneratedRegion.CreateOrUpdateAfterMacro(document, macro.Length, "new");

        var updated = document.Substring(0, change.Start) + change.Text + document.Substring(change.Start + change.Length);

        Assert.Equal(macro + "\n/// Customer record documentation\n/* Keep this note. */\n// <sequelnet-generated>\nnew\n// </sequelnet-generated>", updated);
    }

    [Fact]
    public void CreateOrUpdateAfterMacro_Prefers_A_Named_Region_And_Migrates_An_Adjacent_Generic_Region()
    {
        const string recordName = "Customer";
        const string macro = "/* macro */";
        var namedMarker = GeneratedRegion.BuildStartMarker(recordName);
        var namedElsewhere = namedMarker + "\nold\n// </sequelnet-generated>\n" + macro;

        var namedChange = GeneratedRegion.CreateOrUpdateAfterMacro(namedElsewhere, namedElsewhere.Length, "new", recordName);
        var namedUpdated = namedElsewhere.Substring(0, namedChange.Start) + namedChange.Text + namedElsewhere.Substring(namedChange.Start + namedChange.Length);

        Assert.Equal(namedMarker + "\nnew\n// </sequelnet-generated>\n" + macro, namedUpdated);

        var legacyDocument = macro + "\n// <sequelnet-generated>\nold\n// </sequelnet-generated>";
        var legacyChange = GeneratedRegion.CreateOrUpdateAfterMacro(legacyDocument, macro.Length, "new", recordName);
        var legacyUpdated = legacyDocument.Substring(0, legacyChange.Start) + legacyChange.Text + legacyDocument.Substring(legacyChange.Start + legacyChange.Length);

        Assert.Equal(macro + "\n" + namedMarker + "\nnew\n// </sequelnet-generated>", legacyUpdated);
    }

    [Fact]
    public void CreateOrUpdateAfterMacro_Repairs_A_Corrupted_Adjacent_NamedMarker()
    {
        const string macro = "/* macro */";
        const string corruptMarker = "// <sequelnet-generated record=\"// <sequelnet-generated record=\"LegacyImportRun\">";
        var document = macro + "\n" + corruptMarker + "\nold\n// </sequelnet-generated>";

        var change = GeneratedRegion.CreateOrUpdateAfterMacro(document, macro.Length, "new", "LegacyImportRun");
        var updated = document.Substring(0, change.Start) + change.Text + document.Substring(change.Start + change.Length);

        Assert.Equal(macro + "\n// <sequelnet-generated record=\"LegacyImportRun\">\nnew\n// </sequelnet-generated>", updated);
    }
}