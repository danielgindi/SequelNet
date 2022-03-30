using System.Collections.Generic;
using System.Text;
using SequelNet.Connector;

namespace SequelNet
{
    public class WhereList : List<Where>
    {
        public void BuildCommand(StringBuilder outputBuilder, Where.BuildContext context)
        {
            context = context ?? new Where.BuildContext();

            bool ownsConn = context.Conn == null;
            if (ownsConn)
            {
                context.Conn = ConnectorBase.Create();
            }

            try
            {
                bool isFirst = true;
                bool isForJoinList = this is JoinColumnPair;
                foreach (Where where in this)
                {
                    where.BuildCommand(outputBuilder, isFirst, context);
                    if (isFirst) isFirst = false;
                }
            }
            finally
            {
                if (ownsConn)
                {
                    context.Conn.Dispose();
                }
            }
        }

        public WhereList ClearWhere()
        {
            this.Clear();
            return this;
        }

        public WhereList Where(object thisObject, ValueObjectType thisObjectType, WhereComparison comparison, object thatObject, ValueObjectType thatObjectType)
        {
            this.Clear();
            this.Add(new Where(WhereCondition.AND, thisObject, thisObjectType, comparison, thatObject, thatObjectType));
            return this;
        }

        public WhereList Where(string columnName, object columnValue)
        {
            this.Clear();
            this.Add(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, WhereComparison.EqualsTo, columnValue, ValueObjectType.Value));
            return this;
        }

