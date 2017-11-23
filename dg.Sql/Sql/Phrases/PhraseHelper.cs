using dg.Sql.Connector;
using dg.Sql.Phrases;
using System;
using System.Collections.Generic;
using System.Text;

namespace dg.Sql
{
    public static class PhraseHelper
    {
        #region DateTime

        public static DateTimeAdd DateTimeAdd(object value, ValueObjectType valueType, DateTimeUnit unit, Int64 interval)
        {
            return new DateTimeAdd(value, valueType, unit, interval);
        }

        public static DateTimeAdd DateTimeAdd(string tableName, string columnName, DateTimeUnit unit, Int64 interval)
        {
            return new DateTimeAdd(tableName, columnName, unit, interval);
        }

        public static DateTimeAdd DateTimeAdd(string columnName, DateTimeUnit unit, Int64 interval)
        {
            return new DateTimeAdd(columnName, unit, interval);
        }

        public static DateTimeAdd DateTimeAdd(IPhrase phrase, DateTimeUnit unit, Int64 interval)
        {
            return new DateTimeAdd(phrase, unit, interval);
        }

        public static DateTimeAdd DateTimeAdd(IPhrase phrase, DateTimeUnit unit, string addTableName, string addColumnName)
        {
            return new DateTimeAdd(phrase, unit, addTableName, addColumnName);
        }

        public static DateTimeAdd DateTimeAdd(string tableName, string columnName, DateTimeUnit unit, string addTableName, string addColumnName)
        {
            return new DateTimeAdd(tableName, columnName, unit, addTableName, addColumnName);
        }

        public static DateTimeAdd DateTimeAdd(string columnName, DateTimeUnit unit, string addTableName, string addColumnName)
        {
            return new DateTimeAdd(columnName, unit, addTableName, addColumnName);
        }

        public static DateTimeAdd DateTimeAdd(IPhrase phrase, DateTimeUnit unit, string addColumnName)
        {
            return new DateTimeAdd(phrase, unit, addColumnName);
        }

        public static DateTimeAdd DateTimeAdd(string tableName, string columnName, DateTimeUnit unit, string addColumnName)
        {
            return new DateTimeAdd(tableName, columnName, unit, addColumnName);
        }

        public static DateTimeAdd DateTimeAdd(string columnName, DateTimeUnit unit, string addColumnName)
        {
            return new DateTimeAdd(columnName, unit, addColumnName);
        }

        public static DateTimeDiff DateTimeDiff(DateTimeUnit unit, object value1, ValueObjectType value1Type, object value2, ValueObjectType value2Type)
        {
            return new DateTimeDiff(unit, value1, value1Type, value2, value2Type);
        }

        public static DateTimeDiff DateTimeDiff(DateTimeUnit unit, object value1, ValueObjectType value1Type, DateTime value2)
        {
            return new DateTimeDiff(unit, value1, value1Type, value2);
        }

        public static DateTimeDiff DateTimeDiff(DateTimeUnit unit, DateTime value1, object value2, ValueObjectType value2Type)
        {
            return new DateTimeDiff(unit, value1, value2, value2Type);
        }

        public static DateTimeDiff DateTimeDiff(DateTimeUnit unit, string tableName1, string columnName1, DateTime value2)
        {
            return new DateTimeDiff(unit, tableName1, columnName1, value2);
        }

        public static DateTimeDiff DateTimeDiff(DateTimeUnit unit, DateTime value1, string tableName2, string columnName2)
        {
            return new DateTimeDiff(unit, value1, tableName2, columnName2);
        }

        public static DateTimeDiff DateTimeDiff(DateTimeUnit unit, string tableName1, string columnName1, string tableName2, string columnName2)
        {
            return new DateTimeDiff(unit, tableName1, columnName1, tableName2, columnName2);
        }

        public static DateTimeDiff DateTimeDiff(DateTimeUnit unit, string columnName1, string columnName2)
        {
            return new DateTimeDiff(unit, columnName1, columnName2);
        }

        public static DateTimeDiff DateTimeDiff(DateTimeUnit unit, IPhrase phrase1, IPhrase phrase2)
        {
            return new DateTimeDiff(unit, phrase1, phrase2);
        }

