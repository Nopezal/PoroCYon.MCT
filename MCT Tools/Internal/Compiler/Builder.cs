using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Xna.Framework;
using PoroCYon.Extensions.Collections;
using TAPI;
using PoroCYon.MCT.Internal;
using PoroCYon.MCT.Tools.Compiler;
using PoroCYon.MCT.Tools.Internal.Compiler.Compilers;

namespace PoroCYon.MCT.Tools.Internal.Compiler
{
    class BuildLogger(ModData md) : ILogger
    {
        ModData building = md;

        internal bool succeeded = true;
        internal List<CompilerError> errors = new List<CompilerError>();
        internal List<object> log = new List<object>();

        public string Parameters
        {
            get;
            set;
        }
        public LoggerVerbosity Verbosity
        {
            get;
            set;
        } = LoggerVerbosity.Normal;

        public void Initialize(IEventSource eventSource)
        {
            eventSource.ErrorRaised += (s, e) =>
            {
                CompilerError ce = new CompilerError(building)
                {
                    Cause = new CompilerException(e.Message),
                    FilePath = e.File,
                    IsWarning = false,
                    LocationInFile = new Point(e.ColumnNumber, e.LineNumber),
                    Message = e.Code + ": " + e.Message + " (in project " + e.ProjectFile + ")"
                        + (!String.IsNullOrEmpty(e.HelpKeyword) ? ": " + e.HelpKeyword : String.Empty)
                };

                errors.Add(ce);
                log.Add(ce);
            };
            eventSource.WarningRaised += (s, e) =>
            {
                if (Verbosity >= LoggerVerbosity.Minimal)
                {
                    CompilerError ce = new CompilerError(building)
                    {
                        Cause = new CompilerWarning(e.Message),
                        FilePath = e.File,
                        IsWarning = true,
                        LocationInFile = new Point(e.ColumnNumber, e.LineNumber),
                        Message = e.Code + ": " + e.Message + " (in project " + e.ProjectFile + ")"
                        + (!String.IsNullOrEmpty(e.HelpKeyword) ? ": " + e.HelpKeyword : String.Empty)
                    };

                    errors.Add(ce);
                    log.Add(ce);
                }
            };
            #region commented event hooking (except checking if succeeded)
            eventSource.BuildFinished += (s, e) =>
            {
                //if (Verbosity >= LoggerVerbosity.Normal)
                //    log.Add("Build finished.");
                succeeded &= e.Succeeded;
            };
            //eventSource.BuildStarted += (s, e) =>
            //{
            //    if (Verbosity >= LoggerVerbosity.Normal)
            //        log.Add("Build started.");
            //};
            eventSource.ProjectFinished += (s, e) =>
            {
                //if (Verbosity >= LoggerVerbosity.Normal)
                //    log.Add("Project " + e.ProjectFile + " finished.");
                succeeded &= e.Succeeded;
            };
            //eventSource.ProjectStarted += (s, e) =>
            //{
            //    if (Verbosity >= LoggerVerbosity.Normal)
            //    log.Add("Project " + e.ProjectFile + " started.");
            //};
            eventSource.TargetFinished += (s, e) =>
            {
                //if (Verbosity >= LoggerVerbosity.Detailed)
                //    log.Add("Target " + e.TargetFile + " (" + e.TargetName + ") finished.");
                succeeded &= e.Succeeded;
            };
            //eventSource.TargetStarted += (s, e) =>
            //{
            //    if (Verbosity >= LoggerVerbosity.Detailed)
            //        log.Add("Target " + e.TargetFile + " (" + e.TargetName + ") started.");
            //};
            eventSource.TaskFinished += (s, e) =>
            {
                //if (Verbosity >= LoggerVerbosity.Diagnostic)
                //    log.Add("Task " + e.TaskFile + " (" + e.TaskName + ") finished.");
                succeeded &= e.Succeeded;
            };
            //eventSource.TaskStarted += (s, e) =>
            //{
            //    if (Verbosity >= LoggerVerbosity.Diagnostic)
            //        log.Add("Task " + e.TaskFile + " (" + e.TaskName + ") started.");
            //};
            eventSource.MessageRaised += (s, e) =>
            {
                if (e.Importance == MessageImportance.Low && Verbosity >= LoggerVerbosity.Diagnostic ||
                    e.Importance == MessageImportance.Normal && Verbosity >= LoggerVerbosity.Detailed ||
                    e.Importance == MessageImportance.High && Verbosity >= LoggerVerbosity.Normal)
                    log.Add(e.Message);
            };
            eventSource.StatusEventRaised += (s, e) =>
            {
                if (Verbosity >= LoggerVerbosity.Detailed)
                    log.Add(e.Message);
            };
            #endregion
        }
        public void Shutdown() { }
    }

    class Builder(ModCompiler mc) : CompilerPhase(mc)
    {
        internal List<ICompiler> compilers = new List<ICompiler>();
        internal string MSBOutputPath = Path.GetTempPath() + "MCT\\MSBuild";

        readonly static Type[] ctorTypes = { typeof(ModCompiler) };

