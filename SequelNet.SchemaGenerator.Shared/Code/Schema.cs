using System.Text;

namespace SequelNet.SchemaGenerator;

public partial class GeneratorCore
{
    private static void WriteSchema(StringBuilder stringBuilder, ScriptContext context)
    {
        var w = new CodeWriter(stringBuilder);
        var nullabilitySign = context.NullableEnabled ? "?" : "";

        w.AppendLine($"private static TableSchema{nullabilitySign} _Schema;");

        w.AppendLine("public struct Columns");
        w.AppendLine("{");
        w.Indent();

        foreach (var dalCol in context.Columns)
        {
            var modifier = context.StaticColumns ? "static" : "const";
            w.AppendLine($"public {modifier} string {dalCol.PropertyName} = \"{dalCol.Name}\";");
        }

        w.Unindent();
        w.AppendLine("}");

        w.AppendLine($"public override TableSchema GenerateTableSchema()");
        w.AppendLine("{");
        w.Indent();

        w.AppendLine("if (null == _Schema)");
        w.AppendLine("{");
        w.Indent();

        w.AppendLine("TableSchema schema = new TableSchema();");
        w.AppendLine($"schema.Name = {ToCsharpStringLiteral(context.SchemaName!)};");

        if (!string.IsNullOrEmpty(context.DatabaseOwner))
        {
            w.AppendLine($"schema.DatabaseOwner = {ToCsharpStringLiteral(context.DatabaseOwner!)};");
        }

        foreach (var dalCol in context.Columns)
        {
            stringBuilder.Append("schema.AddColumn(new TableSchema.Column {");
            WriteSchemaAddColumnArguments(dalCol, context, stringBuilder);
            stringBuilder.Append("});");
            w.AppendLine();
        }

        w.AppendLine();
        w.AppendLine("_Schema = schema;");

        if (context.Indices.Count > 0)
        {
            w.AppendLine();

            var propertyNameByColumnKey = BuildPropertyNameByColumnKeyMap(context.Columns);

            foreach (var dalIx in context.Indices)
            {
                stringBuilder.Append("schema.AddIndex(");
                WriteSchemaAddIndexArguments(stringBuilder, dalIx, propertyNameByColumnKey);
                stringBuilder.Append(");");
                w.AppendLine();
            }
        }

        if (context.ForeignKeys.Count > 0)
        {
            w.AppendLine();

            foreach (var dalFk in context.ForeignKeys)
            {
                stringBuilder.Append("schema.AddForeignKey(");
                WriteSchemaAddForeignKeyArguments(stringBuilder, dalFk, context);
                stringBuilder.Append(");");
                w.AppendLine();
            }
        }

        if (context.MySqlEngineName.Length > 0)
            w.AppendLine($"schema.SetMySqlEngine(MySqlEngineType.{context.MySqlEngineName});");

        w.Unindent();
        w.AppendLine("}");

        w.AppendLine();
        w.AppendLine("return _Schema;");

        w.Unindent();
        w.AppendLine("}");
    }

    private static void WriteSchemaAddColumnArguments(DalColumn dalCol, ScriptContext context, StringBuilder stringBuilder)
    {
        var (actualType, effectiveType, isReferenceType) = GetClrTypeName(dalCol, context);

        AppendInit(stringBuilder, "Name", $"Columns.{dalCol.PropertyName}");

        AppendInit(stringBuilder, "Type", $"typeof({actualType})");

        var dataTypeString = GetSchemaDataTypeLiteral(dalCol)
            ?? GetEnumUnderlyingDataTypeLiteral(dalCol)
            ?? "";

        AppendInitIfNotEmpty(stringBuilder, "DataType", string.IsNullOrEmpty(dataTypeString) ? null : dataTypeString);

        AppendInit(stringBuilder, "MaxLength", dalCol.MaxLength.ToString());

        AppendInitIfNotEmpty(stringBuilder, "LiteralType",
            string.IsNullOrEmpty(dalCol.LiteralType) ? null : ToCsharpStringLiteral(dalCol.LiteralType!));

        AppendInit(stringBuilder, "NumberPrecision", dalCol.Precision.ToString());

        AppendInit(stringBuilder, "NumberScale", dalCol.Scale.ToString());

        AppendInitIfTrue(stringBuilder, "AutoIncrement", dalCol.AutoIncrement);

        AppendInitIfTrue(stringBuilder, "IsPrimaryKey", dalCol.IsPrimaryKey);

        AppendInitIfTrue(stringBuilder, "Nullable", dalCol.IsNullable);

        if (dalCol.HasDefault)
        {
            AppendInit(stringBuilder, "Default", dalCol.DefaultValue!);
            AppendInit(stringBuilder, "HasDefault", "true");
        }

        if (!string.IsNullOrEmpty(dalCol.Computed))
        {
            AppendInit(stringBuilder, "ComputedColumn", dalCol.Computed!);
            AppendInit(stringBuilder, "ComputedColumnStored", dalCol.ComputedStored ? "true" : "false");
        }

        if (dalCol.SRID != null)
            AppendInit(stringBuilder, "SRID", dalCol.SRID.ToString()!);

        AppendInitIfNotEmpty(stringBuilder, "Charset",
            string.IsNullOrEmpty(dalCol.Charset) ? null : CsharpString(dalCol.Charset!));

        AppendInitIfNotEmpty(stringBuilder, "Collate",
            string.IsNullOrEmpty(dalCol.Collate) ? null : CsharpString(dalCol.Collate!));

        AppendInitIfNotEmpty(stringBuilder, "Comment",
            string.IsNullOrEmpty(dalCol.Comment) ? null : CsharpString(dalCol.Comment!));
    }

