using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT.Tools.Internal.Validation.Options
{
    class StringOption : Option
    {
        public int length;
        public string defaultValue = String.Empty;

        protected override List<CompilerError> CreateAndValidateOverride(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            AddIfNotNull(SetJsonValue(json, "length",  ref length                    ), errors);
            AddIfNotNull(SetJsonValue(json, "default", ref defaultValue, String.Empty), errors);

            return errors;
        }
    }
}