        public static DateTimeDiff DateTimeDiff(DateTimeUnit unit, IPhrase phrase1, object value2, ValueObjectType value2Type)
        {
            return new DateTimeDiff(unit, phrase1, value2, value2Type);
        }

        public static DateTimeDiff DateTimeDiff(DateTimeUnit unit, object value1, ValueObjectType value1Type, IPhrase phrase2)
        {
            return new DateTimeDiff(unit, value1, value1Type, phrase2);
        }

        public static DateTimeDiff DateTimeDiff(DateTimeUnit unit, IPhrase phrase1, DateTime value2)
        {
            return new DateTimeDiff(unit, phrase1, value2);
        }

        public static DateTimeDiff DateTimeDiff(DateTimeUnit unit, DateTime value1, IPhrase phrase2)
        {
            return new DateTimeDiff(unit, value1, phrase2);
        }

        public static DateTimeDiff DateTimeDiff(DateTimeUnit unit, IPhrase phrase1, string tableName2, string columnName2)
        {
            return new DateTimeDiff(unit, phrase1, tableName2, columnName2);
        }

        public static DateTimeDiff DateTimeDiff(DateTimeUnit unit, string tableName1, string columnName1, IPhrase phrase2)
        {
            return new DateTimeDiff(unit, tableName1, columnName1, phrase2);
        }

        public static DateTimeDiff DateTimeDiff(DateTimeUnit unit, IPhrase phrase1, string columnName2)
        {
            return new DateTimeDiff(unit, phrase1, columnName2);
        }

        public static DateTimeDiff DateTimeDiff(DateTimeUnit unit, string columnName1, IPhrase phrase2)
        {
            return new DateTimeDiff(unit, columnName1, phrase2);
        }

        public static UTC_TIMESTAMP UtcTimestamp()
        {
            return new UTC_TIMESTAMP();
        }
        
        public static Year Year(object value, ValueObjectType valueType)
        {
            return new Year(value, valueType);
        }

        public static Year Year(string tableName, string columnName)
        {
            return new Year(tableName, columnName);
        }

        public static Year Year(string columnName)
        {
            return new Year(columnName);
        }

        public static Year Year(IPhrase phrase)
        {
            return new Year(phrase);
        }
        
        public static Month Month(object value, ValueObjectType valueType)
        {
            return new Month(value, valueType);
        }

        public static Month Month(string tableName, string columnName)
        {
            return new Month(tableName, columnName);
        }

        public static Month Month(string columnName)
        {
            return new Month(columnName);
        }

        public static Month Month(IPhrase phrase)
        {
            return new Month(phrase);
        }
        
        public static Day Day(object value, ValueObjectType valueType)
        {
            return new Day(value, valueType);
        }

        public static Day Day(string tableName, string columnName)
        {
            return new Day(tableName, columnName);
        }

        public static Day Day(string columnName)
        {
            return new Day(columnName);
        }

        public static Day Day(IPhrase phrase)
        {
            return new Day(phrase);
        }
        
        public static Hour Hour(object value, ValueObjectType valueType)
        {
            return new Hour(value, valueType);
        }

        public static Hour Hour(string tableName, string columnName)
        {
            return new Hour(tableName, columnName);
        }

        public static Hour Hour(string columnName)
        {
            return new Hour(columnName);
        }

        public static Hour Hour(IPhrase phrase)
        {
            return new Hour(phrase);
        }
        
        public static Minute Minute(object value, ValueObjectType valueType)
        {
            return new Minute(value, valueType);
        }

        public static Minute Minute(string tableName, string columnName)
        {
            return new Minute(tableName, columnName);
        }

        public static Minute Minute(string columnName)
        {
            return new Minute(columnName);
        }

        public static Minute Minute(IPhrase phrase)
        {
            return new Minute(phrase);
        }
        
        public static Second Second(object value, ValueObjectType valueType)
        {
            return new Second(value, valueType);
        }

        public static Second Second(string tableName, string columnName)
        {
            return new Second(tableName, columnName);
        }

        public static Second Second(string columnName)
        {
            return new Second(columnName);
        }

