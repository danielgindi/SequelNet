﻿using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class Min : IPhrase
    {
        public ValueWrapper Value;

        #region Constructors

        public Min()
        {
            this.Value = new ValueWrapper("*", ValueObjectType.Literal);
        }

        public Min(string tableName, string columnName)
        {
            this.Value = new ValueWrapper(tableName, columnName);
        }

        public Min(string columnName)
            : this(null, columnName)
        {
        }

        public Min(object value, ValueObjectType valueType)
        {
            this.Value = new ValueWrapper(value, valueType);
        }

        public Min(IPhrase phrase)
            : this(phrase, ValueObjectType.Value)
        {
        }

        public Min(Where where)
            : this(where, ValueObjectType.Value)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret;

            ret = @"MIN(";

            ret += Value.Build(conn, relatedQuery);

            ret += ")";

            return ret;
        }
    }
}