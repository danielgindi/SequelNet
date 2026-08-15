using SequelNet;

namespace Tests;

public class TableSchemaTests
{
    [Test]
    public void AddColumn_InfersDataTypeAndPreservesColumnMetadata()
    {
        var schema = new TableSchema("orders", null);

        schema.AddColumn("reference", typeof(string), DataType.Automatic, 32, "varchar(32)", 0, 0, false, false, false, "new", "utf8", "utf8_bin");

        var column = schema.Columns.Single();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(column.Name, Is.EqualTo("reference"));
            Assert.That(column.ActualDataType, Is.EqualTo(DataType.VarChar));
            Assert.That(column.DataTypeDef.MaxLength, Is.EqualTo(32));
            Assert.That(column.LiteralType, Is.EqualTo("varchar(32)"));
            Assert.That(column.Default, Is.EqualTo("new"));
            Assert.That(column.Charset, Is.EqualTo("utf8"));
            Assert.That(column.Collate, Is.EqualTo("utf8_bin"));
        }
    }

    [TestCase(typeof(string), 0, DataType.Text)]
    [TestCase(typeof(byte[]), 0, DataType.Blob)]
    [TestCase(typeof(byte[]), 16, DataType.Binary)]
    [TestCase(typeof(DateTimeOffset), 0, DataType.DateTimeOffset)]
    [TestCase(typeof(ulong), 0, DataType.UnsignedBigInt)]
    [TestCase(typeof(Geometry.Point), 0, DataType.Point)]
    public void Column_InfersAutomaticDataType(Type type, int maxLength, DataType expected)
    {
        var column = new TableSchema.Column { Type = type, MaxLength = maxLength };

        Assert.That(column.ActualDataType, Is.EqualTo(expected));
    }

    [Test]
    public void DataTypeDef_PrefersSridThenLengthThenPrecision()
    {
        var column = new TableSchema.Column
        {
            Type = typeof(Geometry.Point),
            MaxLength = 42,
            NumberPrecision = 12,
            NumberScale = 3,
            SRID = 4326
        };

        var withSrid = column.DataTypeDef;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(withSrid.Type, Is.EqualTo(DataType.Point));
            Assert.That(withSrid.SRID, Is.EqualTo(4326));
            Assert.That(withSrid.MaxLength, Is.EqualTo(0));
            Assert.That(withSrid.Precision, Is.EqualTo(0));
        }

        column.SRID = null;
        Assert.That(column.DataTypeDef.MaxLength, Is.EqualTo(42));

        column.MaxLength = 0;
        var withPrecision = column.DataTypeDef;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(withPrecision.Precision, Is.EqualTo(12));
            Assert.That(withPrecision.Scale, Is.EqualTo(3));
        }
    }

    [Test]
    public void Schema_AddsGeneratedNamesAndManagesTableOptions()
    {
        var schema = new TableSchema("orders", null);

        schema.AddIndex(null, TableSchema.ClusterMode.None, TableSchema.IndexMode.Unique, TableSchema.IndexType.BTREE,
            "tenant_id", 8, SortDirection.DESC, ValueWrapper.Literal("LOWER(code)"));
        schema.AddForeignKey(null, "customer_id", "customers", "id", TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Restrict);
        schema.SetTableOption("engine", "InnoDB");

        var index = schema.Indexes.Single();
        var foreignKey = schema.ForeignKeys.Single();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(index.Name, Is.EqualTo("IX_orders_tenant_id"));
            Assert.That(index.Columns[0].Length, Is.EqualTo(8));
            Assert.That(index.Columns[0].Sort, Is.EqualTo(SortDirection.DESC));
            Assert.That(index.Columns[1].Sort, Is.Null);
            Assert.That(foreignKey.Name, Is.EqualTo("FK_orders_customers_customer_id"));
            Assert.That(schema.GetTableOption("engine"), Is.EqualTo("InnoDB"));
            Assert.That(schema.RemoveTableOption("engine"), Is.True);
            Assert.That(schema.GetTableOption("engine"), Is.Null);
            Assert.That(schema.RemoveTableOption("engine"), Is.False);
        }
    }

    [Test]
    public void NamedLists_FindEntriesCaseInsensitively()
    {
        var schema = new TableSchema("orders", null);
        schema.AddColumn(new TableSchema.Column { Name = "OrderId", Type = typeof(int) });
        schema.AddIndex("IX_Orders_Id", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, "OrderId");
        schema.AddForeignKey("FK_Orders_Customer", "customer_id", "customers", "id", TableSchema.ForeignKeyReference.None, TableSchema.ForeignKeyReference.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(schema.Columns.Find("orderid")!.Name, Is.EqualTo("OrderId"));
            Assert.That(schema.Indexes.Find("ix_orders_id")!.Name, Is.EqualTo("IX_Orders_Id"));
            Assert.That(schema.ForeignKeys.Find("fk_orders_customer")!.Name, Is.EqualTo("FK_Orders_Customer"));
        }
    }
}