        public static Second Second(IPhrase phrase)
        {
            return new Second(phrase);
        }

        #endregion

        #region Hash functions

        public static MD5 MD5(object value, ValueObjectType valueType, bool binary = false)
        {
            return new MD5(value, valueType, binary);
        }

        public static MD5 MD5(string tableName, string columnName, bool binary = false)
        {
            return new MD5(tableName, columnName, binary);
        }

        public static MD5 MD5(string columnName, bool binary = false)
        {
            return new MD5(columnName, binary);
        }

        public static MD5 MD5(IPhrase phrase, bool binary = false)
        {
            return new MD5(phrase, binary);
        }

        public static SHA1 SHA1(object value, ValueObjectType valueType, bool binary = false)
        {
            return new SHA1(value, valueType, binary);
        }

        public static SHA1 SHA1(string tableName, string columnName, bool binary = false)
        {
            return new SHA1(tableName, columnName, binary);
        }

        public static SHA1 SHA1(string columnName, bool binary = false)
        {
            return new SHA1(columnName, binary);
        }

        public static SHA1 SHA1(IPhrase phrase, bool binary = false)
        {
            return new SHA1(phrase, binary);
        }

        #endregion

        #region Json

        public static JsonExtract JsonExtract(object value, ValueObjectType valueType, string path = "$")
        {
            return new JsonExtract(value, valueType, path);
        }

        public static JsonExtract JsonExtract(string tableName, string columnName, string path = "$")
        {
            return new JsonExtract(tableName, columnName, path);
        }

        public static JsonExtract JsonExtract(string columnName, string path = "$")
        {
            return new JsonExtract(columnName, path);
        }

        public static JsonExtract JsonExtract(IPhrase phrase, string path = "$")
        {
            return new JsonExtract(phrase, path);
        }

        public static JsonExtract JsonExtract(Where where, string path = "$")
        {
            return new JsonExtract(where, path);
        }

        #endregion

        #region Flow Control

        public static Case Case()
        {
            return new Case();
        }

        public static Case Case(ValueWrapper value)
        {
            return new Case(value);
        }

        public static Case Case(Where condition)
        {
            return new Case(condition);
        }

        public static Case Case(WhereList condition)
        {
            return new Case(condition);
        }

        public static Case Case(string tableName, string columnName)
        {
            return new Case(tableName, columnName);
        }

        public static Case Case(object value, ValueObjectType valueType)
        {
            return new Case(value, valueType);
        }

        public static IfNull IfNull(
            string firstTableName, string firstColumnName,
            string secondTableName, string secondColumnName)
        {
            return new IfNull(
                firstTableName, firstColumnName,
                secondTableName, secondColumnName);
        }

        public static IfNull IfNull(
             object firstValue, ValueObjectType firstValueType,
             object secondValue, ValueObjectType secondValueType)
        {
            return new IfNull(
                firstValue, firstValueType,
                secondValue, secondValueType);
        }

        public static IfNull IfNull(
             string firstTableName, string firstColumnName,
             object secondValue, ValueObjectType secondValueType)
        {
            return new IfNull(
                firstTableName, firstColumnName,
                secondValue, secondValueType);
        }

        public static IfNull IfNull(
             object firstValue, ValueObjectType firstValueType,
             string secondTableName, string secondColumnName)
        {
            return new IfNull(
                firstValue, firstValueType,
                secondTableName, secondColumnName);
        }

        #endregion

        #region General

        public static RandWeight RandWeight(object value, ValueObjectType valueType)
        {
            return new RandWeight(value, valueType);
        }

        public static RandWeight RandWeight(string tableName, string columnName)
        {
            return new RandWeight(tableName, columnName);
        }

        public static RandWeight RandWeight(string columnName)
        {
            return new RandWeight(columnName);
        }

        public static RandWeight RandWeight(IPhrase phrase)
        {
            return new RandWeight(phrase);
        }

        public static RandWeight RandWeight(Where where)
        {
            return new RandWeight(where);
        }

        public static Union Union(params Query[] queries)
        {
            return new Union(queries);
        }

