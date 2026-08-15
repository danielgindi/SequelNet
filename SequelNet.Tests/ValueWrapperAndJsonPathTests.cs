using SequelNet;

namespace Tests;

public class ValueWrapperAndJsonPathTests
{
    [Test]
    public void ValueWrapper_FactoriesAndNullableCastsRetainValueKind()
    {
        double? missing = null;
        var nullValue = (ValueWrapper)missing;
        var column = ValueWrapper.Column("orders", "total");
        var literal = ValueWrapper.Literal("CURRENT_TIMESTAMP");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(nullValue.Type, Is.EqualTo(ValueObjectType.Value));
            Assert.That(nullValue.Value, Is.Null);
            Assert.That(column.Type, Is.EqualTo(ValueObjectType.ColumnName));
            Assert.That(column.TableName, Is.EqualTo("orders"));
            Assert.That(column.Value, Is.EqualTo("total"));
            Assert.That(literal.Type, Is.EqualTo(ValueObjectType.Literal));
            Assert.That(literal.Value, Is.EqualTo("CURRENT_TIMESTAMP"));
        }
    }

    [Test]
    public void ValueWrapper_EqualityIncludesTableNameTypeAndValue()
    {
        var first = ValueWrapper.Column("orders", "id");
        var same = ValueWrapper.Column("orders", "id");
        var otherTable = ValueWrapper.Column("customers", "id");
        var literal = ValueWrapper.Literal("id");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(first, Is.EqualTo(same));
            Assert.That(first.GetHashCode(), Is.EqualTo(same.GetHashCode()));
            Assert.That(first, Is.Not.EqualTo(otherTable));
            Assert.That(first, Is.Not.EqualTo(literal));
        }
    }

    [Test]
    public void JsonPathExpression_CompilesSpecialPropertiesAndArrayIndexes()
    {
        var expression = new JsonPathExpression(
            JsonPathExpression.Part.Root(),
            JsonPathExpression.Part.Property("store name"),
            JsonPathExpression.Part.IndexAt(4),
            JsonPathExpression.Part.Property("title"));

        var path = expression.GetPath();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(path.Type, Is.EqualTo(ValueObjectType.Value));
            Assert.That(path.Value, Is.EqualTo("$.\"store name\"[4].title"));
            Assert.That(expression.IsEmpty(), Is.False);
        }
    }

    [TestCase("$")]
    [TestCase("")]
    public void JsonPathExpression_RecognizesEmptyCompiledPath(string path)
    {
        Assert.That(new JsonPathExpression(path).IsEmpty(), Is.True);
    }

    [Test]
    public void JsonPathExpression_ParsesQuotedEscapedProperties()
    {
        var parts = JsonPathExpression.GetPathParts("$.\"property\\\\name\"[7]");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(parts, Has.Count.EqualTo(3));
            Assert.That(parts[1].Value.Value, Is.EqualTo("property\\name"));
            Assert.That(parts[1].Indexed, Is.False);
            Assert.That(parts[2].Value.Value, Is.EqualTo(7));
            Assert.That(parts[2].Indexed, Is.True);
        }
    }
}
