using System.Text;

namespace SequelNet.SchemaGenerator;

public partial class GeneratorCore
{
    private static void WriteGetUpdateQuery(StringBuilder stringBuilder, ScriptContext context)
    {
        AppendLine(stringBuilder, "public override Query GetUpdateQuery()");
        AppendLine(stringBuilder, "{");

        bool hasModifiedOn = context.Columns.Find((DalColumn c) => c.PropertyName == "ModifiedOn") != null;

        if (context.AtomicUpdates && (hasModifiedOn))
        {
            AppendLine(stringBuilder, "if (HasMutatedColumns())");
            AppendLine(stringBuilder, "{");
        }

        if (!context.NoModifiedOn)
        {
            if (context.Columns.Find((DalColumn c) => c.PropertyName == "ModifiedOn") != null)
            {
                AppendLine(stringBuilder, "ModifiedOn = DateTime.UtcNow;");
            }
        }

        if (context.AtomicUpdates && (hasModifiedOn))
        {
            AppendLine(stringBuilder, "}");
        }

        if (hasModifiedOn)
        {
            AppendLine(stringBuilder);
        }
        if (!string.IsNullOrEmpty(context.CustomBeforeUpdate))
        {
            AppendLine(stringBuilder, context.CustomBeforeUpdate!);
            AppendLine(stringBuilder);
        }

        AppendLine(stringBuilder, "Query qry = new Query(Schema);");
        AppendLine(stringBuilder);
        foreach (var dalCol in context.Columns)
        {
            if (dalCol.AutoIncrement || dalCol.NoSave)
            {
                continue;
            }

            if (context.AtomicUpdates)
            {
                AppendLine(stringBuilder, $"if (IsColumnMutated(Columns.{dalCol.PropertyName}) || IsAtomicUpdatesDisabled)");
                AppendLine(stringBuilder, "{");
            }

            AppendLine(stringBuilder, $"qry.Update(Columns.{dalCol.PropertyName}, {ValueToDb(dalCol.PropertyName!, dalCol)});");

            if (context.AtomicUpdates)
            {
                AppendLine(stringBuilder, "}");
                AppendLine(stringBuilder);
            }
        }

        var primaryKeyColumns = context.GetPrimaryKeyColumns();

        bool isFirst = true;
        foreach (var dalCol in primaryKeyColumns)
        {
            var whereMethod = isFirst ? "Where" : "AND";
            AppendLine(stringBuilder, $"qry.{whereMethod}(Columns.{dalCol.PropertyName}, {ValueToDb(dalCol.PropertyName!, dalCol)});");
            isFirst = false;
        }

        if (!string.IsNullOrEmpty(context.CustomAfterUpdateQuery))
        {
            AppendLine(stringBuilder, context.CustomAfterUpdateQuery!);
        }

        AppendLine(stringBuilder);
        AppendLine(stringBuilder, "return qry;");

        AppendLine(stringBuilder, "}");
    }

    private static bool WriteUpdateMethod(StringBuilder stringBuilder, ScriptContext context)
    {
        bool hasUpdateMethod = !string.IsNullOrEmpty(context.CustomBeforeUpdate);

        if (hasUpdateMethod)
        {
            AppendLine(stringBuilder, "public override void Update(ConnectorBase conn = null)");
            AppendLine(stringBuilder, "{");

            if (!string.IsNullOrEmpty(context.CustomBeforeUpdate))
            {
                AppendLine(stringBuilder, context.CustomBeforeUpdate!);
                AppendLine(stringBuilder);
            }

            AppendLine(stringBuilder, "super.Update(conn);");

            AppendLine(stringBuilder, "}");
            AppendLine(stringBuilder, "}");
        }

        return hasUpdateMethod;
    }

    private static bool WriteUpdateAsyncMethod(StringBuilder stringBuilder, ScriptContext context)
    {
        bool hasUpdateMethod = !string.IsNullOrEmpty(context.CustomBeforeUpdate);

        if (hasUpdateMethod)
        {
            AppendLine(stringBuilder, "public override Task UpdateAsync(ConnectorBase conn = null, CancellationToken? cancellationToken = null)");
            AppendLine(stringBuilder, "{");

            if (!string.IsNullOrEmpty(context.CustomBeforeUpdate))
            {
                AppendLine(stringBuilder, context.CustomBeforeUpdate!);
                AppendLine(stringBuilder);
            }

            AppendLine(stringBuilder, "super.UpdateAsync(conn, cancellationToken);");

            AppendLine(stringBuilder, "}");
            AppendLine(stringBuilder, "}");
        }

        return hasUpdateMethod;
    }
}