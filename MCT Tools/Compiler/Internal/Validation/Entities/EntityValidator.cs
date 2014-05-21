using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT.Tools.Internal.Validation.Entities
{
    abstract class EntityValidator : ValidatorObject
    {
        public string code;

        protected ModInfo modInfo;

        protected EntityValidator(ModInfo info)
        {
            modInfo = info;
        }
    }
}
