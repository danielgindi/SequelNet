using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases;

public class Abs : IPhrase
{
    public ValueWrapper Value;
    public int DecimalPlaces;

    #region Constructors
    
    public Abs(object value, ValueObjectType valueType, int decimalPlaces = 0)
    {
        this.Value = ValueWrapper.Make(value, valueType);
        this.DecimalPlaces = decimalPlaces;
    }

    public Abs(string tableName, string columnName, int decimalPlaces = 0)
    {
        this.Value = ValueWrapper.Column(tableName, columnName);
        this.DecimalPlaces = decimalPlaces;
    }

    public Abs(string columnName, int decimalPlaces = 0)
        : this(null, columnName, decimalPlaces)
    {
    }

    public Abs(IPhrase phrase, int decimalPlaces = 0)
        : this(phrase, ValueObjectType.Value, decimalPlaces)
    {
    }

    public Abs(Where where)
        : this(where, ValueObjectType.Value)
    {
    }
    
    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        sb.Append("ABS(");

        sb.Append(Value.Build(conn, relatedQuery));

        if (DecimalPlaces != 0)
        {
            sb.Append(',');
            sb.Append(DecimalPlaces);
        }
        sb.Append(')');
    }
    }
}
