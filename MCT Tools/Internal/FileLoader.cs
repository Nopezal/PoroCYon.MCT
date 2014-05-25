using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LitJson;
using PoroCYon.MCT.Internal;

namespace PoroCYon.MCT.Tools.Internal
{
    static class FileLoader
    {
        /// <summary>
        /// Loads JSON and other files from a mod's source folder.
        /// </summary>
        /// <param name="directory">The mod's source folder.</param>
        /// <returns>A tuple containing the list of the JSON files an a list of the other files, together with all errors.</returns>
        internal static Tuple<List<JsonFile>, Dictionary<string, byte[]>, List<CompilerError>> LoadFiles(string directory)
        {
            List<CompilerError> errors = new List<CompilerError>();

            List<JsonFile> jsons = new List<JsonFile>();
            Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();

            JsonFile
                modInfo = null,
                modOptions = null,
                craftGroups = null;

            foreach (string s in Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories))
            {
                byte[] fileBin = null;
                string fileStr = null;

                try
                {
                    fileBin = File.ReadAllBytes(s);
                    fileStr = File.ReadAllText (s);
                }
                catch (IOException e)
                {
                    errors.Add(new CompilerError()
                    {
                        Cause = e,
                        FilePath = s,
                        IsWarning = true,
                        Message = "A file handle is open. Please close the file in the program which owns the file handle.\n"
                            + "This file will not be included in the build, but the build itself will continue."
                    });

                    continue;
                }

                string
                    fileName = Path.GetFileName(s),
                    relativeFileName = s.Substring(directory.Length + 1);

                if (s.EndsWith(".json"))
                {
                    JsonFile current = null;

                    try
                    {
                        current = new JsonFile(s, JsonMapper.ToObject(fileStr));
                    }
                    catch (Exception e)
                    {
                        errors.Add(new CompilerError()
                        {
                            IsWarning = false,
                            Cause = e,
                            Message = "Invalid JSON file: '" + relativeFileName + "'."
                        });
                    }

                    if (relativeFileName == "ModInfo.json")
                        modInfo = current;
                    else if (relativeFileName == "ModOptions.json")
                        modOptions = current;
                    else if (relativeFileName == "CraftGroups.json")
                        craftGroups = current;
                    else
                        jsons.Add(current);
                }
                else
                    files.Add(relativeFileName, fileBin);
            }

            if (modInfo == null)
            {
                modInfo = new JsonFile(String.Empty, JsonMapper.ToObject(CommonToolUtilities.CreateDefaultModInfo(Path.GetDirectoryName(directory))));

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
