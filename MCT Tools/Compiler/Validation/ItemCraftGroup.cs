using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT.Tools.Compiler.Validation
{
    /// <summary>
    /// An item craft group.
    /// </summary>
    public class ItemCraftGroup : ValidatorObject
    {
#pragma warning disable 1591
        public string name;
        public string displayName = String.Empty;
        public string displayItem = String.Empty;
        public string[] items;
#pragma warning restore 1591

        /// <summary>
        /// Creates a new instance of the <see cref="ItemCraftGroup" /> class.
        /// </summary>
        /// <param name="mc"><see cref="CompilerPhase.Compiler" /></param>
        public ItemCraftGroup(ModCompiler mc)
            : base(mc)
        {

        }

        /// <summary>
        /// Create &amp; validate a JSON file.
        /// </summary>
        /// <param name="json">The json to validate</param>
        /// <returns>A collection of all validation errors.</returns>
        public override IEnumerable<CompilerError> CreateAndValidate(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();
            
            AddIfNotNull(SetJsonValue(json, "name",        ref name                 ), errors);
            AddIfNotNull(SetJsonValue(json, "items",       ref items                ), errors);
            AddIfNotNull(SetJsonValue(json, "displayName", ref displayName, name    ), errors);
            AddIfNotNull(SetJsonValue(json, "displayItem", ref displayItem, items[0]), errors);

            return errors;
        }
    }
}