        public static UnionAll UnionAll(params Query[] queries)
        {
            return new UnionAll(queries);
        }

        #endregion

        #region Math

        public static Abs Abs(object value, ValueObjectType valueType, int decimalPlaces = 0)
        {
            return new Abs(value, valueType, decimalPlaces);
        }

        public static Abs Abs(string tableName, string columnName, int decimalPlaces = 0)
        {
            return new Abs(tableName, columnName, decimalPlaces);
        }

        public static Abs Abs(string columnName, int decimalPlaces = 0)
        {
            return new Abs(columnName, decimalPlaces);
        }

        public static Abs Abs(IPhrase phrase, int decimalPlaces = 0)
        {
            return new Abs(phrase, decimalPlaces);
        }

        public static Abs Abs(Where where)
        {
            return new Abs(where);
        }

        public static Divide Divide(
            object value1, ValueObjectType valueType1,
            object value2, ValueObjectType valueType2
            )
        {
            return new Divide(
                value1, valueType1,
                value2, valueType2);
        }

        public static Divide Divide(
            string tableName1, string column1,
            string tableName2, string column2
            )
        {
            return new Divide(
                tableName1, column1,
                tableName2, column2);
        }

        public static Divide Divide(
            string tableName1, string column1,
            object value2, ValueObjectType valueType2
            )
        {
            return new Divide(
                tableName1, column1,
                value2, valueType2);
        }

        public static Divide Divide(
            string tableName1, string column1,
            object value2
            )
        {
            return new Divide(
                tableName1, column1,
                value2);
        }

        public static Divide Divide(
            object value1, ValueObjectType valueType1,
            string tableName2, string column2
            )
        {
            return new Divide(
                value1, valueType1,
                tableName2, column2);
        }

        public static Divide Divide(
            object value1,
            string tableName2, string column2
            )
        {
            return new Divide(
                value1,
                tableName2, column2);
        }

        public static Divide Divide(
            object value1,
            object value2
            )
        {
            return new Divide(value1, value2);
        }

        public static Multiply Multiply(
            object value1, ValueObjectType valueType1,
            object value2, ValueObjectType valueType2
            )
        {
            return new Multiply(
                value1, valueType1,
                value2, valueType2);
        }

        public static Multiply Multiply(
            string tableName1, string column1,
            string tableName2, string column2
            )
        {
            return new Multiply(
                tableName1, column1,
                tableName2, column2);
        }

        public static Multiply Multiply(
            string tableName1, string column1,
            object value2, ValueObjectType valueType2
            )
        {
            return new Multiply(
                tableName1, column1,
                value2, valueType2);
        }

        public static Multiply Multiply(
            string tableName1, string column1,
            object value2
            )
        {
            return new Multiply(
                tableName1, column1,
                value2);
        }

        public static Multiply Multiply(
            object value1, ValueObjectType valueType1,
            string tableName2, string column2
            )
        {
            return new Multiply(
                value1, valueType1,
                tableName2, column2);
        }

        public static Multiply Multiply(
            object value1,
            string tableName2, string column2
            )
        {
            return new Multiply(
                value1,
                tableName2, column2);
        }

        public static Multiply Multiply(
            object value1,
            object value2
            )
        {
            return new Multiply(value1, value2);
        }

        public static Round Round(object value, ValueObjectType valueType, int decimalPlaces = 0)
        {
            return new Round(value, valueType, decimalPlaces);
        }

        public static Round Round(string tableName, string columnName, int decimalPlaces = 0)
        {
            return new Round(tableName, columnName, decimalPlaces);
        }

        public static Round Round(string columnName, int decimalPlaces = 0)
        {
            return new Round(columnName, decimalPlaces);
        }

        public static Round Round(IPhrase phrase, int decimalPlaces = 0)
        {
            return new Round(phrase, decimalPlaces);
        }

        public static Round Round(Where where)
        {
            return new Round(where);
        }

        #endregion

        #region Aggregate

        public static Add Add(
            string tableName1, string columnName1,
            string tableName2, string columnName2)
        {
            return new Add(
                tableName1, columnName1,
                tableName2, columnName2);
        }

