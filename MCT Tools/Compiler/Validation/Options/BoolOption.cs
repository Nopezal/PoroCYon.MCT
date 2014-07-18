using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT.Tools.Compiler.Validation.Options
{
    /// <summary>
    /// A boolean mod option
    /// </summary>
    public class BoolOption : ListOption
    {
        /// <summary>
        /// Create &amp; validate subclass-only fields.
        /// </summary>
        /// <param name="json">The json to validate</param>
        /// <returns>A collection of all validation errors.</returns>
        protected override IEnumerable<CompilerError> CreateAndValidateOverride(JsonFile json)
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
