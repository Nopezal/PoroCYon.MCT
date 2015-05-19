using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using LitJson;
using PoroCYon.Extensions;
using PoroCYon.MCT.Internal;
using PoroCYon.MCT.Tools.Compiler;
using TAPI;

namespace PoroCYon.MCT.Tools.Internal.Compiler
{
	class FileLoader : CompilerPhase
    {
        readonly static string
            JSON_EXT = ".json",
            DOT = ".",

            MODINFO = "ModInfo.json",
            MODOPT = "ModOptions.json",
            CGROUP = "CraftGroups.json",

            L_FILE = "Loaded file ",
            P_IS = ", path is ";

        public FileLoader(ModCompiler mc)
            : base(mc)
        {

        }

        /// <summary>
        /// Loads JSON and other files from a mod's source folder.
        /// </summary>
        /// <param name="directory">The mod's source folder.</param>
        /// <returns>A tuple containing the list of the JSON files an a list of the other files, together with all errors.</returns>
        internal Tuple<List<JsonFile>, Dictionary<string, byte[]>, List<CompilerError>> LoadFiles(string directory)
        {
            List<CompilerError> errors = new List<CompilerError>();

            List<JsonFile> jsons = new List<JsonFile>();
            Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();

            JsonFile
                modInfo     = new JsonFile(),
                modOptions  = new JsonFile(),
                craftGroups = new JsonFile();

            foreach (string s in Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories))
            {
                if (Path.GetDirectoryName(s).ToLowerInvariant().EndsWith(".sln.ide")) // roslyn temp directory, probably has open file handles
                    continue;

                byte[] fileBin = null;
                string fileStr = null;

                try
                {
                    fileBin = File.ReadAllBytes(s);
                    fileStr = File.ReadAllText (s);
                }
                catch (IOException e)
                {
                    errors.Add(new CompilerError(Building)
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
                    fileName         = Path.GetFileName(s),
                    relativeFileName = s.Substring(directory.Length + 1).Replace('\\', '/'),
                    relativeFileNameExt = Path.GetExtension(relativeFileName).ToLowerInvariant();

                relativeFileName = relativeFileName.Remove(relativeFileName.Length - relativeFileNameExt.Length) + relativeFileNameExt;

                if (s.ToLowerInvariant().EndsWith(JSON_EXT))
                {
                    JsonFile current = new JsonFile();

                    try
                    {
                        current = new JsonFile(s, JsonMapper.ToObject(fileStr));
                    }
                    catch (Exception e)
                    {
                        errors.Add(new CompilerError(Building)
                        {
                            IsWarning = false,
                            Cause = e,
                            FilePath = relativeFileName,
                            Message = "Invalid JSON file: '" + relativeFileName + "'."
                        });
                    }

                    if (relativeFileName == MODINFO)
                        modInfo = current;
                    else if (relativeFileName == MODOPT)
                        modOptions = current;
                    else if (relativeFileName == CGROUP)
                        craftGroups = current;
                    else
                        jsons.Add(current);
                }

                if (relativeFileName != MODINFO)
                    files.Add(Path.ChangeExtension(relativeFileName, Path.GetExtension(relativeFileName).ToLowerInvariant()), fileBin);

                Compiler.Log(L_FILE + fileName + P_IS + relativeFileName + DOT, MessageImportance.Low);
            }

            if (modInfo.IsEmpty)
            {
                modInfo = new JsonFile(String.Empty, ModCompile.GenerateBlankModInfo(Path.GetDirectoryName(directory)));

                errors.Add(new CompilerError(Building)
                {
                    IsWarning = true,
                    Cause = new FileNotFoundException("Could not find ModInfo.json."),
                    Message = "Could not find ModInfo.json, using default..."
                });
            }

            jsons.Insert(0, modInfo    );
            jsons.Insert(1, modOptions ); // check if it's null later
            jsons.Insert(2, craftGroups); // same here

            return new Tuple<List<JsonFile>, Dictionary<string, byte[]>, List<CompilerError>>(jsons, files, errors);
        }
    }
}