        public static Add Add(
            string tableName1, string columnName1,
            object value2, ValueObjectType valueType2)
        {
            return new Add(
                tableName1, columnName1,
                value2, valueType2);
        }

        public static Add Add(
            object value1, ValueObjectType valueType1,
            string tableName2, string columnName2)
        {
            return new Add(
                value1, valueType1,
                tableName2, columnName2);
        }

        public static Add Add(
            object value1, ValueObjectType valueType1,
            object value2, ValueObjectType valueType2)
        {
            return new Add(
                value1, valueType1,
                value2, valueType2);
        }

        public static Add Add(string tableName1, string columnName1, Int32 value2)
        {
            return new Add(
                tableName1, columnName1,
                value2);
        }

        public static Add Add(string tableName1, string columnName1, Int64 value2)
        {
            return new Add(
                tableName1, columnName1,
                value2);
        }

        public static Add Add(string tableName1, string columnName1, decimal value2)
        {
            return new Add(
                tableName1, columnName1,
                value2);
        }

        public static Add Add(string tableName1, string columnName1, double value2)
        {
            return new Add(
                tableName1, columnName1,
                value2);
        }

        public static Add Add(string tableName1, string columnName1, float value2)
        {
            return new Add(
                tableName1, columnName1,
                value2);
        }

        public static Add Add(string columnName1, Int32 value2)
        {
            return new Add(
                columnName1,
                value2);
        }

        public static Add Add(string columnName1, Int64 value2)
        {
            return new Add(
                columnName1,
                value2);
        }

        public static Add Add(string columnName1, decimal value2)
        {
            return new Add(
                columnName1,
                value2);
        }

        public static Add Add(string columnName1, double value2)
        {
            return new Add(
                columnName1,
                value2);
        }

        public static Add Add(string columnName1, float value2)
        {
            return new Add(
                columnName1,
                value2);
        }

        public static Avg Avg(object value, ValueObjectType valueType)
        {
            return new Avg(value, valueType);
        }

        public static Avg Avg(string tableName, string columnName)
        {
            return new Avg(tableName, columnName);
        }

        public static Avg Avg(string columnName)
        {
            return new Avg(columnName);
        }

        public static Avg Avg(IPhrase phrase)
        {
            return new Avg(phrase);
        }

        public static Count Count(bool distinct = false)
        {
            return new Count(distinct);
        }

        public static Count Count(string tableName, string columnName, bool distinct = false)
        {
            return new Count(tableName, columnName, distinct);
        }

        public static Count Count(string columnName, bool distinct = false)
        {
            return new Count(columnName, distinct);
        }

        public static Count Count(object value, ValueObjectType valueType, bool distinct = false)
        {
            return new Count(value, valueType, distinct);
        }

        public static Count Count(IPhrase phrase, bool distinct = false)
        {
            return new Count(phrase, distinct);
        }

        public static CountDistinct CountDistinct(object value, ValueObjectType valueType)
        {
            return new CountDistinct(value, valueType);
        }

        public static CountDistinct CountDistinct(string tableName, string columnName)
        {
            return new CountDistinct(tableName, columnName);
        }

        public static CountDistinct CountDistinct(string columnName)
        {
            return new CountDistinct(columnName);
        }

        public static CountDistinct CountDistinct(IPhrase phrase)
        {
            return new CountDistinct(phrase);
        }
        
        public static GroupConcat GroupConcat(bool distinct, string tableName, string columnName, string separator, OrderByList orderBy)
        {
            return new GroupConcat(distinct, tableName, columnName, separator, orderBy);
        }

        public static GroupConcat GroupConcat(bool distinct, string tableName, string columnName, string separator)
        {
            return new GroupConcat(distinct, tableName, columnName, separator);
        }

        public static GroupConcat GroupConcat(bool distinct, string tableName, string columnName, OrderByList orderBy)
        {
            return new GroupConcat(distinct, tableName, columnName, orderBy);
        }

        public static GroupConcat GroupConcat(bool distinct, string tableName, string columnName)
        {
            return new GroupConcat(distinct, tableName, columnName);
        }

