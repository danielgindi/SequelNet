using System.Text;

// Converted from VB macro, REQUIRES MAJOR REFACTORING!

namespace SequelNet.SchemaGenerator
{
    public partial class GeneratorCore
	{
        private static void WriteGetUpdateQuery(StringBuilder stringBuilder, ScriptContext context)
        {
            stringBuilder.AppendFormat("public override Query GetUpdateQuery(){0}{{{0}", "\r\n");

            bool hasModifiedOn = context.Columns.Find((DalColumn c) => c.PropertyName == "ModifiedOn") != null;

            if (context.AtomicUpdates && (hasModifiedOn))
            {
                stringBuilder.AppendFormat(@"if (HasMutatedColumns()){0}{{{0}", "\r\n");
            }

            if (!context.NoModifiedOn)
            {
                if (context.Columns.Find((DalColumn c) => c.PropertyName == "ModifiedOn") != null)
                {
                    stringBuilder.AppendFormat("ModifiedOn = DateTime.UtcNow;{0}", "\r\n");
                }
            }

            if (context.AtomicUpdates && (hasModifiedOn))
            {
                stringBuilder.AppendFormat(@"}}{0}", "\r\n");
            }

            if (hasModifiedOn)
            {
                stringBuilder.Append("\r\n");
            }
            if (!string.IsNullOrEmpty(context.CustomBeforeUpdate))
            {
                stringBuilder.AppendFormat("{1}{0}{0}", "\r\n", context.CustomBeforeUpdate);
            }

            stringBuilder.AppendFormat("Query qry = new Query(Schema);{0}", "\r\n");
            foreach (var dalCol in context.Columns)
            {
                if (dalCol.AutoIncrement || dalCol.NoSave)
                {
                    continue;
                }

                if (context.AtomicUpdates)
                {
                    stringBuilder.AppendFormat(@"if (IsColumnMutated(Columns.{1})){0}{{{0}", "\r\n", dalCol.PropertyName);
                }

                stringBuilder.AppendFormat("qry.Update(Columns.{1}, {2});{0}", "\r\n", dalCol.PropertyName, ValueToDb(dalCol.PropertyName, dalCol));

                if (context.AtomicUpdates)
                {
                    stringBuilder.AppendFormat(@"}}{0}{0}", "\r\n", dalCol.PropertyName);
                }
            }

            var primaryKeyColumns = context.GetPrimaryKeyColumns();

            bool isFirst = true;
            foreach (var dalCol in primaryKeyColumns)
            {
                stringBuilder.AppendFormat("qry.{3}(Columns.{1}, {2});{0}", "\r\n", 
                    dalCol.PropertyName, ValueToDb(dalCol.PropertyName, dalCol),
                    (isFirst ? "Where" : "AND"));
                isFirst = false;
            }

            stringBuilder.AppendFormat("{0}return qry = null;{0}", "\r\n");

            stringBuilder.AppendFormat("}}{0}}}{0}", "\r\n");
        }

        private static void WriteUpdateMethod(StringBuilder stringBuilder, ScriptContext context)
        {
            stringBuilder.AppendFormat("public override void Update(ConnectorBase conn = null){0}{{{0}", "\r\n");

            stringBuilder.AppendFormat("var qry = GetUpdateQuery();{0}{0}", "\r\n");

            if (context.AtomicUpdates)
            {
                stringBuilder.AppendFormat("if (qry.HasInsertsOrUpdates){0}{{{0}", "\r\n");
            }

            stringBuilder.AppendFormat("qry.Execute(conn);{0}", "\r\n");

            if (context.AtomicUpdates)
            {
                stringBuilder.AppendFormat("}}{0}", "\r\n");
                stringBuilder.AppendFormat("{0}MarkAllColumnsNotMutated();{0}", "\r\n");
            }

            stringBuilder.AppendFormat("}}{0}", "\r\n");
        }

        private static void WriteUpdateAsyncMethod(StringBuilder stringBuilder, ScriptContext context)
        {
            stringBuilder.AppendFormat("public override Task UpdateAsync(ConnectorBase connection = null, CancellationToken? cancellationToken = null){0}{{{0}", "\r\n");

            stringBuilder.AppendFormat("var qry = GetUpdateQuery();{0}{0}", "\r\n");

            if (context.AtomicUpdates)
            {
                stringBuilder.AppendFormat("if (qry.HasInsertsOrUpdates){0}{{{0}", "\r\n");
            }

            stringBuilder.AppendFormat("qry.ExecuteAsync(conn, cancellationToken);{0}", "\r\n");

            if (context.AtomicUpdates)
            {
                stringBuilder.AppendFormat("}}{0}", "\r\n");
                stringBuilder.AppendFormat("{0}MarkAllColumnsNotMutated();{0}", "\r\n");
            }

            stringBuilder.AppendFormat("}}{0}", "\r\n");
        }
    }
}