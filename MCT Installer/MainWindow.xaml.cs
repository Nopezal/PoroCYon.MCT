using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;

namespace PoroCYon.MCT.Installer
{
    public enum Page : int
    {
        CannotInstall = -1,

        Welcome = 1,
        License = 2,
        ToInstall = 3,
        VsVersion = 4,
        Install = 5,
        Finished = 6,

        Count = 7
    }

    public partial class MainWindow : Window
    {
        internal static Page currentPage = Page.Welcome;

        internal static bool warnForClose = true;

        internal static MainWindow instance;

        internal static bool CanClose
        {
            get
            {
                return instance.CloseMe.IsEnabled;
            }
            set
            {
                instance.CloseMe.Visibility = (instance.CloseMe.IsEnabled = value) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            instance = this;

            MouseLeftButtonDown += (s, e) => { try { DragMove(); } catch (InvalidOperationException) { } };

            Next.MouseLeftButtonDown += (s, e) =>
            {
                currentPage++;
                UpdateGrid();
            };
            NextBorder.MouseLeftButtonDown += (s, e) =>
            {
                currentPage++;
                UpdateGrid();
            };

            Previous.MouseLeftButtonDown += (s, e) =>
            {
                currentPage--;
                UpdateGrid();
            };
            PreviousBorder.MouseLeftButtonDown += (s, e) =>
            {
                currentPage--;
                UpdateGrid();
            };

            CloseMe.MouseLeftButtonDown += (s, e) =>
            {
                Close();
            };
            MinMe.MouseLeftButtonDown += (s, e) =>
            {
                WindowState = WindowState.Minimized;
            };

            CloseMe.MouseEnter += (s, e) =>
            {
                CloseMe.Background = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(Color.FromArgb((byte)(255d * (2d / 3d)), 200, 0, 0), 0d),
                    new GradientStop(Color.FromArgb((byte)(255d * (2d / 3d)), 200, 0, 0), 2d / 3d),
                    new GradientStop(Colors.Transparent, 1d)
                }, 90d);

                CloseMe.Foreground = Brushes.Red;
            };
            CloseMe.MouseLeave += (s, e) =>
            {
                CloseMe.Background = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(Color.FromArgb((byte)(255d * (1d / 3d)), 200, 0, 0), 0d),
                    new GradientStop(Color.FromArgb((byte)(255d * (1d / 3d)), 200, 0, 0), 2d / 3d),
                    new GradientStop(Colors.Transparent, 1d)
                }, 90d);