    private static void WriteSchemaAddIndexArguments(StringBuilder stringBuilder, DalIndex dalIx, System.Collections.Generic.Dictionary<string, string> propertyNameByColumnKey)
    {
        object[] formatArgs = new object[4];
        formatArgs[0] = (dalIx.IndexName == null ? "null" : ToCsharpStringLiteral(dalIx.IndexName));
        formatArgs[1] = dalIx.ClusterMode.ToString();
        formatArgs[2] = dalIx.IndexMode.ToString();
        formatArgs[3] = dalIx.IndexType.ToString();
        stringBuilder.AppendFormat("{0}, TableSchema.ClusterMode.{1}, TableSchema.IndexMode.{2}, TableSchema.IndexType.{3}", formatArgs);
        foreach (DalIndexColumn indexColumn in dalIx.Columns)
        {
            if (indexColumn.Literal)
            {
                stringBuilder.AppendFormat(", {0}", indexColumn.Name);
            }
            else
            {
                string col = propertyNameByColumnKey.TryGetValue(indexColumn.Name!, out var propName)
                    ? $"Columns.{propName}"
                    : ToCsharpStringLiteral(indexColumn.Name!);
                stringBuilder.AppendFormat(", {0}", col);
            }

            if (!string.IsNullOrEmpty(indexColumn.SortDirection))
            {
                stringBuilder.AppendFormat(", SortDirection.{0}", indexColumn.SortDirection);
            }
        }
    }

    private static void WriteSchemaAddForeignKeyArguments(StringBuilder stringBuilder, DalForeignKey dalFK, ScriptContext context)
    {
        stringBuilder.AppendFormat("{0}, ",
            (dalFK.ForeignKeyName == null ? "null" : ToCsharpStringLiteral(dalFK.ForeignKeyName)));
        if (dalFK.Columns.Count <= 1)
        {
            stringBuilder.AppendFormat("{0}.Columns.{1}, ", context.ClassName, dalFK.Columns[0]);
        }
        else
        {
            stringBuilder.Append("new string[] {");
            foreach (string dalFKCol in dalFK.Columns)
            {
                if (dalFKCol != dalFK.Columns[0])
                {
                    stringBuilder.Append(" ,");
                }
                stringBuilder.AppendFormat("{0}.Columns.{1}", context.ClassName, dalFKCol);
            }
            stringBuilder.Append("}, ");
        }
        if (dalFK.ForeignTable != context.ClassName)
        {
            stringBuilder.AppendFormat("{0}.SchemaName, ", dalFK.ForeignTable);
        }
        else
        {
            stringBuilder.Append("schema.Name, ");
        }
        if (dalFK.ForeignColumns.Count <= 1)
        {
            stringBuilder.AppendFormat("{0}.Columns.{1}, ", dalFK.ForeignTable, dalFK.ForeignColumns[0]);
        }
        else
        {
            stringBuilder.Append("new string[] {");
            foreach (string foreignColumn in dalFK.ForeignColumns)
            {
                if (foreignColumn != dalFK.ForeignColumns[0])
                {
                    stringBuilder.Append(" ,");
                }
                stringBuilder.AppendFormat("{0}.Columns.{1}", dalFK.ForeignTable, foreignColumn);
            }
            stringBuilder.Append("}, ");
        }
        stringBuilder.AppendFormat("TableSchema.ForeignKeyReference.{0}, TableSchema.ForeignKeyReference.{1}", dalFK.OnDelete.ToString(), dalFK.OnUpdate.ToString());
    }
}