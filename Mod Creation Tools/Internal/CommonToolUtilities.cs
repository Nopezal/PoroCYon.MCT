using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using LitJson;
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
            API.SetupVariablesOnce();
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

        internal static JsonType JsonTypeFromType(Type t)
        {
            if (t.IsArray)
                return JsonType.Array;
            if (t == typeof(bool))
                return JsonType.Boolean;
            if (t == typeof(string))
                return JsonType.String;
            if (t == typeof(double) || t == typeof(float) || t == typeof(BigInteger))
                return JsonType.Double;
            if (t == typeof(long) || t == typeof(ulong))
                return JsonType.Long;
            if (t == typeof(int) || t == typeof(uint) ||
                t == typeof(short) || t == typeof(ushort) ||
                t == typeof(sbyte) || t == typeof(byte))
                return JsonType.Int;

            return JsonType.Object;
        }
    }
}
