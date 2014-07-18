using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PoroCYon.MCT.Tools.Compiler
{
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
                {
                    bool hasWarnings = false;

                    for (int i = 0; i < errors.Count; i++)
                        if (errors[i].IsWarning)
                        {
                            hasWarnings = true;
                            break;
                        }

                    s += "Errors" + (hasWarnings ? "/Warnings" : String.Empty) + ": " + errors.Count;
                }

                s += Environment.NewLine + Environment.NewLine;

                for (int i = 0; i < errors.Count; i++)
                    s += errors[i] + Environment.NewLine + Environment.NewLine;
            }

            return s;
        }
    }
}
