using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;

namespace TAPI.$safeprojectname$
{
    [GlobalMod]
    public class ModWorld : TAPI.ModWorld
    {
        public ModWorld(TAPI.ModBase @base)
            : base(@base)
        {

        }

        /// <summary>
        /// Called when the world is updated
        /// </summary>
        public override void OnUpdate()
        {
            // TODO: add your OnUpdate logic here

            base.OnUpdate();
        }
    }
}
