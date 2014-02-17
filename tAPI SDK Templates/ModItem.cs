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
    public class ModItem : TAPI.ModItem
    {
        public ModItem(TAPI.ModBase @base, TAPI.Item i)
            : base(@base, i)
        {

        }

        /// <summary>
        /// Called before the Item is used. Return true wether the Item can be used, false otherwise.
        /// </summary>
        /// <param name="player">The Player who's using the item</param>
        /// <param name="currentState">Wether it should use it or not, set by other mods.</param>
        /// <returns>true wether the Item can be used, false otherwise</returns>
        public override bool CanUse(Player player, bool currentState)
        {
            // TODO: add your CanUse logic here

            return currentState;
        }
    }
}
