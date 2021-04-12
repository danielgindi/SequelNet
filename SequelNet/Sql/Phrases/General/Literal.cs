using SequelNet.Connector;
using System.Text;

namespace SequelNet.Phrases
{
    public class Literal : IPhrase
    {
        public delegate void BuilderDelegate(StringBuilder sb, ConnectorBase conn, Query relatedQuery);
        public BuilderDelegate Builder = null;

        #region Constructors

        public Literal(BuilderDelegate builder)
        {
            this.Builder = builder;
        }

        #endregion

        public void Build(StringBuilder sb, ConnectorBase conn, Query relatedQuery = null)
        {
            Builder.Invoke(sb, conn, relatedQuery);
        }
    }
}
