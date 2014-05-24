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
    public class ModWorld : TAPI.ModWorld
    {
        public ModWorld(TAPI.ModBase @base)
            : base(@base)
        {

        }

        public override void PostUpdate()
        {
            base.PostUpdate();


        }
    }
}
