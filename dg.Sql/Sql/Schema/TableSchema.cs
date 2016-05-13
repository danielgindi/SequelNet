using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dg.Sql
{
    public partial class TableSchema
    {
        private string _DatabaseOwner;
        private string _SchemaName;
        public ColumnList Columns;
        public IndexList Indexes;
        public ForeignKeyList ForeignKeys;
        public TableOptionList TableOptions;

        public string DatabaseOwner
        {
            get { return _DatabaseOwner; }
            set { _DatabaseOwner = value; }
        }

        public string SchemaName
        {
            get { return _SchemaName; }
            set { _SchemaName = value; }
        }

        public TableSchema()
        {
            this.DatabaseOwner = string.Empty;
            this.SchemaName = string.Empty;
            this.Columns = new ColumnList();
            this.Indexes = new IndexList();
            this.ForeignKeys = new ForeignKeyList();
        }

        public TableSchema(string schemaName, ColumnList columns)
        {
            this.DatabaseOwner = string.Empty;
            this.SchemaName = schemaName ?? @"";
            this.Columns = new ColumnList();
            if (columns != null) this.Columns.InsertRange(0, columns);
            this.Indexes = new IndexList();
            this.ForeignKeys = new ForeignKeyList();
        }

        public TableSchema(string databaseOwner, string schemaName, ColumnList columns)
        {
            this.DatabaseOwner = databaseOwner ?? @"";
            this.SchemaName = schemaName ?? @"";
            this.Columns = new ColumnList();
            if (columns != null) this.Columns.InsertRange(0, columns);
            this.Indexes = new IndexList();
            this.ForeignKeys = new ForeignKeyList();
        }

        public TableSchema(string schemaName, ColumnList columns, IndexList indexes, ForeignKeyList foreignKeys)
        {
            this.DatabaseOwner = string.Empty;
            this.SchemaName = schemaName ?? @"";
            this.Columns = new ColumnList();
            if (columns != null) this.Columns.InsertRange(0, columns);
            this.Indexes = new IndexList();
            if (indexes != null) this.Indexes.InsertRange(0, indexes);
            this.ForeignKeys = new ForeignKeyList();
            this.ForeignKeys.InsertRange(0, foreignKeys);
        }

        public TableSchema(string databaseOwner, string schemaName, ColumnList columns, IndexList indexes, ForeignKeyList foreignKeys)
        {
            this.DatabaseOwner = databaseOwner ?? @"";
            this.SchemaName = schemaName ?? @"";
            this.Columns = new ColumnList();
            if (columns != null) this.Columns.InsertRange(0, columns);
            this.Indexes = new IndexList();
            if (indexes != null) this.Indexes.InsertRange(0, indexes);
            this.ForeignKeys = new ForeignKeyList();
            this.ForeignKeys.InsertRange(0, foreignKeys);
        }

        public void AddColumn(Column column)
        {
            if (Columns == null) Columns = new ColumnList();
            Columns.Add(column);
        }

        public void AddColumn(
            string name, System.Type type,
            int maxLength, 
            int numberPrecision, int numberScale, 
            bool autoIncrement, bool isPrimaryKey, 
            bool nullable, object defaultValue)
        {
            Column column = new Column(name, type, maxLength, numberPrecision, numberScale, autoIncrement, isPrimaryKey, nullable, defaultValue);
            AddColumn(column);
        }

        public void AddColumn(
            string name, System.Type type, 
            int maxLength, string literalType, 
            int numberPrecision, int numberScale, 
            bool autoIncrement, bool isPrimaryKey,
            bool nullable, object defaultValue)
        {
            Column column = new Column(name, type, maxLength, literalType, numberPrecision, numberScale, autoIncrement, isPrimaryKey, nullable, defaultValue);
            AddColumn(column);
        }

        public void AddColumn(
            string Name, System.Type type, DataType dataType, 
            int maxLength,
            int numberPrecision, int numberScale,
            bool autoIncrement, bool isPrimaryKey, 
            bool nullable, object defaultValue)
        {
            Column column = new Column(Name, type, dataType, maxLength, numberPrecision, numberScale, autoIncrement, isPrimaryKey, nullable, defaultValue);
            AddColumn(column);
        }

        public void AddColumn(
            string name, System.Type type, DataType dataType,
            int maxLength, string literalType,
            int numberPrecision, int numberScale,
            bool autoIncrement, bool isPrimaryKey, 
            bool nullable, object defaultValue)
        {
            Column column = new Column(name, type, dataType, maxLength, literalType, numberPrecision, numberScale, autoIncrement, isPrimaryKey, nullable, defaultValue);
            AddColumn(column);
        }

        public void AddColumn(
            string name, System.Type type,
            int maxLength,
            int numberPrecision, int numberScale,
            bool autoIncrement, bool isPrimaryKey,
            bool nullable, object defaultValue,
            string charset, string collate)
        {
            Column column = new Column(name, type, maxLength, numberPrecision, numberScale, autoIncrement, isPrimaryKey, nullable, defaultValue, charset, collate);
            AddColumn(column);
        }

        public void AddColumn(
            string name, System.Type type,
            int maxLength, string literalType, 
            int numberPrecision, int numberScale, 
            bool autoIncrement, bool isPrimaryKey,
            bool nullable, object defaultValue, 
            string charset, string collate)
        {
            Column column = new Column(name, type, maxLength, literalType, numberPrecision, numberScale, autoIncrement, isPrimaryKey, nullable, defaultValue, charset, collate);
            AddColumn(column);
        }

        public void AddColumn(
            string name, System.Type type, DataType dataType, 
            int maxLength,
            int numberPrecision, int numberScale, 
            bool autoIncrement, bool isPrimaryKey,
            bool nullable, object defaultValue,
            string charset, string collate)
        {
            Column column = new Column(name, type, dataType, maxLength, numberPrecision, numberScale, autoIncrement, isPrimaryKey, nullable, defaultValue, charset, collate);
            AddColumn(column);
        }

        public void AddColumn(
            string name, System.Type type, DataType dataType, 
            int maxLength, string literalType, 
            int numberPrecision, int numberScale,
            bool autoIncrement, bool isPrimaryKey, 
            bool nullable, object defaultValue,
            string charset, string collate)
        {
            Column column = new Column(name, type, dataType, maxLength, literalType, numberPrecision, numberScale, autoIncrement, isPrimaryKey, nullable, defaultValue, charset, collate);
            AddColumn(column);
        }

        public void AddIndex(Index index)
        {
            if (Indexes == null) Indexes = new IndexList();
            if (index.Name == null)
            {
                if (index.Mode == IndexMode.PrimaryKey)
                {
                    index.Name = @"PK_";
                }
                else
                {
                    index.Name = @"IX_";
                }
                index.Name += SchemaName + @"_";
                for (int idx = 0; idx < index.ColumnNames.Length; idx++)
                {
                    if (idx > 0) index.Name += @"_";
                    index.Name += index.ColumnNames[idx];
                }
            }
            Indexes.Add(index);
        }

        public void AddIndex(string name, ClusterMode cluster, IndexMode mode, IndexType type, params object[] columns)
        {
            Index index = new Index(name, cluster, mode, type, columns);
            AddIndex(index);
        }

        public void AddForeignKey(ForeignKey foreignKey)
        {
            if (ForeignKeys == null) ForeignKeys = new ForeignKeyList();
            if (foreignKey.Name == null)
            {
                foreignKey.Name = @"FK_";
                foreignKey.Name += SchemaName + @"_";
                foreignKey.Name += foreignKey.ForeignTable + @"_";
                for (int idx = 0; idx < foreignKey.Columns.Length; idx++)
                {
                    if (idx > 0) foreignKey.Name += @"_";
                    foreignKey.Name += foreignKey.Columns[idx];
                }
            }
            ForeignKeys.Add(foreignKey);
        }

        public void AddForeignKey(string name, string[] columns, string foreignTable, string[] foreignColumns, ForeignKeyReference onDelete, ForeignKeyReference onUpdate)
        {
            ForeignKey foreignKey = new ForeignKey(name, columns, foreignTable, foreignColumns, onDelete, onUpdate);
            AddForeignKey(foreignKey);
        }

        public void AddForeignKey(string name, string column, string foreignTable, string foreignColumns, ForeignKeyReference onDelete, ForeignKeyReference onUpdate)
        {
            ForeignKey foreignKey = new ForeignKey(name, column, foreignTable, foreignColumns, onDelete, onUpdate);
            AddForeignKey(foreignKey);
        }

        public void SetTableOption(string optionName, string option)
        {
            if (TableOptions == null) TableOptions = new TableOptionList();
            TableOptions[optionName] = option;
        }

        public bool RemoveTableOption(string optionName)
        {
            if (TableOptions != null)
            {
                return TableOptions.Remove(optionName);
            }
            return false;
        }

        public string GetTableOption(string optionName)
        {
            if (TableOptions != null)
            {
                string value;
                if (TableOptions.TryGetValue(optionName, out value))
                {
                    return value;
                }
            }
            return null;
        }
    }
}
