using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Ionic.Zip;
using PoroCYon.XnaExtensions;
using TAPI.SDK.Internal;

namespace TAPI.SDK.Tools.Builder
{
    /// <summary>
    /// The tAPI SDK Mod builder
    /// </summary>
    public static class ModBuilder
    {
        /// <summary>
        /// Builds a mod from a managed .dll file
        /// </summary>
        /// <param name="dllFile">The .dll file to build</param>
        public static void Build(string dllFile)
        {
            #region setup
            string
                modName = Path.GetFileNameWithoutExtension(dllFile),
                modPath = CommonToolUtilities.modsBinDir + "\\" + modName,
                modFile = modPath + ".tapi";

            if (File.Exists(modFile))
                File.Delete(modFile);

            Assembly asm = Assembly.LoadFrom(dllFile);

            List<Tuple<string, byte[]>> files = new List<Tuple<string, byte[]>>();
            #endregion

            string modInfo = null;

            #region load all resources into the list
            foreach (string res in asm.GetManifestResourceNames())
                using (Stream stream = asm.GetManifestResourceStream(res))
                {
                    MemoryStream ms = new MemoryStream();
                    stream.CopyTo(ms);

                    bool foundExt = false;
                    int index = 0;

                    for (int i = res.Length - 1; i >= 0; i--)
                    {
                        // find file name
                        if (res[i] == '.')
                            if (foundExt)
                            {
                                index = i + 1;
                                break;
                            }
                            else
                                foundExt = true;
                    }

                    // not putting ModInfo.json in it
                    if (res.Substring(index) != "ModInfo.json")
                        files.Add(new Tuple<string, byte[]>(res.Substring(index), ms.ToArray()));
                    else
                    {
                        StreamReader r = new StreamReader(new MemoryStream(ms.ToArray()));
                        modInfo = r.ReadToEnd();

                        if (modInfo.IsEmpty())
                            modInfo = CommonToolUtilities.CreateDefaultModInfo(modName);
                        else
                            try
                            {
                                JsonData j = JsonMapper.ToObject(modInfo);

                                if (!j.Has("displayName") || !j.Has("author"))
                                    modInfo = CommonToolUtilities.CreateDefaultModInfo(modName);
                            }
                            catch (JsonException) // invalid JSON thrown in JsonMapper.ToObject
                            {
                                modInfo = CommonToolUtilities.CreateDefaultModInfo(modName);
                            }

                        r.Close();
                    }
                    ms.Dispose();
                }
            #endregion

            File.Copy(dllFile, modPath + ".dll");

            WriteData(modInfo, modPath, files);
        }
        static void WriteData(string modInfo, string outputPath, List<Tuple<string, byte[]>> files)
        {
            JsonData json = JsonMapper.ToObject(modInfo);

            BinBuffer bb = new BinBuffer();

            bb.Write(Constants.versionAssembly);

            bb.Write(modInfo);

            bb.Write(files.Count);
            foreach (Tuple<string, byte[]> pfile in files)
            {
                bb.Write(pfile.Item1);
                bb.Write(pfile.Item2.Length);
            }
            foreach (Tuple<string, byte[]> pfile in files)
                bb.Write(pfile.Item2);

            bb.Write(File.ReadAllBytes(outputPath + ".dll"));

            bb.Pos = 0;

            //File.WriteAllBytes(outputPath + ".tapimod", bb.ReadBytes(bb.GetSize()));

            File.Delete(outputPath + ".tapi");
            using (ZipFile zip = new ZipFile())
            {
                string dir = Mods.pathDirModsUnsorted;
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                zip.AddEntry("Mod.tapimod", bb.ReadBytes(bb.GetSize()));
                zip.AddEntry("ModInfo.json", Encoding.UTF8.GetBytes(modInfo));

                foreach (Tuple<string, byte[]> file in files)
                    zip.AddEntry(file.Item1, file.Item2);
                zip.Save(outputPath + ".tapi");
            }
            if (json == null || !json.Has("extractDLL") || !(bool)json["extractDLL"])
                File.Delete(outputPath + ".dll");
        }
    }
}
