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
    public class Prefix : TAPI.ModPrefix
    {
        public Prefix(ModBase @base)
            : base(@base)
        {

        }

        public override void ApplyToItem(Terraria.Item i)
        {
            base.ApplyToItem(i);


        }
        public override void ApplyToPlayer(Player p)
        {
            base.ApplyToPlayer(p);


        }
    }
}
