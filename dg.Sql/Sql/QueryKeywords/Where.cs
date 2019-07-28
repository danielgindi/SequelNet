using System.Text;
using System.Collections;
using dg.Sql.Connector;
using System.Runtime.CompilerServices;

namespace dg.Sql
{
    public class Where
    {
        private WhereComparison _Comparison = WhereComparison.None;
        private WhereCondition _Condition = WhereCondition.AND;
        private object _First = null;
        private ValueObjectType _FirstType = ValueObjectType.Literal;
        private object _Second = null;
        private ValueObjectType _SecondType = ValueObjectType.Literal;
        private object _Third = null;
        private ValueObjectType _ThirdType = ValueObjectType.Literal;
        private string _FirstTableName = null;
        private string _SecondTableName = null;
        private string _ThirdTableName = null;

        public WhereComparison Comparison
        {
            get { return _Comparison; }
            set { _Comparison = value; }
        }

        public WhereCondition Condition
        {
            get { return _Condition; }
            set { _Condition = value; }
        }

        public object First
        {
            get { return _First; }
            set { _First = value; }
        }

        public ValueObjectType FirstType
        {
            get { return _FirstType; }
            set { _FirstType = value; }
        }

        public object Second
        {
            get { return _Second; }
            set { _Second = value; }
        }

        public ValueObjectType SecondType
        {
            get { return _SecondType; }
            set { _SecondType = value; }
        }

        public object Third
        {
            get { return _Third; }
            set { _Third = value; }
        }

        public ValueObjectType ThirdType
        {
            get { return _ThirdType; }
            set { _ThirdType = value; }
        }

        public string FirstTableName
        {
            get { return _FirstTableName; }
            set { _FirstTableName = value; }
        }

        public string SecondTableName
        {
            get { return _SecondTableName; }
            set { _SecondTableName = value; }
        }

        public string ThirdTableName
        {
            get { return _ThirdTableName; }
            set { _ThirdTableName = value; }
        }

        public Where()
        {
        }

        public Where(WhereList whereList)
        {
            First = whereList;
        }

        public Where(object thisLiteral, WhereComparison comparedBy, object thatLiteral)
        {
            Comparison = comparedBy;
            First = thisLiteral;
            Second = thatLiteral;
        }

        public Where(object thisObject, ValueObjectType thisObjectType, WhereComparison comparedBy, object thatObject, ValueObjectType thatObjectType)
        {
            Comparison = comparedBy;
            First = thisObject;
            FirstType = thisObjectType;
            Second = thatObject;
            SecondType = thatObjectType;
        }

        public Where(object thisLiteral, object betweenThisLiteral, object andThatLiteral)
        {
            Comparison = WhereComparison.Between;
            First = thisLiteral;
            Second = betweenThisLiteral;
            Third = andThatLiteral;
        }

        public Where(object thisObject, ValueObjectType thisObjectType,
            object betweenThisObject, ValueObjectType betweenThisObjectType,
            object andThatObject, ValueObjectType andThatObjectType)
        {
            Comparison = WhereComparison.Between;
            First = thisObject;
            FirstType = thisObjectType;
            Second = betweenThisObject;
            SecondType = betweenThisObjectType;
            Third = andThatObject;
            ThirdType = andThatObjectType;
        }

        public Where(string tableName, string columnName,
            WhereComparison comparedBy, object value)
        {
            Comparison = comparedBy;
            FirstTableName = tableName;
            First = columnName;
            FirstType = ValueObjectType.ColumnName;
            Second = value;
            SecondType = ValueObjectType.Value;
        }

        public Where(string tableName, string columnName,
            WhereComparison comparedBy, string thatTableName, string thatColumnName)
        {
            Comparison = comparedBy;
            FirstTableName = tableName;
            First = columnName;
            FirstType = ValueObjectType.ColumnName;
            SecondTableName = thatTableName;
            Second = thatColumnName;
            SecondType = ValueObjectType.ColumnName;
        }

        public Where(IPhrase phrase)
        {
            First = phrase;
            FirstType = ValueObjectType.Value;
            Comparison = WhereComparison.None;
        }

        public Where(IPhrase phrase, WhereComparison comparison, object value)
        {
            First = phrase;
            FirstType = ValueObjectType.Value;
            Comparison = comparison;
            Second = value;
            SecondType = ValueObjectType.Value;
        }

