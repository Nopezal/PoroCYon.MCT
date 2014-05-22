using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LitJson;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using PoroCYon.MCT.Internal;
using TAPI;

namespace PoroCYon.MCT.Tools.Internal
{
    static class Builder
    {
        internal static List<Tuple<ICompiler, string[]>> compilers = new List<Tuple<ICompiler, string[]>>();
        internal static string OutputPath = Path.GetTempPath() + "\\MCT\\MSBuild";

        static void LoadCompilers()
        {
            compilers.Clear();

            foreach (string f in Directory.EnumerateFiles(Consts.MctDirectory, "*.json", SearchOption.TopDirectoryOnly))
            {
                JsonData data;
                string[] names;

                try
                {
                    data = JsonMapper.ToObject(File.ReadAllText(f));
                }
                catch (JsonException)
                {
                    continue;
                }

                if (!data.Has("name"))
                    continue;

                JsonData name = data["name"];
                if (!name.IsString && !name.IsArray)
                    continue;
                if (name.IsString)
                    names = new string[1] { (string)name };
                else
                {
                    names = new string[name.Count];

                    for (int i = 0; i < names.Length; i++)
                        names[i] = (string)name[i];
                }

                // ---

                if (!data.Has("assembly"))
                    continue;

                JsonData asmFile = data["assembly"];
                if (!asmFile.IsString)
                    continue;

                Assembly asm;
                try
                {
                    asm = Assembly.LoadFile((string)asmFile);
                }
                catch
                {
                    continue;
                }

                // ---

                if (!data.Has("type"))
                    continue;

                JsonData typeName = data["type"];
                if (!typeName.IsString)
                    continue;

                ICompiler compiler;
                try
                {
                    compiler = asm.GetType((string)typeName, true, true).GetConstructor(Type.EmptyTypes).Invoke(null) as ICompiler;
                }
                catch
                {
                    continue;
                }

                if (compiler == null)
                    continue;

                compilers.Add(new Tuple<ICompiler, string[]>(compiler, names));
            }
        }

        static Tuple<Assembly, string, List<CompilerError>> Build(ModData mod)
        {
            LoadCompilers();

            return mod.info.MSBuild ? BuildMSBuild(mod) : BuildICompiler(mod);
        }

        static Tuple<Assembly, string, List<CompilerError>> BuildMSBuild(ModData mod)
        {
            List<CompilerError> errors = new List<CompilerError>();
            Assembly asm = null;
            string pdb = null;

            BuildResult result = BuildManager.DefaultBuildManager.Build(new BuildParameters(new ProjectCollection()),
                new BuildRequestData(mod.info.msBuildFile, new Dictionary<string, string>
            {
                { "Configuration", mod.info.includePDB ? "Debug" : "Release" },
                { "Platform",      "x86"                                     },
                { "OutputPath",    OutputPath                                }
            }, "4.0", new string[] { "Build" }, null));

            if (result.OverallResult != BuildResultCode.Success)
                errors.Add(new CompilerError()
                {
                    Cause = result.Exception ?? new CompilerException(),
                    FilePath = mod.info.msBuildFile,
                    IsWarning = false,
                    Message = "Something went wrong when compiling the MSBuild script. Please check it in Visual Studio."
                });
            else
            {
                try
                {
                    asm = Assembly.LoadFile(OutputPath + "\\" + ModsCompile.GetOutputFileName(mod.info.msBuildFile));
                }
                catch (Exception e)
                {
                    errors.Add(new CompilerError()
                    {
                        Cause = e,
                        FilePath = mod.info.msBuildFile,
                        IsWarning = false,
                        Message = "Could not load built assembly. Check the exception for more information."
                    });
                }

                if (mod.info.includePDB)
                {
                    pdb = OutputPath + "\\" + ModsCompile.GetOutputFileName(mod.info.msBuildFile) + ".pdb";

                    if (!File.Exists(pdb))
                        errors.Add(new CompilerError()
                        {
                            Cause = new FileNotFoundException(),
                            FilePath = mod.info.msBuildFile,
                            IsWarning = false,
                            Message = "Could not find the .pdb file."
                        });
                }
            }

            return new Tuple<Assembly, string, List<CompilerError>>(asm, pdb, errors);
        }
        static Tuple<Assembly, string, List<CompilerError>> BuildICompiler(ModData mod)
        {
            List<CompilerError> errors = new List<CompilerError>();

            Assembly asm = null;
            string pdb = null;

            string lang = mod.info.language.ToString().ToLowerInvariant();
            ICompiler compiler = null;

            for (int i = 0; i < compilers.Count; i++)
                if (Array.IndexOf(compilers[i].Item2, lang) != -1)
                    compiler = compilers[i].Item1;

            if (compiler == null)
                errors.Add(new CompilerError()
                {
                    Cause = new KeyNotFoundException(),
                    FilePath = mod.jsons[0].path,
                    IsWarning = false,
                    Message = "Could not find the specified programming language '" + mod.info.language + "'."
                });
            else
                try
                {
                    var result = compiler.Compile(mod.files);

                    asm = result.Item1;

                    if (mod.info.includePDB)
                    {
                        pdb = Path.ChangeExtension(asm.Location, ".pdb");
                        if (!File.Exists(pdb))
                            errors.Add(new CompilerError()
                            {
                                Cause = new FileNotFoundException(),
                                FilePath = mod.info.msBuildFile,
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
                        FilePath = mod.jsons[0].path,
                        IsWarning = false,
                        Message = "Something went wrong when building the mod. Check the Exception for more info."
                    });
                }

            return new Tuple<Assembly, string, List<CompilerError>>(asm, pdb, errors);
        }
    }
}
