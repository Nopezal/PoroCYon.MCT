using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TAPI;

namespace PoroCYon.MCT.Tools.Internal
{
    static class Checker
    {
        // only returns warnings, except for modbase not found.
        internal static IEnumerable<CompilerError> Check(Assembly asm)
        {
            List<CompilerError> errors = new List<CompilerError>();

            bool foundModBase = false;

            foreach (Type t in asm.GetTypes())
                if (t.IsSubclassOf(typeof(ModBase)))
                    foundModBase = true;

            if (!foundModBase)
                errors.Add(new CompilerError()
                {
                    Cause = new TypeLoadException(),
                    FilePath = asm.Location,
                    IsWarning = false,
                    Message = "No ModBase class found."
                });

            // maybe check other stuff, too

            return errors;
        }
    }
}
