using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PoroCYon.MCT.Internal;
using PoroCYon.MCT.Tools.Internal;

namespace PoroCYon.MCT.Tools.Compiler
{
    /// <summary>
    /// The MCT mod compiler
    /// </summary>
    public static class ModCompiler
    {
        /// <summary>
        /// Compiles a mod from its source folder.
        /// </summary>
        /// <param name="folder">The mod's folder. Either an absolute path,
        /// relative to the working directory or the name of a folder in the Mods\Sources folder</param>
        /// <returns>The output of the compiler.</returns>
        public static CompilerOutput CompileFromSource(string folder)
        {
            BeginCompile();

            // if the folder doesn't exist, it's maybe a folder in the Mods\Sources directory?
            if (Path.IsPathRooted(folder) && !Directory.Exists(folder))
                folder = CommonToolUtilities.modsSrcDir + "\\" + folder;

            #region check if folder exists
            if (!Directory.Exists(folder))
                return new CompilerOutput()
                {
                    Succeeded = false,
                    errors = new List<CompilerError>()
                    {
                        new CompilerError()
                        {
                            Cause = new DirectoryNotFoundException("Directory '" + folder + "' not found."),
                            Message = "The mod directory (" + folder + ") was not found"
                        }
                    }
                };
            #endregion

            CompilerOutput outp;

            var readFiles = FileLoader.LoadFiles(folder);
            outp = CreateOutput(readFiles.Item3);
            if (!outp.Succeeded)
                return outp;

            outp = Validate(readFiles.Item1, readFiles.Item2, true);
            if (!outp.Succeeded)
                return outp;

            var compiled = Builder.Build(Validator.current);
            outp = CreateOutput(compiled.Item3);
            if (!outp.Succeeded)
                return outp;

            outp = MainCompileStuff(compiled.Item1);

            return null;
        }
        /// <summary>
        /// Compiles a mod from a managed assembly.
        /// </summary>
        /// <param name="assemblyPath">The path to the assembly file, either relative to the working directory or an absolute path.</param>
        /// <returns>The output of the compiler.</returns>
        public static CompilerOutput CompileFromAssembly(string assemblyPath)
        {
            BeginCompile();

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
                return outp;

            outp = Validate(extracted.Item1, extracted.Item2, true);
            if (!outp.Succeeded)
                return outp;

            outp = MainCompileStuff(asm);

            return outp;
        }

        static CompilerOutput CreateOutput(List<CompilerError> errors)
        {
            bool err = false;

            CompilerOutput outp = new CompilerOutput();

            for (int i = 0; i < errors.Count; i++)
                if (!errors[i].IsWarning)
                    err = true; // houston, we have a problem

            outp.Succeeded = err;
            outp.errors = errors;

            return outp;
        }
        static CompilerOutput Validate(List<JsonFile> jsons, Dictionary<string, byte[]> files, bool validateModInfo = true)
        {
            return CreateOutput(Validator.ValidateJsons(jsons, files, validateModInfo));
        }
        static void BeginCompile()
        {
            if (!Directory.Exists(Path.GetTempPath() + "\\MCT"))
                Directory.CreateDirectory(Path.GetTempPath() + "\\MCT");
        }
        static void EndCompile()
        {
            Directory.Delete(Path.GetTempPath() + "\\MCT", true);
        }
        static CompilerOutput MainCompileStuff(Assembly asm)
        {
            // checker, writer
            // don't forget to set outputFile

            EndCompile();
            
            return null;
        }
    }
}
