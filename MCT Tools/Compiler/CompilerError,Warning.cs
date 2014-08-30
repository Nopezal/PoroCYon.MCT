using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace PoroCYon.MCT.Tools.Compiler
{
    /// <summary>
    /// An error during compile-time containing detailed information of what went wrong.
    /// </summary>
    public class CompilerError
    {
        /// <summary>
        /// Gets the error message of the CompilerError.
        /// </summary>
        public string Message
        {
            get;
            internal set;
        } = CompilerException.DEFAULT_MSG;
        /// <summary>
        /// Gets the absolute path to the file causing the error. The value is null if the file is not known.
        /// </summary>
        public string FilePath
        {
            get;
            internal set;
        }
        /// <summary>
        /// Gets wether the error is actually a warning or not.
        /// </summary>
        public bool IsWarning
        {
            get;
            internal set;
        }
        /// <summary>
        /// Gets the column and line number of the error.
        /// </summary>
        public Point LocationInFile
        {
            get;
            internal set;
        } = new Point(-1, -1);
        /// <summary>
        /// Gets the cause of the error. It is highly possible that it is an instance of the
        /// <see cref="PoroCYon.MCT.Tools.Compiler.CompilerException" />, <see cref="PoroCYon.MCT.Tools.Compiler.CompilerWarning" />
        /// or <see cref="System.IO.IOException" /> classes.
        /// </summary>
        public Exception Cause
        {
            get;
            internal set;
        } = new CompilerException();

        /// <summary>
        /// Returns the string representation of the current instance.
        /// </summary>
        /// <returns>The string representation of the current instance.</returns>
        public override string ToString()
        {
            return (IsWarning ? "Warning: " : "Error: ") +
                Message + Environment.NewLine +
                FilePath +
                (LocationInFile.X >= 0 && LocationInFile.Y >= 0
                    ? " (" + LocationInFile.X + ";" + LocationInFile.Y + ")"
                    : String.Empty) +
                Environment.NewLine + Environment.NewLine +
                Cause;
        }
    }

    /// <summary>
    /// The modder is doing pretty unsafe things. Not meant to be thrown.
    /// </summary>
    [Serializable]
    public class CompilerWarning : Exception
    {
        internal const string DEFAULT_MESSAGE = "Something might get a bit weird when you run the mod.";

        /// <summary>
        /// Creates a new instance of the CompilerWarning class
        /// </summary>
        public CompilerWarning()
            : base(DEFAULT_MESSAGE)
        {

        }
        /// <summary>
        /// Creates a new instance of the CompilerWarning class
        /// </summary>
        /// <param name="message">The message of the Exception.</param>
        public CompilerWarning(string message)
            : base(message)
        {

        }
        /// <summary>
        /// Creates a new instance of the CompilerWarning class
        /// </summary>
        /// <param name="inner">The Exception which caused this Exception.</param>
        public CompilerWarning(Exception inner)
            : base(DEFAULT_MESSAGE, inner)
        {

        }
        /// <summary>
        /// Creates a new instance of the CompilerWarning class
        /// </summary>
        /// <param name="message">The message of the Exception.</param>
        /// <param name="inner">The Exception which caused this Exception.</param>
        public CompilerWarning(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
