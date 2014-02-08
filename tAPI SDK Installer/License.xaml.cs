using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace TAPI.SDK.Installer
{
    public partial class License : UserControl
    {
        public License()
        {
            InitializeComponent();

            StreamReader r = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("TAPI.SDK.Installer.LICENSE.txt"));
            LicenseText.Text = r.ReadToEnd();
            r.Dispose();

            Agree.Checked += (s, e) =>
            {
                MainWindow.SetNext(true);
            };
            Agree.Unchecked += (s, e) =>
            {
                MainWindow.SetNext(false);
            };
            Disagree.Checked += (s, e) =>
            {
                MainWindow.SetNext(false);
            };
            Disagree.Unchecked += (s, e) =>
            {
                MainWindow.SetNext(true);
            };
        }
    }
}
