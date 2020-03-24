using System;
using System.Text;
using System.Windows;

// Converted from VB macro, REQUIRES MAJOR REFACTORING!

namespace SequelNet.SchemaGenerator
{
    public partial class GeneratorCore
	{
		public GeneratorCore()
		{
		}

        public static string GenerateDalClass(string script)
		{
            ScriptContext context = new ScriptContext();
			
            var scriptLines = script.Trim(new char[] { ' ', '*', '/', '\t', '\r', '\n' }).Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            ParseScript(context, scriptLines);

            foreach (var dalIx in context.Indices)
            {
                foreach (var column in dalIx.Columns)
                {
                    if (context.Columns.Find(x => x.Name.Equals(column.Name)) == null && context.Columns.Find(x => x.PropertyName.Equals(column.Name)) == null)
                    {
                        MessageBox.Show(@"Column " + column.Name + @" not found in index " + (dalIx.IndexName ?? ""));
                    }
                }
            }

            if (context.SnakeColumnNames)
            {
                foreach (var column in context.Columns)
                {
                    if (column.HasCustomName) continue;

                    column.Name = SnakeCase(column.Name);
                }
            }

            // Start building the output classes

			var stringBuilder = new StringBuilder();

            if (context.ExportCollection)
            {
                WriteCollection(stringBuilder, context);
            }

            if (context.ExportRecord)
            {
                WriteRecord(stringBuilder, context);
            }

            // Return results
            return stringBuilder.ToString();
		}

        private static void WriteCollection(StringBuilder stringBuilder, ScriptContext context)
        {
            stringBuilder.AppendFormat("public partial class {1}Collection : AbstractRecordList<{1}, {1}Collection> {{{0}}}{0}{0}", "\r\n", context.ClassName);
            foreach (DalEnum dalEn in context.Enums)
            {
                stringBuilder.AppendFormat("public enum {1}{0}{{{0}", "\r\n", dalEn.Name);
                foreach (string item in dalEn.Items)
                {
                    stringBuilder.AppendFormat("{1},{0}", "\r\n", item);
                }
                stringBuilder.AppendFormat("}}{0}{0}", "\r\n");
            }
        }

        private static void WriteRecord(StringBuilder stringBuilder, ScriptContext context)
        {
            stringBuilder.AppendFormat("public partial class {1} : AbstractRecord<{1}>{0}{{{0}", "\r\n", context.ClassName);

            if (context.AtomicUpdates)
            {
                stringBuilder.AppendFormat("#region Static Constructor{0}{0}", "\r\n");
                stringBuilder.AppendFormat("static {1}(){0}", "\r\n", context.ClassName);
                stringBuilder.AppendFormat("{{{0}", "\r\n", context.ClassName);
                stringBuilder.AppendFormat("AtomicUpdates = true;{0}", "\r\n");
                stringBuilder.AppendFormat("}}{0}", "\r\n");
                stringBuilder.AppendFormat("{0}#endregion{0}{0}", "\r\n");
            }

            #region Table Schema

            stringBuilder.AppendFormat("#region Table Schema{0}{0}", "\r\n");
            WriteSchema(stringBuilder, context);
            stringBuilder.AppendFormat("{0}#endregion{0}{0}", "\r\n");

            #endregion

            #region Private Members

            stringBuilder.AppendFormat("#region Private Members{0}{0}", "\r\n");
            WriteValueStoredVariables(stringBuilder, context);
            stringBuilder.AppendFormat("#endregion{0}{0}", "\r\n");

            #endregion

            #region Properties

            stringBuilder.AppendFormat("#region Properties{0}{0}", "\r\n");
            WriteValueProperties(stringBuilder, context);
            stringBuilder.AppendFormat("#endregion{0}{0}", "\r\n");

            #endregion

            #region AbstractRecord members

            stringBuilder.AppendFormat("#region AbstractRecord members{0}{0}", "\r\n");

            // GetPrimaryKeyValue() function
            stringBuilder.AppendFormat("public override object GetPrimaryKeyValue(){0}{{{0}return {1};{0}}}{0}{0}", "\r\n",
                string.IsNullOrEmpty(context.SingleColumnPrimaryKeyName) ? "null" : context.SingleColumnPrimaryKeyName);

            WriteSetPrimaryKeyValueMethod(stringBuilder, context);
            stringBuilder.Append("\r\n");
            WriteGetInsertQuery(stringBuilder, context);
            stringBuilder.Append("\r\n");
            WriteGetUpdateQuery(stringBuilder, context);
            if (WriteUpdateMethod(stringBuilder, context))
                stringBuilder.Append("\r\n");
            stringBuilder.Append("\r\n");
            if (WriteInsertMethod(stringBuilder, context))
                stringBuilder.Append("\r\n");
            if (WriteInsertAsyncMethod(stringBuilder, context))
                stringBuilder.Append("\r\n");
            WriteReadMethod(stringBuilder, context);

            stringBuilder.AppendFormat("{0}#endregion{0}{0}", "\r\n");

            #endregion

            #region Mutated

            WriteMutationMethods(stringBuilder, context);

            #endregion

            #region Helpers

            stringBuilder.AppendFormat("#region Helpers{0}{0}", "\r\n");
            WriteFetchMethods(stringBuilder, context);
            stringBuilder.AppendFormat("{0}#endregion{0}", "\r\n");

            #endregion

            // End of class
            stringBuilder.Append("}");
        }
	}
}