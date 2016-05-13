using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using dg.Sql.Connector;

namespace dg.Sql
{
    public class Where
    {
        private WhereComparision _Comparison = WhereComparision.None;
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

        public WhereComparision Comparision
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

        public Where(object thisLiteral, WhereComparision comparedBy, object thatLiteral)
        {
            Comparision = comparedBy;
            First = thisLiteral;
            Second = thatLiteral;
        }

        public Where(object thisObject, ValueObjectType thisObjectType, WhereComparision comparedBy, object thatObject, ValueObjectType thatObjectType)
        {
            Comparision = comparedBy;
            First = thisObject;
            FirstType = thisObjectType;
            Second = thatObject;
            SecondType = thatObjectType;
        }

        public Where(object thisLiteral, object betweenThisLiteral, object andThatLiteral)
        {
            Comparision = WhereComparision.Between;
            First = thisLiteral;
            Second = betweenThisLiteral;
            Third = andThatLiteral;
        }

        public Where(object thisObject, ValueObjectType thisObjectType,
            object betweenThisObject, ValueObjectType betweenThisObjectType,
            object andThatObject, ValueObjectType andThatObjectType)
        {
            Comparision = WhereComparision.Between;
            First = thisObject;
            FirstType = thisObjectType;
            Second = betweenThisObject;
            SecondType = betweenThisObjectType;
            Third = andThatObject;
            ThirdType = andThatObjectType;
        }

        public Where(string tableName, string columnName,
            WhereComparision comparedBy, object value)
        {
            Comparision = comparedBy;
            FirstTableName = tableName;
            First = columnName;
            FirstType = ValueObjectType.ColumnName;
            Second = value;
            SecondType = ValueObjectType.Value;
        }

        public Where(string tableName, string columnName,
            WhereComparision comparedBy, string thatTableName, string thatColumnName)
        {
            Comparision = comparedBy;
            FirstTableName = tableName;
            First = columnName;
            FirstType = ValueObjectType.ColumnName;
            SecondTableName = thatTableName;
            Second = thatColumnName;
            SecondType = ValueObjectType.ColumnName;
        }

        public Where(WhereCondition condition, WhereList whereList)
        {
            Condition = condition;
            First = whereList;
        }

        public Where(WhereCondition condition, object thisLiteral, WhereComparision comparedBy, object thatLiteral)
        {
            Condition = condition;
            Comparision = comparedBy;
            First = thisLiteral;
            Second = thatLiteral;
        }

        public Where(WhereCondition condition, object thisObject, ValueObjectType thisObjectType, WhereComparision comparedBy, object thatObject, ValueObjectType thatObjectType)
        {
            Condition = condition;
            Comparision = comparedBy;
            First = thisObject;
            FirstType = thisObjectType;
            Second = thatObject;
            SecondType = thatObjectType;
        }

        public Where(WhereCondition condition, object thisLiteral, object betweenThisLiteral, object andThatLiteral)
        {
            Condition = condition;
            Comparision = WhereComparision.Between;
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
            Comparision = WhereComparision.Between;
            First = thisObject;
            FirstType = thisObjectType;
            Second = betweenThisObject;
            SecondType = betweenThisObjectType;
            Third = andThatObject;
            ThirdType = andThatObjectType;
        }

        public Where(WhereCondition condition,
            string tableName, string columnName,
            WhereComparision comparedBy, object value)
        {
            Condition = condition;
            Comparision = comparedBy;
            FirstTableName = tableName;
            First = columnName;
            FirstType = ValueObjectType.ColumnName;
            Second = value;
            SecondType = ValueObjectType.Value;
        }

        public Where(WhereCondition condition,
            string tableName, string columnName,
            WhereComparision comparedBy, string thatTableName, string thatColumnName)
        {
            Condition = condition;
            Comparision = comparedBy;
            FirstTableName = tableName;
            First = columnName;
            FirstType = ValueObjectType.ColumnName;
            SecondTableName = thatTableName;
            Second = thatColumnName;
            SecondType = ValueObjectType.ColumnName;
        }

        public void BuildCommand(StringBuilder outputBuilder, bool isFirst, ConnectorBase conn, Query relatedQuery)
        {
            BuildCommand(outputBuilder, isFirst, conn, relatedQuery, null, null);
        }

        public void BuildCommand(StringBuilder outputBuilder, bool isFirst, ConnectorBase conn, Query relatedQuery, TableSchema rightTableSchema, string rightTableName)
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

