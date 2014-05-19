using System;
using System.Collections.Generic;
using System.Linq;
using TAPI;

namespace PoroCYon.MCT.Tools.Internal
{
    static class Validator
    {
        internal static Dictionary<string, string> modDict;

        internal static List<CompilerError> ValidateJsons(List<JsonFile> jsons, bool validateModInfo = true)
        {
            modDict = Mods.GetInternalNameToPathDictionary(); // dat name

            List<CompilerError> errors = new List<CompilerError>();

            JsonFile
                modInfo = jsons[0],
                modOptons = jsons[1];

            for (int i = 2; i < errors.Count; i++)
            {
                // stuff
            }

            return errors;
        }
    }
}
