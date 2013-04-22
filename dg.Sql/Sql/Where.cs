using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using dg.Sql.Connector;

namespace dg.Sql
{
    public class WhereList : List<Where>
    {
        public void BuildCommand(StringBuilder sb, ConnectorBase connection, Query qry)
        {
            bool bFirst = true;
            foreach (Where where in this)
            {
                where.BuildCommand(sb, bFirst, connection, qry);
                if (bFirst) bFirst = false;
            }
        }
    }
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

        public void BuildCommand(StringBuilder sb, bool bFirst, ConnectorBase connection, Query qry)
        {
            if (!bFirst)
            {
                switch (Condition)
                {
                    case WhereCondition.AND:
                        sb.Append(@" AND ");
                        break;
                    case WhereCondition.OR:
                        sb.Append(@" OR ");
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
                sb.Append(@"1"); // dump a dummy TRUE condition to fill the blank
                return;
            }

            if (First is WhereList)
            {
                sb.Append('(');
                ((WhereList)First).BuildCommand(sb, connection, qry);
                sb.Append(')');
            }
            else
            {
                if (FirstType == ValueObjectType.Value)
                {
                    if (SecondType == ValueObjectType.ColumnName)
                    {
                        TableSchema schema;
                        if (SecondTableName == null || !qry.TableAliasMap.TryGetValue(SecondTableName, out schema))
                        {
                            schema = qry.Schema;
                        }
                        sb.Append(Query.PrepareColumnValue(schema.Columns.Find((string)Second), First, connection));
                    }
                    else sb.Append(connection.PrepareValue(First));
                }
                else if (FirstType == ValueObjectType.ColumnName)
                {
                    if (FirstTableName != null)
                    {
                        sb.Append(connection.EncloseFieldName(FirstTableName));
                        sb.Append('.');
                    }
                    sb.Append(connection.EncloseFieldName((string)First));
                }
                else sb.Append(First == null ? @"NULL" : First);
                if (Comparision != WhereComparision.None)
                {
                    switch (Comparision)
                    {
                        case WhereComparision.EqualsTo:
                            if (First == null || Second == null) sb.Append(@" IS ");
                            else sb.Append(@" = ");
                            break;
                        case WhereComparision.NotEqualsTo:
                            if (First == null || Second == null) sb.Append(@" IS NOT ");
                            else sb.Append(@" <> ");
                            break;
                        case WhereComparision.GreaterThan:
                            sb.Append(@" > ");
                            break;
                        case WhereComparision.GreaterThanOrEqual:
                            sb.Append(@" >= ");
                            break;
                        case WhereComparision.LessThan:
                            sb.Append(@" < ");
                            break;
                        case WhereComparision.LessThanOrEqual:
                            sb.Append(@" <= ");
                            break;
                        case WhereComparision.Is:
                            sb.Append(@" IS ");
                            break;
                        case WhereComparision.IsNot:
                            sb.Append(@" IS NOT ");
                            break;
                        case WhereComparision.Like:
                            sb.Append(@" LIKE ");
                            break;
                        case WhereComparision.Between:
                            sb.Append(@" BETWEEN ");
                            break;
                        case WhereComparision.In:
                            sb.Append(@" IN ");
                            break;
                        case WhereComparision.NotIn:
                            sb.Append(@" NOT IN ");
                            break;
                    }

                    if (Comparision != WhereComparision.In && Comparision != WhereComparision.NotIn)
                    {
                        if (SecondType == ValueObjectType.Value)
                        {
                            if (FirstType == ValueObjectType.ColumnName)
                            {
                                TableSchema schema;
                                if (FirstTableName == null || !qry.TableAliasMap.TryGetValue(FirstTableName, out schema))
                                {
                                    schema = qry.Schema;
                                }
                                sb.Append(Query.PrepareColumnValue(schema.Columns.Find((string)First), Second, connection));
                            }
                            else sb.Append(connection.PrepareValue(Second));
                        }
                        else if (SecondType == ValueObjectType.ColumnName)
                        {
                            if (SecondTableName != null)
                            {
                                sb.Append(connection.EncloseFieldName(SecondTableName));
                                sb.Append('.');
                            }
                            sb.Append(connection.EncloseFieldName((string)Second));
                        }
                        else
                        {
                            if (Second == null) sb.Append(@"NULL");
                            else sb.Append(Second);
                        }
                    }
                    else
                    {
                        if (Second is Query) sb.AppendFormat(@"({0})", Second.ToString());
                        else
                        {
                            ICollection collIn = Second as ICollection;
                            if (collIn != null)
                            {
                                StringBuilder sbIn = new StringBuilder();
                                sbIn.Append('(');
                                bool first = true;
                                TableSchema schema = null;
                                if (FirstType == ValueObjectType.ColumnName)
                                {
                                    if (FirstTableName == null || !qry.TableAliasMap.TryGetValue(FirstTableName, out schema))
                                    {
                                        schema = qry.Schema;
                                    }
                                }
                                foreach (object objIn in collIn)
                                {
                                    if (first) first = false; else sbIn.Append(',');
                                    if (schema != null) sbIn.Append(Query.PrepareColumnValue(schema.Columns.Find((string)First), objIn, connection));
                                    else sbIn.Append(connection.PrepareValue(objIn));
                                }
                                sbIn.Append(')');
                                sb.Append(sbIn.ToString());
                            }
                            else sb.Append(Second);
                        }
                    }

                    if (Comparision == WhereComparision.Between)
                    {
                        sb.Append(@" AND ");
                        if (ThirdType == ValueObjectType.Value)
                        {
                            if (FirstType == ValueObjectType.ColumnName)
                            {
                                TableSchema schema;
                                if (FirstTableName == null || !qry.TableAliasMap.TryGetValue(FirstTableName, out schema))
                                {
                                    schema = qry.Schema;
                                }
                                sb.Append(Query.PrepareColumnValue(schema.Columns.Find((string)First), Third, connection));
                            }
                            else sb.Append(connection.PrepareValue(Third));
                        }
                        else if (ThirdType == ValueObjectType.ColumnName)
                        {
                            if (ThirdTableName != null)
                            {
                                sb.Append(connection.EncloseFieldName(ThirdTableName));
                                sb.Append('.');
                            }
                            sb.Append(connection.EncloseFieldName((string)Third));
                        }
                        else sb.Append(Third == null ? @"NULL" : Third);
                    }

                    if (Comparision == WhereComparision.Like)
                    {
                        if (connection.TYPE == ConnectorBase.SqlServiceType.MYSQL)
                        {
                            sb.Append(@" ESCAPE('\\') ");
                        }
                        else sb.Append(@" ESCAPE('\') ");
                    }
                }
            }
        }
    }
}
