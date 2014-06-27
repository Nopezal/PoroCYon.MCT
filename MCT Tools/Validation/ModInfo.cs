using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LitJson;

namespace PoroCYon.MCT.Tools.Validation
{
    /// <summary>
    /// A ModInfo JSON file (ModInfo.json)
    /// </summary>
    public class ModInfo : ValidatorObject
    {
        internal readonly static string[] EmptyStringArr = new string[0];

#pragma warning disable 1591
        // internal
        public string internalName = null;
        public bool includePDB = false;
        public bool warnOnReload = false;
        public string[] modReferences = EmptyStringArr;
        public string[] dllReferences = EmptyStringArr;
        public bool MSBuild = false;
        public string msBuildFile = null;
        public bool extractDLL = false;

        // informative
        public string displayName = null;
        public string author = "<unknown>";
        public Version version = new Version(1, 0);
        public string info = String.Empty;

        // new compiler stuff
        public string language = null;
        public bool compress = true;
        public bool includeSource = false;
        public bool validate = true; // even if false, ModInfo will always be validated
        public bool check = true;
        public int warningLevel = 4;
        public string[] ignore = EmptyStringArr;
#pragma warning restore 1591

        /// <summary>
        /// Create &amp; validate a JSON file.
        /// </summary>
        /// <param name="json">The json to validate</param>
        /// <returns>A collection of all validation errors.</returns>
        public override IEnumerable<CompilerError> CreateAndValidate(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            AddIfNotNull(SetJsonValue(json, "internalName", ref internalName), errors);
            AddIfNotNull(SetJsonValue(json, "includePDB", ref includePDB, false), errors);
            AddIfNotNull(SetJsonValue(json, "warnOnReload", ref warnOnReload, false), errors);
            AddIfNotNull(SetJsonValue(json, "extractDLL", ref extractDLL, false), errors);

            AddIfNotNull(SetJsonValue(json, "modReferences", ref modReferences, EmptyStringArr), errors);
            for (int i = 0; i < modReferences.Length; i++)
                if (!ModCompiler.modDict.ContainsKey(modReferences[i]))
                    errors.Add(new CompilerError()
                    {
                        Cause = new FileNotFoundException(),
                        FilePath = json.Path,
                        IsWarning = false,
                        Message = "'modReferences[" + i + "]': could not find mod '" + modReferences[i] + "'."
                    });

            AddIfNotNull(SetJsonValue(json, "dllReferences", ref dllReferences, EmptyStringArr), errors);
            for (int i = 0; i < dllReferences.Length; i++)
                try
                {
                    try
                    {
                        Assembly.LoadFrom(dllReferences[i]);
                    }
                    catch
                    {
                        dllReferences[i] = Path.GetDirectoryName(json.Path) + "\\References\\" + dllReferences[i];

                        Assembly.LoadFrom(dllReferences[i]);
                    }
                }
                catch (Exception e)
                {
                    errors.Add(new CompilerError()
                    {
                        Cause = e,
                        FilePath = json.Path,
                        IsWarning = false,
                        Message = "'dllReferences[" + i + "]': Could not find reference '" + dllReferences[i] + "'."
                    });
                }

            AddIfNotNull(SetJsonValue(json, "MSBuild", ref MSBuild, false), errors);
            if (MSBuild)
            {
                AddIfNotNull(SetJsonValue(json, "msBuildFile", ref msBuildFile, Path.GetDirectoryName(json.Path)
                    + "\\" + new DirectoryInfo(Path.GetDirectoryName(json.Path)).Name + ".csproj"), errors);

                if (!File.Exists(msBuildFile))
                    if (File.Exists(ModCompiler.current.OriginPath + "\\" + msBuildFile))
                        msBuildFile = ModCompiler.current.OriginPath + "\\" + msBuildFile;
                    else
                        errors.Add(new CompilerError()
                        {
                            Cause = new FileNotFoundException(),
                            FilePath = json.Path,
                            IsWarning = false,
                            Message = "'msBuildFile': file '" + msBuildFile + "' not found."
                        });
            }

            // ---

            AddIfNotNull(SetJsonValue(json, "displayName", ref displayName, internalName), errors);
            AddIfNotNull(SetJsonValue(json, "author", ref author, "<unknown>"), errors);

            #region version
            if (json.Json.Has("version"))
            {
                JsonData ver = json.Json["version"];

                if (!ver.IsString)
                {
                    if (!ver.IsArray || ver.Count == 0)
                        errors.Add(new CompilerError()
                        {
                            Cause = new InvalidCastException(),
                            FilePath = json.Path,
                            IsWarning = false,
                            Message = "'version' is a " + json.Json.GetJsonType() + ", not a string or an int[]."
                        });

                    int[] values = new int[4] { 1, 0, 0, 0 };

                    for (int i = 0; i < Math.Min(ver.Count, 4); i++)
                        if (!ver[i].IsInt)
                            errors.Add(new CompilerError()
                            {
                                Cause = new InvalidCastException(),
                                FilePath = json.Path,
                                IsWarning = false,
                                Message = "'version[" + i + "]' is a " + json.Json.GetJsonType() + ", not an int."
                            });
                        else
                            values[i] = (int)ver[i];

                    version = new Version(values[0], values[1], values[2], values[3]);
                }
                else
                {
                    try
                    {
                        version = new Version((string)ver);
                    }
                    catch (Exception e)
                    {
                        errors.Add(new CompilerError()
                        {
                            Cause = e,
                            FilePath = json.Path,
                            IsWarning = false,
                            Message = "'version': invalid string format."
                        });
                    }
                }
            }
            #endregion

            AddIfNotNull(SetJsonValue(json, "info", ref info, "Mod " + displayName + " v" + version + " by " + author), errors);

            // ---

            AddIfNotNull(SetJsonValue(json, "language",      ref language,      "C#" ), errors);
            AddIfNotNull(SetJsonValue(json, "compress",      ref compress,      true ), errors);
            AddIfNotNull(SetJsonValue(json, "validate",      ref validate,      true ), errors);
            AddIfNotNull(SetJsonValue(json, "includeSource", ref includeSource, false), errors);
            AddIfNotNull(SetJsonValue(json, "check",         ref check,         true ), errors);
            AddIfNotNull(SetJsonValue(json, "warningLevel",  ref warningLevel,  4    ), errors);
            AddIfNotNull(SetJsonValue(json, "ignore", ref ignore, EmptyStringArr     ), errors);
            if (warningLevel < 0 || warningLevel > 4)
                errors.Add(new CompilerError()
                {
                    Cause = new ArgumentOutOfRangeException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "'warningLevel': value must be an element of [0;4]."
                });

            return errors;
        }
    }
}
