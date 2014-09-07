using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Build.Framework;
using PoroCYon.Extensions.Collections;
using TAPI;
using PoroCYon.MCT.Internal;
using PoroCYon.MCT.Tools.Compiler;
using PoroCYon.MCT.Tools.Compiler.Loggers;

namespace PoroCYon.MCT.Tools
{
    static class Program
    {
        [Serializable]
        class CloseConsoleException : Exception { }

        static bool suppressBanner = false;

        static Dictionary<string, Action> Commands;
        static Dictionary<string, Action<string[]>> ToolCommands;

        static Program()
        {
            Commands = new Dictionary<string, Action>()
            {
                #region help
                {
                    "help", () =>
                    {
                        Console.WriteLine("BUILD\t\tBuilds a tAPI mod from a source folder or a managed .dll file.");
                        Console.WriteLine("DECOMPILE\t\tDecompiles a .tapi or .tapimod file");
                        Console.WriteLine("HELP\t\tDisplays this text. H or ? can also be used for this.");
                        Console.WriteLine();
                        Console.WriteLine("\t\tYou can use HELP, H or ? for any of these commands for arguments info, except for HELP.");
                    }
                },
                #endregion

                { "banner"  , () => suppressBanner = !suppressBanner }
            };

            Commands.Add("?"       , Commands["help"  ]);
            Commands.Add("h"       , Commands["help"  ]);
            Commands.Add("nobanner", Commands["banner"]);

            ToolCommands = new Dictionary<string, Action<string[]>>()
            {
                #region build
                {
                    "build", (args) =>
                    {
                        WriteBanner("mod compiler");

                        int argNum = 0;

                        if (WriteHelp(args[argNum], "BUILD <verbosity?=Quiet> <mod folder/dll file>" +
                                "\tPossible values for 'verbosity' flag: Quiet, Minimal, Normal, Detailed, Diagnostic. All values are case-insensitive."))
                            return;

                        LoggerVerbosity verbosity = LoggerVerbosity.Quiet;

                        if ((string vArg = args[argNum].TrimStart('-', '/').ToLowerInvariant()) == "v" || vArg == "verbose" || vArg == "verbosity")
                        {
                            argNum++;

                            if (Int32.TryParse(args[argNum], NumberStyles.Integer, CultureInfo.InvariantCulture, out int i))
                            {
                                verbosity = (LoggerVerbosity)i;
                                argNum++;
                            }
                            else if (Enum.TryParse(args[argNum], true, out LoggerVerbosity v))
                            {
                                verbosity = v;
                                argNum++;
                            }
                        }

                        ModCompiler compiler = new ModCompiler(new ConsoleMctLogger(verbosity));

                        if (Debugger.IsAttached)
                            compiler.Loggers.Add(new DebugMctLogger(verbosity));

                        string path = args.Skip(argNum).Join(CommonJoinValues.Space);

                        CompilerOutput ret = File.Exists  (path)
                            ? compiler.CompileFromAssembly(path)
                            : compiler.CompileFromSource  (path);

                        // done by ConsoleMctLogger
                        //Console.WriteLine(ret);
                    }
                },
                #endregion

                #region decompile
                {
                    "decompile", (args) =>
                    {
                        string path = args[0];

                        WriteBanner("mod decompiler");

                        if (WriteHelp(path,
    "DECOMPILE <.tapi or .tapimod file> \t\t '.tapi or .tapimod file' should be in Documents\\My Games\\Terraria\\tAPI\\Mods, eg. 'DECOMPILE Unsorted\\YourMod.tapi'"))
                            return;

                        if (!path.Contains('/') && !path.Contains('\\'))
                            path = Mods.pathDirMods + "\\" + path;

                        //if (File.Exists(path + ".tapi"))
                        //    path += ".tapi";
                        //if (File.Exists(path + ".tapimod"))
                        //    path += ".tapimod";

                        ModDecompiler.Decompile(path);
                        Console.WriteLine("Finished decompilation.");
                    }
                },
                #endregion

                #region port
                {
                    "port", (args) =>
                    {
                        string file = args[0];

                        WriteBanner("file porter");

                        if (WriteHelp(file, "PORT <toPort>. \t\t toPort should be the full file name (name + extension) of the file to port. Folder not required."))
                            return;

                        string
                            terrariaDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\My Games\\Terraria\\",
                            ext = Path.GetExtension(file),
                            path;

                        switch (ext)
                        {
                            case ".plr":
                                path = terrariaDir + "Players\\" + file;

                                if (!File.Exists(path))
                                {
                                    Console.Error.WriteLine("The file \"" + path + "\" does not exist.");

                                    return;
                                }

                                try
                                {
                                    FilePorter.PortPlayer(path);
                                }
                                catch (Exception e)
                                {
                                    Console.Error.WriteLine("Could not port the Player file:" + Environment.NewLine + e);
                                }
                                break;
                            case ".wld":
                                path = terrariaDir + "Worlds\\" + file;

                                if (!File.Exists(path))
                                {
                                    Console.Error.WriteLine("The file \"" + path + "\" does not exist.");

                                    return;
                                }

                                try
                                {
                                    FilePorter.PortWorld(path);
                                }
                                catch (Exception e)
                                {
                                    Console.Error.WriteLine("Could not port the world file:" + Environment.NewLine + e);
                                }
                                break;
                            default:
                                Console.Error.WriteLine("Invalid file extension (" + ext ?? "no extension" + ").");
                                break;
                        }
                    }
                },
                #endregion

                { "skip", /* fun _ -> () */ delegate { } }
            };

            ToolCommands.Add("compile", ToolCommands["build"]);
            ToolCommands.Add("ignore" , ToolCommands["skip" ]);
            ToolCommands.Add("nothing", ToolCommands["skip" ]);
        }

