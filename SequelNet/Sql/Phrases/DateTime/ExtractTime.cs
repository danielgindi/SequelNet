using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases;

public class ExtractTime : IPhrase
{
    public ValueWrapper Value;

    #region Constructors

    public ExtractTime(object value, ValueObjectType valueType)
    {
        this.Value = ValueWrapper.Make(value, valueType);
    }

    public ExtractTime(string tableName, string columnName)
    {
        this.Value = ValueWrapper.Column(tableName, columnName);
    }

    public ExtractTime(string columnName)
        : this(null, columnName)
    {
    }

    public ExtractTime(IPhrase phrase)
        : this(phrase, ValueObjectType.Value)
    {
    }

    public ExtractTime(ValueWrapper value)
    {
        this.Value = value;
    }

    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        sb.Append(conn.Language.TimePartOfDateTime(Value.Build(conn, relatedQuery)));
    }
}