        public static GroupConcat GroupConcat(bool distinct, ValueWrapper value, string separator, OrderByList orderBy)
        {
            return new GroupConcat(distinct, value, separator, orderBy);
        }

        public static GroupConcat GroupConcat(bool distinct, ValueWrapper value, string separator)
        {
            return new GroupConcat(distinct, value, separator);
        }

        public static GroupConcat GroupConcat(bool distinct, ValueWrapper value, OrderByList orderBy)
        {
            return new GroupConcat(distinct, value, orderBy);
        }

        public static GroupConcat GroupConcat(bool distinct, ValueWrapper value)
        {
            return new GroupConcat(distinct, value);
        }

        public static GroupConcat GroupConcat(string tableName, string columnName, string separator, OrderByList orderBy)
        {
            return new GroupConcat(tableName, columnName, separator, orderBy);
        }

        public static GroupConcat GroupConcat(string tableName, string columnName, string separator)
        {
            return new GroupConcat(tableName, columnName, separator);
        }

        public static GroupConcat GroupConcat(string tableName, string columnName, OrderByList orderBy)
        {
            return new GroupConcat(tableName, columnName, orderBy);
        }

        public static GroupConcat GroupConcat(string tableName, string columnName)
        {
            return new GroupConcat(tableName, columnName);
        }

        public static GroupConcat GroupConcat(ValueWrapper value, string separator, OrderByList orderBy)
        {
            return new GroupConcat(value, separator, orderBy);
        }

        public static GroupConcat GroupConcat(ValueWrapper value, string separator)
        {
            return new GroupConcat(value, separator);
        }

        public static GroupConcat GroupConcat(ValueWrapper value, OrderByList orderBy)
        {
            return new GroupConcat(value, orderBy);
        }

        public static GroupConcat GroupConcat(ValueWrapper value)
        {
            return new GroupConcat(value);
        }

        public static Max Max(object value, ValueObjectType valueType)
        {
            return new Max(value, valueType);
        }

        public static Max Max(string tableName, string columnName)
        {
            return new Max(tableName, columnName);
        }

        public static Max Max(string columnName)
        {
            return new Max(columnName);
        }

        public static Max Max(IPhrase phrase)
        {
            return new Max(phrase);
        }

        public static Min Min(object value, ValueObjectType valueType)
        {
            return new Min(value, valueType);
        }

        public static Min Min(string tableName, string columnName)
        {
            return new Min(tableName, columnName);
        }

        public static Min Min(string columnName)
        {
            return new Min(columnName);
        }

        public static Min Min(IPhrase phrase)
        {
            return new Min(phrase);
        }

        public static PassThroughAggregate PassThroughAggregate(string aggregateType, object value, ValueObjectType valueType)
        {
            return new PassThroughAggregate(aggregateType, value, valueType);
        }

        public static PassThroughAggregate PassThroughAggregate(string aggregateType, string tableName, string columnName)
        {
            return new PassThroughAggregate(aggregateType, tableName, columnName);
        }

        public static PassThroughAggregate PassThroughAggregate(string aggregateType, string columnName)
        {
            return new PassThroughAggregate(aggregateType, columnName);
        }

        public static PassThroughAggregate PassThroughAggregate(string aggregateType, IPhrase phrase)
        {
            return new PassThroughAggregate(aggregateType, phrase);
        }

        public static StandardDeviationOfPopulation StandardDeviationOfPopulation(object value, ValueObjectType valueType)
        {
            return new StandardDeviationOfPopulation(value, valueType);
        }

        public static StandardDeviationOfPopulation StandardDeviationOfPopulation(string tableName, string columnName)
        {
            return new StandardDeviationOfPopulation(tableName, columnName);
        }

        public static StandardDeviationOfPopulation StandardDeviationOfPopulation(string columnName)
        {
            return new StandardDeviationOfPopulation(columnName);
        }

        public static StandardDeviationOfPopulation StandardDeviationOfPopulation(IPhrase phrase)
        {
            return new StandardDeviationOfPopulation(phrase);
        }

        public static StandardDeviationOfSample StandardDeviationOfSample(object value, ValueObjectType valueType)
        {
            return new StandardDeviationOfSample(value, valueType);
        }

