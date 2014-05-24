using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT.Tools
{
    /// <summary>
    /// An error occured while compiling a mod.
    /// </summary>
    [Serializable]
    public class CompilerException : Exception
    {
        internal const string DEFAULT_MESSAGE = "Something went wrong when compiling a mod.";

        /// <summary>
        /// Creates a new instance of the CompilerException class
        /// </summary>
        public CompilerException()
            : base(DEFAULT_MESSAGE)
        {

        }
        /// <summary>
        /// Creates a new instance of the CompilerException class
        /// </summary>
        /// <param name="message">The message of the Exception.</param>
        public CompilerException(string message)
            : base(message)
        {

        }
        /// <summary>
        /// Creates a new instance of the CompilerException class
        /// </summary>
        /// <param name="inner">The Exception which caused this Exception.</param>
        public CompilerException(Exception inner)
            : base(DEFAULT_MESSAGE, inner)
        {

        }
        /// <summary>
        /// Creates a new instance of the CompilerException class
        /// </summary>
        /// <param name="message">The message of the Exception.</param>
        /// <param name="inner">The Exception which caused this Exception.</param>
        public CompilerException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
