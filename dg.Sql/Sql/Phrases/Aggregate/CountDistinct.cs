using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class CountDistinct : Count
    {
        #region Constructors

        public CountDistinct()
            : base(true)
        {
        }

        public CountDistinct(string tableName, string columnName)
            : base(tableName, columnName, true)
        {
        }

        public CountDistinct(string columnName)
            : base(null, columnName, true)
        {
        }

        public CountDistinct(object theObject, ValueObjectType valueType)
            : base(theObject, valueType, true)
        {
        }

        public CountDistinct(IPhrase phrase)
            : base(phrase, true)
        {
        }

        #endregion
    }
}
