using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace PoroCYon.MCT.Tools.Compiler.Validation.Options
{
    /// <summary>
    /// A keybinding option.
    /// </summary>
    public class KeybindingOption : Option
    {
#pragma warning disable 1591
        public Keys defaultValue;
#pragma warning restore 1591

        /// <summary>
        /// Creates a new instance of the <see cref="KeybindingOption" /> class.
        /// </summary>
        /// <param name="mc"><see cref="CompilerPhase.Compiler" /></param>
        public KeybindingOption(ModCompiler mc)
            : base(mc)
        {

        }

        /// <summary>
        /// Create &amp; validate subclass-only fields.
        /// </summary>
        /// <param name="json">The json to validate</param>
        /// <returns>A collection of all validation errors.</returns>
        protected override IEnumerable<CompilerError> CreateAndValidateOverride(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            string def = String.Empty;

            AddIfNotNull(SetJsonValue(json, "default", ref def), errors);

            Keys key;
            if (Enum.TryParse(def, true, out key))
                defaultValue = key;
            else
                errors.Add(new CompilerError(Building)
                {
                    Cause = new InvalidCastException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "Key 'default': not a valid Keys enumeration value." 
                });

            return errors;
        }
    }
}
