using System;
using System.Collections.Generic;
using System.Linq;
using PoroCYon.Extensions;

namespace PoroCYon.MCT.Tools.Compiler.Validation.Entities
{
    /// <summary>
    /// A wall.
    /// </summary>
    public class Wall(ModCompiler mc) : EntityValidator(mc)
    {
#pragma warning disable 1591
        public bool house = true;
        public bool dungeon = false;
        public bool light = false;

        public int blend = 0;
        public int sound = 0;
        public int soundGroup = 0;
        public int dust = 0;

        public Union<string, int> drop = 0;
#pragma warning restore 1591

        /// <summary>
        /// Create &amp; validate a JSON file.
        /// </summary>
        /// <param name="json">The json to validate</param>
        /// <returns>A collection of all validation errors.</returns>
        public override IEnumerable<CompilerError> CreateAndValidate(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            errors.AddRange(CreateAndValidateBase(json, "Walls", "Walls"));

            AddIfNotNull(SetJsonValue(json, "house", ref house, true), errors);
            AddIfNotNull(SetJsonValue(json, "dungeon", ref dungeon, false), errors);
            AddIfNotNull(SetJsonValue(json, "light", ref light, false), errors);

            AddIfNotNull(SetJsonValue(json, "blend", ref blend, 0), errors);
            AddIfNotNull(SetJsonValue(json, "sound", ref sound, 0), errors);
            AddIfNotNull(SetJsonValue(json, "soundGroup", ref soundGroup, 0), errors);
            AddIfNotNull(SetJsonValue(json, "dust", ref dust, 0), errors);

            AddIfNotNull(SetJsonValue(json, "drop", ref drop, drop), errors);

            return errors;
        }
    }
}
