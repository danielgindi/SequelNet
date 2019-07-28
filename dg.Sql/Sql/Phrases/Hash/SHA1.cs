﻿using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class SHA1 : IPhrase
    {
        public ValueWrapper Value;
        public bool Binary = false;

        #region Constructors

        public SHA1(object value, ValueObjectType valueType, bool binary = false)
        {
            this.Value = new ValueWrapper(value, valueType);
            this.Binary = binary;
        }

        public SHA1(string tableName, string columnName, bool binary = false)
        {
            this.Value = new ValueWrapper(tableName, columnName);
            this.Binary = binary;
        }

        public SHA1(string columnName, bool binary = false)
            : this(null, columnName, binary)
        {
        }

        public SHA1(IPhrase phrase, bool binary = false)
            : this(phrase, ValueObjectType.Value, binary)
        {
        }

        public SHA1(Where where, bool binary = false)
            : this(where, ValueObjectType.Value, binary)
        {
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret = "";

            ret += Value.Build(conn, relatedQuery);

            return Binary ? conn.Language.func_SHA1_Binary(ret) : conn.Language.func_SHA1_Hex(ret);
        }
    }
}
