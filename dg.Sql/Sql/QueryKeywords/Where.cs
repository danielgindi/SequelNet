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

        public void BuildCommand(StringBuilder OutputBuilder, bool bFirst, ConnectorBase Connection, Query RelatedQuery)
        {
            BuildCommand(OutputBuilder, bFirst, Connection, RelatedQuery, null, null);
        }

        public void BuildCommand(StringBuilder OutputBuilder, bool bFirst, ConnectorBase Connection, Query RelatedQuery, TableSchema RightTableSchema, string RightTableName)
        {
            if (!bFirst)
            {
                switch (Condition)
                {
                    case WhereCondition.AND:
                        OutputBuilder.Append(@" AND ");
                        break;
                    case WhereCondition.OR:
                        OutputBuilder.Append(@" OR ");
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
                OutputBuilder.Append(@"1"); // dump a dummy TRUE condition to fill the blank
                return;
            }

            if (First is WhereList)
            {
                OutputBuilder.Append('(');
                ((WhereList)First).BuildCommand(OutputBuilder, Connection, RelatedQuery, RightTableSchema, RightTableName);
                OutputBuilder.Append(')');
            }
            else
            {
                if (FirstType == ValueObjectType.Value)
                {
                    if (SecondType == ValueObjectType.ColumnName)
                    {
                        if (object.ReferenceEquals(SecondTableName, JoinColumnPair.RIGHT_TABLE_PLACEHOLDER_ID))
                        {
                            OutputBuilder.Append(Query.PrepareColumnValue(RightTableSchema.Columns.Find((string)Second), First, Connection));
                        }
                        else
                        {
                            TableSchema schema;
                            if (SecondTableName == null || !RelatedQuery.TableAliasMap.TryGetValue(SecondTableName, out schema))
                            {
                                schema = RelatedQuery.Schema;
                            }
                            OutputBuilder.Append(Query.PrepareColumnValue(schema.Columns.Find((string)Second), First, Connection));
                        }
                    }
                    else
                    {
                        OutputBuilder.Append(Connection.PrepareValue(First));
                    }
                }
                else if (FirstType == ValueObjectType.ColumnName)
                {
                    if (FirstTableName != null)
                    {
                        if (object.ReferenceEquals(FirstTableName, JoinColumnPair.RIGHT_TABLE_PLACEHOLDER_ID))
                        {
                            OutputBuilder.Append(Connection.EncloseFieldName(RightTableName));
                        }
                        else
                        {
                            OutputBuilder.Append(Connection.EncloseFieldName(FirstTableName));
                        }
                        OutputBuilder.Append('.');
                    }
                    OutputBuilder.Append(Connection.EncloseFieldName((string)First));
                }
                else
                {
                    OutputBuilder.Append(First == null ? @"NULL" : First);
                }

                if (Comparision != WhereComparision.None)
                {
                    switch (Comparision)
                    {
                        case WhereComparision.EqualsTo:
                            if (First == null || Second == null) OutputBuilder.Append(@" IS ");
                            else OutputBuilder.Append(@" = ");
                            break;
                        case WhereComparision.NotEqualsTo:
                            if (First == null || Second == null) OutputBuilder.Append(@" IS NOT ");
                            else OutputBuilder.Append(@" <> ");
                            break;
                        case WhereComparision.GreaterThan:
                            OutputBuilder.Append(@" > ");
                            break;
                        case WhereComparision.GreaterThanOrEqual:
                            OutputBuilder.Append(@" >= ");
                            break;
                        case WhereComparision.LessThan:
                            OutputBuilder.Append(@" < ");
                            break;
                        case WhereComparision.LessThanOrEqual:
                            OutputBuilder.Append(@" <= ");
                            break;
                        case WhereComparision.Is:
                            OutputBuilder.Append(@" IS ");
                            break;
                        case WhereComparision.IsNot:
                            OutputBuilder.Append(@" IS NOT ");
                            break;
                        case WhereComparision.Like:
                            OutputBuilder.Append(@" LIKE ");
                            break;
                        case WhereComparision.Between:
                            OutputBuilder.Append(@" BETWEEN ");
                            break;
                        case WhereComparision.In:
                            OutputBuilder.Append(@" IN ");
                            break;
                        case WhereComparision.NotIn:
                            OutputBuilder.Append(@" NOT IN ");
                            break;
                    }

                    if (Comparision != WhereComparision.In && Comparision != WhereComparision.NotIn)
                    {
                        if (SecondType == ValueObjectType.Value)
                        {
                            if (Second is Query)
                            {
                                OutputBuilder.Append('(');
                                OutputBuilder.Append(((Query)Second).BuildCommand(Connection));
                                OutputBuilder.Append(')');
                            }
                            else
                            {
                                if (FirstType == ValueObjectType.ColumnName)
                                {
                                    // Match SECOND value to FIRST's column type
                                    if (object.ReferenceEquals(FirstTableName, JoinColumnPair.RIGHT_TABLE_PLACEHOLDER_ID))
                                    {
                                        OutputBuilder.Append(Query.PrepareColumnValue(RightTableSchema.Columns.Find((string)First), Second, Connection));
                                    }
                                    else
                                    {
                                        TableSchema schema;
                                        if (FirstTableName == null || !RelatedQuery.TableAliasMap.TryGetValue(FirstTableName, out schema))
                                        {
                                            schema = RelatedQuery.Schema;
                                        }
                                        OutputBuilder.Append(Query.PrepareColumnValue(schema.Columns.Find((string)First), Second, Connection));
                                    }
                                }
                                else
                                {
                                    OutputBuilder.Append(Connection.PrepareValue(Second));
                                }
                            }
                        }
                        else if (SecondType == ValueObjectType.ColumnName)
                        {
                            if (SecondTableName != null)
                            {
                                if (object.ReferenceEquals(SecondTableName, JoinColumnPair.RIGHT_TABLE_PLACEHOLDER_ID))
                                {
                                    OutputBuilder.Append(Connection.EncloseFieldName(RightTableName));
                                }
                                else
                                {
                                    OutputBuilder.Append(Connection.EncloseFieldName(SecondTableName));
                                }
                                OutputBuilder.Append('.');
                            }
                            OutputBuilder.Append(Connection.EncloseFieldName((string)Second));
                        }
                        else
                        {
                            if (Second == null) OutputBuilder.Append(@"NULL");
                            else OutputBuilder.Append(Second);
                        }
                    }
                    else
                    {
                        if (Second is Query) OutputBuilder.AppendFormat(@"({0})", Second.ToString());
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
                                    schema = RightTableSchema;
                                }
                                else
                                {
                                    if (FirstTableName == null || !RelatedQuery.TableAliasMap.TryGetValue(FirstTableName, out schema))
                                    {
                                        schema = RelatedQuery.Schema;
                                    }
                                }

                                foreach (object objIn in collIn)
                                {
                                    if (first) first = false; else sbIn.Append(',');
                                    if (schema != null) sbIn.Append(Query.PrepareColumnValue(schema.Columns.Find((string)First), objIn, Connection));
                                    else sbIn.Append(Connection.PrepareValue(objIn));
                                }
                                sbIn.Append(')');
                                OutputBuilder.Append(sbIn.ToString());
                            }
                            else OutputBuilder.Append(Second);
                        }
                    }

                    if (Comparision == WhereComparision.Between)
                    {
                        OutputBuilder.Append(@" AND ");
                        if (ThirdType == ValueObjectType.Value)
                        {
                            if (FirstType == ValueObjectType.ColumnName)
                            {
                                TableSchema schema = null;
                                if (object.ReferenceEquals(FirstTableName, JoinColumnPair.RIGHT_TABLE_PLACEHOLDER_ID))
                                {
                                    schema = RightTableSchema;
                                }
                                else
                                {
                                    if (FirstTableName == null || !RelatedQuery.TableAliasMap.TryGetValue(FirstTableName, out schema))
                                    {
                                        schema = RelatedQuery.Schema;
                                    }
                                }
                                OutputBuilder.Append(Query.PrepareColumnValue(schema.Columns.Find((string)First), Third, Connection));
                            }
                            else OutputBuilder.Append(Connection.PrepareValue(Third));
                        }
                        else if (ThirdType == ValueObjectType.ColumnName)
                        {
                            if (ThirdTableName != null)
                            {
                                if (object.ReferenceEquals(ThirdTableName, JoinColumnPair.RIGHT_TABLE_PLACEHOLDER_ID))
                                {
                                    OutputBuilder.Append(Connection.EncloseFieldName(RightTableName));
                                }
                                else
                                {
                                    OutputBuilder.Append(Connection.EncloseFieldName(ThirdTableName));
                                }
                                OutputBuilder.Append('.');
                            }
                            OutputBuilder.Append(Connection.EncloseFieldName((string)Third));
                        }
                        else OutputBuilder.Append(Third == null ? @"NULL" : Third);
                    }

                    if (Comparision == WhereComparision.Like)
                    {
                        OutputBuilder.Append(' ');
                        OutputBuilder.Append(Connection.LikeEscapingStatement);
                        OutputBuilder.Append(' ');
                    }
                }
            }
        }
    }
}
