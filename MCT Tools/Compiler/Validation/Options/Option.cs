using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT.Tools.Compiler.Validation.Options
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

        /// <summary>
        /// Creates a new instance of the <see cref="Option" /> class.
        /// </summary>
        /// <param name="mc"><see cref="CompilerPhase.Compiler" /></param>
        protected Option(ModCompiler mc)
            : base(mc)
        {

        }

        /// <summary>
        /// Creates a new option object.
        /// </summary>
        /// <param name="mc">The <see cref="ModCompiler" /> that is compiling the <see cref="Option" />.</param>
        /// <param name="json">The JSON useto create the objects. (ModOptions.json)</param>
        /// <returns>A tuple containing the option, and a collection of errors.</returns>
        public static Tuple<Option, IEnumerable<CompilerError>> NewOption(ModCompiler mc, JsonFile json)
        {
            Dictionary<string, Func<Option>> Options = new Dictionary<string, Func<Option>>();

            // only lower-case
            Options.Add("string",     () => new StringOption    (mc));
            Options.Add("integer",    () => new IntegerOption   (mc));
            Options.Add("float",      () => new FloatOption     (mc));
            Options.Add("keybinding", () => new KeybindingOption(mc));
            Options.Add("list",       () => new ListOption      (mc));
            Options.Add("boolean",    () => new BoolOption      (mc));
            Options.Add("dynamic",    () => new DynamicOption   (mc));

            Option ret = null;
            List<CompilerError> errors = new List<CompilerError>();

            if (!json.Json.Has("type"))
                errors.Add(new CompilerError(mc.building)
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
                    errors.Add(new CompilerError(mc.building)
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
