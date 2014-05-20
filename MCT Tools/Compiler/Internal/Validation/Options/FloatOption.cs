using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT.Tools.Internal.Validation.Options
{
    class FloatOption : Option
    {
        public float minimum;
        public float maximum;
        public float defaultValue;
        public float step;
        public int precision;

        protected override List<CompilerError> CreateAndValidateOverride(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            AddIfNotNull(SetJsonValue(json, "minimum",   ref minimum              ), errors);
            AddIfNotNull(SetJsonValue(json, "maximum",   ref maximum              ), errors);
            AddIfNotNull(SetJsonValue(json, "default",   ref defaultValue, minimum), errors);
            AddIfNotNull(SetJsonValue(json, "step",      ref step,         0.1f   ), errors);
            AddIfNotNull(SetJsonValue(json, "precision", ref precision,    1      ), errors);

            return errors;
        }
    }
}
