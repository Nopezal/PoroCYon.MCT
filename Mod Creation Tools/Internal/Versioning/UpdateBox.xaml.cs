
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
using TAPI;

namespace PoroCYon.MCT.Internal.Versioning
{
    public partial class UpdateBox : UserControl
    {
        public UpdateBox()
        {
            if (Assembly.GetCallingAssembly() != Assembly.GetExecutingAssembly())
                throw new InvalidOperationException("You shall not create an UpdateBox");

            InitializeComponent();

            CloseBorder.MouseEnter += (s, e) =>
            {
                CloseBorder.BorderBrush = Brushes.OrangeRed;
                CloseText.Foreground = Brushes.OrangeRed;
            };
            CloseBorder.MouseLeave += (s, e) =>
            {
                CloseBorder.BorderBrush = Brushes.Lime;
                CloseText.Foreground = Brushes.Lime;
            };
            CloseText.MouseEnter += (s, e) =>
            {
                CloseBorder.BorderBrush = Brushes.OrangeRed;
                CloseText.Foreground = Brushes.OrangeRed;
            };
            CloseText.MouseLeave += (s, e) =>
            {
                CloseBorder.BorderBrush = Brushes.Lime;
                CloseText.Foreground = Brushes.Lime;
            };

            UpdateBorder.MouseEnter += (s, e) =>
            {
                UpdateBorder.BorderBrush = Brushes.OrangeRed;
                UpdateText.Foreground = Brushes.OrangeRed;
            };
            UpdateBorder.MouseLeave += (s, e) =>
            {
                UpdateBorder.BorderBrush = Brushes.Lime;
                UpdateText.Foreground = Brushes.Lime;
            };
            UpdateText.MouseEnter += (s, e) =>
            {
                UpdateBorder.BorderBrush = Brushes.OrangeRed;
                UpdateText.Foreground = Brushes.OrangeRed;
            };
            UpdateText.MouseLeave += (s, e) =>
            {
                UpdateBorder.BorderBrush = Brushes.Lime;
                UpdateText.Foreground = Brushes.Lime;
            };

            CloseBorder.MouseLeftButtonDown += (s, e) =>
            {
                UpdateBoxInjector.RemoveInjection();
            };
            CloseText.MouseLeftButtonDown += (s, e) =>
            {
                UpdateBoxInjector.RemoveInjection();
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
        }
    }
}

#pragma warning restore 1591
