using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases;

public class MD5 : IPhrase
{
    public ValueWrapper Value;
    public bool Binary = false;

    #region Constructors

    public MD5(object value, ValueObjectType valueType, bool binary = false)
    {
        this.Value = ValueWrapper.Make(value, valueType);
        this.Binary = binary;
    }

    public MD5(string tableName, string columnName, bool binary = false)
    {
        this.Value = ValueWrapper.Column(tableName, columnName);
        this.Binary = binary;
    }

    public MD5(string columnName, bool binary = false)
        : this(null, columnName, binary)
    {
    }

    public MD5(IPhrase phrase, bool binary = false)
        : this(phrase, ValueObjectType.Value, binary)
    {
    }

    public MD5(Where where, bool binary = false)
        : this(where, ValueObjectType.Value, binary)
    {
    }

    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        sb.Append(Binary 
            ? conn.Language.Md5Binary(Value.Build(conn, relatedQuery)) 
            : conn.Language.Md5Hex(Value.Build(conn, relatedQuery)));
    }
}
