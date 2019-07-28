using SequelNet.Connector;

namespace SequelNet.Phrases
{
    public class IfNull : IPhrase
    {
        public ValueWrapper Value1;
        public ValueWrapper Value2;

        #region Constructors

        public IfNull(
            string firstTableName, string firstColumnName,
            string secondTableName, string secondColumnName)
        {
            this.Value1 = new ValueWrapper(firstTableName, firstColumnName);
            this.Value2 = new ValueWrapper(secondTableName, secondColumnName);
        }

        public IfNull(
             object firstValue, ValueObjectType firstValueType,
             object secondValue, ValueObjectType secondValueType)
        {
            this.Value1 = new ValueWrapper(firstValue, firstValueType);
            this.Value2 = new ValueWrapper(secondValue, secondValueType);
        }

        public IfNull(
             string firstTableName, string firstColumnName,
             object secondValue, ValueObjectType secondValueType)
        {
            this.Value1 = new ValueWrapper(firstTableName, firstColumnName);
            this.Value2 = new ValueWrapper(secondValue, secondValueType);
        }

        public IfNull(
             object firstValue, ValueObjectType firstValueType,
             string secondTableName, string secondColumnName)
        {
            this.Value1 = new ValueWrapper(firstValue, firstValueType);
            this.Value2 = new ValueWrapper(secondTableName, secondColumnName);
        }

        #endregion

        public string BuildPhrase(ConnectorBase conn, Query relatedQuery = null)
        {
            string ret;
            if (conn.TYPE == ConnectorBase.SqlServiceType.MYSQL || conn.TYPE == ConnectorBase.SqlServiceType.POSTGRESQL)
                ret = @"IFNULL(";
            else ret = @"ISNULL(";

            ret += Value1.Build(conn, relatedQuery);

            ret += ", ";

            ret += Value2.Build(conn, relatedQuery);

            ret += ")";

            return ret;
        }
    }
}
