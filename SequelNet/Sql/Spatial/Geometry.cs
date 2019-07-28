using System.Text;
using SequelNet.Connector;

namespace SequelNet
{
    public abstract partial class Geometry
    {
        public abstract void BuildValue(StringBuilder sb, ConnectorBase conn);
        public abstract void BuildValueForCollection(StringBuilder sb, ConnectorBase conn);
        public int? SRID;

        public abstract bool IsEmpty { get; }

        public abstract bool IsValid { get; }

        public bool IsGeographyType
        { 
            get; 
            set; 
        }
    }
}
