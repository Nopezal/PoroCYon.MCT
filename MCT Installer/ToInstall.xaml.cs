using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PoroCYon.MCT.Installer
{
    public partial class ToInstall : UserControl
    {
        public static bool InstallPdb, FSharpCompiler;

        public ToInstall()
        {
            InstallPdb = true;

            InitializeComponent();

            PDBs.Checked += (s, e) => InstallPdb = PDBs.IsChecked == true;
            PDBs.Unchecked += (s, e) => InstallPdb = PDBs.IsChecked == true;

            FSharp.Checked += (s, e) => FSharpCompiler = FSharp.IsChecked == true;
            FSharp.Unchecked += (s, e) => FSharpCompiler = FSharp.IsChecked == true;
        }
    }
}
