using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LitJson;

namespace PoroCYon.MCT.Tools.Internal.Validation
{
    class ModInfo
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

            #region internalName
            if (!json.json.Has("internalName"))
                errors.Add(new CompilerError()
                {
                    Cause = new KeyNotFoundException(),
                    FilePath = json.path,
                    IsWarning = false,
                    Message = "Could not find key 'internalName' in ModInfo.json file " + json.path
                });
            else if (!json.json["internalName"].IsString)
                errors.Add(new CompilerError()
                {
                    Cause = new InvalidCastException(),
                    FilePath = json.path,
                    IsWarning = false,
                    Message = "'internalName' in ModInfo.json file is a " + json.json.GetJsonType() + ", not a string."
                });
            else
                ret.internalName = (string)json.json["internalName"];
            #endregion

            #region includePDB
            if (json.json.Has("includePDB"))
            {
                if (!json.json["includePDB"].IsBoolean)
                    errors.Add(new CompilerError()
                    {
                        Cause = new InvalidCastException(),
                        FilePath = json.path,
                        IsWarning = false,
                        Message = "'includePDB' in ModInfo.json file is a " + json.json.GetJsonType() + ", not a bool."
                    });
                else
                    ret.includePDB = (bool)json.json["includePDB"];
            }
            #endregion

            #region warnOnReload
            if (json.json.Has("warnOnReload"))
            {
                if (!json.json["warnOnReload"].IsBoolean)
                    errors.Add(new CompilerError()
                    {
                        Cause = new InvalidCastException(),
                        FilePath = json.path,
                        IsWarning = false,
                        Message = "'warnOnReload' in ModInfo.json file is a " + json.json.GetJsonType() + ", not a bool."
                    });
                else
                    ret.warnOnReload = (bool)json.json["warnOnReload"];
            }
            #endregion

            #region modReferences
            if (json.json.Has("modReferences"))
            {
                if (!json.json["modReferences"].IsArray)
                    errors.Add(new CompilerError()
                    {
                        Cause = new InvalidCastException(),
                        FilePath = json.path,
                        IsWarning = false,
                        Message = "'modReferences' in ModInfo.json file is a " + json.json.GetJsonType() + ", not an array."
                    });
                else
                {
                    List<string> refs = new List<string>();

                    for (int i = 0; i < json.json["modReferences"].Count; i++)
                    {
                        JsonData @ref = json.json["modReferences"][i];

                        if (!@ref.IsString)
                            errors.Add(new CompilerError()
                            {
                                Cause = new InvalidCastException(),
                                FilePath = json.path,
                                IsWarning = false,
                                Message = "'modReferences[" + i + "]' in ModInfo.json file is a " + json.json.GetJsonType() + ", not a string."
                            });
                        else
                        {
                            if (Validator.modDict.ContainsKey((string)@ref))
                                refs.Add((string)@ref);
                            else
                                errors.Add(new CompilerError()
                                {
                                    Cause = new FileNotFoundException(),
                                    FilePath = json.path,
                                    IsWarning = false,
                                    Message = "'modReferences[" + i + "]' in ModInfo.json file: could not find mod '" + (string)@ref + "'."
                                });
                        }
                    }

                    ret.modReferences = refs.ToArray();
                }
            }
            #endregion

            #region dllReferences
            if (json.json.Has("dllReferences"))
            {
                if (!json.json["dllReferences"].IsArray)
                    errors.Add(new CompilerError()
                    {
                        Cause = new InvalidCastException(),
                        FilePath = json.path,
                        IsWarning = false,
                        Message = "'dllReferences' in ModInfo.json file is a " + json.json.GetJsonType() + ", not an array."
                    });
                else
                {
                    List<string> refs = new List<string>();

                    for (int i = 0; i < json.json["dllReferences"].Count; i++)
                    {
                        JsonData @ref = json.json["dllReferences"][i];

                        if (!@ref.IsString)
                            errors.Add(new CompilerError()
                            {
                                Cause = new InvalidCastException(),
                                FilePath = json.path,
                                IsWarning = false,
                                Message = "'dllReferences[" + i + "]' in ModInfo.json file is a " + json.json.GetJsonType() + ", not a string."
                            });
                        else
                        {
                            string refPath = (string)@ref;

                            if (!File.Exists(refPath))
                                refPath = Path.GetDirectoryName(json.path) + "\\References";

                            if (!File.Exists(refPath))
                            {
                                errors.Add(new CompilerError()
                                {
                                    Cause = new InvalidCastException(),
                                    FilePath = json.path,
                                    IsWarning = false,
                                    Message = "dllReferences in ModInfo.json file: Could not find reference '" + (string)@ref + "'."
                                });
                            }
                            else
                                refs.Add((string)@ref);
                        }
                    }

                    ret.dllReferences = refs.ToArray();
                }
            }
            #endregion

