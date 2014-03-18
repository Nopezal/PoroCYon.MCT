
#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using TAPI;

namespace PoroCYon.MCT.Internal.Versioning
{
    public partial class UpdateBox : UserControl
    {
        public UpdateBox()
        {
            if (Assembly.GetCallingAssembly() != Assembly.GetExecutingAssembly())
                throw new InvalidOperationException("You shall not create an UpdateBox");

            try
            {
                Resources.MergedDictionaries.Add(Application.LoadComponent(new Uri(
                    "PresentationFramework.AeroLite;V4.0.0.0;31bf3856ad364e35;component\\themes/aerolite.normalcolor.xaml", UriKind.Relative)) as ResourceDictionary);
            }
            catch { } // assembly not found

            InitializeComponent();

            CloseBorder.MouseEnter += (s, e) =>
            {
                CloseBorder.BorderBrush = Brushes.LightGray;
                CloseText.Foreground = Brushes.LightGray;

                CloseBorder.RenderTransform = new ScaleTransform(1.15d, 1.15d, CloseBorder.ActualWidth / 2d, CloseBorder.ActualHeight / 2d);
                CloseText.RenderTransform = new ScaleTransform(1.15d, 1.15d, CloseText.ActualWidth / 2d, CloseText.ActualHeight / 2d);
            };
            CloseBorder.MouseLeave += (s, e) =>
            {
                CloseBorder.BorderBrush = Brushes.White;
                CloseText.Foreground = Brushes.White;

                CloseBorder.RenderTransform = new ScaleTransform(1d, 1d, CloseBorder.ActualWidth / 2d, CloseBorder.ActualHeight / 2d);
                CloseText.RenderTransform = new ScaleTransform(1d, 1d, CloseText.ActualWidth / 2d, CloseText.ActualHeight / 2d);
            };
            CloseText.MouseEnter += (s, e) =>
            {
                CloseBorder.BorderBrush = Brushes.LightGray;
                CloseText.Foreground = Brushes.LightGray;

                CloseBorder.RenderTransform = new ScaleTransform(1.15d, 1.15d, CloseBorder.ActualWidth / 2d, CloseBorder.ActualHeight / 2d);
                CloseText.RenderTransform = new ScaleTransform(1.15d, 1.15d, CloseText.ActualWidth / 2d, CloseText.ActualHeight / 2d);
            };
            CloseText.MouseLeave += (s, e) =>
            {
                CloseBorder.BorderBrush = Brushes.White;
                CloseText.Foreground = Brushes.White;

                CloseBorder.RenderTransform = new ScaleTransform(1d, 1d, CloseBorder.ActualWidth / 2d, CloseBorder.ActualHeight / 2d);
                CloseText.RenderTransform = new ScaleTransform(1d, 1d, CloseText.ActualWidth / 2d, CloseText.ActualHeight / 2d);
            };

            UpdateBorder.MouseEnter += (s, e) =>
            {
                UpdateBorder.BorderBrush = Brushes.LightGray;
                UpdateText.Foreground = Brushes.LightGray;

                UpdateBorder.RenderTransform = new ScaleTransform(1.15d, 1.15d, UpdateBorder.ActualWidth / 2d, UpdateBorder.ActualHeight / 2d);
                UpdateText.RenderTransform = new ScaleTransform(1.15d, 1.15d, UpdateText.ActualWidth / 2d, UpdateText.ActualHeight / 2d);
            };
            UpdateBorder.MouseLeave += (s, e) =>
            {
                UpdateBorder.BorderBrush = Brushes.White;
                UpdateText.Foreground = Brushes.White;

                UpdateBorder.RenderTransform = new ScaleTransform(1d, 1d, UpdateBorder.ActualWidth / 2d, UpdateBorder.ActualHeight / 2d);
                UpdateText.RenderTransform = new ScaleTransform(1d, 1d, UpdateText.ActualWidth / 2d, UpdateText.ActualHeight / 2d);
            };
            UpdateText.MouseEnter += (s, e) =>
            {
                UpdateBorder.BorderBrush = Brushes.LightGray;
                UpdateText.Foreground = Brushes.LightGray;

                UpdateBorder.RenderTransform = new ScaleTransform(1.15d, 1.15d, UpdateBorder.ActualWidth / 2d, UpdateBorder.ActualHeight / 2d);
                UpdateText.RenderTransform = new ScaleTransform(1.15d, 1.15d, UpdateText.ActualWidth / 2d, UpdateText.ActualHeight / 2d);
            };
            UpdateText.MouseLeave += (s, e) =>
            {
                UpdateBorder.BorderBrush = Brushes.White;
                UpdateText.Foreground = Brushes.White;

                UpdateBorder.RenderTransform = new ScaleTransform(1d, 1d, UpdateBorder.ActualWidth / 2d, UpdateBorder.ActualHeight / 2d);
                UpdateText.RenderTransform = new ScaleTransform(1d, 1d, UpdateText.ActualWidth / 2d, UpdateText.ActualHeight / 2d);
            };

            CloseBorder.MouseLeftButtonDown += (s, e) =>
            {
                UpdateBoxInjector.RemoveInjection();
                UpdateChecker.LastUpdateAvailable = false;
            };
            CloseText.MouseLeftButtonDown += (s, e) =>
            {
                UpdateBoxInjector.RemoveInjection();
                UpdateChecker.LastUpdateAvailable = false;
            };

            UpdateBorder.MouseLeftButtonDown += (s, e) =>
            {
                Process.Start("https://dl.dropboxusercontent.com/u/151130168/MCT/MCT%20Installer.exe");
                Constants.mainInstance.Exit();
            };
            UpdateText.MouseLeftButtonDown += (s, e) =>
            {
                Process.Start("https://dl.dropboxusercontent.com/u/151130168/MCT/MCT%20Installer.exe");
                Constants.mainInstance.Exit();
            };

            XmlNode
                version = UpdateChecker.GetXml().ChildNodes[1],
                current = version.ChildNodes[0];

            CurrentVersion.Text = "Current: " + MctConstants.VERSION_STRING;
            NewVersion.Text = "New: " + current.Attributes["String"].Value;

            foreach (XmlNode xn in version.ChildNodes[1])
            {
                Changelog.Items.Add("v" + xn.Attributes["Version"].Value);

                foreach (XmlNode Xn in xn.ChildNodes)
                    Changelog.Items.Add("  " + Xn.InnerText);
            }
        }
    }
}

#pragma warning restore 1591
