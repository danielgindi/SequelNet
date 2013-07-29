using EnvDTE;
using EnvDTE80;
using Extensibility;
using System;
using System.Windows.Forms;

namespace dg.Sql.SchemaGeneratorAddIn
{
	public class Connect : IDTExtensibility2, IDTCommandTarget
	{
		public const string GenerateCommandName = "GenerateDalFromSelection";
		public const string CommandPrefix = "dg.Sql.SchemaGeneratorAddIn.Connect.";
		public object[] contextGuids;
		private DTE2 _applicationObject;
		private AddIn _addInInstance;

		public Connect()
		{
		}

		public void OnAddInsUpdate(ref Array custom)
		{
		}

		public void OnBeginShutdown(ref Array custom)
		{
		}

		public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
		{
			this._applicationObject = (DTE2)application;
			this._addInInstance = (AddIn)addInInst;
            contextGuids = new object[] { ContextGuids.vsContextGuidCodeWindow, ContextGuids.vsContextGuidTextEditor };

			try
			{
				object[] objArray1 = this.contextGuids;
                this._applicationObject.Commands.AddNamedCommand(this._addInInstance, GenerateCommandName, GenerateCommandName, "Generate a schema code from selection, into the clipboard", true, 0, ref contextGuids, (int)(vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled));
			}
			catch
			{
			}
		}

		public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
		{
		}

		public void OnStartupComplete(ref Array custom)
		{
		}

		public void QueryStatus(string CmdName, vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText)
		{
			vsCommandStatus vsCommandStatu;
            if (CmdName == "dg.Sql.SchemaGeneratorAddIn.Connect.GenerateDalFromSelection")
            {
                if (NeededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
                {
                    vsCommandStatu = (SchemaGenerator.HasSelection(this._applicationObject) ? vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled : vsCommandStatus.vsCommandStatusSupported);
                    StatusOption = vsCommandStatu;
                }
            }
		}

        public void Exec(string CmdName, vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            if (CmdName == "dg.Sql.SchemaGeneratorAddIn.Connect.GenerateDalFromSelection")
            {
                if (ExecuteOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
                {
                    try
                    {
                        SchemaGenerator.GenerateDalClass(this._applicationObject);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.ToString());
                    }
                    Handled = true;
                }
            }
        }
	}
}