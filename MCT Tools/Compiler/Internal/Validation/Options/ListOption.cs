using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT.Tools.Internal.Validation.Options
{
    class ListOption : Option
    {
        public List<object> values = new List<object>();
        public object defaultValue;

        protected override List<CompilerError> CreateAndValidateOverride(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            if (!json.json.Has("list"))
                errors.Add(new CompilerError()
                {
                    Cause = new KeyNotFoundException(),
                    FilePath = json.path,
                    IsWarning = false,
                    Message = "Key 'list' not found."
                });
            else
            {
                var list = json.json["list"];

                for (int i = 0; i < list.Count; i++)
                    values.Add((string)list[i]);
            }

            SetDefault(json);

            return errors;
        }

        protected CompilerError SetDefault(JsonFile json)
        {
            if (json.json.Has("default"))
            {
                var def = json.json["default"];

                if (def.IsString)
                {
                    if (!values.Contains(defaultValue = (string)def))
                        return new CompilerError()
                        {
                            Cause = new KeyNotFoundException(),
                            FilePath = json.path,
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
                            FilePath = json.path,
                            IsWarning = false,
                            Message = "The index " + id + " is either below 0 or out of the bounds of the list."
                        };
                }
                else
                    return new CompilerError()
                    {
                        Cause = new InvalidCastException(),
                        FilePath = json.path,
                        IsWarning = false,
                        Message = "Key 'default' is a " + def.GetJsonType() + ", not an int or a string."
                    };
            }

            return null;
        }
    }
}
