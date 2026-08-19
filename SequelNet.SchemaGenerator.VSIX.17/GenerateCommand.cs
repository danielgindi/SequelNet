using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Task = System.Threading.Tasks.Task;

namespace SequelNet.SchemaGenerator.VSIX
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class GenerateCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("14b92cc9-41de-45e1-b4cd-f2ca6c8315cb");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private GenerateCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static GenerateCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in GenerateCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new GenerateCommand(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            DTE2 dte = (DTE2)Package.GetGlobalService(typeof(DTE));

            try
            {
                Document activeDocument = dte.ActiveDocument;
                TextDocument textDocument = activeDocument.Object() as TextDocument;
                TextSelection selection = textDocument.Selection;
                var documentStart = textDocument.StartPoint.CreateEditPoint();
                string documentText = documentStart.GetText(textDocument.EndPoint);
                string script;
                int macroEndOffset;

                if (!selection.IsEmpty)
                {
                    script = MacroLocator.Normalize(selection.Text);
                    macroEndOffset = ToDocumentOffset(documentText, selection.BottomPoint.AbsoluteCharOffset);
                }
                else
                {
                    var macro = MacroLocator.Find(documentText, ToDocumentOffset(documentText, selection.ActivePoint.AbsoluteCharOffset));
                    if (macro == null)
                    {
                        VsShellUtilities.ShowMessageBox(
                            this.package,
                            "Select a SequelNet macro, place the cursor inside one, or keep exactly one valid macro comment in the file.",
                            "No macro found",
                            OLEMSGICON.OLEMSGICON_INFO,
                            OLEMSGBUTTON.OLEMSGBUTTON_OK,
                            OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                        return;
                    }

                    script = macro.Script;
                    macroEndOffset = macro.End;
                }
                var generated = GeneratorCore.GenerateDalClass(script);
                foreach (var warning in generated.Warnings)
                    MessageBox.Show(warning);
                string code = generated.Code;
                string recordName = generated.Context.ClassName ?? string.Empty;

                // Keep the macro in the file as the source of truth, and create or
                // update only the generated region immediately after it.
                var change = GeneratedRegion.CreateOrUpdateAfterMacro(documentText, macroEndOffset, code, recordName);
                string changeText = ApplyDefaultIndentation(
                    change.Text,
                    LeadingWhitespaceAt(documentText, change.Start),
                    GetIndentSize(textDocument));

                if (change.Start < 0 || change.Length < 0 || change.Start + change.Length > documentText.Length)
                    throw new InvalidOperationException("The generated-region edit range is outside the current document.");

                var editStart = textDocument.StartPoint.CreateEditPoint();
                editStart.MoveToAbsoluteOffset(ToDteAbsoluteOffset(documentText, change.Start));

                if (change.Length == 0)
                {
                    editStart.Insert(changeText);
                }
                else
                {
                    var editEnd = textDocument.StartPoint.CreateEditPoint();
                    editEnd.MoveToAbsoluteOffset(ToDteAbsoluteOffset(documentText, change.Start + change.Length));
                    editStart.ReplaceText(editEnd, changeText, (int)vsEPReplaceTextOptions.vsEPReplaceTextAutoformat);
                }

                var updatedDocumentText = documentText.Substring(0, change.Start) + changeText +
                    documentText.Substring(change.Start + change.Length);
                FormatGeneratedRegion(dte, textDocument, updatedDocumentText, change.Start, changeText.Length);
            }
            catch (Exception exception)
            {
                VsShellUtilities.ShowMessageBox(
                    this.package,
                    exception.ToString(),
                    "Failed to generate DAL code from script",
                    OLEMSGICON.OLEMSGICON_INFO,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
        }

        private static string ApplyDefaultIndentation(string text, string baseIndentation, int indentSize)
        {
            return Regex.Replace(text, @"^([ ]*)(?=\S)", match =>
            {
                string indentation = match.Groups[1].Value;
                int levels = indentation.Length / 4;
                return baseIndentation + new string(' ', indentSize * levels) + indentation.Substring(levels * 4);
            }, RegexOptions.Multiline);
        }

        private static string LeadingWhitespaceAt(string text, int offset)
        {
            int lineStart = text.LastIndexOf('\n', Math.Max(0, offset - 1)) + 1;
            int end = lineStart;
            while (end < text.Length && (text[end] == ' ' || text[end] == '\t'))
                end++;

            return text.Substring(lineStart, end - lineStart);
        }

        private static int GetIndentSize(TextDocument textDocument)
        {
            try
            {
                return Math.Max(1, textDocument.IndentSize);
            }
            catch
            {
                return 4;
            }
        }

        private static void FormatGeneratedRegion(DTE2 dte, TextDocument textDocument, string documentText, int start, int length)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                var selection = textDocument.Selection;
                selection.MoveToAbsoluteOffset(ToDteAbsoluteOffset(documentText, start), false);
                selection.MoveToAbsoluteOffset(ToDteAbsoluteOffset(documentText, start + length), true);
                dte.ExecuteCommand("Edit.FormatSelection");
            }
            catch
            {
                // Generation succeeds even when the active editor does not expose
                // Visual Studio's formatting command (for example, a text editor).
            }
        }

        // EnvDTE counts a CRLF sequence as one absolute character, whereas
        // .NET string offsets count both characters. All generator offsets are
        // string offsets, so convert at the DTE boundary in both directions.
        private static int ToDocumentOffset(string documentText, int dteAbsoluteOffset)
        {
            var remaining = Math.Max(0, dteAbsoluteOffset - 1);
            var documentOffset = 0;
            while (documentOffset < documentText.Length && remaining > 0)
            {
                if (documentText[documentOffset] == '\r' &&
                    documentOffset + 1 < documentText.Length &&
                    documentText[documentOffset + 1] == '\n')
                    documentOffset++;

                documentOffset++;
                remaining--;
            }
            return documentOffset;
        }

        private static int ToDteAbsoluteOffset(string documentText, int documentOffset)
        {
            var dteAbsoluteOffset = 1;
            for (var index = 0; index < documentOffset; index++)
            {
                if (documentText[index] == '\r' &&
                    index + 1 < documentOffset &&
                    documentText[index + 1] == '\n')
                    index++;

                dteAbsoluteOffset++;
            }
            return dteAbsoluteOffset;
        }
    }
}