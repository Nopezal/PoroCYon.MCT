﻿using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace PoroCYon.MCT.Internal
{
    static class Consts
    {
        internal const string GUID = "af310c8e-783e-4aa0-a9cf-09aa1f41632f";
        internal const string TPOOReason = "Performance critical to inline across NGen image boundaries";
        internal readonly static string MctDirectory = Main.SavePath + "\\MCT\\";

        internal const int ENUM_OFFSET = 128;
    }

    enum InternalNetMessages : byte
    {
        SyncRandom_Sync = Consts.ENUM_OFFSET + 0,
        SyncRandom_CTOR = Consts.ENUM_OFFSET + 1,
        SyncRandom_DTOR = Consts.ENUM_OFFSET + 2
    }
    enum InternalModMessages : byte
	{

    }
}
