using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TAPI.SDK.Installer
{
    public partial class ToInstall : UserControl
    {
        public static bool InstallPdb;

        public ToInstall()
        {
            InstallPdb = true;

            InitializeComponent();

            PDBs.Checked += (s, e) =>
            {
                InstallPdb = PDBs.IsChecked == true;
            };
            PDBs.Unchecked += (s, e) =>
            {
                InstallPdb = PDBs.IsChecked == true;
            };
        }
    }
}
