using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Ionic.Zip;
using TAPI;
using PoroCYon.MCT.Internal;
using LitJson;

namespace PoroCYon.MCT.Tools
{
    /// <summary>
    /// The MCT Mod decompiler
    /// </summary>
    public static class ModDecompiler
    {
        internal readonly static string decompDir = Consts.MctDirectory + "Decompiled";

        /// <summary>
        /// Decompiles a .tapimod file
        /// </summary>
        /// <param name="modFile">The .tapimod file to decompile</param>
        public static void Decompile(string modFile)
        {
            if (!File.Exists(Mods.pathCompiled + "\\" + modFile))
                throw new FileNotFoundException(Mods.pathCompiled + "\\" + modFile);

            string
                modName = Path.GetFileNameWithoutExtension(modFile),
                decompPath = decompDir + "\\" + modName;

            byte[] tapimod = new byte[0];

            List<Tuple<string, byte[]>>
                zipFiles = new List<Tuple<string, byte[]>>(),
                tapimodFiles = new List<Tuple<string,byte[]>>();

            ushort modVersion = 0;

            if (Directory.Exists(decompPath))
                Directory.Delete(decompPath, true);
            Directory.CreateDirectory(decompPath);

            #region load data from zip
            if (modFile.EndsWith(".tapi"))
                using (ZipFile zf = ZipFile.Read(Mods.pathCompiled + "\\" + modFile))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        ZipEntry ze = zf["!Mod.tapimod"];
                        if (ze != null)
                        {
                            ze.Extract(ms);
                            tapimod = ms.ToArray();
                        }
                    }
                    //using (MemoryStream ms = new MemoryStream())
                    //{
                    //    ZipEntry ze = zf["ModInfo.json"];
                    //    if (ze != null)
                    //    {
                    //        ze.Extract(ms);
                    //        modInfoZip = ms.ToArray();
                    //    }
                    //}
                    foreach (ZipEntry ze in zf.Entries)
                        if (ze.FileName != "!Mod.tapimod"/* && ze.FileName != "ModInfo.json"*/)
                            using (MemoryStream ms = new MemoryStream())
                            {
                                ze.Extract(ms);
                                zipFiles.Add(new Tuple<string, byte[]>(ze.FileName, ms.ToArray()));
                            }
                }
            #endregion
            else if (modFile.EndsWith(".tapimod"))
                tapimod = File.ReadAllBytes(Mods.pathCompiled + "\\" + modFile);
            else
                throw new FileLoadException("File is not a .tapi or .tapimod file!");

			// write files from zip data (.pdb is automatically included)
			foreach (Tuple<string, byte[]> t in zipFiles)
			{
				string name = t.Item1;

				if (t.Item1 == "!DebugInformation.pdb")
					name = modName + ".pdb";

				string dir = decompPath + "\\_FromZip\\" + name;

				if (!Directory.Exists(Path.GetDirectoryName(dir)))
					Directory.CreateDirectory(Path.GetDirectoryName(dir));

				File.WriteAllBytes(dir, t.Item2);
			}

			// create binary buffer of .tapimod file
			BinBuffer bb = new BinBuffer(new BinBufferByte(tapimod));

            // read/write tAPI version
            File.WriteAllText(decompPath + "\\Mod_Version.txt", (modVersion = bb.ReadUShort()).ToString());

            // read ModInfo.json
            string modInfoStr;
            File.WriteAllText(decompPath + "\\ModInfo.json", modInfoStr = bb.ReadString());

			JsonData modInfo = JsonMapper.ToObject(modInfoStr);

			// get the icon
			if (modInfo.Has("icon"))
			{
				string icon = (string)modInfo["icon"];

				File.WriteAllBytes(decompPath + "\\" + icon + ".png", bb.ReadBytes(bb.ReadInt()));
			}
			else
				Debug.Assert(bb.ReadInt() == 0);

			// write .dll
			File.WriteAllBytes(decompPath + "\\" + modName + ".dll", bb.ReadBytes(bb.ReadInt()));

			// read (normal) files
			// one for images, one for other files
			ushort count = bb.ReadUShort();
			for (int i = 0; i < count; i++)
			{
				string path = decompPath + "\\" + bb.ReadString() + ".png";

				if (!Directory.Exists(Path.GetDirectoryName(path)))
					Directory.CreateDirectory(Path.GetDirectoryName(path));

				File.WriteAllBytes(path, bb.ReadBytes(bb.ReadInt()));
			}
			count = bb.ReadUShort();
			for (int i = 0; i < count; i++)
			{
				string path = decompPath + "\\" + bb.ReadString();

				if (!Directory.Exists(Path.GetDirectoryName(path)))
					Directory.CreateDirectory(Path.GetDirectoryName(path));

				File.WriteAllBytes(path, bb.ReadBytes(bb.ReadInt()));
			}
        }
    }
}
