using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class Replace : BasePhrase
    {
        string Source;
        string SourceTableName;
        ValueObjectType SourceType;
        string Search;
        string SearchTableName;
        ValueObjectType SearchType;
        string ReplaceWith;
        string ReplaceWithTableName;
        ValueObjectType ReplaceWithType;

        public Replace(
            string SourceTableName, string Source, ValueObjectType SourceType,
            string SearchTableName, string Search, ValueObjectType SearchType,
            string ReplaceWithTableName, string ReplaceWith, ValueObjectType ReplaceWithType)
        {
            this.SourceTableName = SourceTableName;
            this.Source = Source;
            this.SourceType = SourceType;
            this.SearchTableName = SearchTableName;
            this.Search = Search;
            this.SearchType = SearchType;
            this.ReplaceWithTableName = ReplaceWithTableName;
            this.ReplaceWith = ReplaceWith;
            this.ReplaceWithType = ReplaceWithType;
        }
        public Replace(
             string Source, ValueObjectType SourceType,
             string Search, ValueObjectType SearchType,
             string ReplaceWith, ValueObjectType ReplaceWithType)
            : this(
            null, Source, SourceType,
            null, Search, SearchType,
            null, ReplaceWith, ReplaceWithType)
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
