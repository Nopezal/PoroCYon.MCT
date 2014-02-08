using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;

namespace TAPI.$safeprojectname$
{
    public class Item : TAPI.ModItem
    {
        public Item(TAPI.ModBase @base, TAPI.Item i)
            : base(@base, i)
        {

        }

        /// <summary>
        /// Called when the Item is initialized (Item.Initialize is called)
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            // TODO: add your Initialize logic here
        }
    }
}
