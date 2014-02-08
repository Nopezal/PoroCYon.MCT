using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TAPI.SDK.Internal;

namespace TAPI.Packer.Extended
{
    static class Program
    {
        static void Main(string[] args)
        {
            CommonToolUtilities.Init();

            bool noArgs = false;

            try
            {
                do
                {
                    Console.WriteLine("tAPI r" + Constants.versionRelease + " "
                        + (Constants.versionSubrelease == null ? "" : Constants.versionSubrelease) + " extended mod packer");

                    string input = "";

                    if (args.Length < 1)
                    {
                        string[] dirs = Directory.GetDirectories(CommonToolUtilities.modsSrcDir);

                        // kinda necessary...
                        while (dirs.Length == 0)
                        {
                            Console.WriteLine("You uhm... have no mod sources! Press any key to refresh.");

                            Console.ReadKey(true);

                            continue;
                        }

                        #region choose mod
                        Console.WriteLine("Type help or choose a mod to build:");

                        for (int i = 0; i < dirs.Length; i++)
                        {
                            string[] split = dirs[i].Split('\\');
                            Console.WriteLine((i + 1) + ": " + split[split.Length - 1]);
                        }

                        List<string> modsToBuild = new List<string>();
                        while (modsToBuild.Count < 1)
                        {
                            try
                            {
                                input = Console.ReadLine();

                                if (input.ToLower().Trim() == "help")
                                {
                                    Console.WriteLine("Enter the number of the mod you want to build, and press enter.\nEasy, isn't it?");
                                    break;
                                }

                                string[] split = input.Split(',');
                                int temp;

                                for (int i = 0; i < split.Length; i++)
                                    if (Int32.TryParse(split[i], out temp))
                                        modsToBuild.Add(dirs[temp - 1]);
                            }
                            catch (Exception e)
                            {
                                Console.Error.WriteLine(e);
                                Console.WriteLine(e);

                                modsToBuild = new List<string>();
                            }
                        }
                        #endregion

                        args = modsToBuild.ToArray();
                    }

                    #region build
                    List<int> failed = new List<int>();

                    int num = 0;
                    foreach (string arg in args)
                    {
                        Console.WriteLine("Trying to build " + arg + "...");

                        CompilerException e;
                        if ((e = ModCompiler.Compile(arg, CommonToolUtilities.modsBinDir)) != null)
                        {
                            failed.Add(num);
                            Console.Error.WriteLine(e);
                            Console.WriteLine(e);
                        }

                        num++;
                    }

                    if (num == args.Length)
                    {
                        if (failed.Count == 0)
                            Console.Out.WriteLine("All mods built successfully!");
                        else
                        {
                            string issue = "There were some problems...";

                            if (failed.Count > 0)
                            {
                                string[] split = args[failed[0]].Split('\\');
                                string modName = split[split.Length - 1];

                                issue += " failed to compile: " + (failed[0] + 1) + " (" + modName + ")";

                                for (int j = 1; j < failed.Count; j++)
                                {
                                    split = args[failed[j]].Split('\\');
                                    modName = split[split.Length - 1];
                                    issue += "," + (failed[j] + 1) + " (" + modName + ")";
                                }
                            }
                            Console.Error.WriteLine(issue);
                            Console.WriteLine(issue);
                        }
                    }
                    #endregion

                    Console.Clear();

                } while (noArgs);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error:\n " + e);
                Console.WriteLine("Error:\n " + e);

                if (noArgs)
                {
                    Console.WriteLine("Press any key to continue.");

                    Console.ReadKey(true);
                }
            }
        }
    }
}
