using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using TAPI.SDK.Net;

namespace TAPI.SDK
{
    /// <summary>
    /// The invasion types
    /// </summary>
    public enum InvasionType : int
    {
        /// <summary>
        /// No invasion (use <see cref="TAPI.SDK.World"/>.StopInvasion to stop an invasion)
        /// </summary>
        None = 0,

        /// <summary>
        /// The Goblin Army
        /// </summary>
        GoblinArmy = 1,
        /// <summary>
        /// The Frost Legion
        /// </summary>
        FrostLegion = 2,
        /// <summary>
        /// The Pirates
        /// </summary>
        Pirates = 3
    }

    public static class World
    {
        /// <summary>
        /// Starts an invasion.
        /// </summary>
        /// <param name="invasion">
        /// The invasion to start.
        /// If <paramref name="invasion"/> equals <see cref="TAPI.SDK.InvasionType"/>.None, use <see cref="TAPI.SDK.World"/>.StopInvasion to stop all invasions.
        /// </param>
        public static void StartInvasion(InvasionType invasion)
        {
            Main.invasionDelay = 0;
            Main.StartInvasion((int)invasion);
        }
        /// <summary>
        /// Stops all active invasions
        /// </summary>
        public static void StopInvasion()
        {
            Main.invasionSize = 0;
            InvasionWarning();
            Main.invasionType = Main.invasionDelay = 0;
        }

        /// <summary>
        /// Makes it rain
        /// </summary>
        public static void StartRain()
        {
            Main.StartRain();
        }
        /// <summary>
        /// Stop raining
        /// </summary>
        public static void StopRain()
        {
            Main.StopRain();
        }

        /// <summary>
        /// Start a Blood Moon
        /// </summary>
        public static void StartBloodMoon()
        {
            Main.bloodMoon = true;
            Main.time = 0d;
            Main.dayTime = false;
            NetHelper.SendText(Lang.misc[8], new Color(50, 255, 130));
        }
        /// <summary>
        /// Stops a Blood Moon
        /// </summary>
        public static void StopBloodMoon()
        {
            Main.bloodMoon = false;
        }
        /// <summary>
        /// Starts a Solar Eclipse
        /// </summary>
        public static void StartEclipse()
        {
            Main.eclipse = true;
            Main.time = 0d;
            Main.dayTime = true;

            NetHelper.SendText(Lang.misc[20], new Color(50, 255, 130));
        }
        /// <summary>
        /// Stops a Solar Eclipse
        /// </summary>
        public static void StopEclipse()
        {
            Main.eclipse = false;
        }

        /// <summary>
        /// Starts a Pumpkin Moon
        /// </summary>
        public static void StartPumpkinMoon()
        {
            Main.dayTime = false;
            Main.startPumpkinMoon();
        }
        /// <summary>
        /// Stops a Pumpkin Moon
        /// </summary>
        public static void StopPumpkinMoon()
        {
            Main.pumpkinMoon = false;
            NPC.waveKills = NPC.waveCount = 0;
        }
        /// <summary>
        /// Starts a Frost Moon
        /// </summary>
        public static void StartFrostMoon()
        {
            Main.dayTime = false;
            Main.startSnowMoon();
        }
        /// <summary>
        /// Stops a Frost Moon
        /// </summary>
        public static void StopFrostMoon()
        {
            Main.snowMoon = false;
            NPC.waveKills = NPC.waveCount = 0;
        }

        static void InvasionWarning()
        {
            string text;

            if (Main.invasionSize <= 0)
            {
                if (Main.invasionType == 2)
                    text = Lang.misc[4];
                else if (Main.invasionType == 3)
                    text = Lang.misc[24];
                else
                    text = Lang.misc[0];
            }
            else
            {
                if (Main.invasionX < Main.spawnTileX)
                {
                    if (Main.invasionType == 2)
                        text = Lang.misc[5];
                    else if (Main.invasionType == 3)
                        text = Lang.misc[25];
                    else
                        text = Lang.misc[1];
                }
                else if (Main.invasionX > Main.spawnTileX)
                {
                    if (Main.invasionType == 2)
                        text = Lang.misc[6];
                    else if (Main.invasionType == 3)
                        text = Lang.misc[26];
                    else
                        text = Lang.misc[2];
                }
                else if (Main.invasionType == 2)
                    text = Lang.misc[7];
                else if (Main.invasionType == 3)
                    text = Lang.misc[27];
                else
                    text = Lang.misc[3];
            }

            if (Main.netMode == 0)
                Main.NewText(text, 175, 75, 255, false);
            if (Main.netMode == 2)
                NetMessage.SendData(25, -1, -1, text, 255, 175f, 75f, 255f, 0);
        }
    }
}
