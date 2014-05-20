using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT.Tools.Internal.Validation.Options
{
    class DynamicOption : Option
    {
        readonly static List<CompilerError> errors = new List<CompilerError>(); // only alloc once

        protected override List<CompilerError> CreateAndValidateOverride(JsonFile json)
        {
            return errors;
        }
    }
}
