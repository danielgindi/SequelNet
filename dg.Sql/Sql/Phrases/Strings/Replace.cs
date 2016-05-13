using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class Replace : IPhrase
    {
        public string Source;
        public string SourceTableName;
        public ValueObjectType SourceType;
        public string Search;
        public string SearchTableName;
        public ValueObjectType SearchType;
        public string ReplaceWith;
        public string ReplaceWithTableName;
        public ValueObjectType ReplaceWithType;

        public Replace(
            string sourceTableName, string source, ValueObjectType sourceType,
            string searchTableName, string search, ValueObjectType searchType,
            string replaceWithTableName, string replaceWith, ValueObjectType replaceWithType)
        {
            this.SourceTableName = sourceTableName;
            this.Source = source;
            this.SourceType = sourceType;
            this.SearchTableName = searchTableName;
            this.Search = search;
            this.SearchType = searchType;
            this.ReplaceWithTableName = replaceWithTableName;
            this.ReplaceWith = replaceWith;
            this.ReplaceWithType = replaceWithType;
        }

        public Replace(
             string source, ValueObjectType sourceType,
             string search, ValueObjectType searchType,
             string replaceWith, ValueObjectType replaceWithType)
            : this(
            null, source, sourceType,
            null, search, searchType,
            null, replaceWith, replaceWithType)
        {
        }

        public string BuildPhrase(ConnectorBase conn)
        {
            string ret = @"REPLACE(";

            if (SourceType == ValueObjectType.ColumnName)
            {
                if (SourceTableName != null && SourceTableName.Length > 0)
                {
                    ret += conn.EncloseFieldName(SourceTableName);
                    ret += ".";
                }
                ret += conn.EncloseFieldName(Source);
            }
            else if (SourceType == ValueObjectType.Value)
            {
                ret += conn.PrepareValue(Source);
            }
            else ret += Source;

            ret += ",";

            if (SearchType == ValueObjectType.ColumnName)
            {
                if (SearchTableName != null && SearchTableName.Length > 0)
                {
                    ret += conn.EncloseFieldName(SearchTableName);
                    ret += ".";
                }
                ret += conn.EncloseFieldName(Search);
            }
            else if (SearchType == ValueObjectType.Value)
            {
                ret += conn.PrepareValue(Search);
            }
            else ret += Search;

            ret += ",";

            if (ReplaceWithType == ValueObjectType.ColumnName)
            {
                if (ReplaceWithTableName != null && ReplaceWithTableName.Length > 0)
                {
                    ret += conn.EncloseFieldName(ReplaceWithTableName);
                    ret += ".";
                }
                ret += conn.EncloseFieldName(ReplaceWith);
            }
            else if (ReplaceWithType == ValueObjectType.Value)
            {
                ret += conn.PrepareValue(ReplaceWith);
            }
            else ret += ReplaceWith;

            ret += ")";

            return ret;
        }
    }
}
