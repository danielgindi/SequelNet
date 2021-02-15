using System;
using System.Text;
using SequelNet.Connector;

namespace SequelNet.Phrases
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
            this.Value1 = ValueWrapper.Column(tableName1, columnName1);
            this.Value2 = ValueWrapper.Column(tableName2, columnName2);
        }

        public Subtract(
            string tableName1, string columnName1,
            object value2, ValueObjectType valueType2)
        {
            this.Value1 = ValueWrapper.Column(tableName1, columnName1);
            this.Value2 = ValueWrapper.Make(value2, valueType2);
        }

        public Subtract(
            object value1, ValueObjectType valueType1,
            string tableName2, string columnName2)
        {
            this.Value1 = ValueWrapper.Make(value1, valueType1);
            this.Value2 = ValueWrapper.Column(tableName2, columnName2);
        }

        public Subtract(
            object value1, ValueObjectType valueType1,
            object value2, ValueObjectType valueType2)
        {
            this.Value1 = ValueWrapper.Make(value1, valueType1);
            this.Value2 = ValueWrapper.Make(value2, valueType2);
        }

        public Subtract(string tableName1, string columnName1, Int32 value2)
        {
            this.Value1 = ValueWrapper.Column(tableName1, columnName1);
            this.Value2 = ValueWrapper.From(value2);
        }

        public Subtract(string tableName1, string columnName1, Int64 value2)
        {
            this.Value1 = ValueWrapper.Column(tableName1, columnName1);
            this.Value2 = ValueWrapper.From(value2);
        }

        public Subtract(string tableName1, string columnName1, decimal value2)
        {
            this.Value1 = ValueWrapper.Column(tableName1, columnName1);
            this.Value2 = ValueWrapper.From(value2);
        }

        public Subtract(string tableName1, string columnName1, double value2)
        {
            this.Value1 = ValueWrapper.Column(tableName1, columnName1);
            this.Value2 = ValueWrapper.From(value2);
        }

        public Subtract(string tableName1, string columnName1, float value2)
        {
            this.Value1 = ValueWrapper.Column(tableName1, columnName1);
            this.Value2 = ValueWrapper.From(value2);
        }

        public Subtract(string columnName1, Int32 value2)
        {
            this.Value1 = ValueWrapper.Column(columnName1);
            this.Value2 = ValueWrapper.From(value2);
        }

        public Subtract(string columnName1, Int64 value2)
        {
            this.Value1 = ValueWrapper.Column(columnName1);
            this.Value2 = ValueWrapper.From(value2);
        }

        public Subtract(string columnName1, decimal value2)
        {
            this.Value1 = ValueWrapper.Column(columnName1);
            this.Value2 = ValueWrapper.From(value2);
        }

        public Subtract(string columnName1, double value2)
        {
            this.Value1 = ValueWrapper.Column(columnName1);
            this.Value2 = ValueWrapper.From(value2);
        }

        public Subtract(string columnName1, float value2)
        {
            this.Value1 = ValueWrapper.Column(columnName1);
            this.Value2 = ValueWrapper.From(value2);
        }

        #endregion

        public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
        {
            sb.Append(Value1.Build(conn, relatedQuery));
            sb.Append(@"-");
            sb.Append(Value2.Build(conn, relatedQuery));
        }
    }
}
