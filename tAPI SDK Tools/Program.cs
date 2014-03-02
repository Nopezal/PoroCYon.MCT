using System;
using System.Collections.Generic;
using System.Linq;
using TAPI.SDK.Internal;
using TAPI.SDK.Tools.Builder;
using TAPI.SDK.Tools.Decompiler;
using TAPI.SDK.Tools.Packer;

namespace TAPI.SDK.Tools
{
    static class Program
    {
        static Dictionary<string, Action<string>> Commands;
        static Program()
        {
            Commands = new Dictionary<string, Action<string>>()
            {
                {
                    "pack",
                    (path) =>
                    {
                        if (!path.Contains('/') && !path.Contains('\\'))
                            path = Mods.pathDirModsSources + "\\" + path;

                        CompilerException ce = ModPacker.Pack(path, Mods.pathDirModsUnsorted);
                        if (ce != null)
                            Console.Error.WriteLine(ce);
                    }
                },
                {
                    "build",
                    (path) => ModBuilder.Build(path)
                },
                {
                    "decompile",
                    (path) => 
                    {
                        if (!path.Contains('/') && !path.Contains('\\'))
                            path = Mods.pathDirModsUnsorted + "\\" + path;

                        ModDecompiler.Decompile(path);
                    }
                }
            };
            Commands.Add("compile", Commands["build"]);
        }

        static void Main(string[] args)
        {
            for (int i = 0; i < args.Length - 1; i++)
            {
                string s = args[i];

                if (s.StartsWith("--"))
                    s = s.Substring(2);
                if (s.StartsWith("-"))
                    s = s.Substring(1);
                if (s.StartsWith("/"))
                    s = s.Substring(1);

                for (int j = 0; j < Commands.Count; j++)
                {
                    string c = Commands.Keys.ElementAt(j);

                    if (s == c || (s[0] == c[0] && s.Length == 0))
                        Commands[c](args[++i]);
                }
            }
        }
    }
}
