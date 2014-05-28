using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LitJson;
using TAPI;
using PoroCYon.MCT.Internal;
using PoroCYon.MCT.Tools.Internal;

namespace PoroCYon.MCT.Tools
{
    /// <summary>
    /// The MCT mod compiler
    /// </summary>
    public static class ModCompiler
    {
        internal readonly static string BuildingList = Consts.MctDirectory + ".building.json";
        internal static ModData current;
        internal static Dictionary<string, string> modDict;
        static JsonData list;

        /// <summary>
        /// Compiles a mod from its source folder.
        /// </summary>
        /// <param name="folder">The mod's folder. Either an absolute path,
        /// relative to the working directory or the name of a folder in the Mods\Sources folder</param>
        /// <returns>The output of the compiler.</returns>
        public static CompilerOutput CompileFromSource(string folder)
        {
            if (folder.EndsWith("\\"))
                folder = folder.Remove(folder.Length - 1);

            // if the folder doesn't exist, it's maybe a folder in the Mods\Sources directory?
            if (!Path.IsPathRooted(folder))
                if (!Directory.Exists(folder))
                    folder = CommonToolUtilities.modsSrcDir + "\\" + folder;
                else
                    folder = Path.GetFullPath(folder);

            try
            {
                if (!BeginCompile(folder))
                    return CreateOutput(new List<CompilerError>()
                    {
                        new CompilerError()
                        {
                            Cause = new CompilerException("Mod already building!"),
                            FilePath = folder,
                            IsWarning = true,
                            Message = "The mod is already being built."
                        }
                    });
                current.OriginPath = folder;
                current.OriginName = new DirectoryInfo(folder).Name;

                #region check if folder exists
                if (!Directory.Exists(folder))
                {
                    EndCompile(folder);

                    return new CompilerOutput()
                    {
                        Succeeded = false,
                        errors = new List<CompilerError>()
                        {
                            new CompilerError()
                            {
                                Cause = new DirectoryNotFoundException("Directory '" + folder + "' not found."),
                                Message = "The mod directory (" + folder + ") was not found",
                                IsWarning = false,
                                FilePath = folder
                            }
                        }
                    };
                }
                #endregion

                CompilerOutput outp;

                var readFiles = FileLoader.LoadFiles(folder);
                outp = CreateOutput(readFiles.Item3);
                if (!outp.Succeeded)
                {
                    EndCompile(folder);
                    return outp;
                }

                outp = Validate(readFiles.Item1, readFiles.Item2, true);
                if (!outp.Succeeded)
                {
                    EndCompile(folder);
                    return outp;
                }

                var compiled = Builder.Build(current);
                outp = CreateOutput(compiled.Item3);
                outp.Succeeded &= compiled.Item1 != null;
                if (!outp.Succeeded)
                {
                    EndCompile(folder);
                    return outp;
                }

                return MainCompileStuff(current, compiled.Item1);
            }
            catch (Exception e)
            {
                EndCompile(folder);

                return CreateOutput(new List<CompilerError>()
                {
                    new CompilerError()
                    {
                        Cause = e,
                        FilePath = folder,
                        IsWarning = false,
                        Message = "An unexpected error occured while compiling."
                    }
                });
                //try
                //{
                //    EndCompile(folder);
                //}
                //catch (Exception ex)
                //{
                //    throw new AggregateException(e, ex);
                //}

                //throw new CompilerException(e.Message, e);
            }
        }
        /// <summary>
        /// Compiles a mod from a managed assembly.
        /// </summary>
        /// <param name="assemblyPath">The path to the assembly file, either relative to the working directory or an absolute path.</param>
        /// <returns>The output of the compiler.</returns>
        public static CompilerOutput CompileFromAssembly(string assemblyPath)
        {
            if (!BeginCompile(assemblyPath))
                return CreateOutput(new List<CompilerError>()
                {
                    new CompilerError()
                    {
                        Cause = new CompilerException("Mod already building!"),
                        FilePath = assemblyPath,
                        IsWarning = true,
                        Message = "The mod is already being built."
                    }
                });
            current.OriginPath = assemblyPath;
            current.OriginName = Path.GetFileNameWithoutExtension(assemblyPath);

            try
            {
                #region check if file exists
                if (!File.Exists(assemblyPath))
                    return new CompilerOutput()
                    {
                        Succeeded = false,
                        errors = new List<CompilerError>()
                    {
                        new CompilerError()
                        {
                            Cause = new FileNotFoundException("File '" + assemblyPath + "' not found."),
                            Message = "The assembly to build the mod from (" + assemblyPath + ") was not found."
                        }
                    }
                    };
                #endregion

                Assembly asm;

                try
                {
                    asm = Assembly.LoadFile(assemblyPath);
                }
                #region check if assembly is valid
                catch (BadImageFormatException e)
                {
                    return new CompilerOutput()
                    {
                        Succeeded = false,
                        errors = new List<CompilerError>()
                    {
                        new CompilerError()
                        {
                            Cause = e,
                            Message = "The assembly is not a manged assembly -or- is not compiled with the x86 architecture.",
                            FilePath = assemblyPath
                        }
                    }
                    };
                }
                catch (Exception e)
                {
                    return new CompilerOutput()
                    {
                        Succeeded = false,
                        errors = new List<CompilerError>()
                    {
                        new CompilerError()
                        {
                            Cause = e,
                            Message = "The assembly could not be loaded.",
                            FilePath = assemblyPath
                        }
                    }
                    };
                }
                #endregion

                CompilerOutput outp;

                var extracted = Extractor.ExtractData(asm);
                outp = CreateOutput(extracted.Item3);
                if (!outp.Succeeded)
                {
                    EndCompile(assemblyPath);
                    return outp;
                }

                outp = Validate(extracted.Item1, extracted.Item2, true);
                if (!outp.Succeeded)
                {
                    EndCompile(assemblyPath);
                    return outp;
                }

                return MainCompileStuff(current, asm);
            }
            catch (Exception e)
            {
                EndCompile(assemblyPath);

                return CreateOutput(new List<CompilerError>()
                {
                    new CompilerError()
                    {
                        Cause = e,
                        FilePath = assemblyPath,
                        IsWarning = false,
                        Message = "An unexpected error occured while compiling."
                    }
                });
            }
        }

