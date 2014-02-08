using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;

namespace TAPI.$safeprojectname$
{
    public class ModBase : TAPI.ModBase
    {
        public ModBase()
            : base()
        {

        }

        /// <summary>
        /// Called when the mod is loaded
        /// </summary>
        public override void OnLoad()
        {
            base.OnLoad();

            // TODO: add your OnLoad logic here
        }
        /// <summary>
        /// Called when all mods are loaded
        /// </summary>
        public override void OnAllModsLoaded()
        {
            base.OnAllModsLoaded();

            // TODO: add your OnAllModsLoaded logic here
        }
        /// <summary>
        /// Called when the mod is unloaded
        /// </summary>
        public override void OnUnload()
        {
            base.OnUnload();

            // TODO: add your OnUnload logic here
        }

        /// <summary>
        /// Called when the game is updated (Main.Update is called)
        /// </summary>
        public override void OnGameUpdate()
        {
            base.OnGameUpdate();
        }
        /// <summary>
        /// Called when the game is drawn (Main.Draw is called)
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the game</param>
        public override void OnGameDraw(SpriteBatch sb)
        {
            base.OnGameDraw(sb);

            // TODO: add your OnGameDraw logic here
        }

        /// <summary>
        /// Called when a mod calls CallInMod to this mod
        /// </summary>
        /// <param name="mod">The calling mod</param>
        /// <param name="arguments">The arguments the calling mod passed</param>
        /// <returns>The return value of the method</returns>
        public override object OnModCall(TAPI.ModBase mod, params object[] arguments)
        {
            return null; // return base.OnModCall(mod, arguments); returns a NotSupportedException
        }
    }
}
