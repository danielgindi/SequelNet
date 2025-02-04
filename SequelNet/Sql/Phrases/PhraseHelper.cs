﻿using SequelNet.Phrases;
using System;
using System.Collections.Generic;

namespace SequelNet;

public static class PhraseHelper
{
    #region DateTime

    public static ConvertUtcToTz ConvertUtcToTz(ValueWrapper value, ValueWrapper timezone)
    {
        return new ConvertUtcToTz(value, timezone);
    }

    public static ConvertUtcToTz ConvertUtcToTz(ValueWrapper value, string timezone)
    {
        return new ConvertUtcToTz(value, timezone);
    }
    
    public static CreateDate CreateDate(int year, int month, int day)
    {
        return new CreateDate(year, month, day);
    }
    
    public static CreateTime CreateTime(int hours, int minutes, int seconds, int milliseconds = 0)
    {
        return new CreateTime(hours, minutes, seconds, milliseconds);
    }

    public static DateTimeAdd DateTimeAdd(ValueWrapper to, DateTimeUnit unit, ValueWrapper add)
    {
        return new DateTimeAdd(to, unit, add);
    }

    public static DateTimeAdd DateTimeAdd(IPhrase to, DateTimeUnit unit, ValueWrapper add)
    {
        return new DateTimeAdd(to, unit, add);
    }

    public static DateTimeAdd DateTimeAdd(IPhrase to, DateTimeUnit unit, IPhrase add)
    {
        return new DateTimeAdd(to, unit, add);
    }

    public static DateTimeAdd DateTimeAdd(ValueWrapper to, DateTimeUnit unit, IPhrase add)
    {
        return new DateTimeAdd(to, unit, add);
    }

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

