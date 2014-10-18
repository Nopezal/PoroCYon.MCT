using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT
{
    /// <summary>
    /// Common constants of the MCT
    /// </summary>
    public static class MctConstants
    {
        /// <summary>
        /// The version of the MCT as a string
        /// </summary>
        public const string VERSION_STRING = "2.0.0.3";
        /// <summary>
        /// The version of the MCT as a <see cref="System.Version"/>
        /// </summary>
        public readonly static Version VERSION = new Version(VERSION_STRING);
    }
}
