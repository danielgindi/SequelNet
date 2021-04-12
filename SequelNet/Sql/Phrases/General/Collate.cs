using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases
{
    public class Collate : IPhrase
    {
        public ValueWrapper Value;
        public string Collation;
        public SortDirection Direction = SortDirection.ASC;

        #region Constructors

        public Collate(ValueWrapper value, string collation, SortDirection direction = SortDirection.ASC)
        {
            this.Value = value;
            this.Collation = collation;
            this.Direction = direction;
        }

        public Collate(object value, ValueObjectType valueType, string collation, SortDirection direction = SortDirection.ASC)
            : this(ValueWrapper.Make(value, valueType), collation, direction)
        {
        }

        public Collate(string tableName, string columnName, string collation, SortDirection direction = SortDirection.ASC)
            : this(ValueWrapper.Column(tableName, columnName), collation, direction)
        {
        }

        public Collate(string columnName, string collation, SortDirection direction = SortDirection.ASC)
            : this(ValueWrapper.Column(columnName), collation, direction)
        {
        }

        public Collate(IPhrase phrase, string collation, SortDirection direction = SortDirection.ASC)
            : this(ValueWrapper.From(phrase), collation, direction)
        {
        }

        #endregion

        public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
        {
            conn.Language.BuildCollate(Value, Collation, Direction, sb, conn, relatedQuery);
        }
    }
}
