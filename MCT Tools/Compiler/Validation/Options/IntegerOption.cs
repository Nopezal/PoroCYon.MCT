using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT.Tools.Compiler.Validation.Options
{
    /// <summary>
    /// An integer option.
    /// </summary>
    public class IntegerOption : Option
    {
#pragma warning disable 1591
        public int minimum;
        public int maximum;
        public int defaultValue = 0;
#pragma warning restore 1591

        /// <summary>
        /// Create &amp; validate subclass-only fields.
        /// </summary>
        /// <param name="json">The json to validate</param>
        /// <returns>A collection of all validation errors.</returns>
        protected override IEnumerable<CompilerError> CreateAndValidateOverride(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            AddIfNotNull(SetJsonValue(json, "minimum",      ref minimum        ), errors);
            AddIfNotNull(SetJsonValue(json, "maximum",      ref maximum        ), errors);
            AddIfNotNull(SetJsonValue(json, "defaultValue", ref defaultValue, 0), errors);

            return errors;
        }
    }
}
