using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Build.Framework;

namespace PoroCYon.MCT.Tools.Compiler.Loggers
{
    public class DebugMctLogger : MctLogger
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DebugMctLogger" /> class.
        /// </summary>
        public DebugMctLogger()
            : this(LoggerVerbosity.Quiet)
        {

        }
        /// <summary>
        /// Creates a new instance of the <see cref="DebugMctLogger" /> class.
        /// </summary>
        /// <param name="verbosity">The verbosity of the <see cref="MctLogger" />.</param>
        public DebugMctLogger(LoggerVerbosity verbosity)
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
                Debug.WriteLine(message);
        }
        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="exception">The error to log.</param>
        /// <param name="comment">An optional comment to the error.</param>
        public override void LogError(Exception exception, string comment = null)
        {
            if (Verbosity >= LoggerVerbosity.Minimal)
                Debug.Fail(comment ?? "No comment.", exception.ToString());
        }
        /// <summary>
        /// Logs the build result.
        /// </summary>
        /// <param name="output">The result of the build.</param>
        public override void LogResult(CompilerOutput output)
        {
            if (Verbosity >= LoggerVerbosity.Quiet)
            {
                if (output.Succeeded)
                    Debug.WriteLine(output);
                else
                    Debug.Fail("Build failed.", output.ToString());
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
