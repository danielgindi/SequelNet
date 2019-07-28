using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class Replace : IPhrase
    {
        public ValueWrapper SourceValue;
        public ValueWrapper SearchValue;
        public ValueWrapper ReplaceWithValue;

        #region Constructors

        public Replace(
            string sourceTableName, string sourceColumn,
            string searchTableName, string searchColumn,
            string replaceWithTableName, string replaceWithColumn)
        {
            this.SourceValue = new ValueWrapper(sourceTableName, sourceColumn);
            this.SearchValue = new ValueWrapper(searchTableName, searchColumn);
            this.ReplaceWithValue = new ValueWrapper(replaceWithTableName, replaceWithColumn);
        }

        public Replace(
            object source, ValueObjectType sourceType,
            string searchTableName, string searchColumn,
            string replaceWithTableName, string replaceWithColumn)
        {
            this.SourceValue = new ValueWrapper(source, sourceType);
            this.SearchValue = new ValueWrapper(searchTableName, searchColumn);
            this.ReplaceWithValue = new ValueWrapper(replaceWithTableName, replaceWithColumn);
        }

        public Replace(
            string sourceTableName, string sourceColumn,
            object search, ValueObjectType searchType,
            string replaceWithTableName, string replaceWithColumn)
        {
            this.SourceValue = new ValueWrapper(sourceTableName, sourceColumn);
            this.SearchValue = new ValueWrapper(search, searchType);
            this.ReplaceWithValue = new ValueWrapper(replaceWithTableName, replaceWithColumn);
        }

        public Replace(
            string sourceTableName, string sourceColumn,
            string searchTableName, string searchColumn,
            object replace, ValueObjectType replaceWithType)
        {
            this.SourceValue = new ValueWrapper(sourceTableName, sourceColumn);
            this.SearchValue = new ValueWrapper(searchTableName, searchColumn);
            this.ReplaceWithValue = new ValueWrapper(replace, replaceWithType);
        }

        public Replace(
            object source, ValueObjectType sourceType,
            object search, ValueObjectType searchType,
            string replaceWithTableName, string replaceWithColumn)
        {
            this.SourceValue = new ValueWrapper(source, sourceType);
            this.SearchValue = new ValueWrapper(search, searchType);
            this.ReplaceWithValue = new ValueWrapper(replaceWithTableName, replaceWithColumn);
        }

        public Replace(
            string sourceTableName, string sourceColumn,
            object search, ValueObjectType searchType,
            object replace, ValueObjectType replaceWithType)
        {
            this.SourceValue = new ValueWrapper(sourceTableName, sourceColumn);
            this.SearchValue = new ValueWrapper(search, searchType);
            this.ReplaceWithValue = new ValueWrapper(replace, replaceWithType);
        }

        public Replace(
            object source, ValueObjectType sourceType,
            object search, ValueObjectType searchType,
            object replace, ValueObjectType replaceWithType)
        {
            this.SourceValue = new ValueWrapper(source, sourceType);
            this.SearchValue = new ValueWrapper(search, searchType);
            this.ReplaceWithValue = new ValueWrapper(replace, replaceWithType);
        }
        
        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret = @"REPLACE(";

            ret += SourceValue.Build(conn, relatedQuery);

            ret += ",";

            ret += SearchValue.Build(conn, relatedQuery);

            ret += ",";

            ret += ReplaceWithValue.Build(conn, relatedQuery);

            ret += ")";

            return ret;
        }
    }
}
