using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Build.Framework;
using PoroCYon.Extensions;
using PoroCYon.Extensions.IO;
using LitJson;
using TAPI;
using PoroCYon.MCT.Internal;
using PoroCYon.MCT.Tools.Compiler;
using PoroCYon.MCT.Tools.Internal.Compiler;
using Ionic.Zip;

namespace PoroCYon.MCT.Tools
{
	using BinBuffer = TAPI.BinBuffer;

    /// <summary>
    /// The MCT mod compiler
    /// </summary>
    public class ModCompiler
    {
        internal readonly static string BuildingList = Consts.MctDirectory + ".building.json";

        internal ModData building;
        internal Dictionary<string, string> modDict;

        static JsonData list;

		static JsonData ModInfoFromModStream(Stream stream)
		{
			BinBuffer bb = new BinBuffer(new BinBufferStream(stream));
			bb.ReadInt(); // version
			return JsonMapper.ToObject(bb.ReadString());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static Dictionary<string, string> GetInternalNameToPathDictionary()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (Directory.Exists(Mods.pathCompiled))
			{
				string[] files = Directory.GetFiles(Mods.pathCompiled);
				for (int i = 0; i < files.Length; i++)
				{
					string modFile = files[i];
					if (modFile.EndsWith(".tapimod") || modFile.EndsWith(".tapi"))
					{
						string[] split = modFile.Split('\\', '/');
						string fileName = split[split.Length - 1];

						if (modFile.EndsWith(".tapimod"))
							using (FileStream fs = new FileStream(modFile, FileMode.Open))
							{
								JsonData modInfo = ModInfoFromModStream(fs);
								dictionary[(string)modInfo["internalName"]] = modFile;
							}
						else if (modFile.EndsWith(".tapi"))
							using (ZipFile zipFile = ZipFile.Read(modFile))
							{
								if (zipFile.ContainsEntry("Mod.tapimod"))
								{
									ZipEntry zipEntry = zipFile["Mod.tapimod"];

									using (MemoryStream ms = new MemoryStream())
									{
										zipEntry.Extract(ms);
										ms.Position = 0L;
										JsonData modInfo = ModInfoFromModStream(ms);
										dictionary[(string)modInfo["internalName"]] = modFile;
									}
								}
							}
					}
				}
			}
			return dictionary;
		}

		/// <summary>
		/// Gets the collection of loggers attached to the <see cref="ModCompiler" />.
		/// </summary>
		public List<MctLogger> Loggers
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ModCompiler" /> class.
        /// </summary>
        public ModCompiler()
            : this(new List<MctLogger>())
        {

        }
        /// <summary>
        /// Creates a new instance of the <see cref="ModCompiler" /> class.
        /// </summary>
        /// <param name="loggers">A collection of loggers to fill the <see cref="Loggers" /> collection with.</param>
        public ModCompiler(IEnumerable<MctLogger> loggers)
        {
            Loggers = new List<MctLogger>(loggers);
        }
        /// <summary>
        /// Creates a new instance of the <see cref="ModCompiler" /> class.
        /// </summary>
        /// <param name="loggers">A collection of loggers to fill the <see cref="Loggers" /> collection with.</param>
        public ModCompiler(params MctLogger[] loggers)
            : this((IEnumerable<MctLogger>)loggers)
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
            for (int i = 0; i < Loggers.Count; i++)
                Loggers[i].compiler_wr = new WeakReference<ModCompiler>(this);

            folder = Environment.ExpandEnvironmentVariables(folder);

            if (folder.EndsWith("\\"))
                folder = folder.Remove(folder.Length - 1);

            if (folder[1] != ':' && !folder.StartsWith("\\")) // <drive>:\path or \\ServerName\path
                // if the folder doesn't exist, it's maybe a folder in the Mods\Sources directory?
                if (!Directory.Exists(folder))
                    folder = Mods.pathSources + "\\" + folder;
                else
                    folder = Path.GetFullPath(folder);

            Log("Compiling source " + folder, MessageImportance.Normal);

            building = new ModData(this);