        static bool WriteHelp(string arg, string help)
        {
            string couldBeHelp = arg.TrimStart('-', '/').ToLowerInvariant();

            if (couldBeHelp == "help" || couldBeHelp == "h" || couldBeHelp == "?")
            {
                Console.WriteLine(help);

                return true;
            }

            return false;
        }
        static void WriteBanner(string tool)
        {
            if (!suppressBanner)
            {
                Console.WriteLine();
                Console.WriteLine("MCT Tools " + tool);
                Console.WriteLine("Mod Creation Tools v" + MctConstants.VERSION_STRING);
                Console.WriteLine("MCT Copyright © PoroCYon 2014");
                Console.WriteLine();
            }
        }

        [STAThread]
        static void Main(string[] args)
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            CommonToolUtilities.Init();

            for (int i = 0; i < args.Length; i++)
            {
                string cmd = args[i].TrimStart('-', '/');

                for (int j = 0; j < Commands.Count; j++)
                {
                    string name = Commands.Keys.ElementAt(j);

                    if (cmd.ToLowerInvariant() == name.ToLowerInvariant() || (Char.ToLowerInvariant(name[0]) == Char.ToLowerInvariant(name[0]) && cmd.Length == 0))
                    {
                        try
                        {
                            Commands[name]();
                        }
                        catch (CloseConsoleException)
                        {
                            return;
                        }
                        catch (Exception e)
                        {
                            Console.Error.WriteLine("Error: " + e);
                        }
                        goto NEXT; // those velociraptors aren't real
                    }
                }

                for (int j = 0; j < ToolCommands.Count; j++)
                {
                    string name = ToolCommands.Keys.ElementAt(j);

                    if (cmd.ToLowerInvariant() == name.ToLowerInvariant() || (Char.ToLowerInvariant(name[0]) == Char.ToLowerInvariant(name[0]) && cmd.Length == 0))
                    {
                        try
                        {
                            ToolCommands[name](args.Skip(++i).ToArray());
                        }
                        catch (Exception e)
                        {
                            Console.Error.WriteLine("Error: " + e);
                        }
                        break;
                    }
                }

            NEXT:
                ;
            }
        }
    }
}
