using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Win32;

namespace TAPI.SDK.Installer
{
    public partial class Installing : UserControl
    {
        static DispatcherTimer timer;

        public Installing()
        {
            InitializeComponent();

            Progress.ValueChanged += (s, e) =>
            {
                ProgressPercent.Text = Progress.Value + "%";
            };

            timer = new DispatcherTimer()
            {
                Interval = new TimeSpan(1)
            };
            timer.Tick += (s, e) =>
            {
                Action<int, string> UpdateProgress = (procent, text) =>
                {
                    Progress.Value = procent;
                    ProgressText.Text = text;

                    Dispatcher.Invoke(((Action)delegate { }), DispatcherPriority.Render);
                };
                Action<string, string> WriteToFile = (resource, path) =>
                {
                    MemoryStream ms = new MemoryStream();
                    Assembly.GetCallingAssembly().GetManifestResourceStream(resource).CopyTo(ms);
                    File.WriteAllBytes(path, ms.ToArray());
                    ms.Dispose();
                };

                string steamDir = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam").GetValue("SourceModInstallPath").ToString();
                steamDir = steamDir.Substring(0, steamDir.Length - "sourcemods".Length) + "common\\Terraria\\";

                UpdateProgress(0, "Deploying library PoroCYon.XnaExtensions...");
                WriteToFile("TAPI.SDK.Installer.Binaries.PoroCYon.XnaExtensions.dll", steamDir + "\\PoroCYon.XnaExtensions.dll");

                UpdateProgress(20, "Deploying the SDK Library...");
                WriteToFile("TAPI.SDK.Installer.Binaries.TAPI.SDK.dll", steamDir + "\\TAPI.SDK.dll");

                UpdateProgress(50, "Deploying the .dll mod builder...");
                WriteToFile("TAPI.SDK.Installer.Binaries.tAPI SDK Mod Builder.exe", steamDir + "\\tAPI SDK Mod Builder.exe");

                UpdateProgress(60, "Deploying the .tapimod decompiler...");
                WriteToFile("TAPI.SDK.Installer.Binaries.tAPI SDK Mod Decompiler.exe", steamDir + "\\tAPI SDK Mod Decompiler.exe");

                //UpdateProgress(70, "Deploying the .tapi extractor...");
                //WriteToFile("TAPI.SDK.Installer.Binaries.tAPI SDK Mod Extractor.exe", steamDir + "\\tAPI SDK Mod Extractor.exe");

                UpdateProgress(80, "Deploying the extended .tapimod packer (multi-language support)...");
                WriteToFile("TAPI.SDK.Installer.Binaries.tAPI Extended Packer.exe", steamDir + "\\tAPI Extended Packer.exe");

                //UpdateProgress(90, "Deploying the tAPI SDK Tools...");
                //WriteToFile("TAPI.SDK.Installer.Binaries.tAPI SDK Tools.exe", steamDir + "\\tAPI SDK Tools.exe");

                UpdateProgress(100, "Finished installing");

                MainWindow.instance.Dispatcher.Invoke(((Action)delegate
                {
                    MainWindow.currentPage++;
                    MainWindow.instance.UpdateGrid();
                }), DispatcherPriority.Render);

                timer.Stop();
            };

            timer.Start();
        }
    }
}
