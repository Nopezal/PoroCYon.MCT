using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using LitJson;
using TAPI;
using Terraria;

namespace PoroCYon.MCT.Internal
{
    internal static class CommonToolUtilities
    {
        internal static void Init()
		{
			Mods.path         = Main.SavePath.Combine("Mods"   );
			Mods.pathSources  = Mods.path    .Combine("Sources");
			Mods.pathCompiled = Mods.path    .Combine("Local"  );

			if (!Mods.path.Exists)
				 Mods.path.CreateDirectory();
			if (!Mods.pathCompiled.Exists)
				 Mods.pathCompiled.CreateDirectory();
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
