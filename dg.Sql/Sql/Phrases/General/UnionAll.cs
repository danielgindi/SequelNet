using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class UnionAll : Union
    {
        public UnionAll(params Query[] queries)
            : base(queries)
        {
            this.All = true;
        }
    }
}
