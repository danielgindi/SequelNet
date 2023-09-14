using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases;

public class Substring : IPhrase
{
    public ValueWrapper Value;
    public ValueWrapper From;
    public ValueWrapper? Length;

    #region Constructors

    public Substring(object value, ValueObjectType valueType, int from, int? length = null)
    {
        this.Value = ValueWrapper.Make(value, valueType);
        this.From = ValueWrapper.From(from);
        this.Length = length != null ? ValueWrapper.From(length.Value) : (ValueWrapper?)null;
    }

    public Substring(object value, ValueObjectType valueType, ValueWrapper from, ValueWrapper? length = null)
    {
        this.Value = ValueWrapper.Make(value, valueType);
        this.From = from;
        this.Length = length;
    }

    public Substring(ValueWrapper value, ValueWrapper from, ValueWrapper? length = null)
    {
        this.Value = value;
        this.From = from;
        this.Length = length;
    }

    public Substring(string tableName, string columnName, int from, int? length = null)
    {
        this.Value = ValueWrapper.Column(tableName, columnName);
        this.From = ValueWrapper.From(from);
        this.Length = length != null ? ValueWrapper.From(length.Value) : (ValueWrapper?)null;
    }

    public Substring(string columnName, int from, int? length = null)
        : this(null, columnName, from, length)
    {
    }

    public Substring(IPhrase phrase, int from, int? length = null)
        : this(phrase, ValueObjectType.Value, from, length)
    {
    }

    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        sb.Append(conn.Language.BuildSubstring(conn, Value, From, Length, relatedQuery));
    }
}