        static CompilerOutput CreateOutput(List<CompilerError> errors)
        {
            bool err = false;

            CompilerOutput outp = new CompilerOutput();

            for (int i = 0; i < errors.Count; i++)
                if (!errors[i].IsWarning)
                    err = true; // houston, we have a problem

            outp.Succeeded = !err;
            outp.errors = errors;

            return outp;
        }
        static CompilerOutput Validate(List<JsonFile> jsons, Dictionary<string, byte[]> files, bool validateModInfo = true)
        {
            return CreateOutput(Validator.ValidateJsons(jsons, files, validateModInfo));
        }
        static bool BeginCompile(string path)
        {
            if (AppendBuilding(path))
                return false;

            current = new ModData();

            modDict = Mods.GetInternalNameToPathDictionary(); // dat name

            if (!Directory.Exists(Path.GetTempPath() + "\\MCT"))
                Directory.CreateDirectory(Path.GetTempPath() + "\\MCT");

            return true;
        }
        static void EndCompile(string path)
        {
            RemoveBuilding(path);
            //Directory.Delete(Path.GetTempPath() + "\\MCT", true);
        }
        static CompilerOutput MainCompileStuff(ModData mod, Assembly asm)
        {
            if (asm == null)
                return null;

            mod.Assembly = asm;

            List<CompilerError> errors = new List<CompilerError>();

            errors.AddRange(Checker.Check(asm));
            errors.AddRange(Writer.Write(mod));

            CompilerOutput outp = CreateOutput(errors);
            if (outp.Succeeded)
                outp.outputFile = CommonToolUtilities.modsBinDir + "\\" + mod.Info.internalName + (mod.Info.compress ? ".tapi" : ".tapimod");

            EndCompile(mod.OriginPath);

            return outp;
        }
        static bool AppendBuilding(string path)
        {
            string json = "[]";
            if (File.Exists(BuildingList))
                json = File.ReadAllText(BuildingList);

            list = JsonMapper.ToObject(json);
            for (int i = 0; i < list.Count; i++)
                if ((string)list[i] == path)
                    return true;
            //List<JsonData> arr = new List<JsonData>(list.Count + 1);

            //for (int i = 0; i < list.Count; i++)
            //    arr.Add(list[i]);
            //arr.Add(path);

            //list = new JsonData();
            list.Add(path);

            json = list.ToJson();
            File.WriteAllText(BuildingList, json);
            return false;
        }
        static void RemoveBuilding(string path)
        {
            string json = String.Empty;
            if (File.Exists(BuildingList))
                json = File.ReadAllText(BuildingList);

            list = JsonMapper.ToObject(json);
            List<JsonData> arr = new List<JsonData>();

            for (int i = 0; i < list.Count; i++)
                if ((string)list[i] != path)
                    arr.Add((string)list[i]);

            list = JsonMapper.ToObject("[]");

            for (int i = 0; i < arr.Count; i++)
                list.Add(arr[i]);

            json = list.ToJson();
            File.WriteAllText(BuildingList, json);
        }
    }
}