        void LoadCompilers()
        {
            if (!Directory.Exists(Consts.MctDirectory + "\\Compilers"))
                Directory.CreateDirectory(Consts.MctDirectory + "\\Compilers");

            compilers.Clear();

            compilers.Add(new CSharpCompiler (Compiler));
            compilers.Add(new JScriptCompiler(Compiler));
            compilers.Add(new VBCompiler     (Compiler));

            if (!Directory.Exists(Consts.MctDirectory))
                Directory.CreateDirectory(Consts.MctDirectory);

            foreach (string f in Directory.EnumerateFiles(Consts.MctDirectory + "\\Compilers", "*.dll", SearchOption.TopDirectoryOnly))
            {
                Assembly asm;

                try
                {
                    asm = Assembly.LoadFile(f);
                }
                catch
                {
                    continue;
                }

                foreach (Type t in asm.GetTypes())
                {
                    if (Array.IndexOf(t.GetInterfaces(), typeof(ICompiler)) == -1)
                        continue;

                    try
                    {
                        ICompiler c = t.GetConstructor(ctorTypes).Invoke(new[] { Compiler }) as ICompiler;

                        if (c != null)
                            compilers.Add(c);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }

        IEnumerable<CompilerError> ClearFolders(ModData mod)
        {
            List<CompilerError> errors = new List<CompilerError>();

            List<string> toRemove = new List<string>();

            foreach (string key in mod.Files.Keys)
            {
                if (key.Contains("bin/"))
                    toRemove.Add(key);
                else if (key.Contains("obj/"))
                    toRemove.Add(key);
                else if (key.Contains("Debug/"))
                    toRemove.Add(key);
                else if (key.Contains("Release/"))
                    toRemove.Add(key);
                else if (key.Contains("ipch/"))
                    toRemove.Add(key);
                else if (key.Contains(".git/"))
                    toRemove.Add(key);
                else if (key.Contains(".sln.ide/"))
                    toRemove.Add(key);
                else if (key.EndsWith(".sdf"))
                    toRemove.Add(key);
                else if (key.EndsWith(".opensdf"))
                    toRemove.Add(key);
                else if (key.EndsWith(".suo"))
                    toRemove.Add(key);
                else if (key.EndsWith(".user"))
                    toRemove.Add(key);
                else if (key.EndsWith(".cache"))
                    toRemove.Add(key);
                else if (key.EndsWith(".gitignore"))
                    toRemove.Add(key);
                else if (key.EndsWith(".gitattributes"))
                    toRemove.Add(key);
                else if (key.EndsWith(".db"))
                    toRemove.Add(key);

                else if (!File.Exists(mod.OriginPath))
                {
                    foreach (string ignore in mod.Info.ignore)
                    {
                        try
                        {
                            foreach (string d in Directory.EnumerateFiles(mod.OriginPath, ignore, SearchOption.AllDirectories))
                                if (d.Substring(mod.OriginPath.Length) == key)
                                    toRemove.Add(d);
                        }
                        catch (Exception e)
                        {
                            errors.Add(new CompilerError(Building)
                            {
                                Cause = e,
                                FilePath = mod.OriginPath,
                                IsWarning = false,
                                Message = "Error when searching for files to exclude."
                            });
                        }
                        try
                        {
                            foreach (string d in Directory.EnumerateDirectories(mod.OriginPath, ignore, SearchOption.AllDirectories))
                                if (d.Substring(mod.OriginPath.Length) == Path.GetDirectoryName(key))
                                    toRemove.Add(d);
                        }
                        catch (Exception e)
                        {
                            errors.Add(new CompilerError(Building)
                            {
                                Cause = e,
                                FilePath = mod.OriginPath,
                                IsWarning = false,
                                Message = "Error when searching for direcries to exclude."
                            });
                        }
                    }
                }
            }
            foreach (string r in toRemove)
                mod.files.Remove(r);

            return errors;
        }

        internal Tuple<Assembly, string, List<CompilerError>> Build()
        {
            List<CompilerError> errors = new List<CompilerError>();

            errors.AddRange(ClearFolders(Building));

            LoadCompilers();

            var ret = Building.Info.MSBuild ? BuildMSBuild() : BuildICompiler();

            errors.AddRange(ret.Item3);

            return new Tuple<Assembly, string, List<CompilerError>>(ret.Item1, ret.Item2, errors);
        }

        Tuple<Assembly, string, List<CompilerError>> BuildMSBuild()
        {
            Compiler.Log("Building using MSBuild.", MessageImportance.Low);

            List<CompilerError> errors = new List<CompilerError>();
            Assembly asm = null;
            string pdb = null;

            if (!Building.Info.includeSource)
            {
                List<string> toRemove = new List<string>();
                string ext = Path.GetExtension(Building.Info.msBuildFile);
                string[] probableFileExt = new[] { ext.Remove(ext.IndexOf("proj")) };

                if (probableFileExt[0] == ".vcx")
                    probableFileExt = new[] { "cpp", "cxx", "c", "c++", "hpp", "hxx", "h", "h++" };
                if (probableFileExt[0] == ".cs")
                    probableFileExt = new[] { "cs", "csx" };
                if (probableFileExt[0] == ".vb")
                    probableFileExt = new[] { "vb", "vba", "vbs" };
                if (probableFileExt[0] == ".js")
                    probableFileExt = new[] { "js", "ts" };
                if (probableFileExt[0] == ".fs")
                    probableFileExt = new[] { "fs", "fsx" };

                foreach (string key in Building.Files.Keys)
                    for (int i = 0; i < probableFileExt.Length; i++)
                        if (key.EndsWith(probableFileExt[i]))
                            toRemove.Add(key);
                foreach (string r in toRemove)
                    Building.files.Remove(r);
            }

            BuildLogger logger = new BuildLogger(Building);
            BuildResult result = BuildManager.DefaultBuildManager.Build(new BuildParameters(new ProjectCollection())
                { Loggers = new List<ILogger>() { logger }.Union(Compiler.Loggers.CastAll(ml => ml.GetMSBuildLogger()).Where(il => il != null)) /* P: */ },
                new BuildRequestData(Building.Info.msBuildFile, new Dictionary<string, string>
                {
                    { "Configuration", Building.Info.includePDB ? "Debug" : "Release" },
                    { "Platform",      "x86"                                     },
                    { "OutputPath",    MSBOutputPath                             }
                }, "4.0", new string[] { "Build" }, null));

            errors.AddRange(logger.errors);

            if (result.OverallResult == BuildResultCode.Success && logger.succeeded)
            {
                try
                {
                    asm = Assembly.LoadFile(MSBOutputPath + "\\" + ModsCompile.GetOutputFileName(Building.Info.msBuildFile));
                }
                catch (Exception e)
                {
                    errors.Add(new CompilerError(Building)
                    {
                        Cause = e,
                        FilePath = Building.Info.msBuildFile,
                        IsWarning = false,
                        Message = "Could not load built assembly. Check the exception for more information."
                    });
                }

                if (Building.Info.includePDB)
                {
                    pdb = MSBOutputPath + "\\" + ModsCompile.GetPdbFileName(Building.Info.msBuildFile);

                    if (!File.Exists(pdb))
                        errors.Add(new CompilerError(Building)
                        {
                            Cause = new FileNotFoundException(),
                            FilePath = Building.Info.msBuildFile,
                            IsWarning = false,
                            Message = "Could not find the .pdb file."
                        });
                }
            }

            //if (Directory.Exists(MSBOutputPath))
            //    Directory.Delete(MSBOutputPath, true);

            return new Tuple<Assembly, string, List<CompilerError>>(asm, pdb, errors);
        }
        Tuple<Assembly, string, List<CompilerError>> BuildICompiler()
        {
            List<CompilerError> errors = new List<CompilerError>();

            Assembly asm = null;
            string pdb = null;

            string lang = Building.Info.language.ToString().ToLowerInvariant();
            ICompiler compiler = null;

            for (int i = 0; i < compilers.Count; i++)
                if (Array.IndexOf(compilers[i].LanguageNames, lang) != -1)
                {
                    compiler = compilers[i];
                    break;
                }

            Compiler.Log("Building using an ICompiler, language=" + lang + ", compiler=" + (compiler?.GetType().ToString() ?? "null"), MessageImportance.Low);

            if (compiler == null)
                errors.Add(new CompilerError(Building)
                {
                    Cause = new KeyNotFoundException(),
                    FilePath = Building.jsons[0].Path,
                    IsWarning = false,
                    Message = "Could not find the specified programming language '" + Building.Info.language + "'."
                });
            else
                try
                {
                    var result = compiler.Compile(Building);
                    errors.AddRange(result.Item2);

                    asm = result.Item1;

                    if (asm != null && Building.Info.includePDB)
                    {
                        pdb = Path.ChangeExtension(asm.Location, ".pdb");

                        if (!File.Exists(pdb))
                            errors.Add(new CompilerError(Building)
                            {
                                Cause = new FileNotFoundException(),
                                FilePath = Building.Info.msBuildFile,
                                IsWarning = false,
                                Message = "Could not find the .pdb file."
                            });
                    }

                    if (!Building.Info.includeSource)
                    {
                        List<string> toRemove = new List<string>();

                        foreach (string key in Building.Files.Keys)
                            for (int i = 0; i < compiler.FileExtensions.Length; i++)
                                if (key.EndsWith(compiler.FileExtensions[i]))
                                    toRemove.Add(key);
                        foreach (string r in toRemove)
                            Building.files.Remove(r);
                    }
                }
                catch (Exception e)
                {
                    errors.Add(new CompilerError(Building)
                    {
                        Cause = e,
                        FilePath = Building.jsons[0].Path,
                        IsWarning = false,
                        Message = "Something went wrong when building the mod. Check the Exception for more info."
                    });
                }

            return new Tuple<Assembly, string, List<CompilerError>>(asm, pdb, errors);
        }
    }
}
