﻿using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases;

public class Month : IPhrase
{
    public ValueWrapper Value;

    #region Constructors

    public Month(object value, ValueObjectType valueType)
    {
        this.Value = ValueWrapper.Make(value, valueType);
    }

    public Month(string tableName, string columnName)
    {
        this.Value = ValueWrapper.Column(tableName, columnName);
    }

    public Month(string columnName)
        : this(null, columnName)
    {
    }

    public Month(IPhrase phrase)
        : this(phrase, ValueObjectType.Value)
    {
    }

    public Month(ValueWrapper value)
    {
        this.Value = value;
    }

    #endregion

    public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
    {
        sb.Append(conn.Language.MonthPartOfDateTime(Value.Build(conn, relatedQuery)));
    }
    }
}
