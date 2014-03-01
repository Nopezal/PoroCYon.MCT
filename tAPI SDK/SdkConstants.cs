using System;
using System.Collections.Generic;
using System.Linq;

namespace TAPI.SDK
{
    /// <summary>
    /// Common constants of the tAPI SDK
    /// </summary>
    public static class SdkConstants
    {
        /// <summary>
        /// The version of the tAPI SDK as a string
        /// </summary>
        public const string VERSION_STRING = "1.0.0.0";
        /// <summary>
        /// The version of the tAPI SDK as a <see cref="System.Version"/>
        /// </summary>
        public readonly static Version VERSION = new Version(VERSION_STRING);
        /// <summary>
        /// The version number of the tAPI SDK
        /// </summary>
        public const uint VERSION_NUM = 1u;
    }
}
