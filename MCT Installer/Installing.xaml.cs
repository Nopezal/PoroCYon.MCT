using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Win32;

namespace PoroCYon.MCT.Installer
{
    public partial class Installing : UserControl
    {
        readonly static string steamDir = Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam").GetValue("SourceModInstallPath").ToString();

        static Queue<Tuple<string, byte[]>> downloaded = new Queue<Tuple<string, byte[]>>();
        static bool finishedDownloading = false;
        static int total = 0, applied = 0;

        static Installing()
        {
            steamDir = steamDir.Substring(0, steamDir.Length - "sourcemods".Length) + "common\\Terraria\\";
        }

        public Installing()
        {
            InitializeComponent();

            finishedDownloading = false;

            AquireProgress.ValueChanged += (s, e) => AquireProgressPercent.Text = (int)AquireProgress.Value + "%";
            ApplyProgress.ValueChanged  += (s, e) => ApplyProgressPercent.Text  = (int)ApplyProgress.Value  + "%";

            #region aquire
            Thread aquire = new Thread(() =>
            {
                try
                {
                    Action<double, string> UpdateProgress = (procent, text) =>
                    {
                        Dispatcher.Invoke(((Action)delegate
                        {
                            if (procent >= 0d && procent <= 100d)
                                AquireProgress.Value = procent;
                            if (!String.IsNullOrEmpty(text))
                                AquireProgressText.Text = "Aquiring: " + text;
                        }), DispatcherPriority.Render);
                    };

                    WebClient client = new WebClient();

                    const string baseUri = "https://dl.dropboxusercontent.com/u/151130168/MCT/";

                    List<string> ToDownload = new List<string>()
                    {
                        "PoroCYon.XnaExtensions.dll",  "PoroCYon.XnaExtensions.xml",
                        "PoroCYon.MCT.dll",            "PoroCYon.MCT.xml",
                        "MCT Tools.exe",               "MCT Tools.xml",

                        "PoroCYon.MCT.tapi"
                    };

                    if (ToInstall.FSharpCompiler)
                    {
                        ToDownload.Add("FSharp.Core.dll");
                        ToDownload.Add("FSharp.Compiler.CodeDom.dll");
                        ToDownload.Add("FSharp.Core.xml");
                        ToDownload.Add("FSharp.Compiler.CodeDom.xml");

                        ToDownload.Add("Compilers\\FSharpCompiler.dll");
                    }
                    if (ToInstall.InstallPdb)
                    {
                        ToDownload.Add("PoroCYon.XnaExtensions.pdb");

                        ToDownload.Add("PoroCYon.MCT.pdb");

                        ToDownload.Add("MCT Tools.pdb");

                        if (ToInstall.FSharpCompiler)
                        {
                            ToDownload.Add("FSharp.Compiler.CodeDom.pdb");

                            ToDownload.Add("Compilers\\FSharpCompiler.pdb");
                        }
                    }
                    if (VsVersions.ChosenVersions != 0)
                    {
                        const string T = "Templates\\tAPI ", Z = ".zip";

                        ToDownload.Add(T + "global ModItem class" + Z);
                        ToDownload.Add(T + "global ModNPC class" + Z);
                        ToDownload.Add(T + "global ModPrefix class" + Z);
                        ToDownload.Add(T + "global ModProjectile class" + Z);
                        ToDownload.Add(T + "global ModTile class" + Z);

                        ToDownload.Add(T + "Item JSON file" + Z);

                        ToDownload.Add(T + "ModBase class" + Z);
                        ToDownload.Add(T + "ModBuff class" + Z);
                        ToDownload.Add(T + "ModInfo.json file" + Z);
                        ToDownload.Add(T + "ModInterface class" + Z);
                        ToDownload.Add(T + "ModItem class" + Z);
                        ToDownload.Add(T + "ModNPC class" + Z);
                        ToDownload.Add(T + "ModPlayer class" + Z);
                        ToDownload.Add(T + "ModPrefix class" + Z);
                        ToDownload.Add(T + "ModProjectile class" + Z);
                        ToDownload.Add(T + "ModTile class" + Z);
                        ToDownload.Add(T + "ModWorld class" + Z);

                        ToDownload.Add(T + "NPC JSON file" + Z);
                        ToDownload.Add(T + "Prefix JSON file" + Z);
                        ToDownload.Add(T + "Projectile JSON file" + Z);
                        ToDownload.Add(T + "Tile JSON file" + Z);
                        ToDownload.Add(T + "Wall JSON file" + Z);

                        ToDownload.Add(T + "Mod" + Z);
                    }

                    applied = total = ToDownload.Count;

                    for (int i = 0; i < ToDownload.Count; i++)
                    {
                        UpdateProgress(-1d, Path.GetFileName(ToDownload[i]));
                        downloaded.Enqueue(new Tuple<string, byte[]>(ToDownload[i], client.DownloadData(baseUri + ToDownload[i])));
                        UpdateProgress(100d / (total / (i + 1d)), null);
                    }

                    UpdateProgress(100d, "finished");

                    client.Dispose();

                    finishedDownloading = true;
                }
                catch (Exception e)
                {
                    Program.DisplayError(e);
                }
            });
            aquire.SetApartmentState(ApartmentState.STA);
            aquire.Start();
            #endregion

            #region envvar
            Thread envvar = new Thread(() =>
            {
                try
                {
                    Environment.SetEnvironmentVariable("TAPIBINDIR", steamDir.Remove(steamDir.Length - 1), EnvironmentVariableTarget.Machine);
                    Environment.SetEnvironmentVariable("TAPIMODDIR", Environment.GetFolderPath(Environment.SpecialFolder.Personal)
                    + "\\My Games\\Terraria\\tAPI\\Mods", EnvironmentVariableTarget.Machine);
                    Environment.SetEnvironmentVariable("TAPIMODSRCDIR", Environment.GetFolderPath(Environment.SpecialFolder.Personal)
                    + "\\My Games\\Terraria\\tAPI\\Mods\\Sources", EnvironmentVariableTarget.Machine);
                    Environment.SetEnvironmentVariable("TAPIMODOUTDIR", Environment.GetFolderPath(Environment.SpecialFolder.Personal)
                    + "\\My Games\\Terraria\\tAPI\\Mods\\Unsorted", EnvironmentVariableTarget.Machine);
                    Environment.SetEnvironmentVariable("MCTDIR", Environment.GetFolderPath(Environment.SpecialFolder.Personal)
                    + "\\My Games\\Terraria\\tAPI\\MCT", EnvironmentVariableTarget.Machine);

                    RegistryKey hklm = Registry.LocalMachine;
                    RegistryKey mctk = hklm.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\mct.exe", RegistryKeyPermissionCheck.ReadWriteSubTree);
                    mctk.SetValue(null, steamDir + "MCT Tools.exe");
                    mctk.SetValue("Path", steamDir);

                    string
                    path = Environment.GetEnvironmentVariable("path"),
                    mcttPath = steamDir + "MCTT_EXEC"; // second t intentional

                    if (!Directory.Exists(mcttPath))
                        Directory.CreateDirectory(mcttPath);

                    using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("PoroCYon.MCT.Installer.mct.bat"))
                    {
                        File.WriteAllText(mcttPath + "\\mct.bat", new StreamReader(s).ReadToEnd());
                    }

                    if (!path.Contains(mcttPath))
                        Environment.SetEnvironmentVariable("path", path + (String.IsNullOrEmpty(path) ? "" : ";") + mcttPath, EnvironmentVariableTarget.Machine);
                }
                catch (Exception e)
                {
                    Program.DisplayError(e);
                }
            });
            envvar.SetApartmentState(ApartmentState.STA);
            envvar.Start();
            #endregion