            #region MSBuild
            if (json.json.Has("MSBuild"))
            {
                if (!json.json["MSBuild"].IsBoolean)
                    errors.Add(new CompilerError()
                    {
                        Cause = new InvalidCastException(),
                        FilePath = json.path,
                        IsWarning = false,
                        Message = "'MSBuild' in ModInfo.json file is a " + json.json.GetJsonType() + ", not a bool."
                    });
                else
                    ret.MSBuild = (bool)json.json["MSBuild"];
            }
            #endregion

            #region msBuildFile
            if (json.json.Has("msBuildFile"))
            {
                if (!json.json["msBuildFile"].IsString)
                    errors.Add(new CompilerError()
                    {
                        Cause = new InvalidCastException(),
                        FilePath = json.path,
                        IsWarning = false,
                        Message = "'msBuildFile' in ModInfo.json file is a " + json.json.GetJsonType() + ", not a string."
                    });
                else if (!File.Exists(Path.GetDirectoryName(json.path) + "\\" + (string)json.json["msBuildFile"]))
                    errors.Add(new CompilerError()
                    {
                        Cause = new FileNotFoundException(),
                        FilePath = json.path,
                        IsWarning = false,
                        Message = "'msBuildFile' in ModInfo.json file: could not find MSBuild script '" + (string)json.json["msBuildFile"] + "'."
                    });
                else
                    ret.msBuildFile = (string)json.json["msBuildFile"];
            }
            #endregion

            #region displayName
            if (json.json.Has("displayName"))
            {
                if (!json.json["displayName"].IsString)
                    errors.Add(new CompilerError()
                    {
                        Cause = new InvalidCastException(),
                        FilePath = json.path,
                        IsWarning = false,
                        Message = "'displayName' in ModInfo.json file is a " + json.json.GetJsonType() + ", not a string."
                    });
                else
                    ret.displayName = (string)json.json["displayName"];
            }
            else
                ret.displayName = ret.internalName;
            #endregion

            #region author
            if (json.json.Has("author"))
            {
                if (!json.json["author"].IsString)
                    errors.Add(new CompilerError()
                    {
                        Cause = new InvalidCastException(),
                        FilePath = json.path,
                        IsWarning = false,
                        Message = "'author' in ModInfo.json file is a " + json.json.GetJsonType() + ", not a string."
                    });
                else
                    ret.author = (string)json.json["author"];
            }
            else
                ret.author = "<unknown>";
            #endregion

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
                            Message = "'version' in ModInfo.json file is a " + json.json.GetJsonType() + ", not a string or an int[]."
                        });

                    int[] values = new int[4] { 1, 0, 0, 0 };

                    for (int i = 0; i < Math.Min(ver.Count, 4); i++)
                        if (!ver[i].IsInt)
                            errors.Add(new CompilerError()
                            {
                                Cause = new InvalidCastException(),
                                FilePath = json.path,
                                IsWarning = false,
                                Message = "'version[" + i + "]' in ModInfo.json file is a " + json.json.GetJsonType() + ", not an int."
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
                            Message = "'version' in ModInfo.json file: invalid string format."
                        });
                    }
                }
            }
            #endregion

            #region info
            if (json.json.Has("info"))
            {
                if (!json.json["info"].IsString)
                    errors.Add(new CompilerError()
                    {
                        Cause = new InvalidCastException(),
                        FilePath = json.path,
                        IsWarning = false,
                        Message = "'info' in ModInfo.json file is a " + json.json.GetJsonType() + ", not a string."
                    });
                else
                    ret.info = (string)json.json["info"];
            }
            else
                ret.info = "Mod " + ret.displayName + " v" + ret.version + " by " + ret.author;
            #endregion

            /* language, compress, validate, check */

            return new Tuple<ModInfo, List<CompilerError>>(ret, errors);
        }
    }
}
