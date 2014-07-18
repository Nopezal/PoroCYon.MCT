using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT.Tools.Compiler.Validation.Options
{
    /// <summary>
    /// A string option.
    /// </summary>
    public class StringOption : Option
    {
#pragma warning disable 1591
        public int length;
        public string defaultValue = String.Empty;
#pragma warning restore 1591

        /// <summary>
        /// Create &amp; validate subclass-only fields.
        /// </summary>
        /// <param name="json">The json to validate</param>
        /// <returns>A collection of all validation errors.</returns>
        protected override IEnumerable<CompilerError> CreateAndValidateOverride(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            AddIfNotNull(SetJsonValue(json, "length",  ref length                    ), errors);
            AddIfNotNull(SetJsonValue(json, "default", ref defaultValue, String.Empty), errors);

            return errors;
        }
    }
}
