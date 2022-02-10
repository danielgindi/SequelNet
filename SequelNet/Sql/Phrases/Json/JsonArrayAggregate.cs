using System;
using System.Text;
using SequelNet.Connector;

namespace SequelNet.Phrases
{
    /// <summary>
    /// Aggregates the selected value into a json array
    /// </summary>
    public class JsonArrayAggregate : IPhrase
    {
        public ValueWrapper Value;
        public bool IsBinary = false;

        #region Constructors
        public JsonArrayAggregate(ValueWrapper value, bool binary = false)
        {
            this.Value = value;
            this.IsBinary = binary;
        }

        public JsonArrayAggregate(object value, ValueObjectType valueType, bool binary = false)
            : this(ValueWrapper.Make(value, valueType), binary)
        {
        }

        public JsonArrayAggregate(string tableName, string columnName, bool binary = false)
            : this(ValueWrapper.Column(tableName, columnName), binary)
        {
        }

        public JsonArrayAggregate(string columnName, bool binary = false)
            : this(null, columnName, binary)
        {
        }

        public JsonArrayAggregate(IPhrase phrase, bool binary = false)
            : this(phrase, ValueObjectType.Value, binary)
        {
        }

        public JsonArrayAggregate(Where where, bool binary = false)
            : this(where, ValueObjectType.Value, binary)
        {
        }

        #endregion

        public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
        {
            conn.Language.BuildJsonArrayAggregate(Value, IsBinary, sb, conn, relatedQuery);
        }
    }
}
