using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases
{
    public class DateTimeFormat : IPhrase
    {
        public ValueWrapper Value;
        public FormatOptions Format;

        #region Constructors

        public DateTimeFormat(object value, ValueObjectType valueType, FormatOptions format)
        {
            this.Value = ValueWrapper.Make(value, valueType);
            this.Format = format;
        }

        public DateTimeFormat(string tableName, string columnName, FormatOptions format)
        {
            this.Value = ValueWrapper.Column(tableName, columnName);
            this.Format = format;
        }

        public DateTimeFormat(string columnName, FormatOptions format)
            : this(null, columnName, format)
        {
        }

        public DateTimeFormat(IPhrase phrase, FormatOptions format)
            : this(phrase, ValueObjectType.Value, format)
        {
        }

        public DateTimeFormat(ValueWrapper value, FormatOptions format)
        {
            this.Value = value;
            this.Format = format;
        }

        #endregion

        public enum FormatOptions
        {
            Date,
            DateTime,
            DateTimeFFF,
            DateTimeZ,
            DateTimeFFFZ,
            Time,
            TimeFFF,
        }

        public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
        {
            sb.Append(conn.Language.DateTimeFormat(Value.Build(conn, relatedQuery), Format));
        }
    }
}
