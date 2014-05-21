﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT.Tools.Internal.Validation.Entities
{
    class Projectile : EntityValidator
    {
        // bools
        public bool friendly = false;
        public bool pet = false;
        public bool tileCollide = true;

        // ints
        public int frameCount = 1;
        public int aiStyle = 0;
        public int damage = 0;
        public int alpha = 0;
        public int timeLeft = 3600;

        internal override List<CompilerError> CreateAndValidate(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            errors.AddRange(CreateAndValidateBase(json, "Projectile", "Projectiles"));

            AddIfNotNull(SetJsonValue(json, "friendly",    ref friendly,    false), errors);
            AddIfNotNull(SetJsonValue(json, "pet",         ref pet,         false), errors);
            AddIfNotNull(SetJsonValue(json, "tileCollide", ref tileCollide, true ), errors);

            AddIfNotNull(SetJsonValue(json, "frameCount", ref frameCount, 1   ), errors);
            AddIfNotNull(SetJsonValue(json, "aiStyle",    ref aiStyle,    0   ), errors);
            AddIfNotNull(SetJsonValue(json, "damage",     ref damage,     0   ), errors);
            AddIfNotNull(SetJsonValue(json, "alpha",      ref alpha,      0   ), errors);
            AddIfNotNull(SetJsonValue(json, "timeLeft",   ref timeLeft,   3600), errors);

            return errors;
        }
    }
}