    public static UtcTimestamp UtcTimestamp()
    {
        return new UtcTimestamp();
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

    public static Year Year(ValueWrapper value)
    {
        return new Year(value);
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

    public static Month Month(ValueWrapper value)
    {
        return new Month(value);
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

    public static Day Day(ValueWrapper value)
    {
        return new Day(value);
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

    public static Hour Hour(ValueWrapper value)
    {
        return new Hour(value);
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

    public static Minute Minute(ValueWrapper value)
    {
        return new Minute(value);
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

    public static Second Second(ValueWrapper value)
    {
        return new Second(value);
    }

    public static ExtractDate ExtractDate(object value, ValueObjectType valueType)
    {
        return new ExtractDate(value, valueType);
    }

    public static ExtractDate ExtractDate(string tableName, string columnName)
    {
        return new ExtractDate(tableName, columnName);
    }

    public static ExtractDate ExtractDate(string columnName)
    {
        return new ExtractDate(columnName);
    }

    public static ExtractDate ExtractDate(IPhrase phrase)
    {
        return new ExtractDate(phrase);
    }

    public static ExtractDate ExtractDate(ValueWrapper value)
    {
        return new ExtractDate(value);
    }

    public static ExtractTime ExtractTime(object value, ValueObjectType valueType)
    {
        return new ExtractTime(value, valueType);
    }

    public static ExtractTime ExtractTime(string tableName, string columnName)
    {
        return new ExtractTime(tableName, columnName);
    }

    public static ExtractTime ExtractTime(string columnName)
    {
        return new ExtractTime(columnName);
    }

    public static ExtractTime ExtractTime(IPhrase phrase)
    {
        return new ExtractTime(phrase);
    }

    public static ExtractTime ExtractTime(ValueWrapper value)
    {
        return new ExtractTime(value);
    }

    public static DateTimeFormat DateTimeFormat(object value, ValueObjectType valueType, DateTimeFormat.FormatOptions format)
    {
        return new DateTimeFormat(value, valueType, format);
    }

    public static DateTimeFormat DateTimeFormat(string tableName, string columnName, DateTimeFormat.FormatOptions format)
    {
        return new DateTimeFormat(tableName, columnName, format);
    }

    public static DateTimeFormat DateTimeFormat(string columnName, DateTimeFormat.FormatOptions format)
    {
        return new DateTimeFormat(columnName, format);
    }

    public static DateTimeFormat DateTimeFormat(IPhrase phrase, DateTimeFormat.FormatOptions format)
    {
        return new DateTimeFormat(phrase, format);
    }

    public static DateTimeFormat DateTimeFormat(ValueWrapper value, DateTimeFormat.FormatOptions format)
    {
        return new DateTimeFormat(value, format);
    }

    public static UnixTimestamp UnixTimestamp(object value, ValueObjectType valueType)
    {
        return new UnixTimestamp(value, valueType);
    }

    public static UnixTimestamp UnixTimestamp(string tableName, string columnName)
    {
        return new UnixTimestamp(tableName, columnName);
    }

    public static UnixTimestamp UnixTimestamp(string columnName)
    {
        return new UnixTimestamp(columnName);
    }

    public static UnixTimestamp UnixTimestamp(IPhrase phrase)
    {
        return new UnixTimestamp(phrase);
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

    public static JsonArray JsonArray()
    {
        return new JsonArray();
    }

    public static JsonArray JsonArray(params ValueWrapper[] values)
    {
        return new JsonArray(values);
    }

    public static JsonArray JsonArray(IEnumerable<ValueWrapper> values)
    {
        return new JsonArray(values);
    }

    public static JsonArray JsonArray(params object[] values)
    {
        return new JsonArray(values);
    }

    public static JsonArray JsonArray(params string[] values)
    {
        return new JsonArray(values);
    }

    public static JsonArray JsonArray(params Int64[] values)
    {
        return new JsonArray(values);
    }

    public static JsonArray JsonArray(params Int32[] values)
    {
        return new JsonArray(values);
    }

    static public JsonArrayAggregate JsonArrayAggregate(ValueWrapper value, bool binary = false)
    {
        return new JsonArrayAggregate(value, binary);
    }

    static public JsonArrayAggregate JsonArrayAggregate(object value, ValueObjectType valueType, bool binary = false)
    {
        return new JsonArrayAggregate(value, valueType, binary);
    }

    static public JsonArrayAggregate JsonArrayAggregate(string tableName, string columnName, bool binary = false)
    {
        return new JsonArrayAggregate(tableName, columnName, binary);
    }

    static public JsonArrayAggregate JsonArrayAggregate(string columnName, bool binary = false)
    {
        return new JsonArrayAggregate(columnName, binary);
    }

    static public JsonArrayAggregate JsonArrayAggregate(IPhrase phrase, bool binary = false)
    {
        return new JsonArrayAggregate(phrase, binary);
    }

    static public JsonArrayAggregate JsonArrayAggregate(Where where, bool binary = false)
    {
        return new JsonArrayAggregate(where, binary);
    }

    static public JsonArrayAppend JsonArrayAppend(
        object doc, ValueObjectType docType,
        string path,
        object value, ValueObjectType valueType)
    {
        return new JsonArrayAppend(doc, docType, path, value, valueType);
    }

    static public JsonArrayAppend JsonArrayAppend(
        string docTableName, string docColumnName,
        string path,
        object value, ValueObjectType valueType)
    {
        return new JsonArrayAppend(docTableName, docColumnName, path, value, valueType);
    }

    static public JsonArrayAppend JsonArrayAppend(
        string docColumnName,
        string path,
        object value, ValueObjectType valueType)
    {
        return new JsonArrayAppend(docColumnName, path, value, valueType);
    }

    static public JsonArrayAppend JsonArrayAppend(
        IPhrase doc,
        string path,
        object value, ValueObjectType valueType)
    {
        return new JsonArrayAppend(doc, path, value, valueType);
    }

    static public JsonArrayAppend JsonArrayAppend(
        IPhrase doc,
        string path,
        IPhrase value)
    {
        return new JsonArrayAppend(doc, path, value);
    }

    static public JsonArrayAppend JsonArrayAppend(
        IPhrase doc,
        string path,
        params ValueWrapper[] values)
    {
        return new JsonArrayAppend(doc, path, values);
    }

    static public JsonArrayInsert JsonArrayInsert(
        object doc, ValueObjectType docType,
        string path,
        object value, ValueObjectType valueType)
    {
        return new JsonArrayInsert(doc, docType, path, value, valueType);
    }

    static public JsonArrayInsert JsonArrayInsert(
        string docTableName, string docColumnName,
        string path,
        object value, ValueObjectType valueType)
    {
        return new JsonArrayInsert(docTableName, docColumnName, path, value, valueType);
    }

    static public JsonArrayInsert JsonArrayInsert(
        string docColumnName,
        string path,
        object value, ValueObjectType valueType)
    {
        return new JsonArrayInsert(docColumnName, path, value, valueType);
    }

    static public JsonArrayInsert JsonArrayInsert(
        IPhrase doc,
        string path,
        object value, ValueObjectType valueType)
    {
        return new JsonArrayInsert(doc, path, value, valueType);
    }

    static public JsonArrayInsert JsonArrayInsert(
        IPhrase doc,
        string path,
        IPhrase value)
    {
        return new JsonArrayInsert(doc, path, value);
    }

    public static JsonContains JsonContains(ValueWrapper target, ValueWrapper candidate, JsonPathExpression path = null)
    {
        return new JsonContains(target, candidate, path);
    }

    public static JsonContains JsonContains(object target, ValueObjectType targetType, object candidate, ValueObjectType candidateType, JsonPathExpression path = null)
    {
        return new JsonContains(target, targetType, candidate, candidateType, path);
    }

    public static JsonContains JsonContains(string targetTableName, string targetColumnName, object candidate, ValueObjectType candidateType, JsonPathExpression path = null)
    {
        return new JsonContains(targetTableName, targetColumnName, candidate, candidateType, path);
    }

    public static JsonContains JsonContains(IPhrase target, object candidate, ValueObjectType candidateType, JsonPathExpression path = null)
    {
        return new JsonContains(target, candidate, candidateType, path);
    }

    public static JsonContains JsonContains(ValueWrapper target, ValueWrapper candidate, string path)
    {
        return new JsonContains(target, candidate, path);
    }

    public static JsonContains JsonContains(object target, ValueObjectType targetType, object candidate, ValueObjectType candidateType, string path)
    {
        return new JsonContains(target, targetType, candidate, candidateType, path);
    }

    public static JsonContains JsonContains(string targetTableName, string targetColumnName, object candidate, ValueObjectType candidateType, string path)
    {
        return new JsonContains(targetTableName, targetColumnName, candidate, candidateType, path);
    }

    public static JsonContains JsonContains(IPhrase target, object candidate, ValueObjectType candidateType, string path)
    {
        return new JsonContains(target, candidate, candidateType, path);
    }

    public static JsonExtract JsonExtract(object value, ValueObjectType valueType, JsonPathExpression path, bool unquote = true)
    {
        return new JsonExtract(value, valueType, path, unquote);
    }

    public static JsonExtract JsonExtract(string tableName, string columnName, JsonPathExpression path, bool unquote = true)
    {
        return new JsonExtract(tableName, columnName, path, unquote);
    }

    public static JsonExtract JsonExtract(string columnName, JsonPathExpression path, bool unquote = true)
    {
        return new JsonExtract(columnName, path, unquote);
    }

    public static JsonExtract JsonExtract(IPhrase phrase, JsonPathExpression path, bool unquote = true)
    {
        return new JsonExtract(phrase, path, unquote);
    }

    public static JsonExtract JsonExtract(Where where, JsonPathExpression path, bool unquote = true)
    {
        return new JsonExtract(where, path, unquote);
    }

    public static JsonExtract JsonExtract(object value, ValueObjectType valueType, string path, bool unquote = true)
    {
        return new JsonExtract(value, valueType, path, unquote);
    }

    public static JsonExtract JsonExtract(string tableName, string columnName, string path, bool unquote = true)
    {
        return new JsonExtract(tableName, columnName, path, unquote);
    }

    public static JsonExtract JsonExtract(string columnName, string path, bool unquote = true)
    {
        return new JsonExtract(columnName, path, unquote);
    }

    public static JsonExtract JsonExtract(IPhrase phrase, string path, bool unquote = true)
    {
        return new JsonExtract(phrase, path, unquote);
    }

    public static JsonExtract JsonExtract(Where where, string path, bool unquote = true)
    {
        return new JsonExtract(where, path, unquote);
    }

    static public JsonValue JsonValue(object value, ValueObjectType valueType, JsonPathExpression path,
        DataTypeDef returnType = null,
        JsonValue.DefaultAction onEmpty = Phrases.JsonValue.DefaultAction.Value, object onEmptyValue = null,
        JsonValue.DefaultAction onError = Phrases.JsonValue.DefaultAction.Value, object onErrorValue = null)
    {
        return new JsonValue(value, valueType, path,
            returnType, onEmpty, onEmptyValue, onError, onErrorValue);
    }

    static public JsonValue JsonValue(string tableName, string columnName, JsonPathExpression path,
        DataTypeDef returnType = null,
        JsonValue.DefaultAction onEmpty = Phrases.JsonValue.DefaultAction.Value, object onEmptyValue = null,
        JsonValue.DefaultAction onError = Phrases.JsonValue.DefaultAction.Value, object onErrorValue = null)
    {
        return new JsonValue(tableName, columnName, path,
            returnType, onEmpty, onEmptyValue, onError, onErrorValue);
    }

    static public JsonValue JsonValue(string columnName, JsonPathExpression path,
        DataTypeDef returnType = null,
        JsonValue.DefaultAction onEmpty = Phrases.JsonValue.DefaultAction.Value, object onEmptyValue = null,
        JsonValue.DefaultAction onError = Phrases.JsonValue.DefaultAction.Value, object onErrorValue = null)
    {
        return new JsonValue(columnName, path,
            returnType, onEmpty, onEmptyValue, onError, onErrorValue);
    }

    static public JsonValue JsonValue(IPhrase phrase, JsonPathExpression path,
        DataTypeDef returnType = null,
        JsonValue.DefaultAction onEmpty = Phrases.JsonValue.DefaultAction.Value, object onEmptyValue = null,
        JsonValue.DefaultAction onError = Phrases.JsonValue.DefaultAction.Value, object onErrorValue = null)
    {
        return new JsonValue(phrase, path,
            returnType, onEmpty, onEmptyValue, onError, onErrorValue);
    }

    static public JsonValue JsonValue(Where where, JsonPathExpression path,
        DataTypeDef returnType = null,
        JsonValue.DefaultAction onEmpty = Phrases.JsonValue.DefaultAction.Value, object onEmptyValue = null,
        JsonValue.DefaultAction onError = Phrases.JsonValue.DefaultAction.Value, object onErrorValue = null)
    {
        return new JsonValue(where, path,
            returnType, onEmpty, onEmptyValue, onError, onErrorValue);
    }

    static public JsonValue JsonValue(object value, ValueObjectType valueType, string path,
        DataTypeDef returnType = null,
        JsonValue.DefaultAction onEmpty = Phrases.JsonValue.DefaultAction.Value, object onEmptyValue = null,
        JsonValue.DefaultAction onError = Phrases.JsonValue.DefaultAction.Value, object onErrorValue = null)
    {
        return new JsonValue(value, valueType, path,
            returnType, onEmpty, onEmptyValue, onError, onErrorValue);
    }

    static public JsonValue JsonValue(string tableName, string columnName, string path,
        DataTypeDef returnType = null,
        JsonValue.DefaultAction onEmpty = Phrases.JsonValue.DefaultAction.Value, object onEmptyValue = null,
        JsonValue.DefaultAction onError = Phrases.JsonValue.DefaultAction.Value, object onErrorValue = null)
    {
        return new JsonValue(tableName, columnName, path,
            returnType, onEmpty, onEmptyValue, onError, onErrorValue);
    }

    static public JsonValue JsonValue(string columnName, string path,
        DataTypeDef returnType = null,
        JsonValue.DefaultAction onEmpty = Phrases.JsonValue.DefaultAction.Value, object onEmptyValue = null,
        JsonValue.DefaultAction onError = Phrases.JsonValue.DefaultAction.Value, object onErrorValue = null)
    {
        return new JsonValue(columnName, path,
            returnType, onEmpty, onEmptyValue, onError, onErrorValue);
    }

    static public JsonValue JsonValue(IPhrase phrase, string path,
        DataTypeDef returnType = null,
        JsonValue.DefaultAction onEmpty = Phrases.JsonValue.DefaultAction.Value, object onEmptyValue = null,
        JsonValue.DefaultAction onError = Phrases.JsonValue.DefaultAction.Value, object onErrorValue = null)
    {
        return new JsonValue(phrase, path,
            returnType, onEmpty, onEmptyValue, onError, onErrorValue);
    }

    static public JsonValue JsonValue(Where where, string path,
        DataTypeDef returnType = null,
        JsonValue.DefaultAction onEmpty = Phrases.JsonValue.DefaultAction.Value, object onEmptyValue = null,
        JsonValue.DefaultAction onError = Phrases.JsonValue.DefaultAction.Value, object onErrorValue = null)
    {
        return new JsonValue(where, path,
            returnType, onEmpty, onEmptyValue, onError, onErrorValue);
    }

    static public JsonInsert JsonInsert(
        object doc, ValueObjectType docType,
        string path,
        object value, ValueObjectType valueType)
    {
        return new JsonInsert(doc, docType, path, value, valueType);
    }

    static public JsonInsert JsonInsert(
        string docTableName, string docColumnName,
        string path,
        object value, ValueObjectType valueType)
    {
        return new JsonInsert(docTableName, docColumnName, path, value, valueType);
    }

    static public JsonInsert JsonInsert(
        string docColumnName,
        string path,
        object value, ValueObjectType valueType)
    {
        return new JsonInsert(docColumnName, path, value, valueType);
    }

    static public JsonInsert JsonInsert(
        IPhrase doc,
        string path,
        object value, ValueObjectType valueType)
    {
        return new JsonInsert(doc, path, value, valueType);
    }

    static public JsonInsert JsonInsert(
        IPhrase doc,
        string path,
        IPhrase value)
    {
        return new JsonInsert(doc, path, value);
    }

    public static JsonLength JsonLength(object value, ValueObjectType valueType)
    {
        return new JsonLength(value, valueType);
    }

    public static JsonLength JsonLength(string tableName, string columnName)
    {
        return new JsonLength(tableName, columnName);
    }

    public static JsonLength JsonLength(string columnName)
    {
        return new JsonLength(columnName);
    }

    public static JsonLength JsonLength(IPhrase phrase)
    {
        return new JsonLength(phrase);
    }

    public static JsonLength JsonLength(Where where)
    {
        return new JsonLength(where);
    }

    public static JsonObject JsonObject()
    {
        return new JsonObject();
    }

    public static JsonObject JsonObject(Dictionary<ValueWrapper, ValueWrapper> values)
    {
        return new JsonObject(values);
    }

    public static JsonObject JsonObject(Dictionary<string, ValueWrapper> values)
    {
        return new JsonObject(values);
    }

    public static JsonObject JsonObject(Dictionary<string, object> values)
    {
        return new JsonObject(values);
    }

    public static JsonObject JsonObject(Dictionary<string, string> values)
    {
        return new JsonObject(values);
    }

    public static JsonObject JsonObject(Dictionary<string, Int64> values)
    {
        return new JsonObject(values);
    }

    public static JsonObject JsonObject(Dictionary<string, Int32> values)
    {
        return new JsonObject(values);
    }

    public static JsonObjectAggregate JsonObjectAggregate(
        ValueWrapper key, ValueWrapper value,
        bool binary = false)
    {
        return new JsonObjectAggregate(key, value, binary);
    }

    public static JsonObjectAggregate JsonObjectAggregate(
        object key, ValueObjectType keyType,
        object value, ValueObjectType valueType,
        bool binary = false)
    {
        return new JsonObjectAggregate(key, keyType, value, valueType, binary);
    }

    public static JsonObjectAggregate JsonObjectAggregate(ValueWrapper key, IPhrase value, bool binary = false)
    {
        return new JsonObjectAggregate(key, value, binary);
    }

    static public JsonSet JsonSet(
        ValueWrapper doc,
        string path,
        object value, ValueObjectType valueType)
    {
        return new JsonSet(doc, path, value, valueType);
    }

    static public JsonSet JsonSet(
        object doc, ValueObjectType docType,
        string path,
        object value, ValueObjectType valueType)
    {
        return new JsonSet(doc, docType, path, value, valueType);
    }

    static public JsonSet JsonSet(
        string docTableName, string docColumnName,
        string path,
        object value, ValueObjectType valueType)
    {
        return new JsonSet(docTableName, docColumnName, path, value, valueType);
    }

    static public JsonSet JsonSet(
        string docColumnName,
        string path,
        object value, ValueObjectType valueType)
    {
        return new JsonSet(docColumnName, path, value, valueType);
    }

    static public JsonSet JsonSet(
        IPhrase doc,
        string path,
        object value, ValueObjectType valueType)
    {
        return new JsonSet(doc, path, value, valueType);
    }

    static public JsonSet JsonSet(
        IPhrase doc,
        string path,
        IPhrase value)
    {
        return new JsonSet(doc, path, value);
    }

    static public JsonSet JsonSet(
        ValueWrapper doc,
        params JsonPathValue[] pathValues)
    {
        return new JsonSet(doc, pathValues);
    }

    static public JsonSet JsonSet(
        IPhrase doc,
        params JsonPathValue[] pathValues)
    {
        return new JsonSet(doc, pathValues);
    }

    static public JsonRemove JsonRemove(
        ValueWrapper doc,
        params string[] paths)
    {
        return new JsonRemove(doc, paths);
    }

    static public JsonRemove JsonRemove(
        object doc, ValueObjectType docType,
        params string[] paths)
    {
        return new JsonRemove(doc, docType, paths);
    }

    static public JsonRemove JsonRemove(
        string docTableName, string docColumnName,
        params string[] paths)
    {
        return new JsonRemove(docTableName, docColumnName, paths);
    }

    static public JsonRemove JsonRemove(
        string docColumnName,
        params string[] paths)
    {
        return new JsonRemove(docColumnName, paths);
    }

    static public JsonRemove JsonRemove(
        IPhrase doc,
        params string[] paths)
    {
        return new JsonRemove(doc, paths);
    }

    static public MemberOfJsonArray MemberOfJsonArray(ValueWrapper value, ValueWrapper jsonArray)
    {
        return new MemberOfJsonArray(value, jsonArray);
    }

    static public MemberOfJsonArray MemberOfJsonArray(object value, ValueObjectType valueType, object jsonArray, ValueObjectType jsonArrayType)
    {
        return new MemberOfJsonArray(value, valueType, jsonArray, jsonArrayType);
    }

    static public MemberOfJsonArray MemberOfJsonArray(IPhrase value, ValueWrapper jsonArray)
    {
        return new MemberOfJsonArray(value, jsonArray);
    }

    static public MemberOfJsonArray MemberOfJsonArray(ValueWrapper value, IPhrase jsonArray)
    {
        return new MemberOfJsonArray(value, jsonArray);
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
        ValueWrapper first,
        ValueWrapper second)
    {
        return new IfNull(first, second);
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
         string firstTableName, string firstColumnName,
         ValueWrapper second)
    {
        return new IfNull(
            firstTableName, firstColumnName,
            second);
    }

    public static IfNull IfNull(
         object firstValue, ValueObjectType firstValueType,
         string secondTableName, string secondColumnName)
    {
        return new IfNull(
            firstValue, firstValueType,
            secondTableName, secondColumnName);
    }

    public static IfNull IfNull(
         ValueWrapper first,
         string secondTableName, string secondColumnName)
    {
        return new IfNull(
            first,
            secondTableName, secondColumnName);
    }

    public static NullIf NullIf(
        ValueWrapper first,
        ValueWrapper second)
    {
        return new NullIf(first, second);
    }
    
    public static NullIf NullIf(
        string firstTableName, string firstColumnName,
        string secondTableName, string secondColumnName)
    {
        return new NullIf(
            firstTableName, firstColumnName,
            secondTableName, secondColumnName);
    }

    public static NullIf NullIf(
         object firstValue, ValueObjectType firstValueType,
         object secondValue, ValueObjectType secondValueType)
    {
        return new NullIf(
            firstValue, firstValueType,
            secondValue, secondValueType);
    }

    public static NullIf NullIf(
         string firstTableName, string firstColumnName,
         object secondValue, ValueObjectType secondValueType)
    {
        return new NullIf(
            firstTableName, firstColumnName,
            secondValue, secondValueType);
    }

    public static NullIf NullIf(
         string firstTableName, string firstColumnName,
         ValueWrapper second)
    {
        return new NullIf(
            firstTableName, firstColumnName,
            second);
    }

    public static NullIf NullIf(
         object firstValue, ValueObjectType firstValueType,
         string secondTableName, string secondColumnName)
    {
        return new NullIf(
            firstValue, firstValueType,
            secondTableName, secondColumnName);
    }

    public static NullIf NullIf(
         ValueWrapper first,
         string secondTableName, string secondColumnName)
    {
        return new NullIf(
            first,
            secondTableName, secondColumnName);
    }

    #endregion

    #region General

    public static Collate Collate(ValueWrapper value, string collation, SortDirection direction = SortDirection.ASC)
    {
        return new Collate(value, collation, direction);
    }

    public static Collate Collate(object value, ValueObjectType valueType, string collation, SortDirection direction = SortDirection.ASC)
    {
        return new Collate(value, valueType, collation, direction);
    }

    public static Collate Collate(string tableName, string columnName, string collation, SortDirection direction = SortDirection.ASC)
    {
        return new Collate(tableName, columnName, collation, direction);
    }

    public static Collate Collate(string columnName, string collation, SortDirection direction = SortDirection.ASC)
    {
        return new Collate(columnName, collation, direction);
    }

    public static Collate Collate(IPhrase phrase, string collation, SortDirection direction = SortDirection.ASC)
    {
        return new Collate(phrase, collation, direction);
    }

    public static Cast Cast(ValueWrapper value, DataTypeDef typeDef)
    {
        return new Cast(value, typeDef);
    }

    public static Cast Cast(object value, ValueObjectType valueType, DataTypeDef typeDef)
    {
        return new Cast(value, valueType, typeDef);
    }

    public static Cast Cast(string tableName, string columnName, DataTypeDef typeDef)
    {
        return new Cast(tableName, columnName, typeDef);
    }

    public static Cast Cast(string columnName, DataTypeDef typeDef)
    {
        return new Cast(columnName, typeDef);
    }

    public static Cast Cast(IPhrase phrase, DataTypeDef typeDef)
    {
        return new Cast(phrase, typeDef);
    }

    public static Literal Literal(Literal.BuilderDelegate builder)
    {
        return new Literal(builder);
    }

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

    public static Exists Exists(Query query)
    {
        return new Exists(query);
    }

    public static NotExists NotExists(Query query)
    {
        return new NotExists(query);
    }

    #endregion

    #region Math

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

    public static Add Add(object value1, object value2)
    {
        return new Add(value1, value2);
    }

    public static Subtract Subtract(
        string tableName1, string columnName1,
        string tableName2, string columnName2)
    {
        return new Subtract(
            tableName1, columnName1,
            tableName2, columnName2);
    }

    public static Subtract Subtract(
        string tableName1, string columnName1,
        object value2, ValueObjectType valueType2)
    {
        return new Subtract(
            tableName1, columnName1,
            value2, valueType2);
    }

    public static Subtract Subtract(
        object value1, ValueObjectType valueType1,
        string tableName2, string columnName2)
    {
        return new Subtract(
            value1, valueType1,
            tableName2, columnName2);
    }

    public static Subtract Subtract(
        object value1, ValueObjectType valueType1,
        object value2, ValueObjectType valueType2)
    {
        return new Subtract(
            value1, valueType1,
            value2, valueType2);
    }

    public static Subtract Subtract(string tableName1, string columnName1, Int32 value2)
    {
        return new Subtract(
            tableName1, columnName1,
            value2);
    }

    public static Subtract Subtract(string tableName1, string columnName1, Int64 value2)
    {
        return new Subtract(
            tableName1, columnName1,
            value2);
    }

    public static Subtract Subtract(string tableName1, string columnName1, decimal value2)
    {
        return new Subtract(
            tableName1, columnName1,
            value2);
    }

    public static Subtract Subtract(string tableName1, string columnName1, double value2)
    {
        return new Subtract(
            tableName1, columnName1,
            value2);
    }

    public static Subtract Subtract(string tableName1, string columnName1, float value2)
    {
        return new Subtract(
            tableName1, columnName1,
            value2);
    }

    public static Subtract Subtract(string columnName1, Int32 value2)
    {
        return new Subtract(
            columnName1,
            value2);
    }

    public static Subtract Subtract(string columnName1, Int64 value2)
    {
        return new Subtract(
            columnName1,
            value2);
    }

    public static Subtract Subtract(string columnName1, decimal value2)
    {
        return new Subtract(
            columnName1,
            value2);
    }

    public static Subtract Subtract(string columnName1, double value2)
    {
        return new Subtract(
            columnName1,
            value2);
    }

    public static Subtract Subtract(string columnName1, float value2)
    {
        return new Subtract(
            columnName1,
            value2);
    }

    public static Subtract Subtract(object value1, object value2)
    {
        return new Subtract(value1, value2);
    }

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
    
    public static Greatest Greatest(
        object value1, ValueObjectType valueType1,
        object value2, ValueObjectType valueType2
        )
    {
        return new Greatest(
            value1, valueType1,
            value2, valueType2);
    }

    public static Greatest Greatest(
        string tableName1, string column1,
        string tableName2, string column2
        )
    {
        return new Greatest(
            tableName1, column1,
            tableName2, column2);
    }

    public static Greatest Greatest(
        string tableName1, string column1,
        object value2, ValueObjectType valueType2
        )
    {
        return new Greatest(
            tableName1, column1,
            value2, valueType2);
    }

    public static Greatest Greatest(
        string tableName1, string column1,
        object value2
        )
    {
        return new Greatest(
            tableName1, column1,
            value2);
    }

    public static Greatest Greatest(
        object value1, ValueObjectType valueType1,
        string tableName2, string column2
        )
    {
        return new Greatest(
            value1, valueType1,
            tableName2, column2);
    }

    public static Greatest Greatest(
        object value1,
        string tableName2, string column2
        )
    {
        return new Greatest(
            value1,
            tableName2, column2);
    }

    public static Greatest Greatest(
        object value1,
        object value2
        )
    {
        return new Greatest(value1, value2);
    }

    public static Least Least(
        object value1, ValueObjectType valueType1,
        object value2, ValueObjectType valueType2
        )
    {
        return new Least(
            value1, valueType1,
            value2, valueType2);
    }

    public static Least Least(
        string tableName1, string column1,
        string tableName2, string column2
        )
    {
        return new Least(
            tableName1, column1,
            tableName2, column2);
    }

    public static Least Least(
        string tableName1, string column1,
        object value2, ValueObjectType valueType2
        )
    {
        return new Least(
            tableName1, column1,
            value2, valueType2);
    }

    public static Least Least(
        string tableName1, string column1,
        object value2
        )
    {
        return new Least(
            tableName1, column1,
            value2);
    }

    public static Least Least(
        object value1, ValueObjectType valueType1,
        string tableName2, string column2
        )
    {
        return new Least(
            value1, valueType1,
            tableName2, column2);
    }

    public static Least Least(
        object value1,
        string tableName2, string column2
        )
    {
        return new Least(
            value1,
            tableName2, column2);
    }

    public static Least Least(
        object value1,
        object value2
        )
    {
        return new Least(value1, value2);
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

    public static Floor Floor(object value, ValueObjectType valueType)
    {
        return new Floor(value, valueType);
    }

    public static Floor Floor(string tableName, string columnName)
    {
        return new Floor(tableName, columnName);
    }

    public static Floor Floor(string columnName)
    {
        return new Floor(columnName);
    }

    public static Floor Floor(IPhrase phrase)
    {
        return new Floor(phrase);
    }

    public static Floor Floor(Where where)
    {
        return new Floor(where);
    }

    public static Ceil Ceil(object value, ValueObjectType valueType)
    {
        return new Ceil(value, valueType);
    }

    public static Ceil Ceil(string tableName, string columnName)
    {
        return new Ceil(tableName, columnName);
    }

    public static Ceil Ceil(string columnName)
    {
        return new Ceil(columnName);
    }

    public static Ceil Ceil(IPhrase phrase)
    {
        return new Ceil(phrase);
    }

    public static Ceil Ceil(Where where)
    {
        return new Ceil(where);
    }

    #endregion

    #region Aggregate

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

    public static Every Every(object value, ValueObjectType valueType)
    {
        return new Every(value, valueType);
    }

    public static Every Every(string tableName, string columnName)
    {
        return new Every(tableName, columnName);
    }

    public static Every Every(string columnName)
    {
        return new Every(columnName);
    }

    public static Every Every(IPhrase phrase)
    {
        return new Every(phrase);
    }

    public static Every Every(Where where)
    {
        return new Every(where);
    }

    public static Every Every(WhereList where)
    {
        return new Every(where);
    }

    public static GroupConcat GroupConcat(bool distinct, string tableName, string columnName, string separator, OrderByList orderBy = null)
    {
        return new GroupConcat(distinct, tableName, columnName, separator, orderBy);
    }

    public static GroupConcat GroupConcat(bool distinct, string tableName, string columnName, char separator, OrderByList orderBy = null)
    {
        return new GroupConcat(distinct, tableName, columnName, separator, orderBy);
    }

    public static GroupConcat GroupConcat(bool distinct, string tableName, string columnName, OrderByList orderBy = null)
    {
        return new GroupConcat(distinct, tableName, columnName, orderBy);
    }

    public static GroupConcat GroupConcat(bool distinct, ValueWrapper value, string separator, OrderByList orderBy = null)
    {
        return new GroupConcat(distinct, value, separator, orderBy);
    }

    public static GroupConcat GroupConcat(bool distinct, ValueWrapper value, char separator, OrderByList orderBy = null)
    {
        return new GroupConcat(distinct, value, separator, orderBy);
    }

    public static GroupConcat GroupConcat(bool distinct, ValueWrapper value, OrderByList orderBy = null)
    {
        return new GroupConcat(distinct, value, orderBy);
    }

    public static GroupConcat GroupConcat(string tableName, string columnName, string separator, OrderByList orderBy = null)
    {
        return new GroupConcat(tableName, columnName, separator, orderBy);
    }

    public static GroupConcat GroupConcat(string tableName, string columnName, char separator, OrderByList orderBy = null)
    {
        return new GroupConcat(tableName, columnName, separator, orderBy);
    }

    public static GroupConcat GroupConcat(string tableName, string columnName, OrderByList orderBy = null)
    {
        return new GroupConcat(tableName, columnName, orderBy);
    }

    public static GroupConcat GroupConcat(ValueWrapper value, string separator, OrderByList orderBy = null)
    {
        return new GroupConcat(value, separator, orderBy);
    }

    public static GroupConcat GroupConcat(ValueWrapper value, char separator, OrderByList orderBy = null)
    {
        return new GroupConcat(value, separator, orderBy);
    }

    public static GroupConcat GroupConcat(ValueWrapper value, OrderByList orderBy = null)
    {
        return new GroupConcat(value, orderBy);
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

    public static Max Max(IPhrase phrase, bool distinct = false)
    {
        return new Max(phrase, distinct);
    }

    public static Max Max(Where where, bool distinct = false)
    {
        return new Max(where, distinct);
    }

    public static Max Max(WhereList where, bool distinct = false)
    {
        return new Max(where, distinct);
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

    public static Min Min(IPhrase phrase, bool distinct = false)
    {
        return new Min(phrase, distinct);
    }

    public static Min Min(Where where, bool distinct = false)
    {
        return new Min(where, distinct);
    }

    public static Min Min(WhereList where, bool distinct = false)
    {
        return new Min(where, distinct);
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

    public static Some Some(object value, ValueObjectType valueType)
    {
        return new Some(value, valueType);
    }

    public static Some Some(string tableName, string columnName)
    {
        return new Some(tableName, columnName);
    }

    public static Some Some(string columnName)
    {
        return new Some(columnName);
    }

    public static Some Some(IPhrase phrase)
    {
        return new Some(phrase);
    }

    public static Some Some(Where where)
    {
        return new Some(where);
    }

    public static Some Some(WhereList where)
    {
        return new Some(where);
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

    public static Sum Sum(IPhrase phrase, bool distinct = false)
    {
        return new Sum(phrase, distinct);
    }

    public static Sum Sum(Where where, bool distinct = false)
    {
        return new Sum(where, distinct);
    }

    public static Sum Sum(WhereList where, bool distinct = false)
    {
        return new Sum(where, distinct);
    }

    #endregion

    #region Spatial

    public static GeographyContains GeographyContains(ValueWrapper outer, ValueWrapper inner)
    {
        return new GeographyContains(outer, inner);
    }

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

    public static GeographyDistance GeographyDistance(ValueWrapper from, ValueWrapper to)
    {
        return new GeographyDistance(from, to);
    }

    public static GeographyDistance GeographyDistance(
        object outerValue, ValueObjectType outerValueType,
        object innerValue, ValueObjectType innerValueType)
    {
        return new GeographyDistance(
            outerValue, outerValueType,
            innerValue, innerValueType);
    }

    public static GeographyDistance GeographyDistance(
        object outerValue, ValueObjectType outerValueType,
        string innerColumnName)
    {
        return new GeographyDistance(
            outerValue, outerValueType,
            innerColumnName);
    }

    public static GeographyDistance GeographyDistance(
        object outerValue, ValueObjectType outerValueType,
        string innerTableName, string innerColumnName)
    {
        return new GeographyDistance(
            outerValue, outerValueType,
            innerTableName, innerColumnName);
    }

    public static GeographyDistance GeographyDistance(
        Geometry outerValue,
        string innerColumnName)
    {
        return new GeographyDistance(
            outerValue,
            innerColumnName);
    }

    public static GeographyDistance GeographyDistance(
        Geometry outerValue,
        string innerTableName, string innerColumnName)
    {
        return new GeographyDistance(
            outerValue,
            innerTableName, innerColumnName);
    }

    public static GeographyDistance GeographyDistance(Geometry outerValue, Geometry innerValue)
    {
        return new GeographyDistance(
            outerValue,
            innerValue);
    }

    public static GeographyDistance GeographyDistance(
        string outerColumnName,
        Geometry innerObject)
    {
        return new GeographyDistance(
            outerColumnName,
            innerObject);
    }

    public static GeographyDistance GeographyDistance(
        string outerTableName, string outerColumnName,
        Geometry innerObject)
    {
        return new GeographyDistance(
            outerTableName, outerColumnName,
            innerObject);
    }

    public static GeographySphericalDistance GeographySphericalDistance(ValueWrapper from, ValueWrapper to)
    {
        return new GeographySphericalDistance(from, to);
    }

    public static GeographySphericalDistance GeographySphericalDistance(
        object outerValue, ValueObjectType outerValueType,
        object innerValue, ValueObjectType innerValueType)
    {
        return new GeographySphericalDistance(
            outerValue, outerValueType,
            innerValue, innerValueType);
    }

    public static GeographySphericalDistance GeographySphericalDistance(
        object outerValue, ValueObjectType outerValueType,
        string innerColumnName)
    {
        return new GeographySphericalDistance(
            outerValue, outerValueType,
            innerColumnName);
    }

    public static GeographySphericalDistance GeographySphericalDistance(
        object outerValue, ValueObjectType outerValueType,
        string innerTableName, string innerColumnName)
    {
        return new GeographySphericalDistance(
            outerValue, outerValueType,
            innerTableName, innerColumnName);
    }

    public static GeographySphericalDistance GeographySphericalDistance(
        Geometry outerValue,
        string innerColumnName)
    {
        return new GeographySphericalDistance(
            outerValue,
            innerColumnName);
    }

    public static GeographySphericalDistance GeographySphericalDistance(
        Geometry outerValue,
        string innerTableName, string innerColumnName)
    {
        return new GeographySphericalDistance(
            outerValue,
            innerTableName, innerColumnName);
    }

    public static GeographySphericalDistance GeographySphericalDistance(Geometry outerValue, Geometry innerValue)
    {
        return new GeographySphericalDistance(
            outerValue,
            innerValue);
    }

    public static GeographySphericalDistance GeographySphericalDistance(
        string outerColumnName,
        Geometry innerObject)
    {
        return new GeographySphericalDistance(
            outerColumnName,
            innerObject);
    }

    public static GeographySphericalDistance GeographySphericalDistance(
        string outerTableName, string outerColumnName,
        Geometry innerObject)
    {
        return new GeographySphericalDistance(
            outerTableName, outerColumnName,
            innerObject);
    }

    public static GeographySphericalDistanceMath GeographySphericalDistance(
        ValueWrapper fromLat, ValueWrapper fromLng,
        ValueWrapper toLat, ValueWrapper toLng
        )
    {
        return new GeographySphericalDistanceMath(fromLat, fromLng, toLat, toLng);
    }

    public static ST_X ST_X(string tableName, string columnName)
    {
        return new ST_X(tableName, columnName);
    }
    
    public static ST_X ST_X(IPhrase phrase)
    {
        return new ST_X(phrase);
    }

    public static ST_X ST_X(ValueWrapper value)
    {
        return new ST_X(value);
    }

    public static ST_Y ST_Y(string tableName, string columnName)
    {
        return new ST_Y(tableName, columnName);
    }
    
    public static ST_Y ST_Y(IPhrase phrase)
    {
        return new ST_Y(phrase);
    }
    
    public static ST_Y ST_Y(ValueWrapper value)
    {
        return new ST_Y(value);
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
        ValueWrapper source,
        string searchTableName, string searchColumn,
        string replaceWithTableName, string replaceWithColumn)
    {
        return new Replace(
            source,
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
        ValueWrapper search,
        string replaceWithTableName, string replaceWithColumn)
    {
        return new Replace(
            sourceTableName, sourceColumn,
            search,
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
        string sourceTableName, string sourceColumn,
        string searchTableName, string searchColumn,
        ValueWrapper replace)
    {
        return new Replace(
            sourceTableName, sourceColumn,
            searchTableName, searchColumn,
            replace);
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
        ValueWrapper source,
        ValueWrapper search,
        string replaceWithTableName, string replaceWithColumn)
    {
        return new Replace(
            source,
            search,
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
        string sourceTableName, string sourceColumn,
        ValueWrapper search,
        ValueWrapper replace)
    {
        return new Replace(
            sourceTableName, sourceColumn,
            search,
            replace);
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

    public static Replace Replace(ValueWrapper source, ValueWrapper search, ValueWrapper replace)
    {
        return new Replace(source, search, replace);
    }
    
    public static Substring Substring(object value, ValueObjectType valueType, int from, int? length = null)
    {
        return new Substring(value, valueType, from, length);
    }

    public static Substring Substring(object value, ValueObjectType valueType, ValueWrapper from, ValueWrapper? length = null)
    {
        return new Substring(value, valueType, from, length);
    }
    
    public static Substring Substring(ValueWrapper value, ValueWrapper from, ValueWrapper? length = null)
    {
        return new Substring(value, from, length);
    }

    public static Substring Substring(string tableName, string columnName, int from, int? length = null)
    {
        return new Substring(tableName, columnName, from, length);
    }

    public static Substring Substring(string columnName, int from, int? length = null)
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

    public static FindString FindString(
        ValueWrapper needle,
        ValueWrapper haystack,
        ValueWrapper? startAt = null)
    {
        return new FindString(needle, haystack, startAt);
    }

    public static FindString FindString(
        ValueWrapper needle,
        ValueWrapper haystack,
        int startAt)
    {
        return new FindString(needle, haystack, startAt);
    }

    #endregion
}
