using System;
using System.Collections.Generic;
using System.Linq;
using TAPI;

namespace PoroCYon.MCT
{
    /// <summary>
    /// Contains various utility methods
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Converts a TimeSpan instance to game-time ticks
        /// </summary>
        /// <param name="span">The TimeSpan to convert</param>
        /// <returns><paramref name="span"/> in game-time ticks</returns>
        public static int TimeSpanToTicks(TimeSpan span)
        {
            return (int)(span.TotalMilliseconds / API.main.TargetElapsedTime.TotalMilliseconds);
        }
        /// <summary>
        /// Converts game-time ticks to a TimeSpan instance
        /// </summary>
        /// <param name="ticks">The game-time ticks to convert</param>
        /// <returns><paramref name="ticks"/> as a TimeSpan</returns>
        public static TimeSpan TicksToTimeSpan(int ticks)
        {
            return new TimeSpan(0, 0, 0, 0, (int)(ticks * API.main.TargetElapsedTime.TotalMilliseconds));
        }
    }
}
