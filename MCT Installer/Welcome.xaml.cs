using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace PoroCYon.MCT.Installer
{
    public partial class Welcome : UserControl
    {
        public Welcome()
        {
            InitializeComponent();

            RedistLink.MouseLeftButtonDown += (s, e) =>
            {
                MainWindow.CanClose = true;
                MainWindow.warnForClose = false;

                Process.Start("https://dl.dropboxusercontent.com/u/151130168/MCT/MCT-redist.zip");

                MainWindow.instance.Close();
            };
        }
    }
}
