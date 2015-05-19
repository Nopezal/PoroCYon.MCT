using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            catch (Exception e) when (!Debugger.IsAttached)
            {
                DisplayError(e);
            }
        }

        internal static void DisplayError(Exception e)
        {
            MessageBox.Show("An error has occured.\nPlease show this message to PoroCYon, but make sure\n"
                + "you're running this as an admin, and you've ran Terraria and tAPI at least once.\n\n"
                + e, "Mod Creation Tools Installer: Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
