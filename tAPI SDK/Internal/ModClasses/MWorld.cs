using System;
using System.Collections.Generic;
using System.Linq;
using TAPI.SDK.GUI;
using TAPI.SDK.Input;

namespace TAPI.SDK.Internal.ModClasses
{
    [GlobalMod]
    sealed class MWorld : ModWorld
    {
        public MWorld(ModBase @base)
            : base(@base)
        {

        }

        public override void PostUpdate()
        {
            GInput.Update();

            SdkUI.Update();

            base.PostUpdate();
        }
    }
}
