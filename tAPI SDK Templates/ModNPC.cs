﻿using System;
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
    public class ModNPC : TAPI.ModNPC
    {
        public ModNPC(TAPI.ModBase @base, TAPI.NPC n)
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
