using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public bool validate = true;
        public bool check = true;

        ModInfo() { }

        internal static Tuple<ModInfo, List<CompilerError>> CreateAndValidate(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            ModInfo ret = new ModInfo();

            AddIfNotNull(SetJsonValue(json, "internalName", ref ret.internalName), errors);
            AddIfNotNull(SetJsonValue(json, "includePDB", ref ret.includePDB, false), errors);
            AddIfNotNull(SetJsonValue(json, "warnOnReload", ref ret.warnOnReload, false), errors);

            AddIfNotNull(SetJsonValue(json, "modReferences", ref ret.modReferences, EmptyStringArr), errors);
            for (int i = 0; i < ret.modReferences.Length; i++)
                if (!Validator.modDict.ContainsKey(ret.modReferences[i]))
                    errors.Add(new CompilerError()
                    {
                        Cause = new FileNotFoundException(),
                        FilePath = json.path,
                        IsWarning = false,
                        Message = "'modReferences[" + i + "]': could not find mod '" + ret.modReferences[i] + "'."
                    });

            AddIfNotNull(SetJsonValue(json, "dllReferences", ref ret.dllReferences, EmptyStringArr), errors);
            for (int i = 0; i < ret.dllReferences.Length; i++)
            {
                if (!File.Exists(ret.dllReferences[i]))
                    ret.dllReferences[i] = Path.GetDirectoryName(json.path) + "\\References";

                if (!File.Exists(ret.dllReferences[i]))
                    errors.Add(new CompilerError()
                    {
                        Cause = new InvalidCastException(),
                        FilePath = json.path,
                        IsWarning = false,
                        Message = "'dllReferences[" + i + "]': Could not find reference '" + ret.dllReferences[i] + "'."
                    });
            }

            AddIfNotNull(SetJsonValue(json, "MSBuild", ref ret.MSBuild, false), errors);
            if (ret.MSBuild)
                AddIfNotNull(SetJsonValue(json, "msBuildFile", ref ret.msBuildFile, String.Empty), errors);

            // ---

            AddIfNotNull(SetJsonValue(json, "displayName", ref ret.displayName, ret.internalName), errors);
            AddIfNotNull(SetJsonValue(json, "author", ref ret.author, "<unknown>"), errors);

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

                    ret.version = new Version(values[0], values[1], values[2], values[3]);
                }
                else
                {
                    try
                    {
                        ret.version = new Version((string)ver);
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

            AddIfNotNull(SetJsonValue(json, "info", ref ret.info, "Mod " + ret.displayName + " v" + ret.version + " by " + ret.author), errors);

            // ---

            AddIfNotNull(SetJsonValue(json, "language", ref ret.language, "C#"), errors);
            AddIfNotNull(SetJsonValue(json, "compress", ref ret.compress, true), errors);
            AddIfNotNull(SetJsonValue(json, "validate", ref ret.validate, true), errors);
            AddIfNotNull(SetJsonValue(json, "check",    ref ret.check,    true), errors);

            return new Tuple<ModInfo, List<CompilerError>>(ret, errors);
        }
    }
}
