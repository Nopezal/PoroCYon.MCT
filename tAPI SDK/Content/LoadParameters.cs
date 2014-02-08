using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TAPI.SDK.Content
{
    public struct LoadParameters
    {
        public ModBase ModBase;
        public Assembly Assembly;
        public string Name;
        public string SubClassTypeName;

        public LoadParameters(string name, ModBase @base, string subClassName, Assembly asm)
        {
            Name = name;
            ModBase = @base;
            SubClassTypeName = subClassName;
            Assembly = asm;
        }
    }
}
