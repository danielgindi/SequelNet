using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases;

public class ST_X : IPhrase
{
    public ValueWrapper Value;

    #region Constructors
    
    public ST_X(IPhrase phrase)
    {
        this.Value = ValueWrapper.From(phrase);
    }

    public ST_X(string tableName, string column)
    {
        this.Value = ValueWrapper.Column(tableName, column);
    }

    public ST_X(ValueWrapper value)
    {
        this.Value = value;
    }

    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        sb.Append(conn.Language.ST_X(Value.Build(conn, relatedQuery)));
    }
}
