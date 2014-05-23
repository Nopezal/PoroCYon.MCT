using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT.Tools.Validation.Options
{
    /// <summary>
    /// A floating-point option
    /// </summary>
    public class FloatOption : Option
    {
#pragma warning disable 1591
        public float minimum;
        public float maximum;
        public float defaultValue;
        public float step;
        public int precision;
#pragma warning restore 1591

        /// <summary>
        /// Create &amp; validate subclass-only fields.
        /// </summary>
        /// <param name="json">The json to validate</param>
        /// <returns>A collection of all validation errors.</returns>
        protected override IEnumerable<CompilerError> CreateAndValidateOverride(JsonFile json)
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
