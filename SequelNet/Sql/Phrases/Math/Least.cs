using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases;

public class Least : IPhrase
{
    public ValueWrapper Value1;
    public ValueWrapper Value2;

    #region Constructors
    
    public Least(
        object value1, ValueObjectType valueType1,
        object value2, ValueObjectType valueType2
        )
    {
        this.Value1 = ValueWrapper.Make(value1, valueType1);
        this.Value2 = ValueWrapper.Make(value2, valueType2);
    }

    public Least(
        string tableName1, string column1,
        string tableName2, string column2
        )
    {
        this.Value1 = ValueWrapper.Column(tableName1, column1);
        this.Value2 = ValueWrapper.Column(tableName2, column2);
    }

    public Least(
        string tableName1, string column1,
        object value2, ValueObjectType valueType2
        )
    {
        this.Value1 = ValueWrapper.Column(tableName1, column1);
        this.Value2 = ValueWrapper.Make(value2, valueType2);
    }

    public Least(
        string tableName1, string column1,
        object value2
        )
    {
        this.Value1 = ValueWrapper.Column(tableName1, column1);
        this.Value2 = ValueWrapper.Make(value2, ValueObjectType.Value);
    }

    public Least(
        object value1, ValueObjectType valueType1,
        string tableName2, string column2
        )
    {
        this.Value1 = ValueWrapper.Make(value1, valueType1);
        this.Value2 = ValueWrapper.Column(tableName2, column2);
    }

    public Least(
        object value1,
        string tableName2, string column2
        )
    {
        this.Value1 = ValueWrapper.Make(value1, ValueObjectType.Value);
        this.Value2 = ValueWrapper.Column(tableName2, column2);
    }

    public Least(
        object value1,
        object value2
        )
    {
        this.Value1 = ValueWrapper.Make(value1, ValueObjectType.Value);
        this.Value2 = ValueWrapper.Make(value2, ValueObjectType.Value);
    }

    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        sb.Append("LEAST(");
        
        sb.Append(Value1.Build(conn, relatedQuery));

        sb.Append(", ");
        
        sb.Append(Value2.Build(conn, relatedQuery));

        sb.Append(')');
    }
    }
}
