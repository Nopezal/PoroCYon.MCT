using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zip;
using TAPI;

namespace PoroCYon.MCT.Internal
{
    internal static class CommonToolUtilities
    {
        internal readonly static string
            modsDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\My Games\\Terraria\\tAPI\\Mods",
            modsSrcDir = modsDir + "\\Sources",
            modsBinDir = modsDir + "\\Unsorted";

        internal static void Init()
        {
            Constants.SetupVariablesOnce();
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
