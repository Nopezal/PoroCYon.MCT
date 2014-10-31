using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using TAPI;

namespace PoroCYon.MCT.Internal.ModClasses
{
    sealed class MPlayer : ModPlayer
    {
        public override void MidUpdate()
        {
            // ensure it's the local player >__>
            if (player == Main.localPlayer)
                MctMod.UpdateThings(UpdateMode.PlayerUpdate);

            base.MidUpdate();
        }
	}
}
