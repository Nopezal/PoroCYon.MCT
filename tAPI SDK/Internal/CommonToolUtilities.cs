using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TAPI.SDK.Internal
{
    internal static class CommonToolUtilities
    {
        internal const string GUID = "af310c8e-783e-4aa0-a9cf-09aa1f41632f";

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
    }
}
