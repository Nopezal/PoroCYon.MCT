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
    public class ModPlayer : TAPI.ModPlayer
    {
        public ModPlayer(TAPI.ModBase modBase)
            : base(modBase)
        {

        }

        /// <summary>
        /// Called when the player is updated (Player.Update is called)
        /// </summary>
        /// <param name="player">The Player who is being updated</param>
        public override void OnUpdate(Player player)
        {
            base.OnUpdate(player);

            // TODO: add your OnUpdate logic here
        }
    }
}
