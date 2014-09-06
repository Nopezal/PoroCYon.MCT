using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Build.Framework;
using LitJson;
using TAPI;
using PoroCYon.MCT.Internal;
using PoroCYon.MCT.Tools.Compiler;
using PoroCYon.MCT.Tools.Internal.Compiler;

namespace PoroCYon.MCT.Tools
{
    /// <summary>
    /// The MCT mod compiler
    /// </summary>
    public class ModCompiler
    {
        internal readonly static string BuildingList = Consts.MctDirectory + ".building.json";

        internal ModData building;
        internal Dictionary<string, string> modDict;

        static JsonData list;

        /// <summary>
        /// Gets the collection of loggers attached to the <see cref="ModCompiler" />.
        /// </summary>
        public List<ILogger> Loggers
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ModCompiler" /> class.
        /// </summary>
        public ModCompiler()
            : this(new List<ILogger>())
        {

        }
        /// <summary>
        /// Creates a new instance of the <see cref="ModCompiler" /> class.
        /// </summary>
        /// <param name="loggers">A collection of loggers to fill the <see cref="Loggers" /> collection with.</param>
        public ModCompiler(IEnumerable<ILogger> loggers)
        {
            Loggers = new List<ILogger>(loggers);
        }
        /// <summary>
        /// Creates a new instance of the <see cref="ModCompiler" /> class.
        /// </summary>
        /// <param name="loggers">A collection of loggers to fill the <see cref="Loggers" /> collection with.</param>
        public ModCompiler(params ILogger[] loggers)
            : this((IEnumerable<ILogger>)loggers)
        {

        }

        /// <summary>
        /// Compiles a mod from its source folder.
        /// </summary>
        /// <param name="folder">The mod's folder. Either an absolute path,
        /// relative to the working directory or the name of a folder in the Mods\Sources folder</param>
        /// <returns>The output of the compiler.</returns>
        public CompilerOutput CompileFromSource  (string folder)
        {
            if (folder.EndsWith("\\"))
                folder = folder.Remove(folder.Length - 1);

            if (folder[1] != ':' && !folder.StartsWith("\\")) // <drive>:\path or \\ServerName\path
                // if the folder doesn't exist, it's maybe a folder in the Mods\Sources directory?
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
                building.OriginPath = folder;
                building.OriginName = Path.GetFileName(Path.GetDirectoryName(folder));

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

                var readFiles = new FileLoader(this).LoadFiles(folder);
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

                var compiled = new Builder(this).Build();
                outp = CreateOutput(compiled.Item3);
                outp.Succeeded &= compiled.Item1 != null;
                if (!outp.Succeeded)
                {
                    EndCompile(folder);
                    return outp;
                }

                return MainCompileStuff(building, compiled.Item1);
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
            }
        }
        /// <summary>
        /// Compiles a mod from a managed assembly.
        /// </summary>
        /// <param name="assemblyPath">The path to the assembly file, either relative to the working directory or an absolute path.</param>
        /// <returns>The output of the compiler.</returns>
        public CompilerOutput CompileFromAssembly(string assemblyPath)
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

            building.OriginPath = assemblyPath;
            building.OriginName = Path.GetFileNameWithoutExtension(assemblyPath);

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
                                Message = "The assembly '" + assemblyPath + "' was not found."
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

                return CompileFromAssembly(asm);
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
        /// <summary>
        /// Compiles a mod from a managed assembly.
        /// </summary>
        /// <param name="asm">The assembly to compile.</param>
        /// <returns>The output of the compiler.</returns>
        public CompilerOutput CompileFromAssembly(Assembly asm)
        {
            if (!BeginCompile(asm.Location))
                return CreateOutput(new List<CompilerError>()
                {
                    new CompilerError()
                    {
                        Cause = new CompilerException("Mod already building!"),
                        FilePath = asm.Location,
                        IsWarning = true,
                        Message = "The mod is already being built."
                    }
                });

            try
            {
                CompilerOutput outp;

                var extracted = new Extractor(this).ExtractData(asm);
                outp = CreateOutput(extracted.Item3);
                if (!outp.Succeeded)
                {
                    EndCompile(asm.Location);
                    return outp;
                }

                outp = Validate(extracted.Item1, extracted.Item2, true);
                if (!outp.Succeeded)
                {
                    EndCompile(asm.Location);
                    return outp;
                }

                return MainCompileStuff(building, asm);
            }
            catch (Exception e)
            {
                EndCompile(asm.Location);

                return CreateOutput(new List<CompilerError>()
                {
                    new CompilerError()
                    {
                        Cause = e,
                        FilePath = asm.Location,
                        IsWarning = false,
                        Message = "An unexpected error occured while compiling."
                    }
                });
            }
        }

        internal CompilerOutput CreateOutput(List<CompilerError> errors)
        {
            bool err = false;

            CompilerOutput outp = new CompilerOutput();

            for (int i = 0; i < errors.Count; i++)
                if (!errors[i].IsWarning || !(errors[i].Cause is CompilerWarning))
                    err = true; // houston, we have a problem

            outp.Succeeded = !err;
            outp.errors = errors;

            return outp;
        }

        CompilerOutput Validate(List<JsonFile> jsons, Dictionary<string, byte[]> files, bool validateModInfo = true)
        {
            return CreateOutput(new Validator(this).ValidateJsons(jsons, files, validateModInfo));
        }

        internal string FindSourceFolderFromInternalName(string internalName)
        {
            foreach (string d in Directory.EnumerateFiles(Mods.pathDirModsSources))
            {
                if (File.Exists(d + "\\ModInfo.json"))
                {
                    JsonData j = JsonMapper.ToObject(File.ReadAllText(d + "\\ModInfo.json"));

                    if (j.Has("internalName") && (string)j["internalName"] == internalName)
                        return d;
                }
            }

            return null;
        }

        bool BeginCompile(string path)
        {
            if (!Debugger.IsAttached && AppendBuilding(path))
                return false;

            if (!Directory.Exists(Consts.MctDirectory))
                Directory.CreateDirectory(Consts.MctDirectory);

            if (!File.Exists(Consts.MctDirectory + "\\.building.json"))
                File.WriteAllText(Consts.MctDirectory + "\\.building.json", "[]");

            building = new ModData(this);

            modDict = Mods.GetInternalNameToPathDictionary(); // dat name

            if (!Directory.Exists(Path.GetTempPath() + "\\MCT"))
                Directory.CreateDirectory(Path.GetTempPath() + "\\MCT");

            return true;
        }
        void   EndCompile(string path)
        {
            RemoveBuilding(path);
            //Directory.Delete(Path.GetTempPath() + "\\MCT", true);
        }

        CompilerOutput MainCompileStuff(ModData mod, Assembly asm)
        {
            if (asm == null)
                return null;

            mod.Assembly = asm;

            List<CompilerError> errors = new List<CompilerError>();

            errors.AddRange(new Checker(this).Check());
            errors.AddRange(new Writer (this).Write());

            CompilerOutput outp = CreateOutput(errors);
            if (outp.Succeeded)
                outp.outputFile = CommonToolUtilities.modsBinDir + "\\" + mod.OriginName + (mod.Info.compress ? ".tapi" : ".tapimod");

            EndCompile(mod.OriginPath);

            return outp;
        }

        bool AppendBuilding(string path)
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
        void RemoveBuilding(string path)
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
