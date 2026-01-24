using System.Threading;
using System.Windows;

namespace SequelNet.SchemaGenerator;

public static class ClipboardHelper
{
    private static string clipboardData;

    private static void _SetClipboard()
    {
        Clipboard.SetDataObject(ClipboardHelper.clipboardData, true);
    }

    public static void SetClipboard(string text)
    {
        ClipboardHelper.clipboardData = text;
        Thread thread = new Thread(new ThreadStart(ClipboardHelper._SetClipboard));
        thread.SetApartmentState(ApartmentState.STA);
        thread.IsBackground = true;
        thread.Start();
        thread.Join();
        thread = null;
    }
}