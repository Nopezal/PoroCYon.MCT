using System;
using System.Collections.Generic;
using System.Linq;
using PoroCYon.MCT.Tools.Internal.Porting;

namespace PoroCYon.MCT.Tools
{
    /// <summary>
    /// Ports files in the Vanilla format to the tAPI format.
    /// </summary>
    public unsafe static class FilePorter
    {
        /// <summary>
        /// Ports a Player file.
        /// </summary>
        /// <param name="path">The path to the Player file to port.</param>
        public static void PortPlayer(string path)
        {
            PlayerPorter.WritePlayer(PlayerPorter.ReadPlayer(path));

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Ports a world file.
        /// </summary>
        /// <param name="path">The path to the world file to port.</param>
        public static void PortWorld(string path)
        {
            WorldPorter.WriteWorld(WorldPorter.ReadWorld(path));

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
