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

namespace $safeprojectname$
{
    public class Projectile : TAPI.ModProjectile
    {
        public Projectile(TAPI.ModBase @base, Terraria.Projectile p)
            : base(@base, p)
        {

        }

        public override void AI()
        {
            base.AI();


        }

        public override void Kill()
        {
            base.Kill();


        }
    }
}
