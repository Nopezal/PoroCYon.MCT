using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT.Tools.Compiler.Validation.Entities
{
    /// <summary>
    /// A buff (positive, negative, weapon or pet).
    /// </summary>
    public class Buff : EntityValidator
    {
#pragma warning disable 1591
        public string tip;
        public bool debuff, vanityPet, lightPet, enchantment, noTimer;
#pragma warning restore 1591

        /// <summary>
        /// Create &amp; validate a JSON file.
        /// </summary>
        /// <param name="json">The json to validate</param>
        /// <returns>A collection of all validation errors.</returns>
        public override IEnumerable<CompilerError> CreateAndValidate(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            errors.AddRange(CreateAndValidateBase(json, "Buff", "Buffs"));

            AddIfNotNull(SetJsonValue(json, "tip",         ref tip        ), errors);

            AddIfNotNull(SetJsonValue(json, "debuff",      ref debuff,      false), errors);
            AddIfNotNull(SetJsonValue(json, "vanityPet",   ref vanityPet,   false), errors);
            AddIfNotNull(SetJsonValue(json, "lightPet",    ref lightPet,    false), errors);
            AddIfNotNull(SetJsonValue(json, "enchantment", ref enchantment, false), errors);
            AddIfNotNull(SetJsonValue(json, "noTimer",     ref noTimer,     false), errors);

            return errors;
        }
    }
}
