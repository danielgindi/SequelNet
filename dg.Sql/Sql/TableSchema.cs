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
        public void AddColumn(string Name, System.Type Type, int MaxLength, int NumberPrecision, int NumberScale, bool AutoIncrement, bool IsPrimaryKey, bool Nullable, object Default)
        {
            Column column = new Column(Name, Type, MaxLength, NumberPrecision, NumberScale, AutoIncrement, IsPrimaryKey, Nullable, Default);
            AddColumn(column);
        }
        public void AddColumn(string Name, System.Type Type, DataType DataType,int MaxLength, int NumberPrecision, int NumberScale, bool AutoIncrement, bool IsPrimaryKey, bool Nullable, object Default)
        {
            Column column = new Column(Name, Type, DataType, MaxLength, NumberPrecision, NumberScale, AutoIncrement, IsPrimaryKey, Nullable, Default);
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
        public void AddIndex(string Name, ClusterMode Cluster, IndexMode Mode, IndexType Type, params object[] Columns)
        {
            Index index = new Index(Name, Cluster, Mode, Type, Columns);
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
        public void AddForeignKey(string Name, string[] Columns, string ForeignTable, string[] ForeignColumns, ForeignKeyReference OnDelete, ForeignKeyReference OnUpdate)
        {
            ForeignKey foreignKey = new ForeignKey(Name, Columns, ForeignTable, ForeignColumns, OnDelete, OnUpdate);
            AddForeignKey(foreignKey);
        }
        public void AddForeignKey(string Name, string Column, string ForeignTable, string ForeignColumn, ForeignKeyReference OnDelete, ForeignKeyReference OnUpdate)
        {
            ForeignKey foreignKey = new ForeignKey(Name, Column, ForeignTable, ForeignColumn, OnDelete, OnUpdate);
            AddForeignKey(foreignKey);
        }

        public void SetTableOption(string OptionName, string Option)
        {
            if (TableOptions == null) TableOptions = new TableOptionList();
            TableOptions[OptionName] = Option;
        }
        public bool RemoveTableOption(string OptionName)
        {
            if (TableOptions != null)
            {
                return TableOptions.Remove(OptionName);
            }
            return false;
        }
        public string GetTableOption(string OptionName)
        {
            if (TableOptions != null)
            {
                string value;
                if (TableOptions.TryGetValue(OptionName, out value))
                {
                    return value;
                }
            }
            return null;
        }
    }
}
