﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TAPI;

namespace PoroCYon.MCT.Tools.Compiler.Validation.Entities
{
    /// <summary>
    /// An object that helps with the validation of an entity.
    /// </summary>
    public abstract class EntityValidator : ValidatorObject
    {
#pragma warning disable 1591
        public string code;
        public string displayName;
        public string internalName;

        // appearance
        public string texture = String.Empty;
        public int[] size;
        public float scale = 1f;
        public int[] colour = new int[4] { 255, 255, 255, 0 };
#pragma warning restore 1591

        /// <summary>
        /// Creates a new instance of the <see cref="EntityValidator" /> class.
        /// </summary>
        /// <param name="mc"><see cref="CompilerPhase.Compiler" /></param>
        protected EntityValidator(ModCompiler mc)
            : base(mc)
        {

        }

        internal bool codeDefaultValue = false;

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

            AddIfNotNull(SetJsonValue(json, "code", ref code, null), errors);

            if (code == null)
            {
                code = Defs.ParseName(Building.Info.internalName) + "." + baseType + "." + Defs.ParseName(internalName);

                codeDefaultValue = true;
            }
            else if (code.Contains(':'))
                code = code.Replace(':', '.');
            else
                code = Defs.ParseName(Building.Info.internalName) + "." + code;

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

            AddIfNotNull(SetJsonValue(json, "size", ref size, true, new int[2]), errors);
            if (size != null && size.Length != 2)
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
                    Message = "Invalid item size value."
                });

            AddIfNotNull(SetJsonValue(json, "scale", ref scale, 1f), errors);

            AddIfNotNull(SetJsonValue(json, "color", ref colour, true, new int[3] { 255, 255, 255 }), errors);
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
