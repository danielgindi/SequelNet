using SequelNet.Connector;
using System.Text;

#nullable enable

namespace SequelNet.Phrases;

public class ConvertUtcToTz : IPhrase
{
    public ValueWrapper Value;
    public ValueWrapper Timezone;
    
    public ConvertUtcToTz(ValueWrapper value, ValueWrapper timezone)
    {
        this.Value = value;
        this.Timezone = timezone;
    }
    
    public ConvertUtcToTz(ValueWrapper value, string timezone)
    {
        this.Value = value;
        this.Timezone = ValueWrapper.From(timezone);
    }

    public void Build(StringBuilder sb, ConnectorBase conn, Query? relatedQuery = null)
    {
        conn.Language.BuildConvertUtcToTz(Value, Timezone, sb, conn, relatedQuery);
    }
}