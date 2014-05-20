using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT.Tools.Internal.Validation.Options
{
    class BoolOption : ListOption
    {
        protected override List<CompilerError> CreateAndValidateOverride(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            values.Add(false);
            values.Add(true );

            defaultValue = false;
            AddIfNotNull(SetDefault(json), errors);

            return errors;
        }
    }
}
