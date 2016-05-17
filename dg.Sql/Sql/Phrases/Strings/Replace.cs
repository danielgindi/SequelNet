using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class Replace : IPhrase
    {
        public string SourceTableName;
        public object SourceValue;
        public ValueObjectType SourceType;
        public string SearchTableName;
        public object SearchValue;
        public ValueObjectType SearchType;
        public string ReplaceWithTableName;
        public object ReplaceWithValue;
        public ValueObjectType ReplaceWithType;

        #region Constructors

        [Obsolete]
        public Replace(
            string sourceTableName, string source, ValueObjectType sourceType,
            string searchTableName, string search, ValueObjectType searchType,
            string replaceWithTableName, string replaceWith, ValueObjectType replaceWithType)
        {
            this.SourceTableName = sourceTableName;
            this.SourceValue = source;
            this.SourceType = sourceType;
            this.SearchTableName = searchTableName;
            this.SearchValue = search;
            this.SearchType = searchType;
            this.ReplaceWithTableName = replaceWithTableName;
            this.ReplaceWithValue = replaceWith;
            this.ReplaceWithType = replaceWithType;
        }

        public Replace(
            string sourceTableName, string sourceColumn,
            string searchTableName, string searchColumn,
            string replaceWithTableName, string replaceWithColumn)
        {
            this.SourceTableName = sourceTableName;
            this.SourceValue = sourceColumn;
            this.SourceType = ValueObjectType.ColumnName;
            this.SearchTableName = searchTableName;
            this.SearchValue = searchColumn;
            this.SearchType = ValueObjectType.ColumnName;
            this.ReplaceWithTableName = replaceWithTableName;
            this.ReplaceWithValue = replaceWithColumn;
            this.ReplaceWithType = ValueObjectType.ColumnName;
        }

        public Replace(
            object source, ValueObjectType sourceType,
            string searchTableName, string searchColumn,
            string replaceWithTableName, string replaceWithColumn)
        {
            this.SourceValue = source;
            this.SourceType = sourceType;
            this.SearchTableName = searchTableName;
            this.SearchValue = searchColumn;
            this.SearchType = ValueObjectType.ColumnName;
            this.ReplaceWithTableName = replaceWithTableName;
            this.ReplaceWithValue = replaceWithColumn;
            this.ReplaceWithType = ValueObjectType.ColumnName;
        }

        public Replace(
            string sourceTableName, string sourceColumn,
            object search, ValueObjectType searchType,
            string replaceWithTableName, string replaceWithColumn)
        {
            this.SourceTableName = sourceTableName;
            this.SourceValue = sourceColumn;
            this.SourceType = ValueObjectType.ColumnName;
            this.SearchValue = search;
            this.SearchType = searchType;
            this.ReplaceWithTableName = replaceWithTableName;
            this.ReplaceWithValue = replaceWithColumn;
            this.ReplaceWithType = ValueObjectType.ColumnName;
        }

        public Replace(
            string sourceTableName, string sourceColumn,
            string searchTableName, string searchColumn,
            object replace, ValueObjectType replaceWithType)
        {
            this.SourceTableName = sourceTableName;
            this.SourceValue = sourceColumn;
            this.SourceType = ValueObjectType.ColumnName;
            this.SearchTableName = searchTableName;
            this.SearchValue = searchColumn;
            this.SearchType = ValueObjectType.ColumnName;
            this.ReplaceWithValue = replace;
            this.ReplaceWithType = replaceWithType;
        }

        public Replace(
            object source, ValueObjectType sourceType,
            object search, ValueObjectType searchType,
            string replaceWithTableName, string replaceWithColumn)
        {
            this.SourceValue = source;
            this.SourceType = sourceType;
            this.SearchValue = search;
            this.SearchType = searchType;
            this.ReplaceWithTableName = replaceWithTableName;
            this.ReplaceWithValue = replaceWithColumn;
            this.ReplaceWithType = ValueObjectType.ColumnName;
        }

        public Replace(
            string sourceTableName, string sourceColumn,
            object search, ValueObjectType searchType,
            object replace, ValueObjectType replaceWithType)
        {
            this.SourceTableName = sourceTableName;
            this.SourceValue = sourceColumn;
            this.SourceType = ValueObjectType.ColumnName;
            this.SearchValue = search;
            this.SearchType = searchType;
            this.ReplaceWithValue = replace;
            this.ReplaceWithType = replaceWithType;
        }

        public Replace(
            object source, ValueObjectType sourceType,
            object search, ValueObjectType searchType,
            object replace, ValueObjectType replaceWithType)
        {
            this.SourceValue = source;
            this.SourceType = sourceType;
            this.SearchValue = search;
            this.SearchType = searchType;
            this.ReplaceWithValue = replace;
            this.ReplaceWithType = replaceWithType;
        }
        
        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret = @"REPLACE(";

            if (SourceType == ValueObjectType.ColumnName)
            {
                if (SourceTableName != null && SourceTableName.Length > 0)
                {
                    ret += conn.EncloseFieldName(SourceTableName);
                    ret += ".";
                }
                ret += conn.EncloseFieldName(SourceValue.ToString());
            }
            else if (SourceType == ValueObjectType.Value)
            {
                ret += conn.PrepareValue(SourceValue);
            }
            else ret += SourceValue;

            ret += ",";

            if (SearchType == ValueObjectType.ColumnName)
            {
                if (SearchTableName != null && SearchTableName.Length > 0)
                {
                    ret += conn.EncloseFieldName(SearchTableName);
                    ret += ".";
                }
                ret += conn.EncloseFieldName(SearchValue.ToString());
            }
            else if (SearchType == ValueObjectType.Value)
            {
                ret += conn.PrepareValue(SearchValue);
            }
            else ret += SearchValue;

            ret += ",";

            if (ReplaceWithType == ValueObjectType.ColumnName)
            {
                if (ReplaceWithTableName != null && ReplaceWithTableName.Length > 0)
                {
                    ret += conn.EncloseFieldName(ReplaceWithTableName);
                    ret += ".";
                }
                ret += conn.EncloseFieldName(ReplaceWithValue.ToString());
            }
            else if (ReplaceWithType == ValueObjectType.Value)
            {
                ret += conn.PrepareValue(ReplaceWithValue);
            }
            else ret += ReplaceWithValue;

            ret += ")";

            return ret;
        }
    }
}
