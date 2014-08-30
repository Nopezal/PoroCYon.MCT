using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT.Tools.Compiler.Validation.Entities
{
    /// <summary>
    /// An NPC drop.
    /// </summary>
    public class Drop : ValidatorObject
    {
#pragma warning disable 1591
        public string item = String.Empty;
        public int stack = 1;
        public float chance = 0f;
#pragma warning restore 1591

        /// <summary>
        /// Create &amp; validate a JSON file.
        /// </summary>
        /// <param name="json">The json to validate</param>
        /// <returns>A collection of all validation errors.</returns>
        public override IEnumerable<CompilerError> CreateAndValidate(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            AddIfNotNull(SetJsonValue(json, "stack", ref stack, 1), errors);
            if (stack <= 0)
                errors.Add(new CompilerError()
                {
                    Cause = new ArgumentOutOfRangeException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "'stack' is equal to or below 0. Please remove the Drop object from the array, or change the stack."
                });
            AddIfNotNull(SetJsonValue(json, "item",  ref item), errors);
            AddIfNotNull(SetJsonValue(json, "chance", ref chance, 0f), errors);
            if (chance > 1f)
                errors.Add(new CompilerError()
                {
                    Cause = new ArgumentOutOfRangeException(),
                    FilePath = json.Path,
                    IsWarning = true,
                    Message = "'chance' is above 1. Set it to 1."
                });
            if (chance <= 0f)
                errors.Add(new CompilerError()
                {
                    Cause = new ArgumentOutOfRangeException(),
                    FilePath = json.Path,
                    IsWarning = true,
                    Message = "'chance' is equal to or below 0. Please remove the Drop object from the array, or change it."
                });

            return errors;
        }
    }
}
