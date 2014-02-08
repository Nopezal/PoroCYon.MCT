using System;
using System.Windows;

namespace TAPI.SDK.Installer
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                App.Main();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "tAPI SDK Installer: Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
