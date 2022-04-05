using System.Collections.Generic;

// Converted from VB macro, REQUIRES MAJOR REFACTORING!

namespace SequelNet.SchemaGenerator
{
    public class ScriptContext
    {
        public string ClassName = null;
        public string SchemaName = null;
        public string DatabaseOwner = null;

        public List<DalColumn> Columns = new List<DalColumn>();
        public List<DalIndex> Indices = new List<DalIndex>();
        public List<DalForeignKey> ForeignKeys = new List<DalForeignKey>();
        public List<DalEnum> Enums = new List<DalEnum>();

        public bool StaticColumns = false;
        public bool ExportRecord = true;
        public bool ExportCollection = true;
        public bool AtomicUpdates = false;
        public bool SnakeColumnNames = false;
        public bool InsertAutoIncrement = false;
        public bool NoCreatedOn = false;
        public bool NoModifiedOn = false;

        public string SingleColumnPrimaryKeyName = null;
        public string CustomBeforeInsert = null;
        public string CustomBeforeUpdate = null;
        public string CustomAfterRead = null;
        public string MySqlEngineName = "";

        public List<DalColumn> GetPrimaryKeyColumns()
        {
            // Create a list of all columns that participate in the Primary Key
            var primaryKeyColumns = new List<DalColumn>();

            foreach (var dalCol in Columns)
            {
                if (!dalCol.IsPrimaryKey) continue;
                primaryKeyColumns.Add(dalCol);
            }

            if (primaryKeyColumns.Count > 0)
                return primaryKeyColumns;

            foreach (var dalIx in Indices)
            {
                if (dalIx.IndexMode != DalIndexIndexMode.PrimaryKey) continue;

                foreach (var indexColumn in dalIx.Columns)
                {
                    var column = Columns.Find((DalColumn c) => c.Name == indexColumn.Name || c.PropertyName == indexColumn.Name);
                    if (column == null) continue;
                    primaryKeyColumns.Add(column);
                }
            }

            return primaryKeyColumns;
        }
    }
}