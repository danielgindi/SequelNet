using SequelNet.Connector;

namespace SequelNet.Phrases
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
            this.SourceValue = ValueWrapper.Column(sourceTableName, sourceColumn);
            this.SearchValue = ValueWrapper.Column(searchTableName, searchColumn);
            this.ReplaceWithValue = ValueWrapper.Column(replaceWithTableName, replaceWithColumn);
        }

        public Replace(
            object source, ValueObjectType sourceType,
            string searchTableName, string searchColumn,
            string replaceWithTableName, string replaceWithColumn)
        {
            this.SourceValue = ValueWrapper.Make(source, sourceType);
            this.SearchValue = ValueWrapper.Column(searchTableName, searchColumn);
            this.ReplaceWithValue = ValueWrapper.Column(replaceWithTableName, replaceWithColumn);
        }

        public Replace(
            string sourceTableName, string sourceColumn,
            object search, ValueObjectType searchType,
            string replaceWithTableName, string replaceWithColumn)
        {
            this.SourceValue = ValueWrapper.Column(sourceTableName, sourceColumn);
            this.SearchValue = ValueWrapper.Make(search, searchType);
            this.ReplaceWithValue = ValueWrapper.Column(replaceWithTableName, replaceWithColumn);
        }

        public Replace(
            string sourceTableName, string sourceColumn,
            string searchTableName, string searchColumn,
            object replace, ValueObjectType replaceWithType)
        {
            this.SourceValue = ValueWrapper.Column(sourceTableName, sourceColumn);
            this.SearchValue = ValueWrapper.Column(searchTableName, searchColumn);
            this.ReplaceWithValue = ValueWrapper.Make(replace, replaceWithType);
        }

        public Replace(
            object source, ValueObjectType sourceType,
            object search, ValueObjectType searchType,
            string replaceWithTableName, string replaceWithColumn)
        {
            this.SourceValue = ValueWrapper.Make(source, sourceType);
            this.SearchValue = ValueWrapper.Make(search, searchType);
            this.ReplaceWithValue = ValueWrapper.Column(replaceWithTableName, replaceWithColumn);
        }

        public Replace(
            string sourceTableName, string sourceColumn,
            object search, ValueObjectType searchType,
            object replace, ValueObjectType replaceWithType)
        {
            this.SourceValue = ValueWrapper.Column(sourceTableName, sourceColumn);
            this.SearchValue = ValueWrapper.Make(search, searchType);
            this.ReplaceWithValue = ValueWrapper.Make(replace, replaceWithType);
        }

        public Replace(
            object source, ValueObjectType sourceType,
            object search, ValueObjectType searchType,
            object replace, ValueObjectType replaceWithType)
        {
            this.SourceValue = ValueWrapper.Make(source, sourceType);
            this.SearchValue = ValueWrapper.Make(search, searchType);
            this.ReplaceWithValue = ValueWrapper.Make(replace, replaceWithType);
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
