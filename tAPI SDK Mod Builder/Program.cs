using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TAPI.SDK.Internal;

namespace TAPI.SDK.ModBuilder
{
    static class Program
    {
        static void Main(string[] args)
        {
            // more info about the mod file in tAPI Extended Packer\Program.cs

            CommonToolUtilities.Init();

            bool noArgs = false;

            do
            {
                #region if no args, request file
                if (args.Length < 1)
                    while (true)
                    {
                        Console.WriteLine("Select a .dll file to build: (incl. extension)");

                        string file = Console.ReadLine();

                        if (!File.Exists(file))
                        {
                            Console.WriteLine("The file doesn't exist!");
                            continue;
                        }

                        if (Path.GetExtension(file) != ".dll")
                        {
                            Console.WriteLine("It has to be a .dll file!");
                            continue;
                        }

                        args = new string[1] { file };

                        noArgs = true;

                        break;
                    }
                #endregion

                // no exceptions would occur if everything is ok
                foreach (string arg in args)
                    ModCompiler.Compile(arg);

            } while (noArgs);
        }
    }
}