        public WhereList Where(string columnName, WhereComparison comparison, object columnValue)
        {
            this.Clear();
            this.Add(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, comparison, columnValue, ValueObjectType.Value));
            return this;
        }

        public WhereList Where(IPhrase phrase)
        {
            this.Clear();
            this.Add(new Where(WhereCondition.AND, phrase, ValueObjectType.Value, WhereComparison.None, null, ValueObjectType.Literal));
            return this;
        }

        public WhereList Where(WhereList whereList)
        {
            this.Clear();
            this.Add(new Where(WhereCondition.AND, whereList));
            return this;
        }

        public WhereList Where(string tableName, string columnName, WhereComparison comparison, object columnValue)
        {
            this.Clear();
            this.Add(new Where(WhereCondition.AND, tableName, columnName, comparison, columnValue));
            return this;
        }

        public WhereList Where(string tableName, string columnName, WhereComparison comparison, string otherTableName, string otherColumnName)
        {
            this.Clear();
            this.Add(new Where(WhereCondition.AND, tableName, columnName, comparison, otherTableName, otherColumnName));
            return this;
        }

        public WhereList Where(
            object aValue, ValueObjectType aType,
            object betweenValue, ValueObjectType betweenType,
            object andValue, ValueObjectType andType)
        {
            this.Add(new Where(WhereCondition.AND, aValue, aType, betweenValue, betweenType, andValue, andType));
            return this;
        }

        public WhereList Where(
            string aSchema, object aValue, ValueObjectType aType,
            string betweenSchema, object betweenValue, ValueObjectType betweenType,
            string andSchema, object andValue, ValueObjectType andType)
        {
            this.Add(new Where(WhereCondition.AND,
                aSchema, aValue, aType,
                betweenSchema, betweenValue, betweenType,
                andSchema, andValue, andType));
            return this;
        }

        public WhereList AND(object thisObject, ValueObjectType thisObjectType, WhereComparison comparison, object thatObject, ValueObjectType thatObjectType)
        {
            this.Add(new Where(WhereCondition.AND, thisObject, thisObjectType, comparison, thatObject, thatObjectType));
            return this;
        }

        public WhereList AND(string columnName, object columnValue)
        {
            this.Add(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, WhereComparison.EqualsTo, columnValue, ValueObjectType.Value));
            return this;
        }

        public WhereList AND(string columnName, WhereComparison comparison, object columnValue)
        {
            this.Add(new Where(WhereCondition.AND, columnName, ValueObjectType.ColumnName, comparison, columnValue, ValueObjectType.Value));
            return this;
        }

        public WhereList AND(IPhrase phrase)
        {
            this.Add(new Where(WhereCondition.AND, phrase));
            return this;
        }

        public WhereList AND(IPhrase phrase, WhereComparison comparison, object value)
        {
            this.Add(new Where(WhereCondition.AND, phrase, comparison, value));
            return this;
        }

        public WhereList AND(IPhrase phrase, WhereComparison comparison, object value, ValueObjectType valueType)
        {
            this.Add(new Where(WhereCondition.AND, phrase, comparison, value, valueType));
            return this;
        }

        public WhereList AND(IPhrase phrase, WhereComparison comparison, string tableName, string columnName)
        {
            var w = new Where(WhereCondition.AND, phrase, comparison, tableName, columnName);
            w.SecondTableName = tableName;
            this.Add(w);
            return this;
        }

        public WhereList AND(WhereList whereList)
        {
            this.Add(new Where(WhereCondition.AND, whereList));
            return this;
        }

        public WhereList AND(string tableName, string columnName, WhereComparison comparison, object columnValue)
        {
            this.Add(new Where(WhereCondition.AND, tableName, columnName, comparison, columnValue));
            return this;
        }

        public WhereList AND(string tableName, string columnName, WhereComparison comparison, string otherTableName, string otherColumnName)
        {
            this.Add(new Where(WhereCondition.AND, tableName, columnName, comparison, otherTableName, otherColumnName));
            return this;
        }

        public WhereList AND(
            object aValue, ValueObjectType aType,
            object betweenValue, ValueObjectType betweenType,
            object andValue, ValueObjectType andType)
        {
            this.Add(new Where(WhereCondition.AND, aValue, aType, betweenValue, betweenType, andValue, andType));
            return this;
        }

        public WhereList AND(
            string aSchema, object aValue, ValueObjectType aType,
            string betweenSchema, object betweenValue, ValueObjectType betweenType,
            string andSchema, object andValue, ValueObjectType andType)
        {
            this.Add(new Where(WhereCondition.AND,
                aSchema, aValue, aType,
                betweenSchema, betweenValue, betweenType,
                andSchema, andValue, andType));
            return this;
        }

        public WhereList OR(object thisObject, ValueObjectType thisObjectType, WhereComparison comparison, object thatObject, ValueObjectType thatObjectType)
        {
            this.Add(new Where(WhereCondition.OR, thisObject, thisObjectType, comparison, thatObject, thatObjectType));
            return this;
        }

        public WhereList OR(string columnName, object columnValue)
        {
            this.Add(new Where(WhereCondition.OR, columnName, ValueObjectType.ColumnName, WhereComparison.EqualsTo, columnValue, ValueObjectType.Value));
            return this;
        }

        public WhereList OR(string columnName, WhereComparison comparison, object columnValue)
        {
            this.Add(new Where(WhereCondition.OR, columnName, ValueObjectType.ColumnName, comparison, columnValue, ValueObjectType.Value));
            return this;
        }

        public WhereList OR(IPhrase phrase)
        {
            this.Add(new Where(WhereCondition.OR, phrase));
            return this;
        }

        public WhereList OR(IPhrase phrase, WhereComparison comparison, object value)
        {
            this.Add(new Where(WhereCondition.OR, phrase, comparison, value));
            return this;
        }

        public WhereList OR(IPhrase phrase, WhereComparison comparison, object value, ValueObjectType valueType)
        {
            this.Add(new Where(WhereCondition.OR, phrase, comparison, value, valueType));
            return this;
        }

        public WhereList OR(IPhrase phrase, WhereComparison comparison, string tableName, string columnName)
        {
            var w = new Where(WhereCondition.OR, phrase, comparison, tableName, columnName);
            w.SecondTableName = tableName;
            this.Add(w);
            return this;
        }

        public WhereList OR(WhereList whereList)
        {
            this.Add(new Where(WhereCondition.OR, whereList));
            return this;
        }

        public WhereList OR(string tableName, string columnName, WhereComparison comparison, object columnValue)
        {
            this.Add(new Where(WhereCondition.OR, tableName, columnName, comparison, columnValue));
            return this;
        }

        public WhereList OR(string tableName, string columnName, WhereComparison comparison, string otherTableName, string otherColumnName)
        {
            this.Add(new Where(WhereCondition.OR, tableName, columnName, comparison, otherTableName, otherColumnName));
            return this;
        }

        public WhereList OR(
            object aValue, ValueObjectType aType,
            object betweenValue, ValueObjectType betweenType,
            object andValue, ValueObjectType andType)
        {
            this.Add(new Where(WhereCondition.OR, aValue, aType, betweenValue, betweenType, andValue, andType));
            return this;
        }

        public WhereList OR(
            string aSchema, object aValue, ValueObjectType aType,
            string betweenSchema, object betweenValue, ValueObjectType betweenType,
            string andSchema, object andValue, ValueObjectType andType)
        {
            this.Add(new Where(WhereCondition.OR, 
                aSchema, aValue, aType, 
                betweenSchema, betweenValue, betweenType,
                andSchema, andValue, andType));
            return this;
        }

        public WhereList AddFromList(WhereList whereList)
        {
            foreach (Where where in whereList)
            {
                this.Add(where);
            }
            return this;
        }
    }
}
