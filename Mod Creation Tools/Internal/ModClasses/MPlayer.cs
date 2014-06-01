using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using TAPI;

namespace PoroCYon.MCT.Internal.ModClasses
{
    [GlobalMod]
    sealed class MPlayer : ModPlayer
    {
        public MPlayer(ModBase @base, Player p)
            : base(@base, p)
        {

        }

        public override void OnUpdate()
        {
            // ensure it's the local player >__>
            if (player == Main.localPlayer)
                Mod.UpdateThings(UpdateMode.PlayerUpdate);

            base.OnUpdate();
        }
    }
}
