using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT.Tools.Compiler.Validation.Options
{
    /// <summary>
    /// A dynamic option. Not actually validated.
    /// </summary>
    public class DynamicOption(ModCompiler mc) : Option(mc)
    {
        readonly static IEnumerable<CompilerError> errors = new List<CompilerError>(); // only alloc once

        /// <summary>
        /// Create &amp; validate subclass-only fields.
        /// </summary>
        /// <param name="json">The json to validate</param>
        /// <returns>A collection of all validation errors.</returns>
        protected override IEnumerable<CompilerError> CreateAndValidateOverride(JsonFile json)
        {
            return errors;
        }
    }
}
