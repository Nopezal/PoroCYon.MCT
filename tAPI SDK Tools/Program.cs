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
        static Dictionary<string, Action<string>> Commands = new Dictionary<string, Action<string>>()
        {
            #region pack
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
            #endregion
            #region build
            {
                "build",
                (path) => ModBuilder.Build(path)
            },
            {
                "compile",
                Commands["build"]
            },
            #endregion
            #region decompile
            {
                "decompile",
                (path) => 
                {
                    if (!path.Contains('/') && !path.Contains('\\'))
                        path = Mods.pathDirModsUnsorted + "\\" + path;

                    ModDecompiler.Decompile(path);
                }
            }
            #endregion
        };

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
