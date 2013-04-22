using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;

namespace dg.Sql.Phrases
{
    public class GeographyContains : BasePhrase
    {
        string ContainingTableName;
        object ContainingObject;
        ValueObjectType ContainingObjectType;
        string ContainedTableName;
        object ContainedObject;
        ValueObjectType ContainedObjectType;

        public GeographyContains(string ContainingTableName, object ContainingObject, ValueObjectType ContainingObjectType,
            string ContainedTableName, object ContainedObject, ValueObjectType ContainedObjectType)
        {
            this.ContainingTableName = ContainingTableName;
            this.ContainingObject = ContainingObject;
            this.ContainingObjectType = ContainingObjectType;
            this.ContainedTableName = ContainedTableName;
            this.ContainedObject = ContainedObject;
            this.ContainedObjectType = ContainedObjectType;
        }
        public GeographyContains(object ContainingObject, ValueObjectType ContainingObjectType,
            object ContainedObject, ValueObjectType ContainedObjectType)
        {
            this.ContainingObject = ContainingObject;
            this.ContainingObjectType = ContainingObjectType;
            this.ContainedObject = ContainedObject;
            this.ContainedObjectType = ContainedObjectType;
        }
        public GeographyContains(object ContainingObject, ValueObjectType ContainingObjectType,
            string ContainedColumnName)
        {
            this.ContainingObject = ContainingObject;
            this.ContainingObjectType = ContainingObjectType;
            this.ContainedObject = ContainedColumnName;
            this.ContainedObjectType = ValueObjectType.ColumnName;
        }
        public GeographyContains(object ContainingObject, ValueObjectType ContainingObjectType,
            string ContainedTableName, string ContainedColumnName)
        {
            this.ContainingObject = ContainingObject;
            this.ContainingObjectType = ContainingObjectType;
            this.ContainedTableName = ContainedTableName;
            this.ContainedObject = ContainedColumnName;
            this.ContainedObjectType = ValueObjectType.ColumnName;
        }
        public GeographyContains(Geometry ContainingObject,
            string ContainedColumnName)
        {
            this.ContainingObject = ContainingObject;
            this.ContainingObjectType = ValueObjectType.Value;
            this.ContainedObject = ContainedColumnName;
            this.ContainedObjectType = ValueObjectType.ColumnName;
        }
        public GeographyContains(Geometry ContainingObject,
            string ContainedTableName, string ContainedColumnName)
        {
            this.ContainingObject = ContainingObject;
            this.ContainingObjectType = ValueObjectType.Value;
            this.ContainedTableName = ContainedTableName;
            this.ContainedObject = ContainedColumnName;
            this.ContainedObjectType = ValueObjectType.ColumnName;
        }
        public GeographyContains(Geometry ContainingObject, Geometry ContainedObject)
        {
            this.ContainingObject = ContainingObject;
            this.ContainingObjectType = ValueObjectType.Value;
            this.ContainedObject = ContainedObject;
            this.ContainedObjectType = ValueObjectType.Value;
        }
        public GeographyContains(string ContainingColumnName,
            Geometry ContainedObject)
        {
            this.ContainingObject = ContainingColumnName;
            this.ContainingObjectType = ValueObjectType.ColumnName;
            this.ContainedObject = ContainedObject;
            this.ContainedObjectType = ValueObjectType.Value;
        }
        public GeographyContains(string ContainingTableName, string ContainingColumnName, 
            Geometry ContainedObject)
        {
            this.ContainingTableName = ContainingTableName;
            this.ContainingObject = ContainingColumnName;
            this.ContainingObjectType = ValueObjectType.ColumnName;
            this.ContainedObject = ContainedObject;
            this.ContainedObjectType = ValueObjectType.Value;
        }

        public string BuildPhrase(ConnectorBase conn)
        {
            StringBuilder sb = new StringBuilder();

            if (conn.TYPE == ConnectorBase.SqlServiceType.MSSQL)
            {
            }
            else // MYSQL
            {
                sb.Append(@"MBRContains(");
            }

            if (ContainingObjectType == ValueObjectType.ColumnName)
            {
                if (ContainingTableName != null && ContainingTableName.Length > 0)
                {
                    sb.Append(conn.EncloseFieldName(ContainingTableName));
                    sb.Append(".");
                }
                sb.Append(conn.EncloseFieldName(ContainingObject.ToString()));
            }
            else if (ContainingObjectType == ValueObjectType.Value)
            {
                if (ContainingObject is Geometry)
                {
                    ((Geometry)ContainingObject).BuildValue(sb, conn);
                }
                else
                {
                    sb.Append(conn.PrepareValue(ContainingObject));
                }
            }
            else sb.Append(ContainingObject);

            if (conn.TYPE == ConnectorBase.SqlServiceType.MSSQL)
            {
                sb.Append(@".STContains(");
            }
            else // MYSQL
            {
                sb.Append(@",");
            }

            if (ContainedObjectType == ValueObjectType.ColumnName)
            {
                if (ContainedTableName != null && ContainedTableName.Length > 0)
                {
                    sb.Append(conn.EncloseFieldName(ContainedTableName));
                    sb.Append(".");
                }
                sb.Append(conn.EncloseFieldName(ContainedObject.ToString()));
            }
            else if (ContainedObjectType == ValueObjectType.Value)
            {
                if (ContainedObject is Geometry)
                {
                    ((Geometry)ContainedObject).BuildValue(sb, conn);
                }
                else
                {
                    sb.Append(conn.PrepareValue(ContainedObject));
                }
            }
            else sb.Append(ContainedObject);

            sb.Append(@")");

            return sb.ToString();
        }
    }
}
