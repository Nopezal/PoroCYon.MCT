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
    public class Tile : TAPI.ModTile
    {
        public Tile(TAPI.ModBase @base)
            : base(@base)
        {

        }

        public override void Update()
        {
            base.Update();


        }

        public override void Kill(int x, int y, bool fail, bool effectsOnly, bool noItem)
        {
            base.Kill(x, y, fail, effectsOnly, noItem);


        }
    }
}