        public static StandardDeviationOfSample StandardDeviationOfSample(string tableName, string columnName)
        {
            return new StandardDeviationOfSample(tableName, columnName);
        }

        public static StandardDeviationOfSample StandardDeviationOfSample(string columnName)
        {
            return new StandardDeviationOfSample(columnName);
        }

        public static StandardDeviationOfSample StandardDeviationOfSample(IPhrase phrase)
        {
            return new StandardDeviationOfSample(phrase);
        }

        public static StandardVarianceOfPopulation StandardVarianceOfPopulation(object value, ValueObjectType valueType)
        {
            return new StandardVarianceOfPopulation(value, valueType);
        }

        public static StandardVarianceOfPopulation StandardVarianceOfPopulation(string tableName, string columnName)
        {
            return new StandardVarianceOfPopulation(tableName, columnName);
        }

        public static StandardVarianceOfPopulation StandardVarianceOfPopulation(string columnName)
        {
            return new StandardVarianceOfPopulation(columnName);
        }

        public static StandardVarianceOfPopulation StandardVarianceOfPopulation(IPhrase phrase)
        {
            return new StandardVarianceOfPopulation(phrase);
        }

        public static StandardVarianceOfSample StandardVarianceOfSample(object value, ValueObjectType valueType)
        {
            return new StandardVarianceOfSample(value, valueType);
        }

        public static StandardVarianceOfSample StandardVarianceOfSample(string tableName, string columnName)
        {
            return new StandardVarianceOfSample(tableName, columnName);
        }

        public static StandardVarianceOfSample StandardVarianceOfSample(string columnName)
        {
            return new StandardVarianceOfSample(columnName);
        }

        public static StandardVarianceOfSample StandardVarianceOfSample(IPhrase phrase)
        {
            return new StandardVarianceOfSample(phrase);
        }

        public static Sum Sum(object value, ValueObjectType valueType)
        {
            return new Sum(value, valueType);
        }

        public static Sum Sum(string tableName, string columnName)
        {
            return new Sum(tableName, columnName);
        }

        public static Sum Sum(string columnName)
        {
            return new Sum(columnName);
        }

        public static Sum Sum(IPhrase phrase)
        {
            return new Sum(phrase);
        }

        #endregion

        #region Spatial

        public static GeographyContains GeographyContains(
            object outerValue, ValueObjectType outerValueType,
            object innerValue, ValueObjectType innerValueType)
        {
            return new GeographyContains(
                outerValue, outerValueType,
                innerValue, innerValueType);
        }

        public static GeographyContains GeographyContains(
            object outerValue, ValueObjectType outerValueType,
            string innerColumnName)
        {
            return new GeographyContains(
                outerValue, outerValueType,
                innerColumnName);
        }

        public static GeographyContains GeographyContains(
            object outerValue, ValueObjectType outerValueType,
            string innerTableName, string innerColumnName)
        {
            return new GeographyContains(
                outerValue, outerValueType,
                innerTableName, innerColumnName);
        }

        public static GeographyContains GeographyContains(
            Geometry outerValue,
            string innerColumnName)
        {
            return new GeographyContains(
                outerValue,
                innerColumnName);
        }

        public static GeographyContains GeographyContains(
            Geometry outerValue,
            string innerTableName, string innerColumnName)
        {
            return new GeographyContains(
                outerValue,
                innerTableName, innerColumnName);
        }

        public static GeographyContains GeographyContains(Geometry outerValue, Geometry innerValue)
        {
            return new GeographyContains(
                outerValue,
                innerValue);
        }

        public static GeographyContains GeographyContains(
            string outerColumnName,
            Geometry innerObject)
        {
            return new GeographyContains(
                outerColumnName,
                innerObject);
        }

        public static GeographyContains GeographyContains(
            string outerTableName, string outerColumnName,
            Geometry innerObject)
        {
            return new GeographyContains(
                outerTableName, outerColumnName,
                innerObject);
        }

        public static GeographyDistance GeographyDistance(GeographyDistance.PointWrapper from, GeographyDistance.PointWrapper to)
        {
            return new GeographyDistance(from, to);
        }