            try
            {
                building.OriginPath = folder;
                building.OriginName = Path.GetFileName(folder);

                if (!BeginCompile(folder))
                {
                    LogError(Exception cause = new CompilerException("Mod already building!"));

                    LogResult(var o = CreateOutput(new List<CompilerError>()
                    {
                        new CompilerError(building)
                        {
                            Cause = cause,
                            FilePath = folder,
                            IsWarning = true,
                            Message = "The mod is already being built."
                        }
                    }));

                    return o;
                }

                #region check if folder exists
                if (!Directory.Exists(folder))
                {
                    EndCompile(folder);

                    LogError(Exception cause = new DirectoryNotFoundException("Directory '" + folder + "' not found."));
                    
                    LogResult(var o = new CompilerOutput()
                    {
                        Succeeded = false,
                        errors = new List<CompilerError>()
                        {
                            new CompilerError(building)
                            {
                                Cause = cause,
                                Message = "The mod directory (" + folder + ") was not found",
                                IsWarning = false,
                                FilePath = folder
                            }
                        }
                    });

                    return o;
                }
                #endregion

                CompilerOutput outp;

                Log("Loading files.", MessageImportance.High);

                var readFiles = new FileLoader(this).LoadFiles(folder);
                outp = CreateOutput(readFiles.Item3);
                if (!outp.Succeeded)
                {
                    LogResult(outp);
                    EndCompile(folder);
                    return outp;
                }

                Log("Validating JSONs.", MessageImportance.High);

                outp = Validate(readFiles.Item1, readFiles.Item2, true);
                if (!outp.Succeeded)
                {
                    LogResult(outp);
                    EndCompile(folder);
                    return outp;
                }

                Log("Building sources.", MessageImportance.High);

                var compiled = new Builder(this).Build();
                outp = CreateOutput(compiled.Item3);
                outp.Succeeded &= compiled.Item1 != null;
                if (!outp.Succeeded)
                {
                    LogResult(outp);
                    EndCompile(folder);
                    return outp;
                }

                return MainCompileStuff(building, compiled.Item1);
            }
            catch (Exception e)
            {
                LogError(e, "Unexpected error.");

                EndCompile(folder);

                LogResult(var o = CreateOutput(new List<CompilerError>()
                {
                    new CompilerError(building)
                    {
                        Cause = e,
                        FilePath = folder,
                        IsWarning = false,
                        Message = "An unexpected error occured while compiling."
                    }
                }));

                return o;
            }
        }
        /// <summary>
        /// Compiles a mod from a managed assembly.
        /// </summary>
        /// <param name="assemblyPath">The path to the assembly file, either relative to the working directory or an absolute path.</param>
        /// <returns>The output of the compiler.</returns>
        public CompilerOutput CompileFromAssembly(string assemblyPath)
        {
            for (int i = 0; i < Loggers.Count; i++)
                Loggers[i].compiler_wr = new WeakReference<ModCompiler>(this);

            assemblyPath = Environment.ExpandEnvironmentVariables(assemblyPath);

            Log("Compiling assembly from path " + assemblyPath, MessageImportance.Normal);

            building = new ModData(this);

            building.OriginPath = assemblyPath;
            building.OriginName = Path.GetFileNameWithoutExtension(assemblyPath);

            if (!BeginCompile(assemblyPath))
            {
                LogError(Exception cause = new CompilerException("Mod already building!"));

                LogResult(var o = CreateOutput(new List<CompilerError>()
                {
                    new CompilerError(building)
                    {
                        Cause = cause,
                        FilePath = assemblyPath,
                        IsWarning = true,
                        Message = "The mod is already being built."
                    }
                }));

                return o;
            }

            try
            {
                #region check if file exists
                if (!File.Exists(assemblyPath))
                {
                    LogResult(var o = new CompilerOutput()
                    {
                        Succeeded = false,
                        errors = new List<CompilerError>()
                        {
                            new CompilerError(building)
                            {
                                Cause = new FileNotFoundException("File '" + assemblyPath + "' not found."),
                                Message = "The assembly '" + assemblyPath + "' was not found."
                            }
                        }
                    });

                    return o;
                }
                #endregion

                Assembly asm;

                try
                {
                    asm = Assembly.LoadFile(assemblyPath);
                }
                #region check if assembly is valid
                catch (BadImageFormatException e)
                // if (e is BadImageFormatException || e is DllNotFoundException || e is InvalidProgramException || e is TypeLoadException)
                {
                    LogError(e);

                    LogResult(var o = new CompilerOutput()
                    {
                        Succeeded = false,
                        errors = new List<CompilerError>()
                        {
                            new CompilerError(building)
                            {
                                Cause = e,
                                Message = "The assembly is not a manged assembly -or- is not compiled with the x86 architecture.",
                                FilePath = assemblyPath
                            }
                        }
                    });

                    return o;
                }
                catch (Exception e)
                {
                    LogError(e);

                    LogResult(var o = new CompilerOutput()
                    {
                        Succeeded = false,
                        errors = new List<CompilerError>()
                        {
                            new CompilerError(building)
                            {
                                Cause = e,
                                Message = "The assembly could not be loaded.",
                                FilePath = assemblyPath
                            }
                        }
                    });

                    return o;
                }
                #endregion

                return CompileFromAssembly(asm);
            }
            catch (Exception e)
            {
                LogError(e, "Unexpected error.");

                EndCompile(assemblyPath);

                LogResult(var o = CreateOutput(new List<CompilerError>()
                {
                    new CompilerError(building)
                    {
                        Cause = e,
                        FilePath = assemblyPath,
                        IsWarning = false,
                        Message = "An unexpected error occured while compiling."
                    }
                }));

                return o;
            }
        }
        /// <summary>
        /// Compiles a mod from a managed assembly.
        /// </summary>
        /// <param name="asm">The assembly to compile.</param>
        /// <returns>The output of the compiler.</returns>
        public CompilerOutput CompileFromAssembly(Assembly asm)
        {
            for (int i = 0; i < Loggers.Count; i++)
                Loggers[i].compiler_wr = new WeakReference<ModCompiler>(this);

            Log("Compiling assembly " + asm, MessageImportance.Normal);

            if (building == null)
            {
                building = new ModData(this);

                building.OriginPath = asm.Location;
                building.OriginName = Path.GetFileNameWithoutExtension(asm.Location);
            }

            if (!BeginCompile(asm.Location))
            {
                LogError(Exception cause = new CompilerException("Mod already building!"));

                LogResult(var o = CreateOutput(new List<CompilerError>()
                {
                    new CompilerError(building)
                    {
                        Cause = cause,
                        FilePath = asm.Location,
                        IsWarning = true,
                        Message = "The mod is already being built."
                    }
                }));

                return o;
            }

            try
            {
                CompilerOutput outp;

                Log("Extracting files.", MessageImportance.High);

                var extracted = new Extractor(this).ExtractData(asm);
                outp = CreateOutput(extracted.Item3);
                if (!outp.Succeeded)
                {
                    LogResult(outp);
                    EndCompile(asm.Location);
                    return outp;
                }

                Log("Validating JSONs.", MessageImportance.High);

                outp = Validate(extracted.Item1, extracted.Item2, true);
                if (!outp.Succeeded)
                {
                    LogResult(outp);
                    EndCompile(asm.Location);
                    return outp;
                }

                return MainCompileStuff(building, asm);
            }
            catch (Exception e)
            {
                EndCompile(asm.Location);

                LogError(e, "Unexpected error.");

                LogResult(var o = CreateOutput(new List<CompilerError>()
                {
                    new CompilerError(building)
                    {
                        Cause = e,
                        FilePath = asm.Location,
                        IsWarning = false,
                        Message = "An unexpected error occured while compiling."
                    }
                }));

                return o;
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
            foreach (string d in Directory.EnumerateFiles(Mods.pathSources))
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

            if (building == null)
            {
                building = new ModData(this);

                building.OriginPath = path;
                building.OriginName = Path.GetFileNameWithoutExtension(path);
            }

            modDict = GetInternalNameToPathDictionary(); // dat name

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

            Log("Checking JSON files.", MessageImportance.High);

            errors.AddRange(new Checker(this).Check());

            Log("Writing output file.", MessageImportance.High);

            errors.AddRange(new Writer (this).Write());

            CompilerOutput outp = CreateOutput(errors);
            if (outp.Succeeded)
                outp.outputFile = Mods.pathCompiled + "\\" + mod.OriginName + (mod.Info.compress ? ".tapi" : ".tapimod");

            LogResult(outp);

            EndCompile(mod.OriginPath);

            return outp;
        }

        bool AppendBuilding(string path)
		{
			IOHelper.WaitWhileFileLocked(BuildingList, sleepTime: 5);

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
			IOHelper.WaitWhileFileLocked(BuildingList, sleepTime: 5);

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

        /// <summary>
        /// Logs a message to the loggers of the <see cref="ModCompiler" />.
        /// </summary>
        /// <param name="text">The message to log.</param>
        /// <param name="importance">The importance of the message.</param>
		[DebuggerStepThrough]
        public void Log(string text, MessageImportance importance)
        {
            for (int i = 0; i < Loggers.Count; i++)
                if (Loggers[i].Verbosity >= (LoggerVerbosity)((int)LoggerVerbosity.Minimal + (int)importance))
                    Loggers[i].Log(text, importance);
        }
		/// <summary>
		/// Logs an error the the loggers of the <see cref="ModCompiler" />.
		/// </summary>
		/// <param name="e">The error to log.</param>
		/// <param name="comment">An optional comment on the error.</param>
		[DebuggerStepThrough]
		public void LogError(Exception e, string comment = null)
        {
            for (int i = 0; i < Loggers.Count; i++)
                Loggers[i].LogError(e, comment);
        }
		/// <summary>
		/// Logs the build result to the loggers of the <see cref="ModCompiler" />.
		/// </summary>
		/// <param name="output">The build result.</param>
		[DebuggerStepThrough]
		public void LogResult(CompilerOutput output)
        {
            for (int i = 0; i < Loggers.Count; i++)
                Loggers[i].LogResult(output);
        }
    }
}
