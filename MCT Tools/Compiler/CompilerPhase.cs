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
    public abstract class CompilerPhase
    {
        WeakReference<ModCompiler> compiler_wr;

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

        /// <summary>
        /// Creates a new instance of the <see cref="CompilerPhase" /> class.
        /// </summary>
        /// <param name="mc"><see cref="Compiler" /></param>
        protected CompilerPhase(ModCompiler mc)
        {
            compiler_wr = new WeakReference<ModCompiler>(mc);
        }
    }
}
