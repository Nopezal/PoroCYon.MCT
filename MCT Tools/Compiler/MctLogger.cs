using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Framework;
using PoroCYon.Extensions;

namespace PoroCYon.MCT.Tools.Compiler
{
    /// <summary>
    /// A logger for the <see cref="ModCompiler" />.
    /// </summary>
    public abstract class MctLogger
    {
        internal WeakReference<ModCompiler> compiler_wr;

        /// <summary>
        /// Gets the compiler that is logging the output.
        /// </summary>
        protected ModCompiler Compiler
        {
            get
            {
                if (!compiler_wr.IsAlive)
                    throw new ObjectDisposedException("Compiler");

                return compiler_wr.Target;
            }
        }

        /// <summary>
        /// Gets or sets the verbosity of the <see cref="MctLogger" />.
        /// </summary>
        public virtual LoggerVerbosity Verbosity
        {
            get;
            set;
        } = LoggerVerbosity.Normal;

        /// <summary>
        /// Logs a message with a gien importance.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="importance">The importance of the message.</param>
        public abstract void Log     (string message, MessageImportance importance);
        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="exception">The error to log.</param>
        /// <param name="comment">An optional comment to the error.</param>
        public abstract void LogError(Exception exception, string comment = null);
        /// <summary>
        /// Logs the build result.
        /// </summary>
        /// <param name="output">The result of the build.</param>
        public abstract void LogResult(CompilerOutput output);

        /// <summary>
        /// Gets an MSBuild logger for the <see cref="MctLogger" />.
        /// </summary>
        /// <returns>An <see cref="ILogger" /> that will report progress on an MSBuild compilation. Null to report nothing.</returns>
        public abstract ILogger GetMSBuildLogger();
    }
}
