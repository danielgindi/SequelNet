using SequelNet;

namespace Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void SimplePathTests()
    {
        var path1 = new JsonPathExpression("$.store.book[0].title");
        var path2 = new JsonPathExpression("$.store.book[0].\"title\"");
        var path3 = new JsonPathExpression(
            JsonPathExpression.Part.Root(),
            JsonPathExpression.Part.Property("store"),
            JsonPathExpression.Part.Property("book"),
            JsonPathExpression.Part.IndexAt(0),
            JsonPathExpression.Part.Property("title")
        );

        // All paths should have the same number of parts
        Assert.That(path1.GetParts().Count, Is.EqualTo(path2.GetParts().Count));
        Assert.That(path1.GetParts().Count, Is.EqualTo(path3.GetParts().Count));

        // All parts should be the same
        for (int i = 0; i < path1.GetParts().Count; i++)
        {
            Assert.That(path1.GetParts()[i], Is.EqualTo(path2.GetParts()[i]));
            Assert.That(path1.GetParts()[i], Is.EqualTo(path3.GetParts()[i]));
        }

        // Path 2 has quotes, so it should be different from path 1
        Assert.That((string)path1.GetPath().Value!, Is.Not.EqualTo((string)path2.GetPath().Value!));

        // Path 1 and 3 are the same
        Assert.That((string)path1.GetPath().Value!, Is.EqualTo((string)path3.GetPath().Value!));

        Assert.Pass();
    }
}