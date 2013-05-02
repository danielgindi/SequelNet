using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using dg.Sql.Connector;

namespace dg.Sql
{
    public partial class Query
    {
        public Query Delete()
        {
            this.QueryMode = QueryMode.Delete;
            return this;
        }
    }
}
