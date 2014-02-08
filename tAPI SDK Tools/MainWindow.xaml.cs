using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Win32;

namespace TAPI.SDK.Tools
{
    using MouseEventArgs = System.Windows.Input.MouseEventArgs;
    using MessageBox = System.Windows.MessageBox;

    public partial class MainWindow : Window
    {
        static Process proc;
        static bool procNoInit = true;

        static DispatcherTimer timer = new DispatcherTimer();

        static StreamReader output, error;

        public MainWindow()
        {
            InitializeComponent();

            MouseLeftButtonDown += (s, e) =>
            {
                try
                {
                    DragMove();
                }
                catch { }
            };

            #region control box
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
                    new GradientStop(Colors.Transparent, 1),
                    new GradientStop(new Color()
                    {
                        A = 0x66,
                        R = 255
                    }, 0.666d)
                }, 90d);
                CloseMe.Foreground = new SolidColorBrush(Colors.Red);
            };
            CloseMe.MouseLeave += (s, e) =>
            {
                CloseMe.Background = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(Colors.Transparent, 1),
                    new GradientStop(new Color()
                    {
                        A = 0x54,
                        R = 200
                    }, 0.666d)
                }, 90d);
                CloseMe.Foreground = new SolidColorBrush(new Color()
                {
                    A = 255,
                    R = 200
                });
            };

            MinMe.MouseEnter += (s, e) =>
            {
                MinMe.Background = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(Colors.Transparent, 1),
                    new GradientStop(new Color()
                    {
                        A = 0x66,
                        B = 255
                    }, 0.618d)
                }, 90d);
                MinMe.Foreground = new SolidColorBrush(Colors.Blue);
            };
            MinMe.MouseLeave += (s, e) =>
            {
                MinMe.Background = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(Colors.Transparent, 1),
                    new GradientStop(new Color()
                    {
                        A = 0x54,
                        B = 200
                    }, 0.618d)
                }, 90d);
                MinMe.Foreground = new SolidColorBrush(new Color()
                {
                    A = 255,
                    B = 200
                });
            };
            #endregion

            #region RunProcWithArgs
            Action<string, string> RunProcWithArgs = (procName, fileType) =>
            {
                if (!procNoInit)
                    try
                    {
                        if (!proc.HasExited)
                        {
                            MessageBox.Show(proc.ProcessName + " is still running!", "tAPI SDK Tools", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                    }
                    catch (InvalidOperationException) { }
                    catch (NullReferenceException) { }

                OpenFileDialog dialog = new OpenFileDialog()
                {
                    Multiselect = false,
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\My Games\\Terraria\\tAPI\\Mods",
                    Filter = fileType,
                };
                bool? result = dialog.ShowDialog();

                if (result == true)
                {
                    proc = new Process()
                    {
                        StartInfo = new ProcessStartInfo()
                        {
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            FileName = procName,
                            Arguments = "\"" + Path.GetDirectoryName(dialog.FileName) + "\"",
                            RedirectStandardError = true,
                            RedirectStandardOutput = true
                        }
                    };
                    proc.Start();
                    error = proc.StandardError;
                    output = proc.StandardOutput;
                    Expando.IsExpanded = true;

                    timer.Start();
                }

                procNoInit = false;
            };
            #endregion

            #region timer
            timer = new DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 0, 0, 1000 / 20)
            };
            timer.Tick += (s, e) =>
            {
                Action<string, bool> PrintStuff = (toPrint, isError) =>
                {
                    Log.Inlines.Add(new LineBreak());
                    Log.Inlines.Add(new Run(toPrint) { Foreground = isError ? Brushes.Red : Brushes.Black });

                    Dispatcher.Invoke(((Action)delegate { }), DispatcherPriority.Render);
                };

                try
                {
                    IntPtr h = proc.Handle;
                }
                catch
                {
                    // proc is not assigned to a process which is alive
                    return;
                }

                if (proc.HasExited)
                {
                    Expando.IsExpanded = false;
                    // MAGIC!
                    Dispatcher.Invoke(((Action)delegate { }), DispatcherPriority.Render);

                    timer.Stop();
                }
            };
            #endregion

            #region buttons
            EPacker.MouseLeftButtonDown += (s, e) =>
            {
                RunProcWithArgs("tAPI Extended Packer.exe", "Mod Info/Settings File|*.json");
            };
            Builder.MouseLeftButtonDown += (s, e) =>
            {
                RunProcWithArgs("tAPI SDK Mod Builder.exe", ".NET Assembly|*.dll,*.exe");
            };
            Decompiler.MouseLeftButtonDown += (s, e) =>
            {
                RunProcWithArgs("tAPI SDK Mod Decompiler.exe", "tAPI Mod File|*.tapimod");
            };
            #endregion
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                if (!proc.HasExited)
                {
                    MessageBox.Show("A background process is still running!\n\ntAPI SDK Tools will automatically close when the process has terminated.", "tAPI SDK Tools", MessageBoxButton.OK, MessageBoxImage.Information);
                    e.Cancel = true;
                    Dispatcher.Invoke(((Action)delegate { proc.WaitForExit(); Close(); }), DispatcherPriority.Send);
                }
            }
            catch { }
            base.OnClosing(e);
        }
    }
}
