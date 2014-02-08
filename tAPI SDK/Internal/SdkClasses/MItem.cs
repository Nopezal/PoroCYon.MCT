using System;
using System.Collections.Generic;
using System.Linq;

namespace TAPI.SDK.Internal.SdkClasses
{
    [CLSCompliant(false)]
    sealed class MItem : ModItem
    {
        public MItem(ModBase @base, Item i)
            : base(@base, i)
        {

        }
    }
}