                CloseMe.Foreground = new SolidColorBrush(Color.FromRgb(200, 0, 0));
            };

            MinMe.MouseEnter += (s, e) =>
            {
                MinMe.Background = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(Color.FromArgb((byte)(255d * (2d / 3d)), 0, 0, 200), 0d),
                    new GradientStop(Color.FromArgb((byte)(255d * (2d / 3d)), 0, 0, 200), 2d / 3d),
                    new GradientStop(Colors.Transparent, 1d)
                }, 90d);

                MinMe.Foreground = Brushes.Blue;
            };
            MinMe.MouseLeave += (s, e) =>
            {
                MinMe.Background = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(Color.FromArgb((byte)(255d * (1d / 3d)), 0, 0, 200), 0d),
                    new GradientStop(Color.FromArgb((byte)(255d * (1d / 3d)), 0, 0, 200), 2d / 3d),
                    new GradientStop(Colors.Transparent, 1d)
                }, 90d);

                MinMe.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 200));
            };

            Next.MouseEnter += (s, e) =>
            {
                Next.Foreground = Brushes.OrangeRed;
                NextBorder.BorderBrush = Brushes.OrangeRed;
            };
            NextBorder.MouseEnter += (s, e) =>
            {
                Next.Foreground = Brushes.OrangeRed;
                NextBorder.BorderBrush = Brushes.OrangeRed;
            };

            Next.MouseLeave += (s, e) =>
            {
                Next.Foreground = Brushes.Lime;
                NextBorder.BorderBrush = Brushes.Lime;
            };
            NextBorder.MouseLeave += (s, e) =>
            {
                Next.Foreground = Brushes.Lime;
                NextBorder.BorderBrush = Brushes.Lime;
            };

            Previous.MouseEnter += (s, e) =>
            {
                Previous.Foreground = Brushes.OrangeRed;
                PreviousBorder.BorderBrush = Brushes.OrangeRed;
            };
            PreviousBorder.MouseEnter += (s, e) =>
            {
                Previous.Foreground = Brushes.OrangeRed;
                PreviousBorder.BorderBrush = Brushes.OrangeRed;
            };

            Previous.MouseLeave += (s, e) =>
            {
                Previous.Foreground = Brushes.Lime;
                PreviousBorder.BorderBrush = Brushes.Lime;
            };
            PreviousBorder.MouseLeave += (s, e) =>
            {
                Previous.Foreground = Brushes.Lime;
                PreviousBorder.BorderBrush = Brushes.Lime;
            };

            if (!CheckCanInstall())
                currentPage = Page.CannotInstall;

            UpdateGrid();
        }

        internal void UpdateGrid()
        {
            Contents.Children.Clear();
            SetNext(true);
            SetPrevious(true);
            CanClose = warnForClose = true;

            switch (currentPage)
            {
                case Page.CannotInstall:
                    SetNext(false);
                    SetPrevious(false);
                    Contents.Children.Add(new CannotInstall());
                    break;
                case Page.Welcome:
                    SetPrevious(false);
                    Contents.Children.Add(new Welcome());
                    break;
                case Page.License:
                    SetNext(false);
                    Next.IsEnabled = NextBorder.IsEnabled = false;
                    Next.Visibility = NextBorder.Visibility = Visibility.Collapsed;
                    Contents.Children.Add(new License());
                    break;
                case Page.ToInstall:
                    Contents.Children.Add(new ToInstall());
                    break;
                case Page.VsVersion:
                    Contents.Children.Add(new VsVersions());
                    break;
                case Page.Install:
                    CanClose = false;
                    SetNext(false);
                    SetPrevious(false);
                    Contents.Children.Add(new Installing());
                    break;
                case Page.Finished:
                    warnForClose = false;
                    SetNext(false);
                    SetPrevious(false);
                    Contents.Children.Add(new Ending());
                    break;
            }
        }

        static bool CheckCanInstall()
        {
			if (!NetworkInterface.GetIsNetworkAvailable())
				return false;

            RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam");
            if (regKey == null)
                return false;

            string steamDir = Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam").GetValue("SourceModInstallPath").ToString();
            steamDir = steamDir.Substring(0, steamDir.Length - "sourcemods".Length) + "common\\Terraria\\";

            if (!Directory.Exists(steamDir) || !File.Exists(steamDir + "Terraria.exe") || !File.Exists(steamDir + "tAPI.exe"))
                return false;

            Assembly a = Assembly.LoadFrom(steamDir + "tAPI.exe");
            if ((uint)a.GetType("TAPI.Constants").GetField("versionAssembly").GetValue(null) < 4u) // not r4
                return false;

            #region Dictionary<VSVersion, string> asString = new Dictionary<VSVersion, string>()
            Dictionary<VSVersion, string> asString = new Dictionary<VSVersion, string>()
            {
                {
                    VSVersion.VCSExpress,
                    "VCSExpress\\10.0"
                },
                {
                    VSVersion.VisualStudio10,
                    "VisualStudio\\10.0"
                },
                
                {
                    VSVersion.WDExpress11,
                    "WDExpress\\11.0"
                },
                {
                    VSVersion.VisualStudio11,
                    "VisualStudio\\11.0"
                },
                
                {
                    VSVersion.WDExpress12,
                    "WDExpress\\12.0"
                },
                {
                    VSVersion.VisualStudio12,
                    "VisualStudio\\12.0"
                },
            };
            #endregion

            for (int i = 1; i <= 32; i *= 2)
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\" + asString[(VSVersion)i]);
                if (key != null)
                    if (key.GetValue("FullScreen") != null) // random key
                        VsVersions.PossibleVersions |= (VSVersion)i;
            }

            return true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!CanClose)
                e.Cancel = true;
            else if (warnForClose && MessageBox.Show("This will cancel the installation of the Mod Creation Tools.\n\nContinue?", "Mod Creation Tools Installer",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                e.Cancel = true;

            base.OnClosing(e);
        }

        internal static void SetNext(bool value)
        {
            if (value)
            {
                instance.Next.IsEnabled  = instance.NextBorder.IsEnabled  = true;
                instance.Next.Visibility = instance.NextBorder.Visibility = Visibility.Visible;
            }
            else
            {
                instance.Next.IsEnabled  = instance.NextBorder.IsEnabled  = false;
                instance.Next.Visibility = instance.NextBorder.Visibility = Visibility.Hidden;
            }
        }
        internal static void SetPrevious(bool value)
        {
            if (value)
            {
                instance.Previous.IsEnabled  = instance.PreviousBorder.IsEnabled  = true;
                instance.Previous.Visibility = instance.PreviousBorder.Visibility = Visibility.Visible;
            }
            else
            {
                instance.Previous.IsEnabled  = instance.PreviousBorder.IsEnabled =  false;
                instance.Previous.Visibility = instance.PreviousBorder.Visibility = Visibility.Hidden;
            }
        }
    }
}
