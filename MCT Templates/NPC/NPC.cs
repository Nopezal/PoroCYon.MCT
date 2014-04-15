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

namespace $safeprojectname$
{
    public class NPC : TAPI.ModNPC
    {
        public NPC(TAPI.ModBase @base, Terraria.NPC n)
            : base(@base, n)
        {

        }

        public override void AI()
        {
            base.AI();


        }

        public override void NPCLoot()
        {
            base.NPCLoot();


        }
    }
}