        public Where(IPhrase phrase, WhereComparison comparison, object value, ValueObjectType valueType)
        {
            First = phrase;
            FirstType = ValueObjectType.Value;
            Comparison = comparison;
            Second = value;
            SecondType = valueType;
        }

        public Where(IPhrase phrase, WhereComparison comparison, string tableName, string columnName)
        {
            First = phrase;
            FirstType = ValueObjectType.Value;
            Comparison = comparison;
            SecondTableName = tableName;
            Second = columnName;
            SecondType = ValueObjectType.ColumnName;
        }

        public Where(WhereCondition condition, WhereList whereList)
        {
            Condition = condition;
            First = whereList;
        }

        public Where(WhereCondition condition, object thisLiteral, WhereComparison comparedBy, object thatLiteral)
        {
            Condition = condition;
            Comparison = comparedBy;
            First = thisLiteral;
            Second = thatLiteral;
        }

        public Where(WhereCondition condition, object thisObject, ValueObjectType thisObjectType, WhereComparison comparedBy, object thatObject, ValueObjectType thatObjectType)
        {
            Condition = condition;
            Comparison = comparedBy;
            First = thisObject;
            FirstType = thisObjectType;
            Second = thatObject;
            SecondType = thatObjectType;
        }

        public Where(WhereCondition condition, object thisLiteral, object betweenThisLiteral, object andThatLiteral)
        {
            Condition = condition;
            Comparison = WhereComparison.Between;
            First = thisLiteral;
            Second = betweenThisLiteral;
            Third = andThatLiteral;
        }

        public Where(WhereCondition condition,
            object thisObject, ValueObjectType thisObjectType,
            object betweenThisObject, ValueObjectType betweenThisObjectType,
            object andThatObject, ValueObjectType andThatObjectType)
        {
            Condition = condition;
            Comparison = WhereComparison.Between;
            First = thisObject;
            FirstType = thisObjectType;
            Second = betweenThisObject;
            SecondType = betweenThisObjectType;
            Third = andThatObject;
            ThirdType = andThatObjectType;
        }

        public Where(WhereCondition condition,
            string tableName, string columnName,
            WhereComparison comparedBy, object value)
        {
            Condition = condition;
            Comparison = comparedBy;
            FirstTableName = tableName;
            First = columnName;
            FirstType = ValueObjectType.ColumnName;
            Second = value;
            SecondType = ValueObjectType.Value;
        }

        public Where(WhereCondition condition,
            string tableName, string columnName,
            WhereComparison comparedBy, string thatTableName, string thatColumnName)
        {
            Condition = condition;
            Comparison = comparedBy;
            FirstTableName = tableName;
            First = columnName;
            FirstType = ValueObjectType.ColumnName;
            SecondTableName = thatTableName;
            Second = thatColumnName;
            SecondType = ValueObjectType.ColumnName;
        }
        
        public Where(WhereCondition condition, IPhrase phrase)
        {
            Condition = condition;
            First = phrase;
            FirstType = ValueObjectType.Value;
            Comparison = WhereComparison.None;
        }
        
        public Where(WhereCondition condition, IPhrase phrase, WhereComparison comparison, object value, ValueObjectType valueType = ValueObjectType.Value)
        {
            Condition = condition;
            First = phrase;
            FirstType = ValueObjectType.Value;
            Comparison = comparison;
            Second = value;
            SecondType = valueType;
        }

        public Where(WhereCondition condition, IPhrase phrase, WhereComparison comparison, string tableName, string columnName)
        {
            Condition = condition;
            First = phrase;
            FirstType = ValueObjectType.Value;
            Comparison = comparison;
            SecondTableName = tableName;
            Second = columnName;
            SecondType = ValueObjectType.ColumnName;
        }

        public Where(WhereCondition condition, Query query, WhereComparison comparison, object value, ValueObjectType valueType = ValueObjectType.Value)
        {
            Condition = condition;
            First = query;
            FirstType = ValueObjectType.Value;
            Comparison = comparison;
            Second = value;
            SecondType = valueType;
        }

        public Where(WhereCondition condition, Query query, WhereComparison comparison, string tableName, string columnName)
        {
            Condition = condition;
            First = query;
            FirstType = ValueObjectType.Value;
            Comparison = comparison;
            SecondTableName = tableName;
            Second = columnName;
            SecondType = ValueObjectType.ColumnName;
        }

        #region Builders
        
