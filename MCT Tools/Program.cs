using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TAPI;
using PoroCYon.MCT.Internal;
using System.Diagnostics;

namespace PoroCYon.MCT.Tools
{
    static class Program
    {
        static bool suppressBanner = false;

        static Dictionary<string, Action> Commands;
        static Dictionary<string, Action<string>> ToolCommands;

        static Program()
        {
            Commands = new Dictionary<string, Action>()
            {
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
                { "nobanner", () => suppressBanner = !suppressBanner }
            };
            Commands.Add("?",     Commands["help" ]);

            ToolCommands = new Dictionary<string, Action<string>>()
            {
                {
                    "build", (path) =>
                    {
                        if (!suppressBanner)
                        {
                            Console.WriteLine();
                            Console.WriteLine("MCT Tools mod compiler");
                            Console.WriteLine("Mod Creation Tools v" + MctConstants.VERSION_STRING);
                            Console.WriteLine("MCT Copyright © PoroCYon 2014");
                            Console.WriteLine();
                        }

                        string couldBeHelp = TrimCommand(path).ToLowerInvariant();
                        if (couldBeHelp == "help" || couldBeHelp == "h" || couldBeHelp == "?")
                        {
                            Console.WriteLine("BUILD <mod folder/dll file>");
                            return;
                        }
                        CompilerOutput ret = File.Exists(path) ? ModCompiler.CompileFromAssembly(path) :  ModCompiler.CompileFromSource(path);

                        if (Debugger.IsAttached)
                            Debug.WriteLine(ret);

                        Console.WriteLine(ret);
                        Console.WriteLine();
                    }
                },
                {
                    "decompile", (path) =>
                    {
                        if (!suppressBanner)
                        {
                            Console.WriteLine();
                            Console.WriteLine("MCT Tools mod decompiler");
                            Console.WriteLine("Mod Creation Tools v" + MctConstants.VERSION_STRING);
                            Console.WriteLine("MCT Copyright © PoroCYon 2014");
                            Console.WriteLine();
                        }

                        string couldBeHelp = TrimCommand(path).ToLowerInvariant();
                        if (couldBeHelp == "help" || couldBeHelp == "h" || couldBeHelp == "?")
                        {
                            Console.WriteLine("DECOMPILE <.tapi or .tapimod file> \t\t '.tapi or .tapimod file' should be in Documents\\My Games\\Terraria\\tAPI\\Mods, eg. 'DECOMPILE Unsorted\\YourMod.tapi'");
                            return;
                        }

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
                { "skip", (nothing) => { } }
            };
            ToolCommands.Add("compile", ToolCommands["build"]);
            ToolCommands.Add("ignore", ToolCommands["skip"]);
            ToolCommands.Add("nothing", ToolCommands["skip"]);
        }

        [Serializable]
        class CloseConsoleException : Exception { }

        [STAThread]
        static void Main(string[] args)
        {
            CommonToolUtilities.Init();

            for (int i = 0; i < args.Length; i++)
            {
                string s = TrimCommand(args[i]);

                for (int j = 0; j < Commands.Count; j++)
                {
                    string c = Commands.Keys.ElementAt(j);

                    if (s.ToLowerInvariant() == c.ToLowerInvariant() || (Char.ToLowerInvariant(c[0]) == Char.ToLowerInvariant(c[0]) && s.Length == 0))
                    {
                        try
                        {
                            Commands[c]();
                        }
                        catch (CloseConsoleException)
                        {
                            return;
                        }
                        catch (Exception e)
                        {
                            Console.Error.WriteLine("Error: " + e); // logged to std::cerr
                        }
                        goto NEXT; // those velociraptors aren't real
                    }
                }

                for (int j = 0; j < ToolCommands.Count; j++)
                {
                    string c = ToolCommands.Keys.ElementAt(j);

                    if (s.ToLowerInvariant() == c.ToLowerInvariant() || (Char.ToLowerInvariant(c[0]) == Char.ToLowerInvariant(c[0]) && s.Length == 0))
                    {
                        try
                        {
                            ToolCommands[c](args[++i]);
                        }
                        catch (Exception e)
                        {
                            Console.Error.WriteLine("Error: " + e); // logged to std::cerr
                        }
                        break;
                    }
                }

            NEXT:
                ;
            }
        }
        static string TrimCommand(string command)
        {
            string s = (string)command.Clone();

            if (s.StartsWith("--"))
                s = s.Substring(2);
            if (s.StartsWith("-"))
                s = s.Substring(1);
            if (s.StartsWith("/"))
                s = s.Substring(1);

            return s;
        }
    }
}
