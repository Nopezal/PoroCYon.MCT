using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using PoroCYon.MCT;

namespace TAPI.$safeprojectname$
{
    [GlobalMod]
    public class ModPrefix : TAPI.ModPrefix
    {
        public ModPrefix(ModBase @base, Prefix p)
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
