using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TAPI;

namespace PoroCYon.MCT.Tools.Compiler.Validation.Entities
{
    /// <summary>
    /// An object that helps with the validation of an entity.
    /// </summary>
    public abstract class EntityValidator(ModCompiler mc) : ValidatorObject(mc)
    {
#pragma warning disable 1591
        public string code;
        public string displayName;
        public string internalName;

        internal bool hasCode = false;

        // appearance
        public string texture = String.Empty;
        public int[] size = new int[2] { 16, 16 };
        public float scale = 1f;
        public int[] colour = new int[4] { 255, 255, 255, 0 };
#pragma warning restore 1591

        /// <summary>
        /// Base class field validation.
        /// </summary>
        /// <param name="json">The JSON file to validate.</param>
        /// <param name="baseFolder">Used for the 'texture' key ({baseFolder}/{fileName}.png)</param>
        /// <param name="baseType">Used for the 'code' key ({internalName}.{basetype}.{fileName})</param>
        /// <returns>A collection containing all (possible) errors.</returns>
        protected IEnumerable<CompilerError> CreateAndValidateBase(JsonFile json, string baseFolder, string baseType)
        {
            List<CompilerError> errors = new List<CompilerError>();

            internalName = Building.Info.internalName + ":" + Path.GetFileNameWithoutExtension(json.Path);

            AddIfNotNull(SetJsonValue(json, "displayName", ref displayName, internalName), errors);

            hasCode = json.Json.Has("code");
            AddIfNotNull(SetJsonValue(json, "code", ref code, baseType + "." + Defs.ParseName(internalName)), errors);
            if (code.Contains(':'))
                code = code.Replace(':', '.');
            else
                code = Building.Info.internalName + "." + code;

            AddIfNotNull(SetJsonValue(json, "texture", ref texture, baseFolder + "/" + json.Path), errors);
            if (!Building.files.ContainsKey(texture + ".png"))
                errors.Add(new CompilerError(Building)
                {
                    Cause = new FileNotFoundException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "Could not find item texture '" + texture + ".png'."
                });

            AddIfNotNull(SetJsonValue(json, "size", ref size, new int[2] { -1, -1 }), errors);
            if (size.Length != 2)
            {
                errors.Add(new CompilerError(Building)
                {
                    Cause = new IndexOutOfRangeException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "'size' should be an array of ints with length 2."
                });

                size = new int[2] { 16, 16 };
            }
            AddIfNotNull(SetJsonValue(json, "width" , ref size[0], size[0]), errors);
            AddIfNotNull(SetJsonValue(json, "height", ref size[1], size[1]), errors);
            if (size[0] <= 0 || size[1] <= 0)
                errors.Add(new CompilerError(Building)
                {
                    Cause = new ArgumentOutOfRangeException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "Invalid item size values -or- size values not set."
                });

            AddIfNotNull(SetJsonValue(json, "scale", ref scale, 1f), errors);

            AddIfNotNull(SetJsonValue(json, "color", ref colour, new int[3] { 255, 255, 255 }), errors);
            if (colour.Length < 3 || colour.Length > 4)
                errors.Add(new CompilerError(Building)
                {
                    Cause = new IndexOutOfRangeException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "'color' should be an array of ints with length 3 or 4."
                });
            for (int i = 0; i < colour.Length; i++)
                if (colour[i] < 0 || colour[i] > 255)
                    errors.Add(new CompilerError(Building)
                    {
                        Cause = new ArgumentOutOfRangeException(),
                        FilePath = json.Path,
                        IsWarning = false,
                        Message = "'color[" + i + "] cannot be converted to a color channel. The value must be an element of [0;255]."
                    });

            return errors;
        }
    }
}
