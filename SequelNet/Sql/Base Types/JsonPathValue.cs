﻿#nullable enable

namespace SequelNet;

public class JsonPathValue
{
    public string? Path;
    public ValueWrapper Value;

    #region Constructors

    public JsonPathValue()
    {
        this.Value = new ValueWrapper();
    }

    public JsonPathValue(string path, string? tableName, string columnName)
    {
        this.Path = path;
        this.Value = ValueWrapper.Column(tableName, columnName);
    }

    public JsonPathValue(string path, string column)
    {
        this.Path = path;
        this.Value = ValueWrapper.Column(column);
    }

    public JsonPathValue(string path, object? value, ValueObjectType type)
    {
        this.Path = path;
        this.Value = ValueWrapper.Make(value, type);
    }

    public JsonPathValue(string path, IPhrase value)
    {
        this.Path = path;
        this.Value = ValueWrapper.From(value);
    }

    public JsonPathValue(string path, ValueWrapper value)
    {
        this.Path = path;
        this.Value = value;
    }

    #endregion

    #region Convenience

    public static JsonPathValue From(string path, string? tableName, string columnName)
    {
        return new JsonPathValue(path, tableName, columnName);
    }

    public static JsonPathValue From(string path, string column)
    {
        return new JsonPathValue(path, column);
    }

    public static JsonPathValue From(string path, object? value, ValueObjectType type)
    {
        return new JsonPathValue(path, value, type);
    }
    
    public static JsonPathValue From(string path, IPhrase value)
    {
        return new JsonPathValue(path, value);
    }

    public static JsonPathValue From(string path, ValueWrapper value)
    {
        return new JsonPathValue(path, value);
    }

    #endregion
}
