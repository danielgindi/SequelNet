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
                var sbQueryCond = new StringBuilder();
                var sbQueryStart = new StringBuilder();

                sbQueryStart.AppendFormat("var qry = new Query(Schema){0}", "\r\n");
                var first = true;
                foreach (var dalCol in primaryKeyColumns)
                {
                    if (!first)
                    {
                        sbQueryCond.AppendFormat("{0}.AND(Columns.{1}, {2})", "\r\n", dalCol.PropertyName, ValueToDb(FirstLetterLowerCase(dalCol.PropertyName), dalCol));
                    }
                    else
                    {
                        sbQueryCond.AppendFormat(".Where(Columns.{1}, {2})", "\r\n", dalCol.PropertyName, ValueToDb(FirstLetterLowerCase(dalCol.PropertyName), dalCol));
                        first = false;
                    }
                }

                var colIsDeleted = context.Columns.Find(x => x.Name.Equals("IsDeleted", StringComparison.InvariantCultureIgnoreCase))
                    ?? context.Columns.Find(x => x.Name.Equals("Deleted", StringComparison.InvariantCultureIgnoreCase));

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

                    sbParams.AppendFormat("{0} {1}", dalCol.ActualType, FirstLetterLowerCase(dalCol.PropertyName));
                    sbParamsCall.AppendFormat("{0}", FirstLetterLowerCase(dalCol.PropertyName));
                }

                if (colIsDeleted != null)
                {
                    // FetchById(..., bool includeDeleted = false, ConnectorBase conn = null) function
                    stringBuilder.AppendFormat("public static {1} FetchById({2}, bool includeDeleted = false, ConnectorBase conn = null){0}{{{0}", "\r\n",
                        context.ClassName, sbParams);
                    stringBuilder.AppendFormat("{1}{2};{0}", "\r\n", sbQueryStart, sbQueryCond);
                    if (colIsDeleted != null)
                        stringBuilder.AppendFormat("if (!includeDeleted){0}qry.AND(Columns.{1}, false);{0}", "\r\n", colIsDeleted.PropertyName);
                    stringBuilder.AppendFormat("return FetchByQuery(qry, conn);{0}}}{0}{0}", "\r\n");

                    // FetchById(..., ConnectorBase conn = null) function
                    stringBuilder.AppendFormat("public static {1} FetchById({2}, ConnectorBase conn = null){0}{{{0}return FetchById({3}, false, conn);{0}}}{0}{0}", "\r\n",
                        context.ClassName, sbParams, sbParamsCall);

                    // FetchByIdAsync(..., bool includeDeleted = false, ConnectorBase conn = null, CancellationToken? cancellationToken = null) function
                    stringBuilder.AppendFormat("public static System.Threading.Tasks.Task<{1}> FetchByIdAsync({2}", "\r\n",
                        context.ClassName, sbParams);
                    stringBuilder.AppendFormat(", bool includeDeleted = false, ConnectorBase conn = null, CancellationToken? cancellationToken = null){0}{{{0}", "\r\n");
                    stringBuilder.AppendFormat("{1}{2};{0}", "\r\n", sbQueryStart, sbQueryCond);
                    if (colIsDeleted != null)
                        stringBuilder.AppendFormat("if (!includeDeleted){0}qry.AND(Columns.{1}, false);{0}", "\r\n", colIsDeleted.PropertyName);
                    stringBuilder.AppendFormat("return FetchByQueryAsync(qry, conn, cancellationToken);{0}}}{0}{0}", "\r\n");

                    // FetchByIdAsync(..., ConnectorBase conn, CancellationToken? cancellationToken = null) function
                    stringBuilder.AppendFormat("public static System.Threading.Tasks.Task<{1}> FetchByIdAsync({2}, ConnectorBase conn, CancellationToken? cancellationToken = null){0}{{{0}", "\r\n",
                        context.ClassName, sbParams);
                    stringBuilder.AppendFormat("return FetchByIdAsync({1}, false, conn, cancellationToken);{0}}}{0}{0}", "\r\n", sbParamsCall);

                    // FetchByIdAsync(..., bool includeDeleted, CancellationToken? cancellationToken) function
                    stringBuilder.AppendFormat("public static System.Threading.Tasks.Task<{1}> FetchByIdAsync({2}, bool includeDeleted, CancellationToken? cancellationToken){0}{{{0}", "\r\n",
                        context.ClassName, sbParams);
                    stringBuilder.AppendFormat("return FetchByIdAsync({1}, includeDeleted, null, cancellationToken);{0}}}{0}{0}", "\r\n", sbParamsCall);

                    // FetchByIdAsync(..., CancellationToken? cancellationToken) function
                    stringBuilder.AppendFormat("public static System.Threading.Tasks.Task<{1}> FetchByIdAsync({2}, CancellationToken? cancellationToken){0}{{{0}", "\r\n",
                        context.ClassName, sbParams);
                    stringBuilder.AppendFormat("return FetchByIdAsync({1}, includeDeleted, null, cancellationToken);{0}}}{0}{0}", "\r\n", sbParamsCall);
                }
                else
                {
                    // FetchById(..., ConnectorBase conn = null) function
                    stringBuilder.AppendFormat("public static {1} FetchById({2}, ConnectorBase conn = null){0}{{{0}", "\r\n",
                        context.ClassName, sbParams);
                    stringBuilder.AppendFormat("{1}{2};{0}", "\r\n", sbQueryStart, sbQueryCond);
                    if (colIsDeleted != null)
                        stringBuilder.AppendFormat("if (!includeDeleted){0}qry.AND(Columns.{1}, false);{0}", "\r\n", colIsDeleted.PropertyName);
                    stringBuilder.AppendFormat("return FetchByQuery(qry, conn);{0}}}{0}{0}", "\r\n");

                    // FetchByIdAsync(..., ConnectorBase conn = null, CancellationToken? cancellationToken = null) function
                    stringBuilder.AppendFormat("public static System.Threading.Tasks.Task<{1}> FetchByIdAsync({2}", "\r\n",
                        context.ClassName, sbParams);
                    stringBuilder.AppendFormat(", ConnectorBase conn = null, CancellationToken? cancellationToken = null){0}{{{0}", "\r\n");
                    stringBuilder.AppendFormat("{1}{2};{0}", "\r\n", sbQueryStart, sbQueryCond);
                    if (colIsDeleted != null)
                        stringBuilder.AppendFormat("if (!includeDeleted){0}qry.AND(Columns.{1}, false);{0}", "\r\n", colIsDeleted.PropertyName);
                    stringBuilder.AppendFormat("return FetchByQueryAsync(qry, conn, cancellationToken);{0}}}{0}{0}", "\r\n");

                    // FetchByIdAsync(..., CancellationToken? cancellationToken) function
                    stringBuilder.AppendFormat("public static System.Threading.Tasks.Task<{1}> FetchByIdAsync({2}, CancellationToken? cancellationToken){0}{{{0}", "\r\n",
                        context.ClassName, sbParams);
                    stringBuilder.AppendFormat("return FetchByIdAsync({1}, null, cancellationToken);{0}}}{0}{0}", "\r\n", sbParamsCall);
                }

                if (primaryKeyColumns.Count > 1)
                {
                    var sbQueryDelete = new StringBuilder();

                    if (colIsDeleted != null)
                    {
                        sbQueryDelete.AppendFormat("{0}.Update(Columns.{1}, true)", "\r\n", colIsDeleted.PropertyName);
                    }
                    else
                    {
                        sbQueryDelete.AppendFormat("{0}.Delete()", "\r\n");
                    }

                    // Delete(..., ConnectorBase conn = null) function
                    stringBuilder.AppendFormat("public static int Delete({1}, ConnectorBase conn = null){0}{{{0}", "\r\n", sbParams);
                    stringBuilder.AppendFormat("{1}{2}{3};{0}", "\r\n", sbQueryStart, sbQueryDelete, sbQueryCond);
                    stringBuilder.AppendFormat("return qry.Execute(conn);{0}}}{0}{0}", "\r\n");

                    // DeleteAsync(..., ConnectorBase conn = null, CancellationToken? cancellationToken = null) function
                    stringBuilder.AppendFormat("public static System.Threading.Tasks.Task<int> DeleteAsync({1}, ConnectorBase conn = null, CancellationToken? cancellationToken = null){0}{{{0}", "\r\n", sbParams);
                    stringBuilder.AppendFormat("{1}{2}{3};{0}", "\r\n", sbQueryStart, sbQueryDelete, sbQueryCond);
                    stringBuilder.AppendFormat("return qry.ExecuteAsync(conn, cancellationToken);{0}}}{0}{0}", "\r\n");

                    // DeleteAsync(..., CancellationToken? cancellationToken) function
                    stringBuilder.AppendFormat("public static System.Threading.Tasks.Task<int> DeleteAsync({1}, CancellationToken? cancellationToken){0}{{{0}", "\r\n", sbParams);
                    stringBuilder.AppendFormat("return DeleteAsync({1}, null, cancellationToken);{0}}}{0}", "\r\n", sbParamsCall);
                }
            }
        }
	}
}