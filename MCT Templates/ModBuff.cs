using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using Terraria;
using TAPI;
using PoroCYon.MCT;

namespace $safeprojectname$
{
    [GlobalMod]
    public class ModBuff : TAPI.ModBuff
    {
        public ModBuff(ModBase @base)
            : base(@base)
        {

        }

        public override void Effects(NPC npc, int index)
        {
            base.Effects(npc, index);


        }
        public override void Effects(Player p, int index)
        {
            base.Effects(p, index);


        }
    }
}
