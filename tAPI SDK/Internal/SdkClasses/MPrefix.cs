using System;
using System.Collections.Generic;
using System.Linq;

namespace TAPI.SDK.Internal.SdkClasses
{
    [CLSCompliant(false)]
    sealed class MPrefix : ModPrefix
    {
        public MPrefix(Prefix p, ModBase @base)
            : base(p, @base)
        {

        }
    }
}
