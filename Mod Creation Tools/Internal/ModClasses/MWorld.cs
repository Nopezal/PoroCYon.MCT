using System;
using System.Collections.Generic;
using System.Linq;
using TAPI;
using PoroCYon.MCT.Input;
using PoroCYon.MCT.UI;

namespace PoroCYon.MCT.Internal.ModClasses
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

            //MctUI.Update();
        }
    }
}
