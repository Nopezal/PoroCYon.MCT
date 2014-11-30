using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Windows;
using System.Windows.Forms; // I'm sorry
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.Win32;

namespace PoroCYon.MCT.Installer
{
    using FormsWin32Window  = System.Windows.Forms.IWin32Window;
    using AvalonWin32Window = System.Windows.Forms.IWin32Window;

    using FormsDialogResult = System.Windows.Forms.DialogResult;

    using MessageBox  = System.Windows.MessageBox;
    using MessageForm = System.Windows.Forms.MessageBox;

    public enum Page : sbyte
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

    class WpfWin32Window : FormsWin32Window
    {
        Window wnd;

        internal WpfWin32Window(Window window)
        {
            wnd = window;
        }
        ~WpfWin32Window()
        {
            wnd = null; // avoid circular reference
        }

        public IntPtr Handle
        {
            get
            {
                return new WindowInteropHelper(wnd).Handle;
            }
        }
    }

    public partial class MainWindow : Window
    {
        internal static Page currentPage = Page.Welcome;
        internal static bool warnForClose = true;
        internal static MainWindow instance;
        internal static string CannotInstallError = "";

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

            if (!String.IsNullOrEmpty(CannotInstallError = CheckCanInstall()))
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

        static string CheckCanInstall()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return "You are not connected to the Internet. The installer requres an internet connection.";

            RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam");
            if (regKey == null)
                return "You do not have Steam installed.";

            string steamDir = "";
            try
            {
                steamDir = Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam").GetValue("SteamPath").ToString() // without this call, it would actually be bugging
                    + "\\SteamApps\\common\\Terraria\\";
            }
            catch (NullReferenceException) // key does not exist
            {
                // select dir here

                FolderBrowserDialog fbd = new FolderBrowserDialog()
                {
                    Description = "Steam folder not found, please select the folder manually.",
                    ShowNewFolderButton = false
                };
                DialogResult result = FormsDialogResult.None;

                while (result != FormsDialogResult.OK && result != FormsDialogResult.Yes)
                    result = fbd.ShowDialog(new WpfWin32Window(instance));

                if (String.IsNullOrEmpty(fbd.SelectedPath) || !Directory.Exists(fbd.SelectedPath))
                    return "The directory you entered is invalid.";
            }

            string ret = CheckCanInstall(steamDir);

            if (ret == "The directory you entered is invalid.")
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog()
                {
                    Description = "Steam folder not found, please select the folder manually.",
                    //RootFolder = Environment.SpecialFolder.ProgramFilesX86,
                    ShowNewFolderButton = false
                };
                DialogResult result = FormsDialogResult.None;

                while (result != FormsDialogResult.OK && result != FormsDialogResult.Yes)
                    result = fbd.ShowDialog(new WpfWin32Window(instance));

                ret = CheckCanInstall(fbd.SelectedPath);
            }

            return ret;
        }
        static string CheckCanInstall(string baseFolder)
        {
            if (String.IsNullOrEmpty(baseFolder) || !Directory.Exists(baseFolder))
                return "The directory you entered is invalid.";
            if (!File.Exists(baseFolder + "\\Terraria.exe"))
                return "You do not have Terraria installed";
            if (!File.Exists(baseFolder + "\\tAPI.exe"))
                return "You do not have tAPI installed";

            Assembly a = Assembly.LoadFrom(baseFolder + "\\tAPI.exe"); // no try/catch needed, already checked at previous if-statement
            if ((uint)a.GetType("TAPI.API").GetField("versionAssembly").GetValue(null) < 14u
                    || a.GetType("TAPI.API").GetField("versionAssembly").GetValue(null).ToString()[0] < 'a') // not r14a
                return "You do not have tAPI r14 installed";

            #region Dictionary<VsVersion, string> asString = new Dictionary<VsVersion, string>()
            Dictionary<VsVersion, string> asString = new Dictionary<VsVersion, string>()
            {
                {
                    VsVersion.VCSExpress,
                    "VCSExpress\\10.0"
                },
                {
                    VsVersion.VisualStudio10,
                    "VisualStudio\\10.0"
                },
                {
                    VsVersion.WDExpress11,
                    "WDExpress\\11.0"
                },
                {
                    VsVersion.VisualStudio11,
                    "VisualStudio\\11.0"
                },
                {
                    VsVersion.WDExpress12,
                    "WDExpress\\12.0"
                },
                {
                    VsVersion.VisualStudio12,
                    "VisualStudio\\12.0"
                },
                {
                    VsVersion.VisualStudio14CTP,
                    "VisualStudio\\14.0"
                }
            };
            #endregion

            for (int i = 1; i <= (int)VsVersion.VisualStudio14CTP; i *= 2)
            {
                try
                {
                    RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\" + asString[(VsVersion)i]);
                    if (key != null && key.GetValue("FullScreen") != null) // random key
                        VsVersions.PossibleVersions |= (VsVersion)i;
                }
                catch (NullReferenceException) { } // VS key does not exist, do not return false this time
            }

            return String.Empty;
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
