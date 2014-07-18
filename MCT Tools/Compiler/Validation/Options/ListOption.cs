using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT.Tools.Compiler.Validation.Options
{
    /// <summary>
    /// A list option.
    /// </summary>
    public class ListOption : Option
    {
#pragma warning disable 1591
        public List<object> values = new List<object>();
        public object defaultValue;
#pragma warning restore 1591

        /// <summary>
        /// Create &amp; validate subclass-only fields.
        /// </summary>
        /// <param name="json">The json to validate</param>
        /// <returns>A collection of all validation errors.</returns>
        protected override IEnumerable<CompilerError> CreateAndValidateOverride(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            if (!json.Json.Has("list"))
                errors.Add(new CompilerError()
                {
                    Cause = new KeyNotFoundException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "Key 'list' not found."
                });
            else
            {
                var list = json.Json["list"];

                for (int i = 0; i < list.Count; i++)
                    values.Add((string)list[i]);
            }

            SetDefault(json);

            return errors;
        }

        /// <summary>
        /// Sets the default value of the JSON Option object.
        /// </summary>
        /// <param name="json">The JSON object to check.</param>
        /// <returns>The (possible) compiler error.</returns>
        protected CompilerError SetDefault(JsonFile json)
        {
            if (json.Json.Has("default"))
            {
                var def = json.Json["default"];

                if (def.IsString)
                {
                    if (!values.Contains(defaultValue = (string)def))
                        return new CompilerError()
                        {
                            Cause = new KeyNotFoundException(),
                            FilePath = json.Path,
                            IsWarning = false,
                            Message = "The list does not contain the value '" + defaultValue + "'."
                        };
                }
                else if (def.IsInt)
                {
                    int id = (int)(defaultValue = (int)def);

                    if (id < 0 || id >= values.Count)
                        return new CompilerError()
                        {
                            Cause = new IndexOutOfRangeException(),
                            FilePath = json.Path,
                            IsWarning = false,
                            Message = "The index " + id + " is either below 0 or out of the bounds of the list."
                        };
                }
                else
                    return new CompilerError()
                    {
                        Cause = new InvalidCastException(),
                        FilePath = json.Path,
                        IsWarning = false,
                        Message = "Key 'default' is a " + def.GetJsonType() + ", not an int or a string."
                    };
            }

            return null;
        }
    }
}
