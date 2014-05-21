using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT.Tools.Internal.Validation
{
    class TileCraftGroup : ValidatorObject
    {
        public string name;
        public string displayName;
        public string displayTile;
        public string[] tiles;

        internal override List<CompilerError> CreateAndValidate(JsonFile json)
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
