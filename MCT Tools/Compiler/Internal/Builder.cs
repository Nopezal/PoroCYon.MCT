using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LitJson;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using PoroCYon.MCT.Internal;
//using PoroCYon.MCT.Tools.Compiler.Internal.Compilation;
using TAPI;

namespace PoroCYon.MCT.Tools.Internal
{
    static class Builder
    {
        internal static List<ICompiler> compilers = new List<ICompiler>();
        internal static string MSBOutputPath = Path.GetTempPath() + "\\MCT\\MSBuild";

        static void LoadCompilers()
        {
            compilers.Clear();

            //compilers.Add(new CSharpCompiler());
            //compilers.Add(new JScriptCompiler());
            //compilers.Add(new VBCompiler());

            foreach (string f in Directory.EnumerateFiles(Consts.MctDirectory, "*.dll", SearchOption.TopDirectoryOnly))
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
            LoadCompilers();

            return mod.Info.MSBuild ? BuildMSBuild(mod) : BuildICompiler(mod);
        }

        static Tuple<Assembly, string, List<CompilerError>> BuildMSBuild(ModData mod)
        {
            List<CompilerError> errors = new List<CompilerError>();
            Assembly asm = null;
            string pdb = null;

            BuildResult result = BuildManager.DefaultBuildManager.Build(new BuildParameters(new ProjectCollection()),
                new BuildRequestData(mod.Info.msBuildFile, new Dictionary<string, string>
            {
                { "Configuration", mod.Info.includePDB ? "Debug" : "Release" },
                { "Platform",      "x86"                                     },
                { "OutputPath",    MSBOutputPath                                }
            }, "4.0", new string[] { "Build" }, null));

            if (result.OverallResult != BuildResultCode.Success)
                errors.Add(new CompilerError()
                {
                    Cause = result.Exception ?? new CompilerException(),
                    FilePath = mod.Info.msBuildFile,
                    IsWarning = false,
                    Message = "Something went wrong when compiling the MSBuild script. Please check it in Visual Studio."
                });
            else
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
                    pdb = MSBOutputPath + "\\" + ModsCompile.GetPdbFileName(mod.Info.msBuildFile) + ".pdb";

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

            if (Directory.Exists(MSBOutputPath))
                Directory.Delete(MSBOutputPath, true);

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
                    compiler = compilers[i];

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
