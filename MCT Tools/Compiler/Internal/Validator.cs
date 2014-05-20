﻿using System;
using System.Collections.Generic;
using System.Linq;
using TAPI;
using PoroCYon.MCT.Tools.Internal.Validation;

namespace PoroCYon.MCT.Tools.Internal
{
    using ModInfo = Validation.ModInfo;

    static class Validator
    {
        internal static Dictionary<string, string> modDict;

        internal static List<CompilerError> ValidateJsons(List<JsonFile> jsons, bool validateModInfo = true)
        {
            modDict = Mods.GetInternalNameToPathDictionary(); // dat name

            List<CompilerError> errors = new List<CompilerError>();

            JsonFile
                modInfoJson = jsons[0],
                modOptionsJson = jsons[1];

            ModInfo modInfo = new ModInfo();
            errors.AddRange(modInfo.CreateAndValidate(modInfoJson));

            if (modOptionsJson != null)
            {
                ModOptions modOptions = new ModOptions();
                errors.AddRange(modOptions.CreateAndValidate(modOptionsJson));
            }

            for (int i = 2; i < errors.Count; i++)
            {
                // stuff
            }

            return errors;
        }
    }
}
