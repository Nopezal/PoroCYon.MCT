using System;
using System.Collections.Generic;
using System.Linq;
using TAPI.SDK.Input;
using TAPI.SDK.UI;

namespace TAPI.SDK.Internal.ModClasses
{
    [GlobalMod]
    sealed class MWorld : ModWorld
    {
        public MWorld(ModBase @base)
            : base(@base)
        {

        }

        [CallPriority(Single.Epsilon)]
        public override void PostUpdate()
        {
            base.PostUpdate();

            SdkUI.Update();
        }
    }
}
