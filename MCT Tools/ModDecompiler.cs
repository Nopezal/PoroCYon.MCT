using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zip;
using TAPI;
using PoroCYon.MCT.Internal;

namespace PoroCYon.MCT.Tools
{
    /// <summary>
    /// The MCT Mod decompiler
    /// </summary>
    public static class ModDecompiler
    {
        internal readonly static string decompDir = CommonToolUtilities.modsDir + "\\Decompiled";

        /// <summary>
        /// Decompiles a .tapimod file
        /// </summary>
        /// <param name="modFile">The .tapimod file to decompile</param>
        public static void Decompile(string modFile)
        {
            if (!File.Exists(Mods.pathDirMods + "\\" + modFile))
                throw new FileNotFoundException(Mods.pathDirMods + "\\" + modFile);

            string
                modName = Path.GetFileNameWithoutExtension(modFile),
                decompPath = ModDecompiler.decompDir + "\\" + modName;

            byte[]
                tapimod = new byte[0],
                modInfo = new byte[0],
                modInfoZip = new byte[0];

            List<Tuple<string, byte[]>>
                zipFiles = new List<Tuple<string, byte[]>>(),
                tapimodFiles = new List<Tuple<string,byte[]>>();

            uint versionAssembly = 0u;

            if (!Directory.Exists(decompPath))
                Directory.CreateDirectory(decompPath);

            #region load data from zip
            if (modFile.EndsWith(".tapi"))
                using (ZipFile zf = ZipFile.Read(Mods.pathDirMods + "\\" + modFile))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        ZipEntry ze = zf["Mod.tapimod"];
                        if (ze != null)
                        {
                            ze.Extract(ms);
                            tapimod = ms.ToArray();
                        }
                    }
                    using (MemoryStream ms = new MemoryStream())
                    {
                        ZipEntry ze = zf["ModInfo.json"];
                        if (ze != null)
                        {
                            ze.Extract(ms);
                            modInfoZip = ms.ToArray();
                        }
                    }
                    foreach (ZipEntry ze in zf.Entries)
                        if (ze.FileName != "Mod.tapimod" && ze.FileName != "ModInfo.json")
                            using (MemoryStream ms = new MemoryStream())
                            {
                                ze.Extract(ms);
                                zipFiles.Add(new Tuple<string, byte[]>(ze.FileName, ms.ToArray()));
                            }
                }
            #endregion
            else if (modFile.EndsWith(".tapimod"))
                tapimod = File.ReadAllBytes(modFile);
            else
                throw new FileLoadException("File is not a .tapi or .tapimod file!");

            // create binary buffer of .tapimod file
            BinBuffer bb = new BinBuffer(new BinBufferByte(tapimod));

            // read/write tAPI version
            File.WriteAllText(decompPath + "\\tAPI_Version.txt", (versionAssembly = bb.ReadUInt()).ToString());

            // read ModInfo.json
            File.WriteAllText(decompPath + "\\ModInfo.json", bb.ReadString());

            // write files from zip data (.pdb is automatically included)
            foreach (Tuple<string, byte[]> t in zipFiles)
            {
                string dir = decompPath + "\\_FromZip\\" + t.Item1;

                if (!Directory.Exists(Path.GetDirectoryName(dir)))
                    Directory.CreateDirectory(Path.GetDirectoryName(dir));

                File.WriteAllBytes(dir, t.Item2);
            }

            // read (normal) files
            List<Tuple<string, int>> fileInfo = new List<Tuple<string, int>>();

            int count = bb.ReadInt();
            for (int i = 0; i < count; i++)
                fileInfo.Add(new Tuple<string, int>(bb.ReadString(), bb.ReadInt()));
            for (int i = 0; i < count; i++)
                tapimodFiles.Add(new Tuple<string, byte[]>(fileInfo[i].Item1, bb.ReadBytes(fileInfo[i].Item2)));

            // write them
            foreach (Tuple<string, byte[]> t in tapimodFiles)
            {
                string dir = decompPath + "\\" + t.Item1;

                if (!Directory.Exists(Path.GetDirectoryName(dir)))
                    Directory.CreateDirectory(Path.GetDirectoryName(dir));

                File.WriteAllBytes(dir, t.Item2);
            }

            // write .dll
            File.WriteAllBytes(decompPath + "\\" + modName + ".dll", bb.ReadBytes(bb.BytesLeft()));
        }
    }
}
