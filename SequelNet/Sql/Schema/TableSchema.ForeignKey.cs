using System;
using System.Collections.Generic;

namespace SequelNet;

public partial class TableSchema
{
    public class ForeignKeyList : List<ForeignKey>
    {
        public ForeignKey Find(string foreignKeyName)
        {
            if (this == null) return null;
            foreach (ForeignKey foreignKey in this)
            {
                if (foreignKey.Name.Equals(foreignKeyName, StringComparison.CurrentCultureIgnoreCase)) return foreignKey;
            }
            return null;
        }
    }
    public enum ForeignKeyReference
    {
        None,
        Restrict, // Rejects actions if there's related data
        Cascade, // Automatically deletes or updates matching rows in the child table
        SetNull, // Sets related data to NULL if they are nullable
        NoAction // Rejects actions if there's related data
    }
    public class ForeignKey
    {
        public string Name;
        public string[] Columns;
        public string ForeignTable;
        public string[] ForeignColumns;
        public ForeignKeyReference OnDelete;
        public ForeignKeyReference OnUpdate;

        public ForeignKey() { }
        public ForeignKey(string Name, string[] Columns, string ForeignTable, string[] ForeignColumns, ForeignKeyReference OnDelete, ForeignKeyReference OnUpdate)
        {
            this.Name = Name;
            this.Columns = Columns;
            this.ForeignTable = ForeignTable;
            this.ForeignColumns = ForeignColumns;
            this.OnDelete = OnDelete;
            this.OnUpdate = OnUpdate;
        }
        public ForeignKey(string Name, string Column, string ForeignTable, string ForeignColumn, ForeignKeyReference OnDelete, ForeignKeyReference OnUpdate)
            : this(Name, new string[] { Column }, ForeignTable, new string[] { ForeignColumn }, OnDelete, OnUpdate)
        {
        }
    }
}
