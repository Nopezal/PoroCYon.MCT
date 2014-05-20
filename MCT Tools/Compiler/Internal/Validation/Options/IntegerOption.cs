using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT.Tools.Internal.Validation.Options
{
    class IntegerOption : Option
    {
        public int minimum;
        public int maximum;
        public int defaultValue = 0;

        protected override List<CompilerError> CreateAndValidateOverride(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            AddIfNotNull(SetJsonValue(json, "minimum",      ref minimum        ), errors);
            AddIfNotNull(SetJsonValue(json, "maximum",      ref maximum        ), errors);
            AddIfNotNull(SetJsonValue(json, "defaultValue", ref defaultValue, 0), errors);

            return errors;
        }
    }
}
