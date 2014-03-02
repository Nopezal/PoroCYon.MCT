using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zip;

namespace TAPI.SDK.Internal
{
    internal static class CommonToolUtilities
    {
        internal static JsonData modHashes = null;

        internal static string
            modsSrcDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\My Games\\Terraria\\tAPI\\Mods\\Sources",
            modsBinDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\My Games\\Terraria\\tAPI\\Mods\\Unsorted";

        internal static void Init()
        {
            Constants.SetupVariablesOnce();

            modHashes = File.Exists(modsSrcDir + "\\.ModHashes.json") ?
                JsonMapper.ToObject(File.ReadAllText(modsSrcDir + "\\.ModHashes.json")) :
                JsonMapper.ToObject("{}");
        }
        internal static void RefreshHashes()
        {
            modHashes = File.Exists(modsSrcDir + "\\.ModHashes.json") ?
                JsonMapper.ToObject(File.ReadAllText(modsSrcDir + "\\.ModHashes.json")) :
                JsonMapper.ToObject("{}");
        }

        internal static JsonData GenerateHashes(string modDirectory)
        {
            JsonData j = JsonMapper.ToObject("{}");

            foreach (string fileName in Directory.EnumerateFiles(modDirectory, "*.*", SearchOption.AllDirectories))
                j[fileName.Substring(modDirectory.Length + 1).Replace("\\", "/")] = Constants.ComputeFileMD5(fileName);

            return j;
        }
        internal static void AddHashes(string modName, string modDirectory)
        {
            CommonToolUtilities.modHashes[modName] = GenerateHashes(modDirectory);

            Util.WaitWhileFileLocked(modsSrcDir + "\\.ModHashes.json");
            File.WriteAllText(modsSrcDir + "\\.ModHashes.json", JsonMapper.ToJson(CommonToolUtilities.modHashes));

            RefreshHashes();
        }

        internal static void ZipModData(string output, byte[] tapimodData, byte[] modInfoData)
        {
            if (!Directory.Exists(new FileInfo(output).DirectoryName))
                Directory.CreateDirectory(new FileInfo(output).DirectoryName);

            using (ZipFile zip = new ZipFile())
            {
                zip.AddEntry("Mod.tapimod", tapimodData);
                zip.AddEntry("ModInfo.json", modInfoData);

                //foreach (string fileName in Directory.EnumerateFiles(sourcePath, "*.cs", SearchOption.AllDirectories))
                //    zip.AddEntry(fileName.Substring(sourcePath.Length + 1).Replace('\\', '/'), File.ReadAllBytes(fileName));
                //foreach (Tuple<string, byte[]> file in files)
                //    zip.AddEntry(file.Item1, file.Item2);

                zip.Save(output);
            }
        }
        internal static byte[] UnzipModData(string zip)
        {
            using (ZipFile zf = ZipFile.Read(zip))
            {
                ZipEntry ze = zf["Mod.tapimod"];
                if (ze != null)
                    using (MemoryStream ms = new MemoryStream())
                    {
                        ze.Extract(ms);
                        return ms.ToArray();
                    }
            }

            throw new FileFormatException(new Uri(zip));
        }

        internal static string CreateDefaultModInfo(string modName)
        {
            JsonData j = JsonMapper.ToObject("{}");

            j["displayName"] = "TAPI." + modName;
            j["author"] = Environment.UserName;
            j["info"] = "A mod called " + modName;
            j["internalName"] = modName;

            StringBuilder sb = new StringBuilder();

            JsonMapper.ToJson(j, new JsonWriter(sb) { PrettyPrint = true });

            return sb.ToString();
        }
    }
}
