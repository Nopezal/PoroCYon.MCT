using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;

namespace TAPI.$safeprojectname$
{
    public class ModInterface : TAPI.ModInterface
    {
        public ModInterface(ModBase @base)
            : base(@base)
        {

        }

        /// <summary>
        /// Called after the UI is drawn
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the UI</param>
        public override void PostDrawInterface(SpriteBatch sb)
        {
            // TODO: add your PostDrawInterface logic here

            base.PostDrawInterface(sb);
        }
    }
}
