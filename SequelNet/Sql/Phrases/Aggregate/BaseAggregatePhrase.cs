using System;
using SequelNet.Connector;

namespace SequelNet
{
    public abstract class BaseAggregatePhrase : IPhrase
    {
        public ValueWrapper Value;
        #region Constructors

        public BaseAggregatePhrase()
        {
            this.Value = new ValueWrapper("*", ValueObjectType.Literal);
        }

        public BaseAggregatePhrase(string tableName, string columnName)
        {
            this.Value = new ValueWrapper(tableName, columnName);
        }

        public BaseAggregatePhrase(string columnName)
            : this(null, columnName)
        {
        }

        public BaseAggregatePhrase(object value, ValueObjectType valueType)
        {
            this.Value = new ValueWrapper(value, valueType);
        }

        public BaseAggregatePhrase(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public BaseAggregatePhrase(Where where)
            : this(where, ValueObjectType.Value)
        {
        }

        public BaseAggregatePhrase(WhereList where)
            : this(where, ValueObjectType.Value)
        {
        }

        #endregion

        public virtual string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            throw new NotImplementedException();
        }
    }
}
