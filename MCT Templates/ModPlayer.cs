﻿using System;
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
    [GlobalMod]
    public class ModPlayer : TAPI.ModPlayer
    {
        public ModPlayer(TAPI.ModBase @base, Player p)
            : base(@base, p)
        {

        }

        public override void OnUpdate()
        {
            base.OnUpdate();


        }

        public override void Save(BinBuffer bb)
        {
            base.Save(bb);


        }

        public override void Load(BinBuffer bb)
        {
            base.Load(bb);


        }
    }
}
