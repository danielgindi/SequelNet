using SequelNet.Connector;
using System.Collections.Generic;
using System.Text;

namespace SequelNet
{
    public class OnConflict
    {
        public OnConflict()
        {
        }

        public string OnColumn;
        public string OnConstraint;
        public AssignmentColumnList Sets = new AssignmentColumnList();
    }
}
