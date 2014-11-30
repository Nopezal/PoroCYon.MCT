using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT.Tools.Compiler.Validation.Options
{
    /// <summary>
    /// A dynamic option. Not actually validated.
    /// </summary>
    public class DynamicOption : Option
    {
        readonly static IEnumerable<CompilerError> errors = new List<CompilerError>(); // only alloc once

        /// <summary>
        /// Creates a new instance of the <see cref="DynamicOption" /> class.
        /// </summary>
        /// <param name="mc"><see cref="CompilerPhase.Compiler" /></param>
        public DynamicOption(ModCompiler mc)
            : base(mc)
        {

        }

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
