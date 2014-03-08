using System;
using System.Windows;

namespace PoroCYon.MCT.Installer
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
                MessageBox.Show(e.ToString(), "Mod Creation Tools Installer: Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
