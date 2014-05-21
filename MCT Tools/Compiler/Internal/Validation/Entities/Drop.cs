using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT.Tools.Internal.Validation.Entities
{
    class Drop : ValidatorObject
    {
        public string item = String.Empty;
        public int stack = 1;
        public float chance = 0f;

        internal override List<CompilerError> CreateAndValidate(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            AddIfNotNull(SetJsonValue(json, "stack", ref stack, 1), errors);
            if (stack < 0)
                errors.Add(new CompilerError()
                {
                    Cause = new ArgumentOutOfRangeException(),
                    FilePath = json.path,
                    IsWarning = false,
                    Message = "'stack' is below 0. Please remove the Drop object from the array."
                });
            else if (stack == 1)
                errors.Add(new CompilerError()
                {
                    Cause = new CompilerWarning(),
                    FilePath = json.path,
                    IsWarning = true,
                    Message = "'stack' is equal to 0. It is a good idea to remove the Drop object from the array."
                });
            AddIfNotNull(SetJsonValue(json, "item",  ref item), errors);
            AddIfNotNull(SetJsonValue(json, "chance", ref chance, 0f), errors);
            if (chance > 1f)
                errors.Add(new CompilerError()
                {
                    Cause = new ArgumentOutOfRangeException(),
                    FilePath = json.path,
                    IsWarning = true,
                    Message = "'chance' is above 1. Change it to 1."
                });
            if (chance < 0f)
                errors.Add(new CompilerError()
                {
                    Cause = new ArgumentOutOfRangeException(),
                    FilePath = json.path,
                    IsWarning = true,
                    Message = "'chance' is below 0. Please remove the Drop object from the array."
                });
            else if (chance == 0f)
                errors.Add(new CompilerError()
                {
                    Cause = new CompilerWarning(),
                    FilePath = json.path,
                    IsWarning = true,
                    Message = "'chance' is equal to 0. It is a good idea to remove the Drop object from the array."
                });

            return errors;
        }
    }
}
