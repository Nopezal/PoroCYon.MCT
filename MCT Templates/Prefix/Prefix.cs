using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using PoroCYon.MCT;

namespace TAPI.safeprojectname.Prefix
{
    public class Prefix : TAPI.ModPrefix
    {
        public Prefix(ModBase @base, TAPI.Prefix p)
            : base(@base, p)
        {

        }

        public override void ApplyToItem(Item i)
        {
            base.ApplyToItem(i);


        }

        public override void ApplyToPlayer(Player p)
        {
            base.ApplyToPlayer(p);


        }
    }
}
