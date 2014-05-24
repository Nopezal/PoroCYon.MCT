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

namespace
#if CREATE_TEMPLATE
    safeprojectname
#else
    $safeprojectname$
#endif
{
    public class Item : TAPI.ModItem
    {
        public Item(TAPI.ModBase @base, Terraria.Item i)
            : base(@base, i)
        {

        }

        public override bool? UseItem(Player p)
        {


            return base.UseItem(p);
        }

        public override void Effects(Player p)
        {
            base.Effects(p);


        }
    }
}
