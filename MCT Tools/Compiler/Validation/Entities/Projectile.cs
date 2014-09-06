using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT.Tools.Compiler.Validation.Entities
{
    /// <summary>
    /// A projectile.
    /// </summary>
    public class Projectile(ModCompiler mc) : EntityValidator(mc)
    {
#pragma warning disable 1591
        // bools
        public bool friendly = false;
        public bool hurtsTiles = false;
        public bool ignoreWater = false;
        public bool magic = false;
        public bool manualDirection = false;
        public bool melee = false;
        public bool minion = false;
        public bool ownerHitCheck = false;
        public bool pet = false;
        public bool ranged = true;
        public bool tileCollide = true;

        // ints
        public int aiStyle = 0;
        public int alpha = 0;
        public int damage = 0;
        public int frameCount = 1;
        public int maxUpdates = -1;
        public int npcCritChance = 4;
        public int penetrate = 1;
        public int timeLeft = 3600;

        // floats
        public float knockback = 1f;
        public float npcCritMult = 2f;

        // strings
        public string killedSound = String.Empty;

#pragma warning restore 1591

        /// <summary>
        /// Create &amp; validate a JSON file.
        /// </summary>
        /// <param name="json">The json to validate</param>
        /// <returns>A collection of all validation errors.</returns>
        public override IEnumerable<CompilerError> CreateAndValidate(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            errors.AddRange(CreateAndValidateBase(json, "Projectile", "Projectiles"));

            AddIfNotNull(SetJsonValue(json, "friendly"       , ref friendly       , true ), errors);
            AddIfNotNull(SetJsonValue(json, "hurtsTiles"     , ref hurtsTiles     , false), errors);
            AddIfNotNull(SetJsonValue(json, "ignoreWater"    , ref ignoreWater    , false), errors);
            AddIfNotNull(SetJsonValue(json, "magic"          , ref magic          , false), errors);
            AddIfNotNull(SetJsonValue(json, "manualDirection", ref manualDirection, false), errors);
            AddIfNotNull(SetJsonValue(json, "melee"          , ref melee          , false), errors);
            AddIfNotNull(SetJsonValue(json, "minion"         , ref minion         , false), errors);
            AddIfNotNull(SetJsonValue(json, "ownerHitCheck"  , ref ownerHitCheck  , false), errors);
            AddIfNotNull(SetJsonValue(json, "pet"            , ref pet            , false), errors);
            AddIfNotNull(SetJsonValue(json, "ranged"         , ref ownerHitCheck  , false), errors);
            AddIfNotNull(SetJsonValue(json, "tileCollide"    , ref tileCollide    , true ), errors);

            AddIfNotNull(SetJsonValue(json, "aiStyle"      , ref aiStyle      , 0   ), errors);
            AddIfNotNull(SetJsonValue(json, "alpha"        , ref alpha        , 0   ), errors);
            AddIfNotNull(SetJsonValue(json, "damage"       , ref damage       , 0   ), errors);
            AddIfNotNull(SetJsonValue(json, "frameCount"   , ref frameCount   , 1   ), errors);
            AddIfNotNull(SetJsonValue(json, "maxUpdates"   , ref maxUpdates   , 1   ), errors);
            AddIfNotNull(SetJsonValue(json, "npcCritChance", ref npcCritChance, 4   ), errors);
            AddIfNotNull(SetJsonValue(json, "penetrate"    , ref penetrate    , 1   ), errors);
            AddIfNotNull(SetJsonValue(json, "timeLeft"     , ref timeLeft     , 3600), errors);

            AddIfNotNull(SetJsonValue(json, "knockback"  , ref knockback  , 1f), errors);
            AddIfNotNull(SetJsonValue(json, "npcCritMult", ref npcCritMult, 2f), errors);

            AddIfNotNull(SetJsonValue(json, "killedSound", ref killedSound, String.Empty), errors);

            return errors;
        }
    }
}
