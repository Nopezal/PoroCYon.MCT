using System;
using System.Collections.Generic;
using System.Linq;
using PoroCYon.Extensions.Collections;

namespace PoroCYon.MCT.Tools.Compiler
{
    /// <summary>
    /// An error occured while compiling a mod.
    /// </summary>
    [Serializable]
    public class CompilerException : Exception
    {
        internal readonly static string DefaultMessage = "Something went wrong when compiling a mod.";

        /// <summary>
        /// Creates a new instance of the CompilerException class
        /// </summary>
        public CompilerException()
            : base(DefaultMessage)
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
            : base(DefaultMessage, inner)
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

    /// <summary>
    /// An error occured when compiling a mod, caused by a circular reference.
    /// </summary>
    [Serializable]
    public class CircularReferenceException : CompilerException
    {
        internal readonly static new string DefaultMessage = "The mod or one of its references contains a circular reference.";

        /// <summary>
        /// Creates a new instance of the CircularReferenceException class.
        /// </summary>
        public CircularReferenceException()
            : base(DefaultMessage)
        {

        }
        /// <summary>
        /// Creates a new instance of the CircularReferenceException class.
        /// </summary>
        /// <param name="message">The message of the Exception.</param>
        public CircularReferenceException(string message)
            : base(message)
        {

        }
        /// <summary>
        /// Creates a new instance of the CircularReferenceException class.
        /// </summary>
        /// <param name="inner">The Exception which caused this Exception.</param>
        public CircularReferenceException(Exception inner)
            : base(DefaultMessage, inner)
        {

        }
        /// <summary>
        /// Creates a new instance of the CircularReferenceException class.
        /// </summary>
        /// <param name="message">The message of the Exception.</param>
        /// <param name="inner">The Exception which caused this Exception.</param>
        public CircularReferenceException(string message, Exception inner)
            : base(message, inner)
        {

        }
        /// <summary>
        /// Creates a new instance of the CircularReferenceException class.
        /// </summary>
        /// <param name="referenceChain">The reference chain that represents the circular reference.</param>
        public CircularReferenceException(IEnumerable<string> referenceChain)
            : base(DefaultMessage + " Reference chain: " + referenceChain.Join(" -> "))
        {

        }
    }
}
