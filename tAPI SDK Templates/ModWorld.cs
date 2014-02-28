using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using TAPI.SDK;

namespace TAPI.$safeprojectname$
{
    [GlobalMod]
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

        public override void Load(BinBuffer bb)
        {
            base.Load(bb);


        }

        public override void Save(BinBuffer bb)
        {
            base.Save(bb);


        }
    }
}
