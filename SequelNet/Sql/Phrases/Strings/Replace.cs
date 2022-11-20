using SequelNet.Connector;
using System.Text;

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
            ValueWrapper source,
            string searchTableName, string searchColumn,
            string replaceWithTableName, string replaceWithColumn)
        {
            this.SourceValue = source;
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
            ValueWrapper search,
            string replaceWithTableName, string replaceWithColumn)
        {
            this.SourceValue = ValueWrapper.Column(sourceTableName, sourceColumn);
            this.SearchValue = search;
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
            string sourceTableName, string sourceColumn,
            string searchTableName, string searchColumn,
            ValueWrapper replace)
        {
            this.SourceValue = ValueWrapper.Column(sourceTableName, sourceColumn);
            this.SearchValue = ValueWrapper.Column(searchTableName, searchColumn);
            this.ReplaceWithValue = replace;
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
            ValueWrapper source,
            ValueWrapper search,
            string replaceWithTableName, string replaceWithColumn)
        {
            this.SourceValue = source;
            this.SearchValue = search;
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
            string sourceTableName, string sourceColumn,
            ValueWrapper search,
            ValueWrapper replace)
        {
            this.SourceValue = ValueWrapper.Column(sourceTableName, sourceColumn);
            this.SearchValue = search;
            this.ReplaceWithValue = replace;
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

        public Replace(ValueWrapper source, ValueWrapper search, ValueWrapper replace)
        {
            this.SourceValue = source;
            this.SearchValue = search;
            this.ReplaceWithValue = replace;
        }

        #endregion

        public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
        {
            sb.Append("REPLACE(");
            sb.Append(SourceValue.Build(conn, relatedQuery));
            sb.Append(",");
            sb.Append(SearchValue.Build(conn, relatedQuery));
            sb.Append(",");
            sb.Append(ReplaceWithValue.Build(conn, relatedQuery));
            sb.Append(")");
        }
    }
}
