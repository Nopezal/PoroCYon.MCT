using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace PoroCYon.MCT.Installer
{
    public partial class License : UserControl
    {
        public License()
        {
            InitializeComponent();

            using (StreamReader r = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("PoroCYon.MCT.Installer.LICENSE.txt")))
            {
                LicenseText.Text = r.ReadToEnd();
            }

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
