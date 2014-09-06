using System;
using System.Collections.Generic;
using System.Linq;
using PoroCYon.Extensions;
using PoroCYon.MCT.Tools.Compiler;

namespace PoroCYon.MCT.Tools.Compiler
{
    /// <summary>
    /// A subroutine of the <see cref="ModCompiler" />.
    /// </summary>
    public abstract class CompilerPhase(ModCompiler mc)
    {
        WeakReference<ModCompiler> compiler_wr = new WeakReference<ModCompiler>(mc);

        /// <summary>
        /// Gets the <see cref="ModCompiler" /> that started the current compile action.
        /// </summary>
        protected ModCompiler Compiler
        {
            get
            {
                if (!compiler_wr.IsAlive)
                    throw new ObjectDisposedException("compiler");

                return compiler_wr.Target;
            }
        }
        /// <summary>
        /// Gets the mod that is currently building.
        /// </summary>
        /// <remarks>Some properties might be empty, depending on the current phase.</remarks>
        protected ModData Building
        {
            get
            {
                return Compiler.building;
            }
        }
    }
}
