﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LitJson;
using PoroCYon.MCT.Internal;

namespace PoroCYon.MCT.Tools.Internal
{
    static class Extractor
    {
        /// <summary>
        /// Extracts JSON and other files from a managed assembly's embedded resources.
        /// </summary>
        /// <param name="asm">The assembly to extract the files from.</param>
        /// <returns>A tuple containing the list of the JSON files an a list of the other files, together with all errors.</returns>
        internal static Tuple<List<JsonFile>, Dictionary<string, byte[]>, List<CompilerError>> ExtractData(Assembly asm)
        {
            List<CompilerError> errors = new List<CompilerError>();

            List<JsonFile> jsons = new List<JsonFile>();
            Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();

            JsonFile
                modInfo = null,
                modOptions = null,
                craftGroups = null;

            string asmName = asm.GetName().Name;

            foreach (string res in asm.GetManifestResourceNames())
                using (Stream s = asm.GetManifestResourceStream(res))
                {
                    string name = res;

                    if (name.Length > asmName.Length + 1 && name.StartsWith(asmName + "."))
                        name = name.Substring(asmName.Length + 1);

                    if (name.EndsWith(".json"))
                    {
                        JsonFile current = null;

                        try
                        {
                            string fileName = res;
                            int index;

                            while ((index = fileName.IndexOf('.')) != fileName.LastIndexOf('.') && index != -1) // keep extension
                                fileName = fileName.Remove(index, 1).Insert(index, Path.DirectorySeparatorChar.ToString());

                            current = new JsonFile(fileName, JsonMapper.ToObject(new StreamReader(s)));
                        }
                        catch (Exception e)
                        {
                            errors.Add(new CompilerError()
                            {
                                IsWarning = false,
                                Cause = e,
                                Message = "Invalid JSON file: '" + res + "'."
                            });
                        }

                        if (name == "ModInfo.json")
                            modInfo = current;
                        else if (name == "ModOptions.json")
                            modOptions = current;
                        else if (name == "CraftGroups.json")
                            craftGroups = current;
                        else
                            jsons.Add(current);
                    }
                    else
                    {
                        MemoryStream ms = new MemoryStream();
                        s.CopyTo(ms);

                        ms.Seek(0L, SeekOrigin.Begin);

                        files.Add(name, ms.ToArray());
                    }
                }

            if (modInfo == null)
            {
                modInfo = new JsonFile(String.Empty, JsonMapper.ToObject(CommonToolUtilities.CreateDefaultModInfo(asmName)));

                errors.Add(new CompilerError()
                {
                    IsWarning = true,
                    Cause = new FileNotFoundException("Could not find ModInfo.json."),
                    Message = "Could not find ModInfo.json, using default..."
                });
            }

            jsons.Insert(0, modInfo);
            jsons.Insert(1, modOptions); // check if it's null later
            jsons.Insert(2, craftGroups); // same here

            return new Tuple<List<JsonFile>, Dictionary<string, byte[]>, List<CompilerError>>(jsons, files, errors);
        }
    }
}
