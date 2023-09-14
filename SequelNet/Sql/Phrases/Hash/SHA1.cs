using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases;

public class SHA1 : IPhrase
{
    public ValueWrapper Value;
    public bool Binary = false;

    #region Constructors

    public SHA1(object value, ValueObjectType valueType, bool binary = false)
    {
        this.Value = ValueWrapper.Make(value, valueType);
        this.Binary = binary;
    }

    public SHA1(string tableName, string columnName, bool binary = false)
    {
        this.Value = ValueWrapper.Column(tableName, columnName);
        this.Binary = binary;
    }

    public SHA1(string columnName, bool binary = false)
        : this(null, columnName, binary)
    {
    }

    public SHA1(IPhrase phrase, bool binary = false)
        : this(phrase, ValueObjectType.Value, binary)
    {
    }

    public SHA1(Where where, bool binary = false)
        : this(where, ValueObjectType.Value, binary)
    {
    }

    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        sb.Append(Binary 
            ? conn.Language.Sha1Binary(Value.Build(conn, relatedQuery))
            : conn.Language.Sha1Hex(Value.Build(conn, relatedQuery)));
    }
}
