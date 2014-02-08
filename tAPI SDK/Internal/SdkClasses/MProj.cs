using System;
using System.Collections.Generic;
using System.Linq;

namespace TAPI.SDK.Internal.SdkClasses
{
    [CLSCompliant(false)]
    sealed class MProj : ModProjectile
    {
        public MProj(ModBase @base, Projectile p)
            : base(@base, p)
        {

        }
    }
}
