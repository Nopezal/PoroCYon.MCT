using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using TAPI.SDK;

namespace TAPI.safeprojectname
{
    [GlobalMod]
    public class ModBuff : TAPI.ModBuff
    {
        public ModBuff(ModBase @base)
            : base(@base)
        {

        }

        public override void Effects(NPC n, int index)
        {
            base.Effects(n, index);


        }
        public override void Effects(Player p, int index)
        {
            base.Effects(p, index);


        }
    }
}
