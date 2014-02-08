using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TAPI.SDK.Internal;

namespace TAPI.SDK.ModDecompiler
{
    public static class ModDecompiler
    {
        internal readonly static string
            modsDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\My Games\\Terraria\\tAPI\\Mods",
            decompDir = modsDir + "\\Decompiled Mods";

        public static void Decompile(string modFile)
        {
            CommonToolUtilities.RefreshHashes();

            string
                modName = Path.GetFileNameWithoutExtension(modFile),
                decompPath = ModDecompiler.decompDir + "\\" + modName;

            // might be a good idea
            if (!Directory.Exists(decompPath))
                Directory.CreateDirectory(decompPath);

            // where to store file data
            List<Tuple<string, byte[]>> files = new List<Tuple<string, byte[]>>();
            List<Tuple<string, int>> reading = new List<Tuple<string, int>>();

            // load data into buffer
            BinBuffer bb = new BinBuffer(new BinBufferByte(File.ReadAllBytes(modFile)));

            // first 4 bytes is the version
            int versionAssembly = bb.ReadInt();

            // write tAPI version
            if (!File.Exists(decompPath + "\\tAPI Version " + versionAssembly))
                File.Create(decompPath + "\\tAPI Version " + versionAssembly);

            // write modinfo
            File.WriteAllText(decompPath + "\\ModInfo.json", bb.ReadString());

            // get file amount
            int filesNum = bb.ReadInt();

            // read file name + length
            while (filesNum-- > 0)
                reading.Add(new Tuple<string, int>(bb.ReadString(), bb.ReadInt()));

            // read file data
            foreach (Tuple<string, int> read in reading)
                files.Add(new Tuple<string, byte[]>(read.Item1, bb.ReadBytes(read.Item2)));

            // write files
            foreach (Tuple<string, byte[]> pfile in files)
            {
                if (!Directory.Exists(Path.GetDirectoryName(decompPath + "\\" + pfile.Item1)))
                    Directory.CreateDirectory(Path.GetDirectoryName(decompPath + "\\" + pfile.Item1));
                File.WriteAllBytes(decompPath + "\\" + pfile.Item1, pfile.Item2);
            }

            // write assembly
            File.WriteAllBytes(decompPath + "\\" + modName + ".dll", bb.ReadBytes(bb.BytesLeft()));

            CommonToolUtilities.RefreshHashes();
        }
    }
}