            if (Comparision == WhereComparision.None &&  // Its not a comparison
                // And there's no list or the list is empty
                (!(First is WhereList) || ((WhereList)First).Count == 0) &&
                // And it's not a literal expression
                FirstType != ValueObjectType.Literal &&
                FirstType != ValueObjectType.Value
                )
            {
                outputBuilder.Append(@"1"); // dump a dummy TRUE condition to fill the blank
                return;
            }

            if (First is WhereList)
            {
                outputBuilder.Append('(');
                ((WhereList)First).BuildCommand(outputBuilder, conn, relatedQuery, rightTableSchema, rightTableName);
                outputBuilder.Append(')');
            }
            else
            {
                if (FirstType == ValueObjectType.Value)
                {
                    if (SecondType == ValueObjectType.ColumnName)
                    {
                        if (object.ReferenceEquals(SecondTableName, JoinColumnPair.RIGHT_TABLE_PLACEHOLDER_ID))
                        {
                            outputBuilder.Append(Query.PrepareColumnValue(rightTableSchema.Columns.Find((string)Second), First, conn));
                        }
                        else
                        {
                            TableSchema schema;
                            if (SecondTableName == null || !relatedQuery.TableAliasMap.TryGetValue(SecondTableName, out schema))
                            {
                                schema = relatedQuery.Schema;
                            }
                            outputBuilder.Append(Query.PrepareColumnValue(schema.Columns.Find((string)Second), First, conn));
                        }
                    }
                    else
                    {
                        outputBuilder.Append(conn.PrepareValue(First));
                    }
                }
                else if (FirstType == ValueObjectType.ColumnName)
                {
                    if (FirstTableName != null)
                    {
                        if (object.ReferenceEquals(FirstTableName, JoinColumnPair.RIGHT_TABLE_PLACEHOLDER_ID))
                        {
                            outputBuilder.Append(conn.EncloseFieldName(rightTableName));
                        }
                        else
                        {
                            outputBuilder.Append(conn.EncloseFieldName(FirstTableName));
                        }
                        outputBuilder.Append('.');
                    }
                    outputBuilder.Append(conn.EncloseFieldName((string)First));
                }
                else
                {
                    outputBuilder.Append(First == null ? @"NULL" : First);
                }

                if (Comparision != WhereComparision.None)
                {
                    switch (Comparision)
                    {
                        case WhereComparision.EqualsTo:
                            if (First == null || Second == null) outputBuilder.Append(@" IS ");
                            else outputBuilder.Append(@" = ");
                            break;
                        case WhereComparision.NotEqualsTo:
                            if (First == null || Second == null) outputBuilder.Append(@" IS NOT ");
                            else outputBuilder.Append(@" <> ");
                            break;
                        case WhereComparision.GreaterThan:
                            outputBuilder.Append(@" > ");
                            break;
                        case WhereComparision.GreaterThanOrEqual:
                            outputBuilder.Append(@" >= ");
                            break;
                        case WhereComparision.LessThan:
                            outputBuilder.Append(@" < ");
                            break;
                        case WhereComparision.LessThanOrEqual:
                            outputBuilder.Append(@" <= ");
                            break;
                        case WhereComparision.Is:
                            outputBuilder.Append(@" IS ");
                            break;
                        case WhereComparision.IsNot:
                            outputBuilder.Append(@" IS NOT ");
                            break;
                        case WhereComparision.Like:
                            outputBuilder.Append(@" LIKE ");
                            break;
                        case WhereComparision.Between:
                            outputBuilder.Append(@" BETWEEN ");
                            break;
                        case WhereComparision.In:
                            outputBuilder.Append(@" IN ");
                            break;
                        case WhereComparision.NotIn:
                            outputBuilder.Append(@" NOT IN ");
                            break;
                    }

                    if (Comparision != WhereComparision.In && Comparision != WhereComparision.NotIn)
                    {
                        if (SecondType == ValueObjectType.Value)
                        {
                            if (Second is Query)
                            {
                                outputBuilder.Append('(');
                                outputBuilder.Append(((Query)Second).BuildCommand(conn));
                                outputBuilder.Append(')');
                            }
                            else
                            {
                                if (FirstType == ValueObjectType.ColumnName)
                                {
                                    // Match SECOND value to FIRST's column type
                                    if (object.ReferenceEquals(FirstTableName, JoinColumnPair.RIGHT_TABLE_PLACEHOLDER_ID))
                                    {
                                        outputBuilder.Append(Query.PrepareColumnValue(rightTableSchema.Columns.Find((string)First), Second, conn));
                                    }
                                    else
                                    {
                                        TableSchema schema;
                                        if (FirstTableName == null || !relatedQuery.TableAliasMap.TryGetValue(FirstTableName, out schema))
                                        {
                                            schema = relatedQuery.Schema;
                                        }
                                        outputBuilder.Append(Query.PrepareColumnValue(schema.Columns.Find((string)First), Second, conn));
                                    }
                                }
                                else
                                {
                                    outputBuilder.Append(conn.PrepareValue(Second));
                                }
                            }
                        }
                        else if (SecondType == ValueObjectType.ColumnName)
                        {
                            if (SecondTableName != null)
                            {
                                if (object.ReferenceEquals(SecondTableName, JoinColumnPair.RIGHT_TABLE_PLACEHOLDER_ID))
                                {
                                    outputBuilder.Append(conn.EncloseFieldName(rightTableName));
                                }
                                else
                                {
                                    outputBuilder.Append(conn.EncloseFieldName(SecondTableName));
                                }
                                outputBuilder.Append('.');
                            }
                            outputBuilder.Append(conn.EncloseFieldName((string)Second));
                        }
                        else
                        {
                            if (Second == null) outputBuilder.Append(@"NULL");
                            else outputBuilder.Append(Second);
                        }
                    }
                    else
                    {
                        if (Second is Query) outputBuilder.AppendFormat(@"({0})", Second.ToString());
                        else
                        {
                            ICollection collIn = Second as ICollection;
                            if (collIn != null)
                            {
                                StringBuilder sbIn = new StringBuilder();
                                sbIn.Append('(');
                                bool first = true;

                                TableSchema schema = null;
                                if (object.ReferenceEquals(FirstTableName, JoinColumnPair.RIGHT_TABLE_PLACEHOLDER_ID))
                                {
                                    schema = rightTableSchema;
                                }
                                else
                                {
                                    if (FirstTableName == null || !relatedQuery.TableAliasMap.TryGetValue(FirstTableName, out schema))
                                    {
                                        schema = relatedQuery.Schema;
                                    }
                                }

                                foreach (object objIn in collIn)
                                {
                                    if (first) first = false; else sbIn.Append(',');
                                    if (schema != null) sbIn.Append(Query.PrepareColumnValue(schema.Columns.Find((string)First), objIn, conn));
                                    else sbIn.Append(conn.PrepareValue(objIn));
                                }
                                sbIn.Append(')');
                                outputBuilder.Append(sbIn.ToString());
                            }
                            else outputBuilder.Append(Second);
                        }
                    }

                    if (Comparision == WhereComparision.Between)
                    {
                        outputBuilder.Append(@" AND ");
                        if (ThirdType == ValueObjectType.Value)
                        {
                            if (FirstType == ValueObjectType.ColumnName)
                            {
                                TableSchema schema = null;
                                if (object.ReferenceEquals(FirstTableName, JoinColumnPair.RIGHT_TABLE_PLACEHOLDER_ID))
                                {
                                    schema = rightTableSchema;
                                }
                                else
                                {
                                    if (FirstTableName == null || !relatedQuery.TableAliasMap.TryGetValue(FirstTableName, out schema))
                                    {
                                        schema = relatedQuery.Schema;
                                    }
                                }
                                outputBuilder.Append(Query.PrepareColumnValue(schema.Columns.Find((string)First), Third, conn));
                            }
                            else outputBuilder.Append(conn.PrepareValue(Third));
                        }
                        else if (ThirdType == ValueObjectType.ColumnName)
                        {
                            if (ThirdTableName != null)
                            {
                                if (object.ReferenceEquals(ThirdTableName, JoinColumnPair.RIGHT_TABLE_PLACEHOLDER_ID))
                                {
                                    outputBuilder.Append(conn.EncloseFieldName(rightTableName));
                                }
                                else
                                {
                                    outputBuilder.Append(conn.EncloseFieldName(ThirdTableName));
                                }
                                outputBuilder.Append('.');
                            }
                            outputBuilder.Append(conn.EncloseFieldName((string)Third));
                        }
                        else outputBuilder.Append(Third == null ? @"NULL" : Third);
                    }

                    if (Comparision == WhereComparision.Like)
                    {
                        outputBuilder.Append(' ');
                        outputBuilder.Append(conn.LikeEscapingStatement);
                        outputBuilder.Append(' ');
                    }
                }
            }
        }
    }
}
