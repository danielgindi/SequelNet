using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;
using System.IO;

namespace dg.Sql
{
    public abstract partial class Geometry
    {
        public abstract void BuildValue(StringBuilder sb, ConnectorBase conn);
        public abstract void BuildValueForCollection(StringBuilder sb, ConnectorBase conn);
        public int? SRID;

        public abstract bool IsEmpty { get; }
        public abstract bool IsValid { get; }
    }
}
