using SequelNet;

namespace Tests;

public class JsonPathCoverageTests
{
    [TestCase("simple", "$.simple")]
    [TestCase("snake_case", "$.snake_case")]
    [TestCase("abc123", "$.abc123")]
    [TestCase("éclair", "$.éclair")]
    [TestCase("with space", "$.\"with space\"")]
    [TestCase("1leading", "$.\"1leading\"")]
    [TestCase("hyphen-name", "$.\"hyphen-name\"")]
    [TestCase("with.dot", "$.\"with.dot\"")]
    [TestCase("with[brackets]", "$.\"with[brackets]\"")]
    [TestCase("$", "$.\"$\"")]
    [TestCase("", "$.\"\"")]
    [TestCase("quote\"name", "$.\"quote\\\"name\"")]
    public void ConstructedPath_EscapesPropertiesOnlyWhenNeeded(string property, string expectedPath)
    {
        var path = new JsonPathExpression(JsonPathExpression.Part.Root(), JsonPathExpression.Part.Property(property));

        Assert.That(path.GetPath().Value, Is.EqualTo(expectedPath));
    }

    [TestCase(0, "$[0]")]
    [TestCase(1, "$[1]")]
    [TestCase(17, "$[17]")]
    [TestCase(999, "$[999]")]
    [TestCase(-1, "$[\"-1\"]")]
    public void ConstructedPath_FormatsArrayIndexes(int index, string expectedPath)
    {
        var path = new JsonPathExpression(JsonPathExpression.Part.Root(), JsonPathExpression.Part.IndexAt(index));

        Assert.That(path.GetPath().Value, Is.EqualTo(expectedPath));
    }

    [TestCase("$", 1)]
    [TestCase("$.name", 2)]
    [TestCase("$.store.book[0].title", 5)]
    [TestCase("$.\"store name\"[4]", 3)]
    [TestCase("$[0][1][2]", 4)]
    public void GetPathParts_TracksTheExpectedNumberOfSegments(string path, int expectedPartCount)
    {
        Assert.That(JsonPathExpression.GetPathParts(path), Has.Count.EqualTo(expectedPartCount));
    }
}
