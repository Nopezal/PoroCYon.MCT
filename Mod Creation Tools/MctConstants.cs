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
        public const string VERSION_STRING = "1.0.3.0";
        /// <summary>
        /// The version of the MCT as a <see cref="System.Version"/>
        /// </summary>
        public readonly static Version VERSION = new Version(VERSION_STRING);
        /// <summary>
        /// The version number of the MCT
        /// </summary>
        public const uint VERSION_NUM = 1u;
    }
}
