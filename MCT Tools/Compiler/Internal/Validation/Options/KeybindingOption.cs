using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace PoroCYon.MCT.Tools.Internal.Validation.Options
{
    class KeybindingOption : Option
    {
        public Keys defaultValue;

        protected override List<CompilerError> CreateAndValidateOverride(JsonFile json)
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
                    FilePath = json.path,
                    IsWarning = false,
                    Message = "Key 'default': not a valid Keys enumeration value." 
                });

            return errors;
        }
    }
}
