using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Win32;

namespace TAPI.SDK.Installer
{
    public partial class Installing : UserControl
    {
        readonly static string steamDir = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam").GetValue("SourceModInstallPath").ToString();

        static Queue<Tuple<string, byte[]>> downloaded = new Queue<Tuple<string, byte[]>>();
		static bool finishedDownloading = false;
		static int total = 0;

        static Installing()
        {
            steamDir = steamDir.Substring(0, steamDir.Length - "sourcemods".Length) + "common\\Terraria\\";
        }

        public Installing()
        {
            InitializeComponent();

			finishedDownloading = false;

            AquireProgress.ValueChanged += (s, e) =>
            {
                AquireProgressPercent.Text = AquireProgress.Value + "%";
            };
            ApplyProgress.ValueChanged += (s, e) =>
            {
                ApplyProgressPercent.Text = ApplyProgress.Value + "%";
            };

            Thread aquire = new Thread(() =>
            {
                Action<int, string> UpdateProgress = (procent, text) =>
                {
                    Dispatcher.Invoke(((Action)delegate
                    {
                        if (procent >= 0 && procent <= 100)
                            AquireProgress.Value = procent;
                        if (!String.IsNullOrEmpty(text))
                            AquireProgressText.Text = "Aquiring: " + text;
                    }), DispatcherPriority.Render);
                };

                WebClient client = new WebClient();

                const string baseUri = "https://dl.dropboxusercontent.com/u/151130168/tAPI%20SDK/";

                List<string> ToDownload = new List<string>()
                {
					"PoroCYon.XnaExtensions.dll", "PoroCYon.XnaExtensions.xml",
					
					"TAPI.SDK.dll", "TAPI.SDK.xml",
					
					"tAPI Extended Packer.exe",
					"tAPI SDK Debugger.exe",
					"tAPI SDK Mod Builder.exe",
					"tAPI SDK Mod Decompiler.exe",
                };
                if (ToInstall.InstallPdb)
                {
                    ToDownload.Add("PoroCYon.XnaExtensions.pdb");

                    ToDownload.Add("TAPI.SDK.pdb");

                    ToDownload.Add("tAPI Extended Packer.pdb");
                    ToDownload.Add("tAPI SDK Debugger.pdb");
                    ToDownload.Add("tAPI SDK Mod Builder.pdb");
                    ToDownload.Add("tAPI SDK Mod Decompiler.pdb");
                }

                total = ToDownload.Count;

                for (int i = 0; i < ToDownload.Count; i++)
                {
                    UpdateProgress(-1, Path.GetFileName(ToDownload[i]));
                    downloaded.Enqueue(new Tuple<string, byte[]>(ToDownload[i], client.DownloadData(baseUri + ToDownload[i])));
                    UpdateProgress(100 / ((total + 1) / (i + 1)), null);
                }

                client.Dispose();

                finishedDownloading = true;
            });
            aquire.SetApartmentState(ApartmentState.STA);
            aquire.Start();

            Thread apply = new Thread(() =>
            {
                Action<int, string> UpdateProgress = (procent, text) =>
                {
                    Dispatcher.Invoke(((Action)delegate
					{
						if (procent >= 0 && procent <= 100)
							ApplyProgress.Value = procent;
						if (!String.IsNullOrEmpty(text))
							ApplyProgressText.Text = text;
                    }), DispatcherPriority.Render);
                };

				int i = 0;

				while (!finishedDownloading || downloaded.Count > 0)
				{
					if (downloaded.Count <= 0)
						continue;

					Tuple<string, byte[]> t = downloaded.Dequeue();

					UpdateProgress(-1, t.Item1);
					File.WriteAllBytes(steamDir + t.Item1, t.Item2);
					UpdateProgress(100 / ((total + 1) / (i + 1)), null);

					i++;
				}

				MainWindow.instance.Dispatcher.Invoke(((Action)delegate
				{
					MainWindow.currentPage++;
					MainWindow.instance.UpdateGrid();
				}), DispatcherPriority.Render);
            });
            apply.SetApartmentState(ApartmentState.STA);
            apply.Start();
        }
    }
}
