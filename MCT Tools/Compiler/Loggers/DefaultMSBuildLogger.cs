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
    public class DefaultMSBuildLogger : ILogger
    {
		readonly static string
			Init    = "Initializing MSBuild logger, Parameters=",
			Error   = "MSBuild error.",
            Warning = "MSBuild warning.",

			Colon = ": ",
            Exec = "EXEC",

			BuildStarted   = "Build started: "  ,
			ProjectStarted = "Project started: ",
			TargetStarted  = "Target started: " ,
			TaskStarted    = "Task started: "   ,

			BuildFinished   = "Build finished, Succeeded="  ,
			ProjectFinished = "Project finished, Succeeded=",
			TargetFinished  = "Target finished, Succeeded=" ,
			TaskFinished    = "Task finished, Succeeded="   ,

			Message     = "Message: "     ,
            StatusEvent = "Status event: ";

        readonly static Point EmptyPt = new Point(-1, -1);

        /// <summary>
        /// Gets the <see cref="MctLogger" /> that is collecting the log messages.
        /// </summary>
        public MctLogger Logger
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets the <see cref="ModCompiler" /> that is currently logging.
        /// </summary>
        public ModCompiler Compiler
        {
            get;
            private set;
        }

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
        /// Creates a new instance of the <see cref="DefaultMSBuildLogger" /> class.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="mc"></param>
        public DefaultMSBuildLogger(MctLogger logger, ModCompiler mc)
        {
            Logger = logger;
            Compiler = mc;
        }

        /// <summary>
        /// Subscribes loggers to specific events. This method is called when the logger is registered with the build engine, before any events are raised.
        /// </summary>
        /// <param name="es">The events available to loggers.</param>
        public void Initialize(IEventSource es)
        {
            Logger.Log(ConcatStringBuilder(Init, Parameters), MessageImportance.Normal);

            es.ErrorRaised += (s, e) =>
            {
                Logger.LogError(new CompilerException(Compiler.CreateOutput(new List<CompilerError>
                {
                    new CompilerError(Compiler.building)
                    {
                        FilePath = e.File == Exec ? String.Empty : e.File,
                        IsWarning = false,
                        LocationInFile = e.File == Exec ? EmptyPt : new Point(e.LineNumber, e.ColumnNumber),
                        Message = ConcatStringBuilder(e.Code, e.Message, " (in project ", e.ProjectFile, ")")
                    }
                }).ToString()), Error);
            };
            es.WarningRaised += (s, e) =>
            {
                Logger.LogError(new CompilerWarning(Compiler.CreateOutput(new List<CompilerError>
                {
                    new CompilerError(Compiler.building)
                    {
                        FilePath = e.File,
                        IsWarning = true,
                        LocationInFile = e.File == Exec ? EmptyPt : new Point(e.LineNumber, e.ColumnNumber),
                        Message = ConcatStringBuilder(e.Code, e.Message, " (in project ", e.ProjectFile, ")")
                    }
                }).ToString()), Warning);
            };

            es.BuildStarted   += (s, e) => Logger.Log(ConcatStringBuilder(BuildStarted  , e.Message), MessageImportance.Low   );
            es.ProjectStarted += (s, e) => Logger.Log(ConcatStringBuilder(ProjectStarted, e.Message), MessageImportance.Normal);
            es.TargetStarted  += (s, e) => Logger.Log(ConcatStringBuilder(TargetStarted , e.Message), MessageImportance.Low   );
            es.TaskStarted    += (s, e) => Logger.Log(ConcatStringBuilder(TaskStarted   , e.Message), MessageImportance.Low   );

            es.BuildFinished   += (s, e) =>
            {
                Logger.Log(ConcatStringBuilder(BuildFinished  , e.Succeeded, Colon, e.Message), MessageImportance.Low   );

                Succeeded &= e.Succeeded;
            };
            es.ProjectFinished += (s, e) =>
            {
				Logger.Log(ConcatStringBuilder(ProjectFinished, e.Succeeded, Colon, e.Message), MessageImportance.Normal);

                Succeeded &= e.Succeeded;
            };
            es.TargetFinished  += (s, e) =>
            {
                Logger.Log(ConcatStringBuilder(TargetFinished , e.Succeeded, Colon, e.Message), MessageImportance.Low   );

                //succeeded &= e.Succeeded;
            };
            es.TaskFinished    += (s, e) =>
            {
                Logger.Log(ConcatStringBuilder(TaskFinished   , e.Succeeded, Colon, e.Message), MessageImportance.Low   );

                //succeeded &= e.Succeeded;
            };

            es.MessageRaised     += (s, e) => Logger.Log(ConcatStringBuilder(Message    , e.Message), MessageImportance.Low);
            es.StatusEventRaised += (s, e) => Logger.Log(ConcatStringBuilder(StatusEvent, e.Message), MessageImportance.Low);
        }

		static string ConcatStringBuilder(params object[] toConcat)
		{
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < toConcat.Length; i++)
				sb.Append(toConcat[i]);

			return sb.ToString();
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