            #region apply
            Thread apply = new Thread(() =>
            {
                try
                {
                    Action<double, string> UpdateProgress = (procent, text) =>
                    {
                        Dispatcher.Invoke(((Action)delegate
                        {
                            if (procent >= 0d && procent <= 100d)
                                ApplyProgress.Value = procent;
                            if (!String.IsNullOrEmpty(text))
                                ApplyProgressText.Text = "Applying: " + text;
                        }), DispatcherPriority.Render);
                    };

                    int i = 0;

                    while (!finishedDownloading || applied > 0 || downloaded.Count > 0)
                    {
                        if (downloaded.Count <= 0)
                        {
                            Thread.Sleep(1);
                            continue;
                        }

                        Tuple<string, byte[]> t = downloaded.Dequeue();

                        UpdateProgress(-1d, t.Item1);
                        if (t.Item1.StartsWith("Templates\\"))
                            for (VSVersion v = VSVersion.VCSExpress; v <= VSVersion.VisualStudio12; v = (VSVersion)((int)v * 2))
                            {
                                if ((VsVersions.ChosenVersions & v) == 0)
                                    continue;

                                string version = "";
                                switch (v)
                                {
                                    case VSVersion.VCSExpress:
                                    case VSVersion.VisualStudio10:
                                        version = "10";
                                        break;
                                    case VSVersion.WDExpress11:
                                    case VSVersion.VisualStudio11:
                                        version = "12";
                                        break;
                                    case VSVersion.WDExpress12:
                                    case VSVersion.VisualStudio12:
                                        version = "13";
                                        break;
                                }

                                string folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Visual Studio 20" + version +
                                "\\Templates\\" + (t.Item1 == "Templates\\tAPI Mod.zip" ? "ProjectTemplates" : "ItemTemplates") + "\\Visual C#\\tAPI\\";
                                if (!Directory.Exists(folder))
                                    Directory.CreateDirectory(folder);

                                File.WriteAllBytes(folder + Path.GetFileName(t.Item1), t.Item2);
                            }
                        else if (t.Item1.StartsWith("Compilers\\"))
                        {
                            string folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\My Games\\Terraria\\tAPI\\MCT\\Compilers\\";

                            if (!Directory.Exists(folder))
                                Directory.CreateDirectory(folder);

                            File.WriteAllBytes(folder + Path.GetFileName(t.Item1), t.Item2);
                        }
                        else if (t.Item1.EndsWith(".tapi"))
                        {
                            //if (!Directory.Exists(steamDir + "Temp"))
                            //    Directory.CreateDirectory(steamDir + "Temp");
                            //File.WriteAllBytes(steamDir + "Temp\\PoroCYon.MCT.dll", t.Item2);
                            File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.Personal)
                            + "\\My Games\\Terraria\\tAPI\\Mods\\Unsorted\\" + Path.GetFileName(t.Item1), t.Item2);
                        }
                        else
                            File.WriteAllBytes(steamDir + t.Item1, t.Item2);

                        UpdateProgress(100d / (total / (i + 1d)), null);

                        i++;
                        applied--;
                    }

                    UpdateProgress(99, "Creating environment variables");

                    envvar.Join();

                    //UpdateProgress(99, "Installing & deploying PoroCYon.MCT.tapi placeholder.");

                    //Process p = new Process()
                    //{
                    //    StartInfo = new ProcessStartInfo(steamDir + "\\MCT Tools.exe", "build \"" + steamDir + "Temp\\PoroCYon.MCT.dll" + "\"")
                    //    {
                    //        CreateNoWindow = true,
                    //        UseShellExecute = false,
                    //        RedirectStandardError = true,
                    //        RedirectStandardOutput = true
                    //    }
                    //};
                    //p.Start();
                    //p.WaitForExit();

                    File.Delete(steamDir + "Temp\\PoroCYon.MCT.dll");

                    MainWindow.instance.Dispatcher.Invoke(((Action)delegate
                    {
                        MainWindow.currentPage++;
                        MainWindow.instance.UpdateGrid();
                    }), DispatcherPriority.Render);
                }
                catch (Exception e)
                {
                    Program.DisplayError(e);
                }
            });
            apply.SetApartmentState(ApartmentState.STA);
            apply.Start();
            #endregion
        }
    }
}
