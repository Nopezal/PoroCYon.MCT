using System;
using System.Collections.Generic;
using System.Linq;

namespace TAPI.SDK.Internal.SdkClasses
{
    [CLSCompliant(false)]
    sealed class MNPC : ModNPC
    {
        public MNPC(ModBase @base, NPC n)
            : base(@base, n)
        {

        }
    }
}
