using System;
using System.Text;

namespace SequelNet.SchemaGenerator;

public partial class GeneratorCore
{
    private static void WriteFetchMethods(StringBuilder stringBuilder, ScriptContext context)
    {
        var primaryKeyColumns = context.GetPrimaryKeyColumns();
        var nullabilitySign = context.NullableEnabled ? "?" : "";

        if (primaryKeyColumns.Count > 0)
        {
            var sbQueryCond = new StringBuilder();
            var sbQueryStart = new StringBuilder();

            AppendLine(sbQueryStart, "var qry = new Query(Schema)");
            var first = true;
            foreach (var dalCol in primaryKeyColumns)
            {
                if (!first)
                {
                    sbQueryCond.Append(NewLine);
                    sbQueryCond.Append($".AND(Columns.{dalCol.PropertyName}, {ValueToDb(FirstLetterLowerCase(dalCol.PropertyName!), dalCol)})");
                }
                else
                {
                    sbQueryCond.Append($".Where(Columns.{dalCol.PropertyName}, {ValueToDb(FirstLetterLowerCase(dalCol.PropertyName!), dalCol)})");
                    first = false;
                }
            }

            var colIsDeleted = context.Columns.Find(x => x.Name!.Equals("IsDeleted", StringComparison.InvariantCultureIgnoreCase))
                ?? context.Columns.Find(x => x.Name!.Equals("Deleted", StringComparison.InvariantCultureIgnoreCase));

            var sbParams = new StringBuilder();
            var sbParamsCall = new StringBuilder();
            first = true;
            foreach (var dalCol in primaryKeyColumns)
            {
                if (!first)
                {
                    sbParams.Append(", ");
                    sbParamsCall.Append(", ");
                }
                else
                {
                    first = false;
                }

                var (baseTypeName, actualType, effectiveType, isReferenceType) = GetClrTypeName(dalCol, context);
                sbParams.AppendFormat("{0} {1}", effectiveType, FirstLetterLowerCase(dalCol.PropertyName!));
                sbParamsCall.AppendFormat("{0}", FirstLetterLowerCase(dalCol.PropertyName!));
            }

            if (colIsDeleted != null)
            {
                // FetchById(..., bool includeDeleted = false, ConnectorBase conn = null) function
                AppendLine(stringBuilder, $"public static {context.ClassName}{nullabilitySign} FetchById({sbParams}, bool includeDeleted = false, ConnectorBase{nullabilitySign} conn = null)");
                AppendLine(stringBuilder, "{");
                stringBuilder.Append(sbQueryStart);
                stringBuilder.Append(sbQueryCond);
                AppendLine(stringBuilder, ";");
                AppendLine(stringBuilder, "if (!includeDeleted)");
                AppendLine(stringBuilder, $"qry.AND(Columns.{colIsDeleted.PropertyName}, false);");
                AppendLine(stringBuilder, "return FetchByQuery(qry, conn);");
                AppendLine(stringBuilder, "}");
                AppendLine(stringBuilder);

                // FetchById(..., ConnectorBase conn = null) function
                AppendLine(stringBuilder, $"public static {context.ClassName}{nullabilitySign} FetchById({sbParams}, ConnectorBase{nullabilitySign} conn = null)");
                AppendLine(stringBuilder, "{");
                AppendLine(stringBuilder, $"return FetchById({sbParamsCall}, false, conn);");
                AppendLine(stringBuilder, "}");
                AppendLine(stringBuilder);

                // FetchByIdAsync(..., bool includeDeleted = false, ConnectorBase conn = null, CancellationToken? cancellationToken = null) function
                AppendLine(stringBuilder, $"public static System.Threading.Tasks.Task<{context.ClassName}{nullabilitySign}> FetchByIdAsync({sbParams}, bool includeDeleted = false, ConnectorBase{nullabilitySign} conn = null, CancellationToken? cancellationToken = null)");
                AppendLine(stringBuilder, "{");
                stringBuilder.Append(sbQueryStart);
                stringBuilder.Append(sbQueryCond);
                AppendLine(stringBuilder, ";");
                AppendLine(stringBuilder, "if (!includeDeleted)");
                AppendLine(stringBuilder, $"qry.AND(Columns.{colIsDeleted.PropertyName}, false);");
                AppendLine(stringBuilder, "return FetchByQueryAsync(qry, conn, cancellationToken);");
                AppendLine(stringBuilder, "}");
                AppendLine(stringBuilder);

                // FetchByIdAsync(..., ConnectorBase conn, CancellationToken? cancellationToken = null) function
                AppendLine(stringBuilder, $"public static System.Threading.Tasks.Task<{context.ClassName}{nullabilitySign}> FetchByIdAsync({sbParams}, ConnectorBase conn, CancellationToken? cancellationToken = null)");
                AppendLine(stringBuilder, "{");
                AppendLine(stringBuilder, $"return FetchByIdAsync({sbParamsCall}, false, conn, cancellationToken);");
                AppendLine(stringBuilder, "}");
                AppendLine(stringBuilder);

                // FetchByIdAsync(..., bool includeDeleted, CancellationToken? cancellationToken) function
                AppendLine(stringBuilder, $"public static System.Threading.Tasks.Task<{context.ClassName}{nullabilitySign}> FetchByIdAsync({sbParams}, bool includeDeleted, CancellationToken? cancellationToken)");
                AppendLine(stringBuilder, "{");
                AppendLine(stringBuilder, $"return FetchByIdAsync({sbParamsCall}, includeDeleted, null, cancellationToken);");
                AppendLine(stringBuilder, "}");
                AppendLine(stringBuilder);

                // FetchByIdAsync(..., CancellationToken? cancellationToken) function
                AppendLine(stringBuilder, $"public static System.Threading.Tasks.Task<{context.ClassName}{nullabilitySign}> FetchByIdAsync({sbParams}, CancellationToken? cancellationToken)");
                AppendLine(stringBuilder, "{");
                AppendLine(stringBuilder, $"return FetchByIdAsync({sbParamsCall}, false, null, cancellationToken);");
                AppendLine(stringBuilder, "}");
                AppendLine(stringBuilder);
            }
            else
            {
                // FetchById(..., ConnectorBase conn = null) function
                AppendLine(stringBuilder, $"public static {context.ClassName}{nullabilitySign} FetchById({sbParams}, ConnectorBase{nullabilitySign} conn = null)");
                AppendLine(stringBuilder, "{");
                stringBuilder.Append(sbQueryStart);
                stringBuilder.Append(sbQueryCond);
                AppendLine(stringBuilder, ";");
                AppendLine(stringBuilder, "return FetchByQuery(qry, conn);");
                AppendLine(stringBuilder, "}");
                AppendLine(stringBuilder);

                // FetchByIdAsync(..., ConnectorBase conn = null, CancellationToken? cancellationToken = null) function
                AppendLine(stringBuilder, $"public static System.Threading.Tasks.Task<{context.ClassName}{nullabilitySign}> FetchByIdAsync({sbParams}, ConnectorBase{nullabilitySign} conn = null, CancellationToken? cancellationToken = null)");
                AppendLine(stringBuilder, "{");
                stringBuilder.Append(sbQueryStart);
                stringBuilder.Append(sbQueryCond);
                AppendLine(stringBuilder, ";");
                AppendLine(stringBuilder, "return FetchByQueryAsync(qry, conn, cancellationToken);");
                AppendLine(stringBuilder, "}");
                AppendLine(stringBuilder);

                // FetchByIdAsync(..., CancellationToken? cancellationToken) function
                AppendLine(stringBuilder, $"public static System.Threading.Tasks.Task<{context.ClassName}{nullabilitySign}> FetchByIdAsync({sbParams}, CancellationToken? cancellationToken)");
                AppendLine(stringBuilder, "{");
                AppendLine(stringBuilder, $"return FetchByIdAsync({sbParamsCall}, null, cancellationToken);");
                AppendLine(stringBuilder, "}");
                AppendLine(stringBuilder);
            }

            if (primaryKeyColumns.Count > 1)
            {
                var sbQueryDelete = new StringBuilder();

                if (colIsDeleted != null)
                {
                    sbQueryDelete.Append(NewLine);
                    sbQueryDelete.Append($".Update(Columns.{colIsDeleted.PropertyName}, true)");
                }
                else
                {
                    sbQueryDelete.Append(NewLine);
                    sbQueryDelete.Append(".Delete()");
                }

                // Delete(..., ConnectorBase conn = null) function
                AppendLine(stringBuilder, $"public static int Delete({sbParams}, ConnectorBase{nullabilitySign} conn = null)");
                AppendLine(stringBuilder, "{");
                stringBuilder.Append(sbQueryStart);
                stringBuilder.Append(sbQueryDelete);
                stringBuilder.Append(sbQueryCond);
                AppendLine(stringBuilder, ";");
                AppendLine(stringBuilder, "return qry.Execute(conn);");
                AppendLine(stringBuilder, "}");
                AppendLine(stringBuilder);

                // DeleteAsync(..., ConnectorBase conn = null, CancellationToken? cancellationToken = null) function
                AppendLine(stringBuilder, $"public static System.Threading.Tasks.Task<int> DeleteAsync({sbParams}, ConnectorBase{nullabilitySign} conn = null, CancellationToken? cancellationToken = null)");
                AppendLine(stringBuilder, "{");
                stringBuilder.Append(sbQueryStart);
                stringBuilder.Append(sbQueryDelete);
                stringBuilder.Append(sbQueryCond);
                AppendLine(stringBuilder, ";");
                AppendLine(stringBuilder, "return qry.ExecuteAsync(conn, cancellationToken);");
                AppendLine(stringBuilder, "}");
                AppendLine(stringBuilder);

                // DeleteAsync(..., CancellationToken? cancellationToken) function
                AppendLine(stringBuilder, $"public static System.Threading.Tasks.Task<int> DeleteAsync({sbParams}, CancellationToken? cancellationToken)");
                AppendLine(stringBuilder, "{");
                AppendLine(stringBuilder, $"return DeleteAsync({sbParamsCall}, null, cancellationToken);");
                AppendLine(stringBuilder, "}");
            }
        }
    }
}