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
    public class ModBase : TAPI.ModBase
    {
        public ModBase()
            : base()
        {
            // leave this empty, use OnLoad
        }

        public override void OnLoad()
        {
            base.OnLoad();


        }
        public override void OnAllModsLoaded()
        {
            base.OnAllModsLoaded();


        }
        public override void OnUnload()
        {
            base.OnUnload();


        }

        public override object OnModCall(TAPI.ModBase mod, params object[] arguments)
        {
            return null; // return base.OnModCall(mod, arguments); returns a NotSupportedException
        }

        public override void NetReceive(int msg, BinBuffer bb)
        {
            base.NetReceive(msg, bb);


        }

        public override void PostGameDraw(SpriteBatch sb)
        {


            base.PostGameDraw(sb);
        }
    }
}
