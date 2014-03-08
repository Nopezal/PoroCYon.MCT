using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using PoroCYon.MCT;
using PoroCYon.MCT.GUI;
using PoroCYon.MCT.GUI.Controls;

namespace TAPI.$safeprojectname$
{
    public class ModInterface : TAPI.ModInterface
    {
        public ModInterface(ModBase @base)
            : base(@base)
        {

        }

        public override bool PreDrawInterface(SpriteBatch sb)
        {


            return base.PreDrawInterface(sb);
        }

        public override bool PreDrawInventory(SpriteBatch sb)
        {


            return base.PreDrawInventory(sb);
        }

        public override void PostDrawInventory(SpriteBatch sb)
        {


            base.PostDrawInventory(sb);
        }

        public override void PostDrawInterface(SpriteBatch sb)
        {


            base.PostDrawInterface(sb);
        }
    }
}
