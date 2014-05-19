using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Xna.Framework;

namespace PoroCYon.MCT.Tools
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
        }
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
        }
        /// <summary>
        /// Gets the cause of the error. It is highly possible that it is an instance of the
        /// <see cref="PoroCYon.MCT.Tools.CompilerException" />, <see cref="PoroCYon.MCT.Tools.CompilerWarning" />
        /// or <see cref="System.IO.IOException" /> classes.
        /// </summary>
        public Exception Cause
        {
            get;
            internal set;
        }

        internal CompilerError()
        {
            Message = CompilerException.DEFAULT_MESSAGE;
            FilePath = null;
            LocationInFile = new Point();
            Cause = new CompilerException();
        }

        /// <summary>
        /// Returns the string representation of the current instance.
        /// </summary>
        /// <returns>The string representation of the current instance.</returns>
        public override string ToString()
        {
            return (IsWarning ? "Warning: " : "Error: ") + Message + Environment.NewLine +
                FilePath + "(" + LocationInFile.X + ";" + LocationInFile.Y + ")" + Environment.NewLine +
                Cause;
        }
    }

    /// <summary>
    /// Results of a mod compilation
    /// </summary>
    public class CompilerOutput
    {
        internal string outputFile;
        internal List<CompilerError> errors;

        /// <summary>
        /// Gets wether the compilation succeeded or not.
        /// </summary>
        public bool Succeeded
        {
            get;
            internal set;
        }
        /// <summary>
        /// Gets the output file of the mod (when the compilation succeeded).
        /// </summary>
        public string OutputFile
        {
            get
            {
                if (!Succeeded)
                    throw new InvalidOperationException("The compilation failed, there is no output file.");

                return outputFile;
            }
        }
        /// <summary>
        /// Gets the information of what went wrong during compilation.
        /// If the compilation succeeded, the collection contains all warnings.
        /// </summary>
        public ReadOnlyCollection<CompilerError> Errors
        {
            get
            {
                return errors.AsReadOnly();
            }
        }

        /// <summary>
        /// Returns the string representation of the current instance.
        /// </summary>
        /// <returns>The string representation of the current instance.</returns>
        public override string ToString()
        {
            string s = "Build " + (Succeeded ? "Succeeded" : "Failed") + Environment.NewLine;

            if (Succeeded)
                s += "Output file: " + outputFile;

            if (Errors.Count > 0)
            {
                if (Succeeded)
                    s += "Warnings: " + errors.Count;
                else
                    s += "Errors/Warnings: " + errors.Count;

                s += Environment.NewLine;

                for (int i = 0; i < errors.Count; i++)
                    s += errors[i] + Environment.NewLine;
            }

            return s;
        }
    }
}
