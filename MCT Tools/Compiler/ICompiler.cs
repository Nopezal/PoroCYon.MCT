using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PoroCYon.MCT.Tools.Compiler
{
    /// <summary>
    /// An object that builds a mod's source code (not JSON) into a managed assembly.
    /// </summary>
    public interface ICompiler
    {
        /// <summary>
        /// Gets all file extensions which belong to the compiled language.
        /// </summary>
        string[] FileExtensions
        {
            get;
        }
        /// <summary>
        /// Gets the paths to all assemblies where the compiler's language depends on.
        /// </summary>
        string[] LanguageDependancyAssemblies
        {
            get;
        }
        /// <summary>
        /// Gets all possible names of the compiler's language (used in ModInfo.json)
        /// </summary>
        string[] LanguageNames
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
        Tuple<Assembly, IEnumerable<CompilerError>> Compile(ModData mod);
    }
}
