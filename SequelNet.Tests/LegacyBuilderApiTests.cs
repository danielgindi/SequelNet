using System.Reflection;
using SequelNet;

namespace Tests;

public class LegacyBuilderApiTests
{
    [Test]
    public void AddSelectAliases_AreMarkedObsoleteWithTheirReplacement()
    {
        var addSelectMethods = typeof(Query).GetMethods()
            .Where(method => method.Name.StartsWith("AddSelect", StringComparison.Ordinal))
            .ToArray();

        Assert.That(addSelectMethods, Has.Length.EqualTo(9));

        foreach (var method in addSelectMethods)
        {
            var obsolete = method.GetCustomAttribute<ObsoleteAttribute>();

            Assert.That(obsolete, Is.Not.Null, method.ToString());
            Assert.That(obsolete!.Message, Is.EqualTo(GetReplacementMessage(method.Name)));
        }
    }

    private static string GetReplacementMessage(string methodName)
    {
        return methodName switch
        {
            "AddSelectLiteral" => "Use SelectLiteral(...) instead.",
            "AddSelectValue" => "Use SelectValue(...) instead.",
            _ => "Use Select(...) instead."
        };
    }
}
