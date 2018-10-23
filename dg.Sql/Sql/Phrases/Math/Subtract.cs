using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class Subtract : IPhrase
    {
        public ValueWrapper Value1;
        public ValueWrapper Value2;

        #region Constructors

        public Subtract(
            string tableName1, string columnName1,
            string tableName2, string columnName2)
        {
            this.Value1 = new ValueWrapper(tableName1, columnName1);
            this.Value2 = new ValueWrapper(tableName2, columnName2);
        }

        public Subtract(
            string tableName1, string columnName1,
            object value2, ValueObjectType valueType2)
        {
            this.Value1 = new ValueWrapper(tableName1, columnName1);
            this.Value2 = new ValueWrapper(value2, valueType2);
        }

        public Subtract(
            object value1, ValueObjectType valueType1,
            string tableName2, string columnName2)
        {
            this.Value1 = new ValueWrapper(value1, valueType1);
            this.Value2 = new ValueWrapper(tableName2, columnName2);
        }

        public Subtract(
            object value1, ValueObjectType valueType1,
            object value2, ValueObjectType valueType2)
        {
            this.Value1 = new ValueWrapper(value1, valueType1);
            this.Value2 = new ValueWrapper(value2, valueType2);
        }

        public Subtract(string tableName1, string columnName1, Int32 value2)
        {
            this.Value1 = new ValueWrapper(tableName1, columnName1);
            this.Value2 = new ValueWrapper(value2, ValueObjectType.Value);
        }

        public Subtract(string tableName1, string columnName1, Int64 value2)
        {
            this.Value1 = new ValueWrapper(tableName1, columnName1);
            this.Value2 = new ValueWrapper(value2, ValueObjectType.Value);
        }

        public Subtract(string tableName1, string columnName1, decimal value2)
        {
            this.Value1 = new ValueWrapper(tableName1, columnName1);
            this.Value2 = new ValueWrapper(value2, ValueObjectType.Value);
        }

        public Subtract(string tableName1, string columnName1, double value2)
        {
            this.Value1 = new ValueWrapper(tableName1, columnName1);
            this.Value2 = new ValueWrapper(value2, ValueObjectType.Value);
        }

        public Subtract(string tableName1, string columnName1, float value2)
        {
            this.Value1 = new ValueWrapper(tableName1, columnName1);
            this.Value2 = new ValueWrapper(value2, ValueObjectType.Value);
        }

        public Subtract(string columnName1, Int32 value2)
        {
            this.Value1 = new ValueWrapper(columnName1);
            this.Value2 = new ValueWrapper(value2, ValueObjectType.Value);
        }

        public Subtract(string columnName1, Int64 value2)
        {
            this.Value1 = new ValueWrapper(columnName1);
            this.Value2 = new ValueWrapper(value2, ValueObjectType.Value);
        }

        public Subtract(string columnName1, decimal value2)
        {
            this.Value1 = new ValueWrapper(columnName1);
            this.Value2 = new ValueWrapper(value2, ValueObjectType.Value);
        }

        public Subtract(string columnName1, double value2)
        {
            this.Value1 = new ValueWrapper(columnName1);
            this.Value2 = new ValueWrapper(value2, ValueObjectType.Value);
        }

        public Subtract(string columnName1, float value2)
        {
            this.Value1 = new ValueWrapper(columnName1);
            this.Value2 = new ValueWrapper(value2, ValueObjectType.Value);
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret = @"";

            ret += Value1.Build(conn, relatedQuery);

            ret += @"-";

            ret += Value2.Build(conn, relatedQuery);

            return ret;
        }
    }
}
