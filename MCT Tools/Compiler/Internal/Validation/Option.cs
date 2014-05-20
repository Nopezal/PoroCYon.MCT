using System;
using System.Collections.Generic;
using System.Linq;
using PoroCYon.MCT.Tools.Internal.Validation.Options;

namespace PoroCYon.MCT.Tools.Internal.Validation
{
    abstract class Option : ValidatorObject
    {
        public bool notify = true;
        public bool sync = true;
        public string name;
        public string type;
        public string toolTip = String.Empty;
        public string displayName = String.Empty;

        internal static Dictionary<string, Func<Option>> Options = new Dictionary<string, Func<Option>>(); // func should be paramless ctor ('() => return new MyOpt()')

        internal static Tuple<Option, List<CompilerError>> NewOption(JsonFile json)
        {
            if (Options.Count == 0)
            {
                // only lower-case
                Options.Add("string",     () => { return new StringOption    (); });
                Options.Add("integer",    () => { return new IntegerOption   (); });
                Options.Add("float",      () => { return new FloatOption     (); });
                Options.Add("keybinding", () => { return new KeybindingOption(); });
                Options.Add("list",       () => { return new ListOption      (); });
                Options.Add("boolean",    () => { return new BoolOption      (); });
                Options.Add("dynamic",    () => { return new DynamicOption   (); });
            }

            Option ret = null;
            List<CompilerError> errors = new List<CompilerError>();

            if (!json.json.Has("type"))
                errors.Add(new CompilerError()
                {
                    Cause = new KeyNotFoundException(),
                    FilePath = json.path,
                    IsWarning = false,
                    Message = "Key 'type' not found."
                });
            else
            {
                string type = ((string)json.json["type"]).ToLowerInvariant();

                if (!Options.ContainsKey(type))
                    errors.Add(new CompilerError()
                    {
                        Cause = new KeyNotFoundException(),
                        FilePath = json.path,
                        IsWarning = false,
                        Message = "Did nout found the '" + type + "' option."
                    });
                else
                {
                    Option o = Options[type]();
                    o.type = type;

                    errors.AddRange(o.CreateAndValidate(json));
                }
            }

            return new Tuple<Option, List<CompilerError>>(ret, errors);
        }

        protected abstract List<CompilerError> CreateAndValidateOverride(JsonFile json);

        internal override List<CompilerError> CreateAndValidate(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            AddIfNotNull(SetJsonValue(json, "notify",      ref notify,      true        ), errors);
            AddIfNotNull(SetJsonValue(json, "sync",        ref sync,        true        ), errors);
            AddIfNotNull(SetJsonValue(json, "name",        ref name                     ), errors);
            AddIfNotNull(SetJsonValue(json, "toolTip",     ref toolTip,     String.Empty), errors);
            AddIfNotNull(SetJsonValue(json, "displayName", ref displayName, name        ), errors);

            errors.AddRange(CreateAndValidateOverride(json));

            return errors;
        }
    }
}
