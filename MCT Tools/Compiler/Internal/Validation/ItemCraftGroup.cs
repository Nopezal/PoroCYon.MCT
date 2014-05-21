using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT.Tools.Internal.Validation
{
    class ItemCraftGroup : ValidatorObject
    {
        public string name;
        public string displayName = String.Empty;
        public string displayItem = String.Empty;
        public string[] items;

        internal override List<CompilerError> CreateAndValidate(JsonFile json)
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
