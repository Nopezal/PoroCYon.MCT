using System;
using System.Collections.Generic;
using System.Linq;
using TAPI;
using PoroCYon.MCT.Internal;
using PoroCYon.MCT.Tools.Builder;
using PoroCYon.MCT.Tools.Decompiler;
using PoroCYon.MCT.Tools.Packer;

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
                        Console.WriteLine("BUILD\t\tBuilds a tAPI mod from a managed .dll file.");
                        Console.WriteLine("DECOMPILE\t\tDecompiles a .tapi or .tapimod file");
                        Console.WriteLine("PACK\t\tPacks a tAPI mod from a different language.\n\t\t(JS/VB, specify it in the 'language' key in ModInfo.json)");
                        Console.WriteLine();
                        Console.WriteLine("CLEAR\t\tClears the console window");
                        Console.WriteLine();
                        Console.WriteLine("EXIT\t\tExits the console.");
                        Console.WriteLine();
                        Console.WriteLine("\t\tYou can use HELP, H or ? for any of these commands for arguments info, except for HELP, CLEAR and EXIT");
                    }
                },
                {
                    "clear", 
                    () => Console.Clear() 
                },
                {
                    "exit", 
                    () =>
                    {
                        throw new CloseConsoleException();
                    }
                }
            };
            Commands.Add("cls",   Commands["clear"]);
            Commands.Add("stop",  Commands["exit" ]);
            Commands.Add("close", Commands["exit" ]);
            Commands.Add("?",     Commands["help" ]);

            ToolCommands = new Dictionary<string, Action<string>>()
            {
                {
                    "pack",
                    (path) =>
                    {
                        string couldBeHelp = TrimCommand(path).ToLowerInvariant();
                        if (couldBeHelp == "help" || couldBeHelp == "h" || couldBeHelp == "?")
                        {
                            Console.WriteLine("PACK <mod folder>\t\t 'mod folder' should be in Documents\\My Games\\Terraria\\tAPI\\Mods\\Sources.");
                            return;
                        }

                        if (!path.Contains('/') && !path.Contains('\\'))
                            path = Mods.pathDirModsSources + "\\" + path;

                        ModPacker.Pack(path, Mods.pathDirModsUnsorted);
                    }
                },
                {
                    "build",
                    (path) =>
                    {
                        string couldBeHelp = TrimCommand(path).ToLowerInvariant();
                        if (couldBeHelp == "help" || couldBeHelp == "h" || couldBeHelp == "?")
                        {
                            Console.WriteLine("BUILD <.dll file>");
                            return;
                        }

                        ModBuilder.Build(path);
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

        static void Main(string[] args)
        {
            CommonToolUtilities.Init();

            for (int i = 0; i < args.Length; i++)
            {
                string s = TrimCommand(args[i]);

                for (int j = 0; j <     Commands.Count; j++)
                {
                    string c = Commands.Keys.ElementAt(j);

                    if (s.ToLowerInvariant() == c.ToLowerInvariant()
                        || (Char.ToLowerInvariant(c[0]) == Char.ToLowerInvariant(c[0]) && s.Length == 0))
                    {
                        try
                        {
                            Commands[c]();
                        }
                        catch (CloseConsoleException)
                        {
                            goto EXIT;
                        }
                        catch (Exception e)
                        {
                            Console.Error.WriteLine("error: " + e); // logged to std::cerr
                        }
                        goto NEXT;
                    }
                }

                for (int j = 0; j < ToolCommands.Count; j++)
                {
                    string c = ToolCommands.Keys.ElementAt(j);

                    if (s.ToLowerInvariant() == c.ToLowerInvariant()
                        || (Char.ToLowerInvariant(c[0]) == Char.ToLowerInvariant(c[0]) && s.Length == 0))
                    {
                        try
                        {
                            ToolCommands[c](args[++i]);
                        }
                        catch (Exception e)
                        {
                            Console.Error.WriteLine("error: " + e); // logged to std::cerr
                        }
                        break;
                    }
                }

            NEXT:
                ;
            }
        EXIT:
            ;
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
