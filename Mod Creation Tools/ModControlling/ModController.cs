using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LitJson;
using Terraria;
using TAPI;

namespace PoroCYon.MCT.ModControlling
{
	// TODO: obsolete?

    // Also hacky stuff. Lots of it.

    /// <summary>
    /// How to handle ModEntity attachments when there is already a ModEntity attached.
    /// </summary>
    public enum AttachMode
    {
        /// <summary>
        /// Set the ModEntity if there is no ModEntity attached. Otherwise, throw an exception.
        /// </summary>
        New,
        /// <summary>
        /// Overwrite the ModEntity if there is already one attached.
        /// </summary>
        Overwrite,
        /// <summary>
        /// Append the ModEntity to the array if there is already one attached.
        /// </summary>
        Append
    }

    /// <summary>
    /// Provides extension methods for mod hackery.
    /// </summary>
    public static class ModExtensions
    {
        readonly static FieldInfo timesLoadedInfo
            = typeof(ModBase).GetField("timesLoaded", BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// Gets the value of the internal field 'timesLoaded' of a <see cref="ModBase" /> instance.
        /// </summary>
        /// <param name="base">The <see cref="ModBase" /> which timesLoaded value to fetch.</param>
        /// <returns>The value of the internal field 'timesLoaded' as a 32-bit signed integer.</returns>
        public static int  GetTimesLoaded(this ModBase @base)
        {
            return (int)timesLoadedInfo.GetValue(@base);
        }
        /// <summary>
        /// Sets the value of the internal field 'timesLoaded' of a <see cref="ModBase" /> instance.
        /// </summary>
        /// <param name="base">The <see cref="ModBase" /> which timesLoaded value to get.</param>
        /// <param name="value">The new value of <paramref name="base" />.timesLoaded.</param>
        public static void SetTimesLoaded(this ModBase @base, int value)
        {
            timesLoadedInfo.SetValue(@base, value);
        }
    }
}
