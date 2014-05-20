using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LitJson;

namespace PoroCYon.MCT.Tools.Internal.Validation
{
    class ModOptions : ValidatorObject
    {
        List<Option> opts = new List<Option>();

        internal ReadOnlyCollection<Option> options
        {
            get
            {
                return opts.AsReadOnly();
            }
        }

        internal override List<CompilerError> CreateAndValidate(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            JsonData optionsArr = null;

            if (json.json.IsArray)
                optionsArr = json.json;
            else if (!json.json.Has("options"))
                errors.Add(new CompilerError()
                {
                    Cause = new KeyNotFoundException(),
                    IsWarning = false,
                    FilePath = json.path,
                    Message = "Key 'options' not found."
                });
            else
                optionsArr = json.json["options"];

            if (optionsArr == null)
                return errors;

            for (int i = 0; i < optionsArr.Count; i++)
            {
                var result = Option.NewOption(new JsonFile(json.path, optionsArr[i])); // actual stuff happens here

                opts.Add(result.Item1);
                errors.AddRange(result.Item2);
            }

            return errors;
        }
    }
}
