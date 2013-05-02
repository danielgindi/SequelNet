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
        public Query InsertOrUpdate()
        {
            QueryMode currentMode = this.QueryMode;
            if (currentMode != QueryMode.InsertOrUpdate)
            {
                this.QueryMode = QueryMode.InsertOrUpdate;
                if (currentMode != QueryMode.Insert &&
                    currentMode != QueryMode.Update &&
                    _ListInsertUpdate != null)
                {
                    if (_ListInsertUpdate != null) _ListInsertUpdate.Clear();
                }
            }
            return this;
        }
    }
}
