using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT.Tools.Internal.Validation.Entities
{
    class Wall : EntityValidator
    {
        internal override List<CompilerError> CreateAndValidate(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            errors.AddRange(CreateAndValidateBase(json, "Wall", "Walls"));



            return errors;
        }
    }
    class Tile : EntityValidator
    {
        internal override List<CompilerError> CreateAndValidate(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            errors.AddRange(CreateAndValidateBase(json, "Tile", "Tiles"));



            return errors;
        }
    }
}
