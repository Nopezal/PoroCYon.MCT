using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Xna.Framework;
using TAPI;
using PoroCYon.MCT.Internal;
using PoroCYon.MCT.Tools.Compiler.Internal.Compilation;

namespace PoroCYon.MCT.Tools.Internal
{
    class BuildLogger : ILogger
    {
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
        }

        public void Initialize(IEventSource eventSource)
        {
            eventSource.ErrorRaised += (s, e) =>
            {
                CompilerError ce = new CompilerError()
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
                //if (Verbosity >= LoggerVerbosity.Minimal)
                //{
                    CompilerError ce = new CompilerError()
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
                //}
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
            //eventSource.MessageRaised += (s, e) =>
            //{
            //    if (e.Importance == MessageImportance.Low    && Verbosity >= LoggerVerbosity.Diagnostic ||
            //        e.Importance == MessageImportance.Normal && Verbosity >= LoggerVerbosity.Detailed   ||
            //        e.Importance == MessageImportance.High   && Verbosity >= LoggerVerbosity.Normal       )
            //        log.Add(e.Message);
            //};    
            //eventSource.StatusEventRaised += (s, e) =>
            //{
            //    if (Verbosity >= LoggerVerbosity.Detailed)
            //        log.Add(e.Message);
            //};
            #endregion
        }
        public void Shutdown() { }
    }

    static class Builder
    {
        internal static List<ICompiler> compilers = new List<ICompiler>();
        internal static string MSBOutputPath = Path.GetTempPath() + "MCT\\MSBuild";

        static void LoadCompilers()
        {
            compilers.Clear();

            compilers.Add(new CSharpCompiler());
            compilers.Add(new JScriptCompiler());
            compilers.Add(new VBCompiler());

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
                        ICompiler c = t.GetConstructor(Type.EmptyTypes).Invoke(null) as ICompiler;

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

        internal static Tuple<Assembly, string, List<CompilerError>> Build(ModData mod)
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
                            errors.Add(new CompilerError()
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
                            errors.Add(new CompilerError()
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

            LoadCompilers();

            var ret = mod.Info.MSBuild ? BuildMSBuild(mod) : BuildICompiler(mod);

            errors.AddRange(ret.Item3);

            return new Tuple<Assembly, string, List<CompilerError>>(ret.Item1, ret.Item2, errors);
        }

        static Tuple<Assembly, string, List<CompilerError>> BuildMSBuild(ModData mod)
        {
            List<CompilerError> errors = new List<CompilerError>();
            Assembly asm = null;
            string pdb = null;

            if (!mod.Info.includeSource)
            {
                List<string> toRemove = new List<string>();
                string ext = Path.GetExtension(mod.Info.msBuildFile);
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

                foreach (string key in mod.Files.Keys)
                    for (int i = 0; i < probableFileExt.Length; i++)
                        if (key.EndsWith(probableFileExt[i]))
                            toRemove.Add(key);
                foreach (string r in toRemove)
                    mod.files.Remove(r);
            }

            BuildLogger logger = new BuildLogger();
            BuildResult result = BuildManager.DefaultBuildManager.Build(new BuildParameters(new ProjectCollection())
                { Loggers = new List<ILogger>() { logger } },
                new BuildRequestData(mod.Info.msBuildFile, new Dictionary<string, string>
                {
                    { "Configuration", mod.Info.includePDB ? "Debug" : "Release" },
                    { "Platform",      "x86"                                     },
                    { "OutputPath",    MSBOutputPath                             }
                }, "4.0", new string[] { "Build" }, null));

            //foreach (object o in logger.errors)
            //    if (o is CompilerError)
            //        errors.Add(o as CompilerError);
            //    else
            //    {
            //        if (Debugger.IsAttached)
            //            Debug.WriteLine(o.ToString());
            //        Console.WriteLine(o.ToString());
            //    }

            errors.AddRange(logger.errors);

            if (result.OverallResult == BuildResultCode.Success && logger.succeeded)
            //if (result.OverallResult != BuildResultCode.Success || !logger.succeeded)
            //    errors.Add(new CompilerError()
            //    {
            //        Cause = result.Exception ?? new CompilerException("Something wen wrong when compiling the MSBuild script."),
            //        FilePath = mod.Info.msBuildFile,
            //        IsWarning = false,
            //        Message = "Something went wrong when compiling the MSBuild script. Please check it in Visual Studio. See the build errors for more information."
            //    });
            //else
            {
                try
                {
                    asm = Assembly.LoadFile(MSBOutputPath + "\\" + ModsCompile.GetOutputFileName(mod.Info.msBuildFile));
                }
                catch (Exception e)
                {
                    errors.Add(new CompilerError()
                    {
                        Cause = e,
                        FilePath = mod.Info.msBuildFile,
                        IsWarning = false,
                        Message = "Could not load built assembly. Check the exception for more information."
                    });
                }

                if (mod.Info.includePDB)
                {
                    pdb = MSBOutputPath + "\\" + ModsCompile.GetPdbFileName(mod.Info.msBuildFile);

                    if (!File.Exists(pdb))
                        errors.Add(new CompilerError()
                        {
                            Cause = new FileNotFoundException(),
                            FilePath = mod.Info.msBuildFile,
                            IsWarning = false,
                            Message = "Could not find the .pdb file."
                        });
                }
            }

            //if (Directory.Exists(MSBOutputPath))
            //    Directory.Delete(MSBOutputPath, true);

            return new Tuple<Assembly, string, List<CompilerError>>(asm, pdb, errors);
        }
        static Tuple<Assembly, string, List<CompilerError>> BuildICompiler(ModData mod)
        {
            List<CompilerError> errors = new List<CompilerError>();

            Assembly asm = null;
            string pdb = null;

            string lang = mod.Info.language.ToString().ToLowerInvariant();
            ICompiler compiler = null;

            for (int i = 0; i < compilers.Count; i++)
                if (Array.IndexOf(compilers[i].LanguageNames, lang) != -1)
                {
                    compiler = compilers[i];
                    break;
                }

            if (compiler == null)
                errors.Add(new CompilerError()
                {
                    Cause = new KeyNotFoundException(),
                    FilePath = mod.jsons[0].Path,
                    IsWarning = false,
                    Message = "Could not find the specified programming language '" + mod.Info.language + "'."
                });
            else
                try
                {
                    var result = compiler.Compile(mod);
                    errors.AddRange(result.Item2);

                    asm = result.Item1;

                    if (asm != null && mod.Info.includePDB)
                    {
                        pdb = Path.ChangeExtension(asm.Location, ".pdb");

                        if (!File.Exists(pdb))
                            errors.Add(new CompilerError()
                            {
                                Cause = new FileNotFoundException(),
                                FilePath = mod.Info.msBuildFile,
                                IsWarning = false,
                                Message = "Could not find the .pdb file."
                            });
                    }

                    if (!mod.Info.includeSource)
                    {
                        List<string> toRemove = new List<string>();

                        foreach (string key in mod.Files.Keys)
                            for (int i = 0; i < compiler.FileExtensions.Length; i++)
                                if (key.EndsWith(compiler.FileExtensions[i]))
                                    toRemove.Add(key);
                        foreach (string r in toRemove)
                            mod.files.Remove(r);
                    }
                }
                catch (Exception e)
                {
                    errors.Add(new CompilerError()
                    {
                        Cause = e,
                        FilePath = mod.jsons[0].Path,
                        IsWarning = false,
                        Message = "Something went wrong when building the mod. Check the Exception for more info."
                    });
                }

            return new Tuple<Assembly, string, List<CompilerError>>(asm, pdb, errors);
        }
    }
}
