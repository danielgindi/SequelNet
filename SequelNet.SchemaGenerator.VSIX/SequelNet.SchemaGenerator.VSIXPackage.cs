﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using EnvDTE80;
using SequelNet.SchemaGenerator;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace SequelNet_SchemaGenerator_VSIX
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    /// 
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(AllowsBackgroundLoading = true, UseManagedResourcesOnly = true)]
    [ProvideAutoLoad(UIContextGuids80.CodeWindow, PackageAutoLoadFlags.BackgroundLoad)]

    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]

    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]

    [Guid(GuidList.guidSequelNet_SchemaGenerator_VSIXPkgString)]
    public sealed class SequelNet_SchemaGenerator_VSIXPackage : AsyncPackage
    {
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public SequelNet_SchemaGenerator_VSIXPackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }

        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);

            // Add our command handlers for menu (commands must exist in the .vsct file)
            var mcs = await GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            if ( null != mcs )
            {
                // Create the command for the menu item.
                var menuCommandID = new CommandID(GuidList.guidSequelNet_SchemaGenerator_VSIXCmdSet, (int)PkgCmdIDList.cmdidGenerateDalFromSelection);
                var menuItem = new MenuCommand(MenuItemCallback, menuCommandID );
                mcs.AddCommand( menuItem );
            }
        }

        #endregion

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            if (uiShell == null) return;

            DTE2 dte = (DTE2)Package.GetGlobalService(typeof(DTE));

            try
            {
                Document activeDocument = dte.ActiveDocument;
                TextDocument textDocument = activeDocument.Object() as TextDocument;
                string selectionText = textDocument.Selection.Text.Trim();

                if (selectionText.Length == 0)
                {
                    Guid clsid = Guid.Empty;
                    int result;
                    Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                               0,
                               ref clsid,
                               "No data",
                               "Please select a text snippet with a DAL generation code,\nand then run this command again.",
                               string.Empty,
                               0,
                               OLEMSGBUTTON.OLEMSGBUTTON_OK,
                               OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                               OLEMSGICON.OLEMSGICON_INFO,
                               0,        // false
                               out result));
                    return;
                }

                string code = GeneratorCore.GenerateDalClass(selectionText);

                // Copy result to ClipBoard
                ClipboardHelper.SetClipboard(code);
            }
            catch (Exception exception)
            {
                Guid clsid = Guid.Empty;
                int result;
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                           0,
                           ref clsid,
                           "Failed to generate DAL code from script",
                           exception.ToString(),
                           string.Empty,
                           0,
                           OLEMSGBUTTON.OLEMSGBUTTON_OK,
                           OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                           OLEMSGICON.OLEMSGICON_INFO,
                           0,        // false
                           out result));
            }
        }

    }
}
