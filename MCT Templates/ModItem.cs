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
    public class ModItem : TAPI.ModItem
    {
        public ModItem(TAPI.ModBase @base, Terraria.Item i)
            : base(@base, i)
        {

        }

        public override void UseItem(Player p)
        {


            base.UseItem(p);
        }

        public override void Effects(Player p)
        {


            base.Effects(p);
        }
    }
}
