using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT.Tools
{
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