        public void BuildCommand(StringBuilder outputBuilder, bool isFirst, BuildContext context)
        {
            if (!isFirst)
            {
                switch (Condition)
                {
                    case WhereCondition.AND:
                        outputBuilder.Append(@" AND ");
                        break;
                    case WhereCondition.OR:
                        outputBuilder.Append(@" OR ");
                        break;
                }
            }

            // The list is empty?
            if (Comparison == WhereComparison.None && 
                (First is WhereList && ((WhereList)First).Count == 0)
                )
            {
                outputBuilder.Append(@"1"); // dump a dummy TRUE condition to fill the blank
                return;
            }

            if (First is WhereList)
            {
                outputBuilder.Append('(');
                ((WhereList)First).BuildCommand(outputBuilder, context);
                outputBuilder.Append(')');
            }
            else
            {
                if (Comparison == WhereComparison.NullSafeEqualsTo ||
                    Comparison == WhereComparison.NullSafeNotEqualsTo)
                {
                    context.Conn.BuildNullSafeEqualsTo(
                        this,
                        Comparison == WhereComparison.NullSafeNotEqualsTo,
                        outputBuilder, context);
                }
                else
                {
                    BuildSingleValueFirst(outputBuilder, context);

                    if (Comparison != WhereComparison.None)
                    {
                        switch (Comparison)
                        {
                            case WhereComparison.EqualsTo:
                                if (First == null || Second == null) outputBuilder.Append(@" IS ");
                                else outputBuilder.Append(@" = ");
                                break;
                            case WhereComparison.NotEqualsTo:
                                if (First == null || Second == null) outputBuilder.Append(@" IS NOT ");
                                else outputBuilder.Append(@" <> ");
                                break;
                            case WhereComparison.GreaterThan:
                                outputBuilder.Append(@" > ");
                                break;
                            case WhereComparison.GreaterThanOrEqual:
                                outputBuilder.Append(@" >= ");
                                break;
                            case WhereComparison.LessThan:
                                outputBuilder.Append(@" < ");
                                break;
                            case WhereComparison.LessThanOrEqual:
                                outputBuilder.Append(@" <= ");
                                break;
                            case WhereComparison.Is:
                                outputBuilder.Append(@" IS ");
                                break;
                            case WhereComparison.IsNot:
                                outputBuilder.Append(@" IS NOT ");
                                break;
                            case WhereComparison.Like:
                                outputBuilder.Append(@" LIKE ");
                                break;
                            case WhereComparison.Between:
                                outputBuilder.Append(@" BETWEEN ");
                                break;
                            case WhereComparison.In:
                                outputBuilder.Append(@" IN ");
                                break;
                            case WhereComparison.NotIn:
                                outputBuilder.Append(@" NOT IN ");
                                break;
                        }

                        BuildSingleValueSecond(outputBuilder, context);

                        if (Comparison == WhereComparison.Between)
                        {
                            outputBuilder.Append(@" AND ");

                            BuildSingleValue(
                                outputBuilder,
                                ThirdTableName, Third, ThirdType,
                                FirstTableName, First, FirstType,
                                context);
                        }
                        else if (Comparison == WhereComparison.Like)
                        {
                            outputBuilder.Append(' ');
                            outputBuilder.Append(context.Conn.LikeEscapingStatement);
                            outputBuilder.Append(' ');
                        }
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BuildSingleValueFirst(
            StringBuilder outputBuilder, BuildContext context)
        {
            BuildSingleValue(
                outputBuilder,
                FirstTableName, First, FirstType,
                SecondTableName, Second, SecondType,
                context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BuildSingleValueSecond(
            StringBuilder outputBuilder, BuildContext context)
        {
            BuildSingleValue(
                outputBuilder,
                SecondTableName, Second, SecondType,
                FirstTableName, First, FirstType,
                context);
        }

        private static void BuildSingleValue(
            StringBuilder outputBuilder,
            string firstTableName, object value, ValueObjectType valueType,
            string otherTableName, object otherValue, ValueObjectType otherType,
            BuildContext context)
        {
            if (valueType == ValueObjectType.Value)
            {
                if (value is Query)
                {
                    outputBuilder.Append('(');
                    outputBuilder.Append(((Query)value).BuildCommand(context.Conn));
                    outputBuilder.Append(')');
                }
                else if (value is WhereList)
                {
                    outputBuilder.Append('(');
                    ((WhereList)value).BuildCommand(outputBuilder, context);
                    outputBuilder.Append(')');
                }
                else if (value is ICollection)
                {
                    ICollection collIn = value as ICollection;
                    StringBuilder sbIn = new StringBuilder();
                    sbIn.Append('(');
                    bool first = true;

                    TableSchema schema = null;
                    if (object.ReferenceEquals(otherTableName, JoinColumnPair.RIGHT_TABLE_PLACEHOLDER_ID))
                    {
                        schema = context.RightTableSchema;
                    }
                    else
                    {
                        if (context.RelatedQuery != null)
                        {
                            if (otherTableName == null || !context.RelatedQuery.TableAliasMap.TryGetValue(otherTableName, out schema))
                            {
                                schema = context.RelatedQuery.Schema;
                            }
                        }
                    }

                    foreach (object objIn in collIn)
                    {
                        if (first) first = false;
                        else sbIn.Append(',');

                        if (schema != null && otherValue is string)
                        {
                            sbIn.Append(Query.PrepareColumnValue(schema.Columns.Find((string)otherValue), objIn, context.Conn, context.RelatedQuery));
                        }
                        else
                        {
                            sbIn.Append(context.Conn.PrepareValue(objIn, context.RelatedQuery));
                        }
                    }

                    if (first)
                    {
                        sbIn.Append("NULL"); // Avoid exceptions, create a NULL list, where the condition will always return FALSE
                    }

                    sbIn.Append(')');
                    outputBuilder.Append(sbIn.ToString());
                }
                else if (otherType == ValueObjectType.ColumnName)
                {
                    TableSchema schema = null;
                    if (object.ReferenceEquals(otherTableName, JoinColumnPair.RIGHT_TABLE_PLACEHOLDER_ID))
                    {
                        schema = context.RightTableSchema;
                    }
                    else
                    {
                        if (context.RelatedQuery != null)
                        {
                            if (otherTableName == null || !context.RelatedQuery.TableAliasMap.TryGetValue(otherTableName, out schema))
                            {
                                schema = context.RelatedQuery.Schema;
                            }
                        }
                    }

                    if (schema != null && otherValue is string)
                    {
                        // Try to match value type to the other value type
                        outputBuilder.Append(Query.PrepareColumnValue(schema.Columns.Find((string)otherValue), value, context.Conn, context.RelatedQuery));
                    }
                    else
                    {
                        // Format it according to generic rules
                        outputBuilder.Append(context.Conn.PrepareValue(value, context.RelatedQuery));
                    }
                }
                else
                {
                    outputBuilder.Append(context.Conn.PrepareValue(value, context.RelatedQuery));
                }
            }
            else if (valueType == ValueObjectType.ColumnName)
            {
                if (firstTableName != null)
                {
                    if (object.ReferenceEquals(firstTableName, JoinColumnPair.RIGHT_TABLE_PLACEHOLDER_ID))
                    {
                        outputBuilder.Append(context.Conn.WrapFieldName(context.RightTableName));
                    }
                    else
                    {
                        outputBuilder.Append(context.Conn.WrapFieldName(firstTableName));
                    }
                    outputBuilder.Append('.');
                }
                outputBuilder.Append(context.Conn.WrapFieldName((string)value));
            }
            else
            {
                outputBuilder.Append(value == null ? @"NULL" : value);
            }
        }

        #endregion

        #region Chainable
        
        public WhereList AND(object thisObject, ValueObjectType thisObjectType, WhereComparison comparison, object thatObject, ValueObjectType thatObjectType)
        {
            var wl = new WhereList();
            wl.Add(this);
            return wl.AND(thisObject, thisObjectType, comparison, thatObject, thatObjectType);
        }

        public WhereList AND(string columnName, object columnValue)
        {
            var wl = new WhereList();
            wl.Add(this);
            return wl.AND(columnName, columnValue);
        }

        public WhereList AND(string columnName, WhereComparison comparison, object columnValue)
        {
            var wl = new WhereList();
            wl.Add(this);
            return wl.AND(columnName, comparison, columnValue);
        }

        public WhereList AND(string literalExpression)
        {
            var wl = new WhereList();
            wl.Add(this);
            return wl.AND(literalExpression);
        }

        public WhereList AND(IPhrase phrase)
        {
            var wl = new WhereList();
            wl.Add(this);
            return wl.AND(phrase);
        }

        public WhereList AND(IPhrase phrase, WhereComparison comparison, object value)
        {
            var wl = new WhereList();
            wl.Add(this);
            return wl.AND(phrase, comparison, value);
        }

        public WhereList AND(IPhrase phrase, WhereComparison comparison, object value, ValueObjectType valueType)
        {
            var wl = new WhereList();
            wl.Add(this);
            return wl.AND(phrase, comparison, value, valueType);
        }

        public WhereList AND(IPhrase phrase, WhereComparison comparison, string tableName, string columnName)
        {
            var wl = new WhereList();
            wl.Add(this);
            return wl.AND(phrase, comparison, tableName, columnName);
        }

        public WhereList AND(WhereList whereList)
        {
            var wl = new WhereList();
            wl.Add(this);
            return wl.AND(whereList);
        }

        public WhereList AND(string tableName, string columnName, WhereComparison comparison, object columnValue)
        {
            var wl = new WhereList();
            wl.Add(this);
            return wl.AND(tableName, columnName, comparison, columnValue);
        }

        public WhereList AND(string tableName, string columnName, WhereComparison comparison, string otherTableName, string otherColumnName)
        {
            var wl = new WhereList();
            wl.Add(this);
            return wl.AND(tableName, columnName, comparison, otherTableName, otherColumnName);
        }

        public WhereList AND(string tableName, string columnName, object betweenValue, object andValue)
        {
            var wl = new WhereList();
            wl.Add(this);
            return wl.AND(tableName, columnName, betweenValue, andValue);
        }

        public WhereList AND(string columnName, object betweenValue, object andValue)
        {
            var wl = new WhereList();
            wl.Add(this);
            return wl.AND(columnName, betweenValue, andValue);
        }

        public WhereList OR(object thisObject, ValueObjectType thisObjectType, WhereComparison comparison, object thatObject, ValueObjectType thatObjectType)
        {
            var wl = new WhereList();
            wl.Add(this);
            return wl.OR(thisObject, thisObjectType, comparison, thatObject, thatObjectType);
        }

        public WhereList OR(string columnName, object columnValue)
        {
            var wl = new WhereList();
            wl.Add(this);
            return wl.OR(columnName, columnValue);
        }

        public WhereList OR(string columnName, WhereComparison comparison, object columnValue)
        {
            var wl = new WhereList();
            wl.Add(this);
            return wl.OR(columnName, comparison, columnValue);
        }

        public WhereList OR(string literalExpression)
        {
            var wl = new WhereList();
            wl.Add(this);
            return wl.OR(literalExpression);
        }

        public WhereList OR(IPhrase phrase)
        {
            var wl = new WhereList();
            wl.Add(this);
            return wl.OR(phrase);
        }

        public WhereList OR(IPhrase phrase, WhereComparison comparison, object value)
        {
            var wl = new WhereList();
            wl.Add(this);
            return wl.OR(phrase, comparison, value);
        }

        public WhereList OR(IPhrase phrase, WhereComparison comparison, object value, ValueObjectType valueType)
        {
            var wl = new WhereList();
            wl.Add(this);
            return wl.OR(phrase, comparison, value, valueType);
        }

        public WhereList OR(IPhrase phrase, WhereComparison comparison, string tableName, string columnName)
        {
            var wl = new WhereList();
            wl.Add(this);
            return wl.OR(phrase, comparison, tableName, columnName);
        }

        public WhereList OR(WhereList whereList)
        {
            var wl = new WhereList();
            wl.Add(this);
            return wl.OR(whereList);
        }

        public WhereList OR(string tableName, string columnName, WhereComparison comparison, object columnValue)
        {
            var wl = new WhereList();
            wl.Add(this);
            return wl.OR(tableName, columnName, comparison, columnValue);
        }

        public WhereList OR(string tableName, string columnName, WhereComparison comparison, string otherTableName, string otherColumnName)
        {
            var wl = new WhereList();
            wl.Add(this);
            return wl.OR(tableName, columnName, comparison, otherTableName, otherColumnName);
        }

        public WhereList OR(string tableName, string columnName, object betweenValue, object andValue)
        {
            var wl = new WhereList();
            wl.Add(this);
            return wl.OR(tableName, columnName, betweenValue, andValue);
        }

        public WhereList OR(string columnName, object betweenValue, object andValue)
        {
            var wl = new WhereList();
            wl.Add(this);
            return wl.OR(columnName, betweenValue, andValue);
        }

        #endregion

        public class BuildContext
        {
            public ConnectorBase Conn;
            internal Query RelatedQuery;
            internal TableSchema RightTableSchema;
            internal string RightTableName;
        }
    }
}
