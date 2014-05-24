using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT.Tools.Validation.Options
{
    /// <summary>
    /// The base class for all mod options
    /// </summary>
    public abstract class Option : ValidatorObject
    {
#pragma warning disable 1591
        public bool notify = true;
        public bool sync = true;
        public string name;
        public string type;
        public string toolTip = String.Empty;
        public string displayName = String.Empty;
#pragma warning restore 1591

        internal static Dictionary<string, Func<Option>> Options = new Dictionary<string, Func<Option>>(); // func should be paramless ctor ('() => return new MyOpt()')

        /// <summary>
        /// Creates a new option object.
        /// </summary>
        /// <param name="json">The JSON useto create the objects. (ModOptions.json)</param>
        /// <returns>A tuple containing the option, and a collection of errors.</returns>
        public static Tuple<Option, IEnumerable<CompilerError>> NewOption(JsonFile json)
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

            if (!json.Json.Has("type"))
                errors.Add(new CompilerError()
                {
                    Cause = new KeyNotFoundException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "Key 'type' not found."
                });
            else
            {
                string type = ((string)json.Json["type"]).ToLowerInvariant();

                if (!Options.ContainsKey(type))
                    errors.Add(new CompilerError()
                    {
                        Cause = new KeyNotFoundException(),
                        FilePath = json.Path,
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

            return new Tuple<Option, IEnumerable<CompilerError>>(ret, errors);
        }

        /// <summary>
        /// Create &amp; validate subclass-only fields.
        /// </summary>
        /// <param name="json">The json to validate</param>
        /// <returns>A collection of all validation errors.</returns>
        protected abstract IEnumerable<CompilerError> CreateAndValidateOverride(JsonFile json);

        /// <summary>
        /// Create &amp; validate a JSON file.
        /// </summary>
        /// <param name="json">The json to validate</param>
        /// <returns>A collection of all validation errors.</returns>
        public override IEnumerable<CompilerError> CreateAndValidate(JsonFile json)
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
