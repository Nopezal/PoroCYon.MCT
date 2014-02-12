using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TAPI.SDK.Content
{
    /// <summary>
    /// Common parameter values for ObjectLoader methods
    /// </summary>
    public struct LoadParameters
    {
        /// <summary>
        /// The ModBase of the parameters
        /// (not required for some methods)
        /// </summary>
        public ModBase ModBase;
        /// <summary>
        /// The mod assembly of the parameters
        /// (not required for some methods)
        /// </summary>
        public Assembly Assembly;
        /// <summary>
        /// The name of the object to be added
        /// </summary>
        public string Name;
        /// <summary>
        /// The name of the code class (which is in the given assembly)
        /// (not required for some methods)
        /// </summary>
        public string SubClassTypeName;

        /// <summary>
        /// Creates a new instance of the LoadParameters structure
        /// </summary>
        /// <param name="name">Sets the Name field</param>
        /// <param name="base">Sets the ModBase field</param>
        /// <param name="subClassName">Sets the SubClassTypeName field</param>
        /// <param name="asm">Sets the Assembly field</param>
        public LoadParameters(string name, ModBase @base, string subClassName, Assembly asm)
        {
            Name = name;
            ModBase = @base;
            SubClassTypeName = subClassName;
            Assembly = asm;
        }
    }
}
