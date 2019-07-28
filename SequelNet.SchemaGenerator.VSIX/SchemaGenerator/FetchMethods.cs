using System;
using System.Text;

// Converted from VB macro, REQUIRES MAJOR REFACTORING!

namespace SequelNet.SchemaGenerator
{
    public partial class GeneratorCore
	{
        private static void WriteFetchMethods(StringBuilder stringBuilder, ScriptContext context)
        {
            var primaryKeyColumns = context.GetPrimaryKeyColumns();

            if (primaryKeyColumns.Count > 0)
            {
                bool first;

                // FetchByID(..., SequelConnector conn = null) function
                stringBuilder.AppendFormat("public static {1} FetchByID(", "\r\n", context.ClassName);
                first = true;
                foreach (var dalCol in primaryKeyColumns)
                {
                    if (!first)
                    {
                        stringBuilder.Append(", ");
                    }
                    else
                    {
                        first = false;
                    }
                    stringBuilder.AppendFormat("{0} {1}", dalCol.ActualType, FirstLetterLowerCase(dalCol.PropertyName));
                }
                stringBuilder.AppendFormat(", SequelConnector conn = null){0}{{{0}", "\r\n");

                stringBuilder.AppendFormat("Query qry = new Query(Schema){0}", "\r\n");
                first = true;
                foreach (var dalCol in primaryKeyColumns)
                {
                    if (!first)
                    {
                        stringBuilder.AppendFormat("{0}.AND(Columns.{1}, {2})", "\r\n", dalCol.PropertyName, ValueToDb(FirstLetterLowerCase(dalCol.PropertyName), dalCol));
                    }
                    else
                    {
                        stringBuilder.AppendFormat(".Where(Columns.{1}, {2})", "\r\n", dalCol.PropertyName, ValueToDb(FirstLetterLowerCase(dalCol.PropertyName), dalCol));
                        first = false;
                    }
                }
                stringBuilder.AppendFormat(";{0}using (DataReaderBase reader = qry.ExecuteReader(conn)){0}{{{0}if (reader.Read()){0}{{{0}{1} item = new {1}();{0}item.Read(reader);{0}return item;{0}}}{0}}}{0}return null;{0}}}{0}{0}", "\r\n", context.ClassName);

                if (primaryKeyColumns.Count > 1)
                {
                    // Delete(..., SequelConnector conn = null) function
                    stringBuilder.AppendFormat("public static int Delete(", "\r\n");
                    first = true;
                    foreach (var dalCol in primaryKeyColumns)
                    {
                        if (!first)
                        {
                            stringBuilder.Append(", ");
                        }
                        else
                        {
                            first = false;
                        }
                        stringBuilder.AppendFormat("{0} {1}", dalCol.ActualType, FirstLetterLowerCase(dalCol.PropertyName));
                    }
                    stringBuilder.AppendFormat(", SequelConnector conn = null){0}{{{0}", "\r\n");

                    stringBuilder.AppendFormat("Query qry = new Query(Schema)", "\r\n");

                    var colIsDeleted = context.Columns.Find(x => x.Name.Equals("IsDeleted", StringComparison.InvariantCultureIgnoreCase));
                    var colDeleted = context.Columns.Find(x => x.Name.Equals("IsDeleted", StringComparison.InvariantCultureIgnoreCase));

                    if (colIsDeleted != null)
                    {
                        stringBuilder.AppendFormat("{0}    .Update(Columns.{1}, true)", "\r\n", colIsDeleted.PropertyName);
                    }
                    else if (colDeleted != null)
                    {
                        stringBuilder.AppendFormat("{0}    .Update(Columns.{1}, true)", "\r\n", colDeleted.PropertyName);
                    }
                    else
                    {
                        stringBuilder.AppendFormat("{0}    .Delete()", "\r\n");
                    }

                    first = true;
                    foreach (var dalCol in primaryKeyColumns)
                    {
                        if (!first)
                        {
                            stringBuilder.AppendFormat("{0}    .AND(Columns.{1}, {2})", "\r\n", dalCol.PropertyName, ValueToDb(FirstLetterLowerCase(dalCol.PropertyName), dalCol));
                        }
                        else
                        {
                            stringBuilder.AppendFormat("{0}    .Where(Columns.{1}, {2})", "\r\n", dalCol.PropertyName, ValueToDb(FirstLetterLowerCase(dalCol.PropertyName), dalCol));
                            first = false;
                        }
                    }
                    stringBuilder.AppendFormat(";{0}return qry.Execute(conn);{0}}}{0}", "\r\n");
                }
            }
        }
	}
}