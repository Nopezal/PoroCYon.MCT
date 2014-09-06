using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace PoroCYon.MCT.Tools.Compiler.Validation.Options
{
    /// <summary>
    /// A keybinding option.
    /// </summary>
    public class KeybindingOption(ModCompiler mc) : Option(mc)
    {
#pragma warning disable 1591
        public Keys defaultValue;
#pragma warning restore 1591

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

            if (Enum.TryParse(def, true, out Keys key))
                defaultValue = key;
            else
                errors.Add(new CompilerError()
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
