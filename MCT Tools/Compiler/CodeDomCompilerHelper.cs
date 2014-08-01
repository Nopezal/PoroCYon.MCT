using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Ionic.Zip;
using PoroCYon.Extensions.IO;
using PoroCYon.MCT.Tools.Compiler.Validation;

namespace PoroCYon.MCT.Tools.Compiler
{
    using CodeDomError = System.CodeDom.Compiler.CompilerError;

    /// <summary>
    /// An object that helps with the compilation of <see cref="System.CodeDom.Compiler.CodeDomProvider" /> compilers.
    /// </summary>
    public abstract class CodeDomCompilerHelper : ICompiler
    {
        /// <summary>
        /// Gets the mod which is being compiled.
        /// </summary>
        protected ModData Mod
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets all file extensions which belong to the compiled language.
        /// </summary>
        public abstract string[] FileExtensions
        {
            get;
        }
        /// <summary>
        /// Gets the paths to all assemblies where the compiler's language depends on.
        /// </summary>
        public abstract string[] LanguageDependancyAssemblies
        {
            get;
        }
        /// <summary>
        /// Gets all possible names of the compiler's language (used in ModInfo.json)
        /// </summary>
        public abstract string[] LanguageNames
        {
            get;
        }

        /// <summary>
        /// Compiles all source files of the mod into a managed assembly.
        /// </summary>
        /// <param name="mod">The mod to compile.</param>
        /// <returns>
        /// A tuple containing the built assembly (null if failed), and a collection of all errors.
        /// The assembly must be saved on the disk, because the .pdb file must be loaded afterwards.
        /// </returns>
        public Tuple<Assembly, IEnumerable<CompilerError>> Compile(ModData mod)
        {
            Mod = mod;

            List<CompilerError> errors = new List<CompilerError>();

            CodeDomProvider cdp = CreateCompiler();

            CompilerParameters cp = new CompilerParameters();
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = false;
            cp.IncludeDebugInformation = mod.Info.includePDB;
            cp.OutputAssembly = Path.GetTempPath() + "MCT\\" + mod.Info.internalName + ".dll";
            cp.WarningLevel = mod.Info.warningLevel;

            cp.ReferencedAssemblies.Add("mscorlib.dll");
            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("System.Core.dll");
            cp.ReferencedAssemblies.Add("System.Numerics.dll");
            cp.ReferencedAssemblies.Add("System.Xml.dll");

            cp.ReferencedAssemblies.Add("System.Drawing.dll");
            cp.ReferencedAssemblies.Add("System.Windows.Forms.dll");

            cp.ReferencedAssemblies.Add("Microsoft.Xna.Framework.dll");
            cp.ReferencedAssemblies.Add("Microsoft.Xna.Framework.Game.dll");
            cp.ReferencedAssemblies.Add("Microsoft.Xna.Framework.Graphics.dll");
            cp.ReferencedAssemblies.Add("Microsoft.Xna.Framework.Xact.dll");

            cp.ReferencedAssemblies.AddRange(LanguageDependancyAssemblies);

            cp.ReferencedAssemblies.Add("tAPI.exe"); // hmm hmm hmm

            cp.ReferencedAssemblies.AddRange(mod.Info.dllReferences);
            cp.ReferencedAssemblies.AddRange(mod.Info.modReferences);

            for (int i = 0; i < mod.Info.modReferences.Length; i++)
                try
                {
                    // presence of the mod is checked in ModInfo validation
                    WriteAssembly(ModCompiler.modDict[mod.Info.modReferences[i]], Path.GetTempPath() + "\\MCT\\" + mod.Info.modReferences[i] + ".dll");
                }
                catch (Exception e)
                {
                    errors.Add(new CompilerError()
                    {
                        Cause = e,
                        FilePath = Path.GetTempPath() + "MCT\\" + mod.Info.modReferences[i] + ".dll",
                        IsWarning = false,
                        Message = "Something went wrong when extracting the mod's assembly. See the exception for more details."
                    });
                }

            ModifyCompilerParameters(cp);

            var files = GetFiles();

            for (int i = 0; i < files.Length; i++)
                files[i] = mod.OriginPath + "\\" + files[i].Replace('/', '\\');

            CompilerResults cr = cdp.CompileAssemblyFromFile(cp, files);

            foreach (CodeDomError ce in cr.Errors)
                errors.Add(new CompilerError()
                {
                    Cause = new CompilerException(ce.ErrorNumber),
                    FilePath = ce.FileName,
                    IsWarning = ce.IsWarning,
                    LocationInFile = new Point(ce.Column, ce.Line),
                    Message = (ce.IsWarning ? "Warning" : "Error") + " " + ce.ErrorNumber + ": " + ce.ErrorText,
                });

            return new Tuple<Assembly, IEnumerable<CompilerError>>(cr.Errors.HasErrors ? null : cr.CompiledAssembly, errors);
        }

        /// <summary>
        /// Creates a CodeDomProvider compiler.
        /// </summary>
        /// <returns>A CodeDomProvider compiler</returns>
        protected abstract CodeDomProvider CreateCompiler();

        /// <summary>
        /// Gets all source files.
        /// </summary>
        protected virtual string[] GetFiles()
        {
            List<string> ret = new List<string>();

            foreach (string fileName in Mod.Files.Keys)
                for (int i = 0; i < FileExtensions.Length; i++)
                    if (fileName.EndsWith(FileExtensions[i]))
                        ret.Add(fileName);

            return ret.ToArray();
        }
        /// <summary>
        /// Modifies the compiler parameters.
        /// </summary>
        /// <param name="cp">The compiler parameters to modify.</param>
        protected virtual void ModifyCompilerParameters(CompilerParameters cp) { }

        static void WriteAssembly(string modFile, string assemblyFileName)
        {
            if (modFile.EndsWith(".tapi"))
            {
                using (ZipFile zf = ZipFile.Read(modFile))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        zf["Mod.tapimod"].Extract(ms);
                        ms.Position = 0L;

                        File.WriteAllBytes(assemblyFileName + ".tapimod", ms.ToArray());

                        WriteAssembly(assemblyFileName + ".tapimod", assemblyFileName);

                        File.Delete(assemblyFileName + ".tapimod");
                    }
                }

                return;
            }

            using (FileStream fs = new FileStream(modFile, FileMode.Open))
            {
                using (BinBuffer bb = new BinBuffer(new BinBufferStreamResource(fs)))
                {
                    bb.ReadInt32();
                    bb.ReadString();

                    int
                        num = bb.ReadInt32(),
                        num2 = 0;

                    while (num-- > 0)
                    {
                        bb.ReadString();
                        num2 += bb.ReadInt32();
                    }
                    while (num2-- > 0)
                        bb.ReadByte();

                    File.WriteAllBytes(assemblyFileName, bb.ReadBytes(bb.Left));
                }
            }
        }
    }
}
