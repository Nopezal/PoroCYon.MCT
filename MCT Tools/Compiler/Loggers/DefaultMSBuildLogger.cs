using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Xna.Framework;

namespace PoroCYon.MCT.Tools.Compiler.Loggers
{
    /// <summary>
    /// An <see cref="ILogger" /> meant to be returned from <see cref="MctLogger.GetMSBuildLogger" />.
    /// </summary>
    public class DefaultMSBuildLogger(MctLogger logger, ModCompiler compiler) : ILogger
    {
        /// <summary>
        /// Gets the <see cref="MctLogger" /> that is collecting the log messages.
        /// </summary>
        public MctLogger Logger
        {
            get;
        } = logger;
        /// <summary>
        /// Gets the <see cref="ModCompiler" /> that is currently logging.
        /// </summary>
        public ModCompiler Compiler
        {
            get;
            private set;
        } = compiler;

        /// <summary>
        /// Gets whether the build succeeded or not.
        /// </summary>
        public bool Succeeded
        {
            get;
            private set;
        } = true;

        /// <summary>
        /// Gets or sets the user-defined parameters of the logger.
        /// </summary>
        public string Parameters
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the level of detail to show in the event log.
        /// </summary>
        public LoggerVerbosity Verbosity
        {
            get
            {
                return Logger.Verbosity;
            }
            set
            {
                Logger.Verbosity = value;
            }
        }

        /// <summary>
        /// Subscribes loggers to specific events. This method is called when the logger is registered with the build engine, before any events are raised.
        /// </summary>
        /// <param name="es">The events available to loggers.</param>
        public void Initialize(IEventSource es)
        {
            Logger.Log("Initializing MSBuild logger, Parameters=" + Parameters, MessageImportance.Normal);

            es.ErrorRaised += (s, e) =>
            {
                Logger.LogError(new CompilerException(Compiler.CreateOutput(new List<CompilerError>
                {
                    new CompilerError(Compiler.building)
                    {
                        FilePath = e.File,
                        IsWarning = false,
                        LocationInFile = new Point(e.LineNumber, e.ColumnNumber),
                        Message = e.Code + ": " + e.Message + " (in project " + e.ProjectFile + ")"
                    }
                }).ToString()), "MSBuild error.");
            };
            es.WarningRaised += (s, e) =>
            {
                Logger.LogError(new CompilerWarning(Compiler.CreateOutput(new List<CompilerError>
                {
                    new CompilerError(Compiler.building)
                    {
                        FilePath = e.File,
                        IsWarning = true,
                        LocationInFile = new Point(e.LineNumber, e.ColumnNumber),
                        Message = e.Code + ": " + e.Message + " (in project " + e.ProjectFile + ")"
                    }
                }).ToString()), "MSBuild warning.");
            };

            es.BuildStarted   += (s, e) => Logger.Log("Build started: "   + e.Message, MessageImportance.Low   );
            es.ProjectStarted += (s, e) => Logger.Log("Project started: " + e.Message, MessageImportance.Normal);
            es.TargetStarted  += (s, e) => Logger.Log("Target started: "  + e.Message, MessageImportance.Low   );
            es.TaskStarted    += (s, e) => Logger.Log("Task started: "    + e.Message, MessageImportance.Low   );

            es.BuildFinished   += (s, e) =>
            {
                Logger.Log("Build finished, Succeeded="   + e.Succeeded + ": " + e.Message, MessageImportance.Low   );

                Succeeded &= e.Succeeded;
            };
            es.ProjectFinished += (s, e) =>
            {
                Logger.Log("Project finished, Succeeded=" + e.Succeeded + ": " + e.Message, MessageImportance.Normal);

                Succeeded &= e.Succeeded;
            };
            es.TargetFinished  += (s, e) =>
            {
                Logger.Log("Target finished, Succeeded="  + e.Succeeded + ": " + e.Message, MessageImportance.Low   );

                //succeeded &= e.Succeeded;
            };
            es.TaskFinished    += (s, e) =>
            {
                Logger.Log("Task finished, Succeeded="    + e.Succeeded + ": " + e.Message, MessageImportance.Low   );

                //succeeded &= e.Succeeded;
            };

            es.MessageRaised     += (s, e) => Logger.Log("Message: "      + e.Message, MessageImportance.Low);
            es.StatusEventRaised += (s, e) => Logger.Log("Status event: " + e.Message, MessageImportance.Low);
        }

        /// <summary>
        /// Releases the resources allocated to the logger at the time of initialization or during the build. This method is called when the logger is unregistered from the engine, after all events are raised. A host of MSBuild typically unregisters loggers immediately before quitting.
        /// </summary>
        public void Shutdown()
        {
            Logger.Log("Shutting down MSBuild logger, Succeeded=" + Succeeded + ", Parameters=" + Parameters, MessageImportance.Normal);
        }
    }
}
