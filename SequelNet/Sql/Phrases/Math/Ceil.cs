using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases;

public class Ceil : IPhrase
{
    public ValueWrapper Value;

    #region Constructors

    public Ceil(object value, ValueObjectType valueType)
    {
        this.Value = ValueWrapper.Make(value, valueType);
    }

    public Ceil(string tableName, string columnName)
    {
        this.Value = ValueWrapper.Column(tableName, columnName);
    }

    public Ceil(string columnName)
        : this(null, columnName)
    {
    }

    public Ceil(IPhrase phrase)
        : this(phrase, ValueObjectType.Value)
    {
    }

    public Ceil(Where where)
        : this(where, ValueObjectType.Value)
    {
    }
    
    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        sb.Append("CEIL(");
        sb.Append(Value.Build(conn, relatedQuery));
        sb.Append(')');
    }
    }
}
