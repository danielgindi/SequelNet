using System;
using System.Collections.Generic;
using System.Text;
using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class Add : IPhrase
    {
        public List<ValueWrapper> Values = new List<ValueWrapper>();

        #region Constructors

        public Add(params ValueWrapper[] values)
        {
            this.Values.AddRange(values);
        }

        public Add(
            string tableName1, string columnName1,
            string tableName2, string columnName2)
            : this(
                  ValueWrapper.Column(tableName1, columnName1),
                  ValueWrapper.Column(tableName2, columnName2))
        {
        }

        public Add(
            string tableName1, string columnName1,
            object value2, ValueObjectType valueType2)
            : this(
                  ValueWrapper.Column(tableName1, columnName1),
                  ValueWrapper.Make(value2, valueType2))
        {
        }

        public Add(
            object value1, ValueObjectType valueType1,
            string tableName2, string columnName2)
            : this(
                  ValueWrapper.Make(value1, valueType1),
                  ValueWrapper.Column(tableName2, columnName2))
        {
        }

        public Add(
            object value1, ValueObjectType valueType1,
            object value2, ValueObjectType valueType2)
            : this(
                  ValueWrapper.Make(value1, valueType1),
                  ValueWrapper.Make(value2, valueType2))
        {
        }

        public Add(string tableName1, string columnName1, Int32 value2)
            : this(
                  ValueWrapper.Column(tableName1, columnName1),
                  ValueWrapper.From(value2))
        {
        }

        public Add(string tableName1, string columnName1, Int64 value2)
            : this(
                  ValueWrapper.Column(tableName1, columnName1),
                  ValueWrapper.From(value2))
        {
        }

        public Add(string tableName1, string columnName1, decimal value2)
            : this(
                  ValueWrapper.Column(tableName1, columnName1),
                  ValueWrapper.From(value2))
        {
        }

        public Add(string tableName1, string columnName1, double value2)
            : this(
                  ValueWrapper.Column(tableName1, columnName1),
                  ValueWrapper.From(value2))
        {
        }

        public Add(string tableName1, string columnName1, float value2)
            : this(
                  ValueWrapper.Column(tableName1, columnName1),
                  ValueWrapper.From(value2))
        {
        }

        public Add(string columnName1, Int32 value2)
            : this(
                  ValueWrapper.Column(columnName1),
                  ValueWrapper.From(value2))
        {
        }

        public Add(string columnName1, Int64 value2)
            : this(
                  ValueWrapper.Column(columnName1),
                  ValueWrapper.From(value2))
        {
        }

        public Add(string columnName1, decimal value2)
            : this(
                  ValueWrapper.Column(columnName1),
                  ValueWrapper.From(value2))
        {
        }

        public Add(string columnName1, double value2)
            : this(
                  ValueWrapper.Column(columnName1),
                  ValueWrapper.From(value2))
        {
        }

        public Add(string columnName1, float value2)
            : this(
                  ValueWrapper.Column(columnName1),
                  ValueWrapper.From(value2))
        {
        }

        #endregion

        public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
        {
            bool first = true;

            foreach (var value in Values)
            {
                if (first) first = false;
                else sb.Append(" + ");

                sb.Append(value.Build(conn, relatedQuery));
            }
        }
    }
}
