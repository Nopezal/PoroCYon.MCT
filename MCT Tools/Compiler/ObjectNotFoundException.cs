using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PoroCYon.MCT.Tools.Compiler
{
    /// <summary>
    /// An <see cref="Exception" /> thrown when a content object could not be found.
    /// </summary>
    [Serializable]
    public class ObjectNotFoundException : Exception
    {
        readonly static string
            DEFAULT_MSG = "An object could not be found.",
            MSG_BEGIN   = "Object '",
            MSG_END     = "' could not be found.";

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
        /// <param name="objName">The name of the missing object.</param>
        public ObjectNotFoundException(string objName)
            : this(MSG_BEGIN + objName + MSG_END, null)
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
}
