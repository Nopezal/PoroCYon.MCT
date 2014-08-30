using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using PoroCYon.Extensions.Collections;

namespace PoroCYon.MCT.Tools.Compiler
{
    /// <summary>
    /// An error occured while compiling a mod.
    /// </summary>
    [Serializable]
    public class CompilerException : Exception
    {
        internal readonly static string DEFAULT_MSG = "Something went wrong when compiling a mod.";

        /// <summary>
        /// Creates a new instance of the CompilerException class
        /// </summary>
        public CompilerException()
            : base(DEFAULT_MSG)
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
            : base(DEFAULT_MSG, inner)
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

        /// <summary>
        /// Creates a new instance of the <see cref="CompilerException" /> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo" /> that holds the serialized object data about the <see cref="Exception" /> being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="info" /> parameter is null.</exception>
        /// <exception cref="SerializationException">The class name is null or <see cref="Exception.HResult" /> is zero (0).</exception>
        protected CompilerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }

    /// <summary>
    /// An error occured when compiling a mod, caused by a circular reference.
    /// </summary>
    [Serializable]
    public class CircularReferenceException : CompilerException
    {
        internal readonly static new string DEFAULT_MSG = "The mod or one of its references contains a circular reference.";

        /// <summary>
        /// Creates a new instance of the CircularReferenceException class.
        /// </summary>
        public CircularReferenceException()
            : base(DEFAULT_MSG)
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
            : base(DEFAULT_MSG, inner)
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
            : base(DEFAULT_MSG + " Reference chain: " + referenceChain.Join(" -> "))
        {

        }

        /// <summary>
        /// Creates a new instance of the <see cref="CircularReferenceException" /> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo" /> that holds the serialized object data about the <see cref="Exception" /> being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="info" /> parameter is null.</exception>
        /// <exception cref="SerializationException">The class name is null or <see cref="Exception.HResult" /> is zero (0).</exception>
        protected CircularReferenceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
    /// <summary>
    ///An error occured while compiling a mod, caused by a content object could not be found.
    /// </summary>
    [Serializable]
    public class ObjectNotFoundException : CompilerException
    {
        internal readonly static new string DEFAULT_MSG = "An object could not be found.";
        internal readonly static string
            MSG_BEGIN = "Object '",
            MSG_END   = "' could not be found.";

        /// <summary>
        /// Creates a new instance of the <see cref="ObjectNotFoundException" /> class.
        /// </summary>
        public ObjectNotFoundException()
            : this(DEFAULT_MSG)
        {

        }
        /// <summary>
        /// Creates a new instance of the <see cref="ObjectNotFoundException" /> class.
        /// </summary>
        /// <param name="inner">The <see cref="Exception" /> that caused this <see cref="Exception" />.</param>
        public ObjectNotFoundException(Exception inner)
            : this(DEFAULT_MSG, inner)
        {

        }
        /// <summary>
        /// Creates a new instance of the <see cref="ObjectNotFoundException" /> class.
        /// </summary>
        /// <param name="message">The message of the <see cref="Exception" />.</param>
        /// <param name="inner">The <see cref="Exception" /> that caused this <see cref="Exception" />.</param>
        public ObjectNotFoundException(string message, Exception inner)
            : base(message, inner)
        {

        }

        /// <summary>
        /// Creates a new instance of the <see cref="ObjectNotFoundException" /> class.
        /// </summary>
        /// <param name="objName">The name of the missing object.</param>
        public ObjectNotFoundException(string objName)
            : this(MSG_BEGIN + objName + MSG_END, null)
        {

        }

        /// <summary>
        /// Creates a new instance of the <see cref="ObjectNotFoundException" /> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo" /> that holds the serialized object data about the <see cref="Exception" /> being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="info" /> parameter is null.</exception>
        /// <exception cref="SerializationException">The class name is null or <see cref="Exception.HResult" /> is zero (0).</exception>
        protected ObjectNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
    /// <summary>
    /// An error occured while compiling a mod, caused by a field containing an illegal value.
    /// </summary>
    [Serializable]
    public class ValueNotAllowedException : CompilerException
    {
        internal readonly static new string DEFAULT_MSG = "A certain value is not allowed.";

        /// <summary>
        /// Creates a new instance of the <see cref="ValueNotAllowedException" />.
        /// </summary>
        public ValueNotAllowedException()
            : this(DEFAULT_MSG)
        {

        }
        /// <summary>
        /// Creates a new instance of the <see cref="ValueNotAllowedException" />.
        /// </summary>
        /// <param name="message">The message of the <see cref="Exception" />.</param>
        public ValueNotAllowedException(string message)
            : this(message, (Exception)null)
        {

        }
        /// <summary>
        /// Creates a new instance of the <see cref="ValueNotAllowedException" />.
        /// </summary>
        /// <param name="inner">The <see cref="Exception" /> that caused this <see cref="Exception" />.</param>
        public ValueNotAllowedException(Exception inner)
            : base(DEFAULT_MSG, inner)
        {

        }
        /// <summary>
        /// Creates a new instance of the <see cref="ValueNotAllowedException" />.
        /// </summary>
        /// <param name="message">The message of the <see cref="Exception" />.</param>
        /// <param name="inner">The <see cref="Exception" /> that caused this <see cref="Exception" />.</param>
        public ValueNotAllowedException(string message, Exception inner)
            : base(message, inner)
        {

        }

        /// <summary>
        /// Creates a new instance of the <see cref="ValueNotAllowedException" />.
        /// </summary>
        /// <param name="fieldName">The name of the field that contains the illegal value.</param>
        /// <param name="value">The string representation of the illegal value.</param>
        public ValueNotAllowedException(string fieldName, string value)
            : this("'" + fieldName + "' has an invalid value: '" + value + "'.")
        {

        }

        /// <summary>
        /// Creates a new instance of the <see cref="ValueNotAllowedException" /> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo" /> that holds the serialized object data about the <see cref="Exception" /> being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="info" /> parameter is null.</exception>
        /// <exception cref="SerializationException">The class name is null or <see cref="Exception.HResult" /> is zero (0).</exception>
        protected ValueNotAllowedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
