using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases;

public class IfNull : IPhrase
{
    public ValueWrapper Value1;
    public ValueWrapper Value2;

    #region Constructors

    public IfNull(
        ValueWrapper first,
        ValueWrapper second)
    {
        this.Value1 = first;
        this.Value2 = second;
    }

    public IfNull(
        string firstTableName, string firstColumnName,
        string secondTableName, string secondColumnName)
    {
        this.Value1 = ValueWrapper.Column(firstTableName, firstColumnName);
        this.Value2 = ValueWrapper.Column(secondTableName, secondColumnName);
    }

    public IfNull(
         object firstValue, ValueObjectType firstValueType,
         object secondValue, ValueObjectType secondValueType)
    {
        this.Value1 = ValueWrapper.Make(firstValue, firstValueType);
        this.Value2 = ValueWrapper.Make(secondValue, secondValueType);
    }

    public IfNull(
         string firstTableName, string firstColumnName,
         object secondValue, ValueObjectType secondValueType)
    {
        this.Value1 = ValueWrapper.Column(firstTableName, firstColumnName);
        this.Value2 = ValueWrapper.Make(secondValue, secondValueType);
    }

    public IfNull(
         string firstTableName, string firstColumnName,
         ValueWrapper second)
    {
        this.Value1 = ValueWrapper.Column(firstTableName, firstColumnName);
        this.Value2 = second;
    }

    public IfNull(
         object firstValue, ValueObjectType firstValueType,
         string secondTableName, string secondColumnName)
    {
        this.Value1 = ValueWrapper.Make(firstValue, firstValueType);
        this.Value2 = ValueWrapper.Column(secondTableName, secondColumnName);
    }

    public IfNull(
         ValueWrapper first,
         string secondTableName, string secondColumnName)
    {
        this.Value1 = first;
        this.Value2 = ValueWrapper.Column(secondTableName, secondColumnName);
    }

    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        if (conn.TYPE == ConnectorBase.SqlServiceType.MYSQL || conn.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
            sb.Append("IFNULL(");
        else sb.Append("ISNULL(");

        sb.Append(Value1.Build(conn, relatedQuery));

        sb.Append(", ");

        sb.Append(Value2.Build(conn, relatedQuery));

        sb.Append(")");
    }
}
