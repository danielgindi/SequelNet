using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases;

public class Cast : IPhrase
{
    public ValueWrapper Value;
    public DataTypeDef TypeDef;

    #region Constructors

    public Cast(ValueWrapper value, DataTypeDef typeDef)
    {
        this.Value = value;
        this.TypeDef = typeDef;
    }

    public Cast(object value, ValueObjectType valueType, DataTypeDef typeDef)
        : this(ValueWrapper.Make(value, valueType), typeDef)
    {
    }

    public Cast(string tableName, string columnName, DataTypeDef typeDef)
        : this(ValueWrapper.Column(tableName, columnName), typeDef)
    {
    }

    public Cast(string columnName, DataTypeDef typeDef)
        : this(ValueWrapper.Column(columnName), typeDef)
    {
    }

    public Cast(IPhrase phrase, DataTypeDef typeDef)
        : this(ValueWrapper.From(phrase), typeDef)
    {
    }

    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        conn.Language.BuildCast(Value, TypeDef, sb, conn, relatedQuery);
    }
}