        #endregion

        #region Strings
        
        public static Length Length(object value, ValueObjectType valueType)
        {
            return new Length(value, valueType);
        }

        public static Length Length(string tableName, string columnName)
        {
            return new Length(tableName, columnName);
        }

        public static Length Length(string columnName)
        {
            return new Length(columnName);
        }

        public static Length Length(IPhrase phrase)
        {
            return new Length(phrase);
        }
        
        public static Lower Lower(object value, ValueObjectType valueType)
        {
            return new Lower(value, valueType);
        }

        public static Lower Lower(string tableName, string columnName)
        {
            return new Lower(tableName, columnName);
        }

        public static Lower Lower(string columnName)
        {
            return new Lower(columnName);
        }

        public static Lower Lower(IPhrase phrase)
        {
            return new Lower(phrase);
        }
        
        public static Upper Upper(object value, ValueObjectType valueType)
        {
            return new Upper(value, valueType);
        }

        public static Upper Upper(string tableName, string columnName)
        {
            return new Upper(tableName, columnName);
        }

        public static Upper Upper(string columnName)
        {
            return new Upper(columnName);
        }

        public static Upper Upper(IPhrase phrase)
        {
            return new Upper(phrase);
        }

        public static Replace Replace(
            string sourceTableName, string sourceColumn,
            string searchTableName, string searchColumn,
            string replaceWithTableName, string replaceWithColumn)
        {
            return new Replace(
                sourceTableName, sourceColumn,
                searchTableName, searchColumn,
                replaceWithTableName, replaceWithColumn);
        }

        public static Replace Replace(
            object source, ValueObjectType sourceType,
            string searchTableName, string searchColumn,
            string replaceWithTableName, string replaceWithColumn)
        {
            return new Replace(
                source, sourceType,
                searchTableName, searchColumn,
                replaceWithTableName, replaceWithColumn);
        }

        public static Replace Replace(
            string sourceTableName, string sourceColumn,
            object search, ValueObjectType searchType,
            string replaceWithTableName, string replaceWithColumn)
        {
            return new Replace(
                sourceTableName, sourceColumn,
                search, searchType,
                replaceWithTableName, replaceWithColumn);
        }

        public static Replace Replace(
            string sourceTableName, string sourceColumn,
            string searchTableName, string searchColumn,
            object replace, ValueObjectType replaceWithType)
        {
            return new Replace(
                sourceTableName, sourceColumn,
                searchTableName, searchColumn,
                replace, replaceWithType);
        }

        public static Replace Replace(
            object source, ValueObjectType sourceType,
            object search, ValueObjectType searchType,
            string replaceWithTableName, string replaceWithColumn)
        {
            return new Replace(
                source, sourceType,
                search, searchType,
                replaceWithTableName, replaceWithColumn);
        }

        public static Replace Replace(
            string sourceTableName, string sourceColumn,
            object search, ValueObjectType searchType,
            object replace, ValueObjectType replaceWithType)
        {
            return new Replace(
                sourceTableName, sourceColumn,
                search, searchType,
                replace, replaceWithType);
        }

        public static Replace Replace(
            object source, ValueObjectType sourceType,
            object search, ValueObjectType searchType,
            object replace, ValueObjectType replaceWithType)
        {
            return new Replace(
                source, sourceType,
                search, searchType,
                replace, replaceWithType);
        }
        
        public static Substring Substring(object value, ValueObjectType valueType, int from, int length)
        {
            return new Substring(value, valueType, from, length);
        }

        public static Substring Substring(string tableName, string columnName, int from, int length)
        {
            return new Substring(tableName, columnName, from, length);
        }

        public static Substring Substring(string columnName, int from, int length)
        {
            return new Substring(columnName, from, length);
        }

        public static Substring Substring(IPhrase phrase, int from, int length)
        {
            return new Substring(phrase, from, length);
        }

        public static Concat Concat(params ValueWrapper[] values)
        {
            return new Concat(values);
        }

        public static Concat Concat(bool ignoreNulls, params ValueWrapper[] values)
        {
            return new Concat(ignoreNulls, values);
        }

        #endregion
    }
}
