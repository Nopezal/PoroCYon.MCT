using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LitJson;
using PoroCYon.MCT.Tools.Compiler.Validation.Options;

namespace PoroCYon.MCT.Tools.Compiler.Validation
{
    /// <summary>
    /// A ModOptions JSON file (ModOptions.json)
    /// </summary>
    public class ModOptions(ModCompiler mc) : ValidatorObject(mc)
    {
        List<Option> options = new List<Option>();

        /// <summary>
        /// Gets a collection containing all options.
        /// </summary>
        public ReadOnlyCollection<Option> Options
        {
            get
            {
                return options.AsReadOnly();
            }
        }

        /// <summary>
        /// Create &amp; validate a JSON file.
        /// </summary>
        /// <param name="json">The json to validate</param>
        /// <returns>A collection of all validation errors.</returns>
        public override IEnumerable<CompilerError> CreateAndValidate(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            JsonData optionsArr = null;

            if (json.Json.IsArray)
                optionsArr = json.Json;
            else if (!json.Json.Has("options"))
                errors.Add(new CompilerError()
                {
                    Cause = new KeyNotFoundException(),
                    IsWarning = false,
                    FilePath = json.Path,
                    Message = "Key 'options' not found."
                });
            else
                optionsArr = json.Json["options"];

            if (optionsArr == null)
                return errors;

            for (int i = 0; i < optionsArr.Count; i++)
            {
                var result = Option.NewOption(Compiler, new JsonFile(json.Path, optionsArr[i])); // actual stuff happens here

                options.Add(result.Item1);
                errors.AddRange(result.Item2);

                Option.Options.Clear(); // might mess up with modcompiler refs, thus clearing the dictionary.
            }

            return errors;
        }
    }
}
