﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LitJson;

namespace PoroCYon.MCT.Tools.Internal.Validation
{
    class ModInfo : ValidatorObject
    {
        internal readonly static string[] EmptyStringArr = new string[0];

        // internal
        public string internalName = null;
        public bool includePDB = false;
        public bool warnOnReload = false;
        public string[] modReferences = EmptyStringArr;
        public string[] dllReferences = EmptyStringArr;
        public bool MSBuild = false;
        public string msBuildFile = null;

        // informative
        public string displayName = null;
        public string author = "<unknown>";
        public Version version = new Version(1, 0);
        public string info = String.Empty;

        // new compiler stuff
        public string language = null;
        public bool compress = true;
        public bool validate = true; // even if false, ModInfo will always be validated
        public bool check = true;

        internal override List<CompilerError> CreateAndValidate(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            AddIfNotNull(SetJsonValue(json, "internalName", ref internalName), errors);
            AddIfNotNull(SetJsonValue(json, "includePDB", ref includePDB, false), errors);
            AddIfNotNull(SetJsonValue(json, "warnOnReload", ref warnOnReload, false), errors);

            AddIfNotNull(SetJsonValue(json, "modReferences", ref modReferences, EmptyStringArr), errors);
            for (int i = 0; i < modReferences.Length; i++)
                if (!Validator.modDict.ContainsKey(modReferences[i]))
                    errors.Add(new CompilerError()
                    {
                        Cause = new FileNotFoundException(),
                        FilePath = json.path,
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
                        dllReferences[i] = Path.GetDirectoryName(json.path) + "\\References\\" + dllReferences[i];

                        Assembly.LoadFrom(dllReferences[i]);
                    }
                }
                catch (Exception e)
                {
                    errors.Add(new CompilerError()
                    {
                        Cause = e,
                        FilePath = json.path,
                        IsWarning = false,
                        Message = "'dllReferences[" + i + "]': Could not find reference '" + dllReferences[i] + "'."
                    });
                }

            AddIfNotNull(SetJsonValue(json, "MSBuild", ref MSBuild, false), errors);
            if (MSBuild)
            {
                AddIfNotNull(SetJsonValue(json, "msBuildFile", ref msBuildFile, Path.GetDirectoryName(json.path)
                    + "\\" + new DirectoryInfo(Path.GetDirectoryName(json.path)).Name + ".csproj"), errors);

                if (!File.Exists(msBuildFile))
                    errors.Add(new CompilerError()
                    {
                        Cause = new FileNotFoundException(),
                        FilePath = json.path,
                        IsWarning = false,
                        Message = "'msBuildFile': file '" + msBuildFile + "' not found."
                    });
            }

            // ---

            AddIfNotNull(SetJsonValue(json, "displayName", ref displayName, internalName), errors);
            AddIfNotNull(SetJsonValue(json, "author", ref author, "<unknown>"), errors);

            #region version
            if (json.json.Has("version"))
            {
                JsonData ver = json.json["version"];

                if (!ver.IsString)
                {
                    if (!ver.IsArray || ver.Count == 0)
                        errors.Add(new CompilerError()
                        {
                            Cause = new InvalidCastException(),
                            FilePath = json.path,
                            IsWarning = false,
                            Message = "'version' is a " + json.json.GetJsonType() + ", not a string or an int[]."
                        });

                    int[] values = new int[4] { 1, 0, 0, 0 };

                    for (int i = 0; i < Math.Min(ver.Count, 4); i++)
                        if (!ver[i].IsInt)
                            errors.Add(new CompilerError()
                            {
                                Cause = new InvalidCastException(),
                                FilePath = json.path,
                                IsWarning = false,
                                Message = "'version[" + i + "]' is a " + json.json.GetJsonType() + ", not an int."
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
                            FilePath = json.path,
                            IsWarning = false,
                            Message = "'version': invalid string format."
                        });
                    }
                }
            }
            #endregion

            AddIfNotNull(SetJsonValue(json, "info", ref info, "Mod " + displayName + " v" + version + " by " + author), errors);

            // ---

            AddIfNotNull(SetJsonValue(json, "language", ref language, "C#"), errors);
            AddIfNotNull(SetJsonValue(json, "compress", ref compress, true), errors);
            AddIfNotNull(SetJsonValue(json, "validate", ref validate, true), errors);
            AddIfNotNull(SetJsonValue(json, "check",    ref check,    true), errors);

            return errors;
        }
    }
}
