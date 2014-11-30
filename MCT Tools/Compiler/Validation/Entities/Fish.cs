using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT.Tools.Compiler.Validation.Entities
{
    /// <summary>
    /// The world layer of a <see cref="Fish" />.
    /// </summary>
    public enum WorldLayer
    {
        /// <summary>
        /// Sky
        /// </summary>
        Sky,
        /// <summary>
        /// Surface
        /// </summary>
        Surface,
        /// <summary>
        /// DirtLayer
        /// </summary>
        DirtLayer,
        /// <summary>
        /// RockLayer
        /// </summary>
        RockLayer,
        /// <summary>
        /// Hell
        /// </summary>
        Hell,
        /// <summary>
        /// Underworld
        /// </summary>
        Underworld = Hell // two possibilities, same value
    }
    /// <summary>
    /// The lidquid type of a <see cref="Fish" />.
    /// </summary>
    public enum LiquidType
    {
        /// <summary>
        /// Water
        /// </summary>
        Water,
        /// <summary>
        /// Lava
        /// </summary>
        Lava,
        /// <summary>
        /// Honey
        /// </summary>
        Honey
    }

    /// <summary>
    /// A fish.
    /// </summary>
    public class Fish : ValidatorObject
    {
        readonly static string[] EmptyStringArr = { };

#pragma warning disable 1591
        public float catchChance;
        public WorldLayer worldLayer;
        public LiquidType liquidType;
        public string[] biomes;
        public int poolCount = 0;
#pragma warning restore 1591

        /// <summary>
        /// Creates a new instance of the <see cref="Fish" /> class.
        /// </summary>
        /// <param name="mc"><see cref="CompilerPhase.Compiler" /></param>
        public Fish(ModCompiler mc)
            : base(mc)
        {

        }

        /// <summary>
        /// Create &amp; validate a JSON file.
        /// </summary>
        /// <param name="json">The json to validate</param>
        /// <returns>A collection of all validation errors.</returns>
        public override IEnumerable<CompilerError> CreateAndValidate(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            AddIfNotNull(SetJsonValue(json, "catchChance", ref catchChance, 0.1f), errors);

            AddIfNotNull(SetJsonValue(json, "worldLayer", ref worldLayer), errors);
            AddIfNotNull(SetJsonValue(json, "liquidType", ref liquidType, LiquidType.Water), errors);

            AddIfNotNull(SetJsonValue(json, "biomes", ref biomes, EmptyStringArr), errors); // values are checked by the... checker

            AddIfNotNull(SetJsonValue(json, "poolCount", ref poolCount, 0), errors);

            if (poolCount < 0)
                errors.Add(new CompilerError(Building)
                {
                    Cause = new ArgumentOutOfRangeException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "Invalid value for property 'poolCount' " + poolCount + ", value must be equal to or greater than 0."
                });

            return errors;
        }
    }
}
