using System;
using System.Collections.Generic;
using System.Linq;
using TAPI;
using PoroCYon.MCT.Tools.Internal.Validation;

namespace PoroCYon.MCT.Tools.Internal
{
    using ModInfo = Validation.ModInfo;

    class CurrentMod
    {
        internal ModInfo info;
        internal ModOptions options;
        internal CraftGroups craftGroups;

        internal List<JsonFile> jsons = new List<JsonFile>();
        internal Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();
    }

    static class Validator
    {
        internal static Dictionary<string, string> modDict;
        internal static CurrentMod current = new CurrentMod();

        internal static List<CompilerError> ValidateJsons(List<JsonFile> jsons, Dictionary<string, byte[]> files, bool validateModInfo = true)
        {
            current = new CurrentMod();

            current.jsons = jsons;
            current.files = files;

            modDict = Mods.GetInternalNameToPathDictionary(); // dat name

            List<CompilerError> errors = new List<CompilerError>();

            JsonFile
                modInfoJson     = jsons[0],
                modOptionsJson  = jsons[1],
                craftGroupsJson = jsons[2];

            current.info = new ModInfo();
            errors.AddRange(current.info.CreateAndValidate(modInfoJson));

            current.options = new ModOptions();
            if (modOptionsJson != null)
                errors.AddRange(current.options.CreateAndValidate(modOptionsJson));

            current.craftGroups = new CraftGroups();
            if (craftGroupsJson != null)
                errors.AddRange(current.craftGroups.CreateAndValidate(modOptionsJson));

            for (int i = 2; i < errors.Count; i++)
            {
                // stuff
            }

            current = null;

            return errors;
        }
    }
}
