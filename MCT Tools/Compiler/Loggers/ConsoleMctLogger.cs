using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using PoroCYon.Extensions;

namespace PoroCYon.MCT.Tools.Compiler.Loggers
{
    /// <summary>
    /// A logger for the <see cref="ModCompiler" /> that pipes output to the console window.
    /// </summary>
    public class ConsoleMctLogger : MctLogger
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ConsoleMctLogger" /> class.
        /// </summary>
        public ConsoleMctLogger()
            : this(LoggerVerbosity.Quiet)
        {

        }
        /// <summary>
        /// Creates a new instance of the <see cref="ConsoleMctLogger" /> class.
        /// </summary>
        /// <param name="verbosity">The verbosity of the <see cref="MctLogger" />.</param>
        public ConsoleMctLogger(LoggerVerbosity verbosity)
        {
            Verbosity = verbosity;
        }

        /// <summary>
        /// Logs a message with a gien importance.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="importance">The importance of the message.</param>
        public override void Log(string message, MessageImportance importance)
        {
            if (Verbosity >= (LoggerVerbosity)((int)LoggerVerbosity.Minimal + (int)importance))
                Console.WriteLine(message);
        }
        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="exception">The error to log.</param>
        /// <param name="comment">An optional comment to the error.</param>
        public override void LogError(Exception exception, string comment = null)
        {
            if (Verbosity >= LoggerVerbosity.Minimal)
            {
                Console.Error.Write(exception);

                if (!comment.IsEmpty())
                    Console.Error.Write(" (" + comment + ")");

                Console.Error.WriteLine();
            }
        }
        /// <summary>
        /// Logs the build result.
        /// </summary>
        /// <param name="output">The result of the build.</param>
        public override void LogResult(CompilerOutput output)
        {
            if (Verbosity >= LoggerVerbosity.Quiet)
            {
                TextWriter w = output.Succeeded ? Console.Out : Console.Error;

                w.WriteLine(output.ToString());
            }
        }

        /// <summary>
        /// Gets an MSBuild logger for the <see cref="MctLogger" />.
        /// </summary>
        /// <returns>An <see cref="ILogger" /> that will report progress on an MSBuild compilation. Null to report nothing.</returns>
        public override ILogger GetMSBuildLogger()
        {
            return null;
        }
    }
}
