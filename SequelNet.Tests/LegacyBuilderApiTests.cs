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
            Assert.That(obsolete!.Message, Is.EqualTo(GetReplacementMessage(method)));
        }
    }

    private static string GetReplacementMessage(MethodInfo method)
    {
        var parameters = method.GetParameters();
        if (method.Name == "AddSelect" &&
            parameters.Length == 2 &&
            parameters.All(parameter => parameter.ParameterType == typeof(string)))
            return "Use SelectAs(...) instead.";

        return method.Name switch
        {
            "AddSelectLiteral" => "Use SelectLiteral(...) instead.",
            "AddSelectValue" => "Use SelectValue(...) instead.",
            _ => "Use Select(...) instead."
        };
    }
}
