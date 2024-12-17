namespace SequelNet.Phrases;

#nullable enable

public class CountDistinct : Count
{
    #region Constructors

    public CountDistinct()
        : base(true)
    {
    }

    public CountDistinct(string? tableName, string columnName)
        : base(tableName, columnName, true)
    {
    }

    public CountDistinct(string columnName)
        : base(null, columnName, true)
    {
    }

    public CountDistinct(object? value, ValueObjectType valueType)
        : base(value, valueType, true)
    {
    }

    public CountDistinct(IPhrase phrase)
        : base(phrase, true)
    {
    }

    #endregion
}
