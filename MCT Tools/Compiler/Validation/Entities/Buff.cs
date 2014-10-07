using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TAPI;

namespace PoroCYon.MCT.Tools.Compiler.Validation.Entities
{
    /// <summary>
    /// A buff (positive, negative, weapon or pet).
    /// </summary>
    public class Buff(ModCompiler mc) : ValidatorObject(mc)
    {
#pragma warning disable 1591
        public string code;
        public string displayName;
        public string internalName;

        internal bool hasCode = false;

        public string texture = String.Empty;

        public string tip;
        public bool debuff, vanityPet, lightPet, enchantment, noTimer;
#pragma warning restore 1591

        IEnumerable<CompilerError> CreateAndValidateBase(JsonFile json, string baseFolder, string baseType)
        {
            List<CompilerError> errors = new List<CompilerError>();

            internalName = Building.Info.internalName + ":" + Path.GetFileNameWithoutExtension(json.Path);

            AddIfNotNull(SetJsonValue(json, "displayName", ref displayName, internalName), errors);

            hasCode = json.Json.Has("code");
            AddIfNotNull(SetJsonValue(json, "code", ref code, baseType + "." + Defs.ParseName(internalName)), errors);
            if (code != null)
            {
                if (code.Contains(':'))
                    code = code.Replace(':', '.');
                else
                    code = Building.Info.internalName + "." + code;
            }

            string r = Path.ChangeExtension(json.Path.Substring(Building.OriginPath.Length + 1).Replace('\\', '/'), null);
            AddIfNotNull(SetJsonValue(json, "texture", ref texture, r), errors);
            if (!Building.files.ContainsKey(texture + ".png"))
                errors.Add(new CompilerError(Building)
                {
                    Cause = new FileNotFoundException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "Could not find item texture '" + texture + ".png'."
                });

            return errors;
        }

        /// <summary>
        /// Create &amp; validate a JSON file.
        /// </summary>
        /// <param name="json">The json to validate</param>
        /// <returns>A collection of all validation errors.</returns>
        public override IEnumerable<CompilerError> CreateAndValidate(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            errors.AddRange(CreateAndValidateBase(json, "Buffs", "Buffs"));

            AddIfNotNull(SetJsonValue(json, "tip",         ref tip        ), errors);

            AddIfNotNull(SetJsonValue(json, "debuff",      ref debuff,      false), errors);
            AddIfNotNull(SetJsonValue(json, "vanityPet",   ref vanityPet,   false), errors);
            AddIfNotNull(SetJsonValue(json, "lightPet",    ref lightPet,    false), errors);
            AddIfNotNull(SetJsonValue(json, "enchantment", ref enchantment, false), errors);
            AddIfNotNull(SetJsonValue(json, "noTimer",     ref noTimer,     false), errors);

            return errors;
        }
    }
}
