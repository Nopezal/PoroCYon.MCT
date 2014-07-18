using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT.Tools.Compiler.Validation
{
    /// <summary>
    /// A tile craft group.
    /// </summary>
    public class TileCraftGroup : ValidatorObject
    {
#pragma warning disable 1591
        public string name;
        public string displayName;
        public string displayTile;
        public string[] tiles;
#pragma warning restore 1591

        /// <summary>
        /// Create &amp; validate a JSON file.
        /// </summary>
        /// <param name="json">The json to validate</param>
        /// <returns>A collection of all validation errors.</returns>
        public override IEnumerable<CompilerError> CreateAndValidate(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            AddIfNotNull(SetJsonValue(json, "name",        ref name                 ), errors);
            AddIfNotNull(SetJsonValue(json, "tiles",       ref tiles                ), errors);
            AddIfNotNull(SetJsonValue(json, "displayName", ref displayName, name    ), errors);
            AddIfNotNull(SetJsonValue(json, "displayTile", ref displayTile, tiles[0]), errors);

            return errors;
        }
    }
}
