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
        static Dictionary<string, Action> Commands;
        static Dictionary<string, Action<string>> ToolCommands;

        static Program()
        {
            Commands = new Dictionary<string, Action>()
            {
                {
                    "help",
                    () =>
                    {
                        Console.WriteLine("BUILD\t\tBuilds a tAPI mod from a source folder or a managed .dll file.");
                        Console.WriteLine("DECOMPILE\t\tDecompiles a .tapi or .tapimod file");
                        Console.WriteLine("HELP\t\tDisplays this text. H or ? can also be used for this.");
                        Console.WriteLine();
                        Console.WriteLine("\t\tYou can use HELP, H or ? for any of these commands for arguments info, except for HELP.");
                    }
                },
            };
            Commands.Add("?",     Commands["help" ]);

            ToolCommands = new Dictionary<string, Action<string>>()
            {
                {
                    "build",
                    (path) =>
                    {
                        string couldBeHelp = TrimCommand(path).ToLowerInvariant();
                        if (couldBeHelp == "help" || couldBeHelp == "h" || couldBeHelp == "?")
                        {
                            Console.WriteLine("BUILD <mod folder/dll file>");
                            return;
                        }

                        Debug.WriteLine(File.Exists(path) ? ModCompiler.CompileFromAssembly(path) :  ModCompiler.CompileFromSource(path));
                    }
                },
                {
                    "decompile",
                    (path) => 
                    {
                        string couldBeHelp = TrimCommand(path).ToLowerInvariant();
                        if (couldBeHelp == "help" || couldBeHelp == "h" || couldBeHelp == "?")
                        {
                            Console.WriteLine("DECOMPILE <.tapi or .tapimod file> \t\t '.tapi or .tapimod file' should be in Documents\\My Games\\Terraria\\tAPI\\Mods, eg. 'DECOMPILE Unsorted\\YourMod.tapi'");
                            return;
                        }

                        if (!path.Contains('/') && !path.Contains('\\'))
                            path = Mods.pathDirMods + "\\" + path;

                        ModDecompiler.Decompile(path);
                    }
                }
            };
            ToolCommands.Add("compile", ToolCommands["build"]);
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
