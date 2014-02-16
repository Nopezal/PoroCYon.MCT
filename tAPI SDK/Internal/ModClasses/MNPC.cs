using System;
using System.Collections.Generic;
using System.Linq;

namespace TAPI.SDK.Internal.ModClasses
{
    [GlobalMod]
    sealed class MNPC : ModNPC
    {
        public MNPC(ModBase @base, NPC n)
            : base(@base, n)
        {

        }
    }
}
