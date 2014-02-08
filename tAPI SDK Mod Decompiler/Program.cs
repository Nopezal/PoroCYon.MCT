using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TAPI.SDK.Internal;

namespace TAPI.SDK.ModDecompiler
{
    static class Program
    {
        static void Main(string[] args)
        {
            CommonToolUtilities.Init();

            bool noArgs = false;

            do
            {
                #region if no args, request file
                // always useful
                if (args.Length < 1)
                    while (true)
                    {
                        Console.WriteLine("Select a .tapimod file to decompile: (group + '/' + file name + extension)");

                        string file = Console.ReadLine();

                        if (!File.Exists(ModDecompiler.modsDir + "\\" + file))
                        {
                            Console.WriteLine("The file doesn't exist!");
                            continue;
                        }

                        if (!Path.GetExtension(ModDecompiler.modsDir + "\\" + file).ToLower().EndsWith("tapimod"))
                        {
                            Console.WriteLine("It has to be a .tapimod file!");
                            continue;
                        }

                        args = new string[1] { file };
                        noArgs = true;
                        break;
                    }
                #endregion

                foreach (string arg in args)
                    try
                    {
                        ModDecompiler.Decompile(arg);
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine(e);

                        Console.Write("An error occured:\n" + e);
                    }

            } while (noArgs);
        }
    }
}
