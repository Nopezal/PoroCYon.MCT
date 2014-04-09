using System;
using System.Windows;
using System.Windows.Controls;

namespace PoroCYon.MCT.Installer
{
    public partial class CannotInstall : UserControl
    {
        public CannotInstall()
        {
            InitializeComponent();

            if (!String.IsNullOrEmpty(MainWindow.CannotInstallError))
                Error.Text = MainWindow.CannotInstallError;
        }
    }
}
