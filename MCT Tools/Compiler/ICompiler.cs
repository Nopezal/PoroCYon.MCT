using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PoroCYon.MCT.Tools
{
    /// <summary>
    /// An object that builds a mod's source code (not JSON) into a managed assembly.
    /// </summary>
    public interface ICompiler
    {
        /// <summary>
        /// Compiles all source files of the mod into a managed assembly.
        /// </summary>
        /// <param name="files">All non-json files the mod has.</param>
        /// <returns>
        /// A tuple containing the built assembly (null if failed), and a collection of all errors.
        /// The assembly must be saved on the disk, because the .pdb file must be loaded afterwards.
        /// </returns>
        Tuple<Assembly, IEnumerable<CompilerError>> Compile(Dictionary<string, byte[]> files);
    }
}
