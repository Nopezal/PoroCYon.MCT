using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Ionic.Zip;
using LitJson;
using TAPI;

namespace PoroCYon.MCT.Tools.Compiler.Validation
{
    /// <summary>
    /// A ModInfo JSON file (ModInfo.json)
    /// </summary>
    public class ModInfo : ValidatorObject
    {
        internal readonly static string[] EmptyStringArr = new string[0];

        static List<string> circularPath = new List<string>();

        internal bool checkCircularRefs = true;

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
		public string icon = null;

        // new compiler stuff
        public string language = null;
        public bool compress = true;
        public bool includeSource = false;
        public bool validate = true; // even if false, ModInfo will always be validated
        public bool check = true;
        public int warningLevel = 4;
        public string[] ignore = EmptyStringArr;
        public string outputName;
#pragma warning restore 1591

        ModInfo GetModInfoFromTapiMod(byte[] data)
        {
            BinBuffer bb = new BinBuffer(new BinBufferByte(data));

            uint ver = bb.ReadUInt();

            ModInfo mi = new ModInfo(Compiler) { checkCircularRefs = false };

            var err = mi.CreateAndValidate(new JsonFile(String.Empty, JsonMapper.ToObject(bb.ReadString())));

            if (!Compiler.CreateOutput(err.ToList()).Succeeded)
                return null;

            return mi;
        }
        ModInfo GetModInfoFromTapiZip(ZipFile zf)
        {
            if (zf.ContainsEntry("ModInfo.json"))
            {
                ZipEntry ze = zf["ModInfo.json"];

                using (MemoryStream ms = new MemoryStream())
                {
                    ze.Extract(ms);

                    StreamReader r = new StreamReader(ms);

                    ModInfo mi = new ModInfo(Compiler) { checkCircularRefs = false };

                    var err = mi.CreateAndValidate(new JsonFile(zf.Name, JsonMapper.ToObject(r.ReadToEnd())));

                    if (!Compiler.CreateOutput(err.ToList()).Succeeded)
                        return null;

                    return mi;
                }
            }

            if (zf.ContainsEntry("Mod.tapimod"))
                using (MemoryStream ms = new MemoryStream())
                {
                    return GetModInfoFromTapiMod(ms.ToArray());
                }

            return null;
        }
        internal ModInfo GetModInfoFromTapi(string file)
        {
            if (file.ToLowerInvariant().EndsWith(".tapi"))
                using (ZipFile zf = new ZipFile(file))
                {
                    return GetModInfoFromTapiZip(zf);
                }

            if (file.ToLowerInvariant().EndsWith(".tapimod"))
                return GetModInfoFromTapiMod(File.ReadAllBytes(file));

            return null;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ModInfo" /> class.
        /// </summary>
        /// <param name="mc"><see cref="CompilerPhase.Compiler" /></param>
        public ModInfo(ModCompiler mc)
            : base(mc)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ref">.tapi or .tapimod file.</param>
        /// <returns></returns>
        /// <remarks>Recursive.</remarks>
        bool CheckCircularModRefRec(string @ref)
        {
            ModInfo mi = GetModInfoFromTapi(@ref);

            if (mi == null)
                return false;

            if (circularPath.Contains(mi.internalName))
                return true;

            circularPath.Add(mi.internalName);

            return CheckCircularModRef(mi);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        /// <remarks>Recursive.</remarks>
        bool CheckCircularModRef(ModInfo info)
        {
            for (int i = 0; i < info.modReferences.Length; i++)
            {
                List<string> temp = new List<string>(circularPath);

                if (CheckCircularModRefRec(info.modReferences[i]))
                    return true;

                circularPath = temp; // reset; no circular things were found there
            }

            return false;
        }

        /// <summary>
        /// Create &amp; validate a JSON file.
        /// </summary>
        /// <param name="json">The json to validate</param>
        /// <returns>A collection of all validation errors.</returns>
        public override IEnumerable<CompilerError> CreateAndValidate(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            AddIfNotNull(SetJsonValue(json, "internalName", ref internalName), errors);
            if (internalName == "g")
                errors.Add(new CompilerError(Building)
                {
                    Cause = new ValueNotAllowedException("internalName", internalName),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "The internal mod name 'g' cannot be used, as it is reserved as the prefix for crafting groups."
                });

            AddIfNotNull(SetJsonValue(json, "includePDB", ref includePDB, false), errors);
            AddIfNotNull(SetJsonValue(json, "warnOnReload", ref warnOnReload, false), errors);
            AddIfNotNull(SetJsonValue(json, "extractDLL", ref extractDLL, false), errors);

            AddIfNotNull(SetJsonValue(json, "modReferences", ref modReferences, EmptyStringArr), errors);
            for (int i = 0; i < modReferences.Length; i++)
            {
                if (!Compiler.modDict.ContainsKey(modReferences[i]))
                {
                    string d = Compiler.FindSourceFolderFromInternalName(modReferences[i]);

                    // check circular references (and other things) first......
                    ModInfo mi = new ModInfo(Compiler);
                    var err = mi.CreateAndValidate(new JsonFile(d + "\\ModInfo.json", JsonMapper.ToObject(File.ReadAllText(d + "\\ModInfo.json"))));

                    if (!Compiler.CreateOutput(err.ToList()).Succeeded)
                    {
                        foreach (CompilerError ce in err)
                        {
                            errors.Add(new CompilerError(Building)
                            {
                                Cause = ce.Cause,
                                FilePath = ce.FilePath,
                                IsWarning = ce.IsWarning,
                                LocationInFile = ce.LocationInFile,
                                Message = "Error when parsing an uncompiled referenced mod's ModInfo file: " + ce.Message
                            });
                        }
                    }
                    else
                    {
                        if (d == null || !Directory.Exists(d))
                        {
                            errors.Add(new CompilerError(Building)
                            {
                                FilePath = json.Path,
                                Message = "'modReferences[" + i + "]': could not find mod '" + modReferences[i] + "', either as a binary or in a source directory."
                            });
                        }
                        else
                        {
                            errors.Add(new CompilerError(Building)
                            {
                                Cause = new CompilerWarning(),
                                FilePath = json.Path,
                                IsWarning = true,
                                Message = "'modReferences[" + i + "]': could not find mod '" + modReferences[i]
                                    + "', building it first (matching directory: '" + Path.GetDirectoryName(d) + "')."
                            });

                            Compiler.CompileFromSource(d);
                        }
                    }
                }

                if (checkCircularRefs && CheckCircularModRef(this))
                    errors.Add(new CompilerError(Building)
                    {
                        Cause = new CircularReferenceException(circularPath),
                        FilePath = json.Path,
                        Message = "The mod or one of its references contains a circular reference. See the exception for more details."
                    });
            }

            AddIfNotNull(SetJsonValue(json, "dllReferences", ref dllReferences, true, EmptyStringArr), errors);
            for (int i = 0; i < dllReferences.Length; i++)
                try
                {
                    try
                    {
                        Assembly.ReflectionOnlyLoadFrom(dllReferences[i]);
                    }
                    catch
                    {
                        try
                        {
                            Assembly.ReflectionOnlyLoadFrom(Building.OriginPath + "\\" + dllReferences[i]);
                        }
                        catch
                        {
                            Assembly.ReflectionOnlyLoadFrom(Building.OriginPath + "\\References\\" + dllReferences[i]);
                        }
                    }
                }
                catch (Exception e)
                {
                    errors.Add(new CompilerError(Building)
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
                    if (File.Exists(Building.OriginPath + "\\" + msBuildFile))
                        msBuildFile = Building.OriginPath + "\\" + msBuildFile;
                    else
                        errors.Add(new CompilerError(Building)
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
                        errors.Add(new CompilerError(Building)
                        {
                            Cause = new InvalidCastException(),
                            FilePath = json.Path,
                            IsWarning = true,
                            Message = "'version' is a " + json.Json.GetJsonType() + ", not a string or an int[]."
                        });
                    else
                    {
                        int[] values = new int[4] { 1, 0, 0, 0 };

                        for (int i = 0; i < Math.Min(ver.Count, 4); i++)
                            if (!ver[i].IsInt)
                                errors.Add(new CompilerError(Building)
                                {
                                    Cause = new InvalidCastException(),
                                    FilePath = json.Path,
                                    IsWarning = true,
                                    Message = "'version[" + i + "]' is a " + json.Json.GetJsonType() + ", not an int."
                                });
                            else
                            {
                                values[i] = (int)ver[i];

                                try
                                {
                                    version = new Version(values[0], values[1], values[2], values[3]);
                                }
                                catch (Exception e)
                                {
                                    errors.Add(new CompilerError(Building)
                                    {
                                        Cause = e,
                                        FilePath = json.Path,
                                        IsWarning = true,
                                        Message = "Invalid version format. Consider changing it to the general format '<major>.<minor>.<build>.<revision>'."
                                    });
                                }
                            }
                    }
                }
                else
                {
                    try
                    {
                        version = new Version((string)ver);
                    }
                    catch (Exception e)
                    {
                        errors.Add(new CompilerError(Building)
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

			AddIfNotNull(SetJsonValue(json, "icon", ref icon, null), errors);
			if (icon != null && !Building.Files.ContainsKey(icon + ".png"))
				errors.Add(new CompilerError(Building)
				{
					Cause = new FileNotFoundException(),
					FilePath = json.Path,
					IsWarning = false,
					Message = "Icon '" + icon + ".png' not found."
				});

			// ---

			AddIfNotNull(SetJsonValue(json, "language",      ref language,      "C#" ), errors);
            AddIfNotNull(SetJsonValue(json, "compress",      ref compress,      true ), errors);
            AddIfNotNull(SetJsonValue(json, "validate",      ref validate,      true ), errors);
            AddIfNotNull(SetJsonValue(json, "includeSource", ref includeSource, false), errors);
            AddIfNotNull(SetJsonValue(json, "check",         ref check,         true ), errors);
            AddIfNotNull(SetJsonValue(json, "warningLevel",  ref warningLevel,  4    ), errors);
            AddIfNotNull(SetJsonValue(json, "ignore", ref ignore, EmptyStringArr     ), errors);
            if (warningLevel < 0 || warningLevel > 4)
                errors.Add(new CompilerError(Building)
                {
                    Cause = new ArgumentOutOfRangeException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "'warningLevel': value must be an element of [0;4]."
                });
            AddIfNotNull(SetJsonValue(json, "outputName", ref outputName, Building.OriginName), errors);

            return errors;
        }
    }
}
