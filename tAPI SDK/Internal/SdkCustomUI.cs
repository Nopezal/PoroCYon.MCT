using System;
using System.Collections.Generic;
using System.Linq;
using TAPI.SDK.GUI;

namespace TAPI.SDK.Internal
{
    internal class SdkCustomUI : CustomUI
    {
        internal SdkCustomUI()
            : base()
        {
            IsDrawnAfter = true;
        }
    }
}
