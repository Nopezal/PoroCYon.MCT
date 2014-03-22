using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TAPI;
using PoroCYon.MCT.Net;

namespace PoroCYon.MCT
{
    /// <summary>
    /// The invasion types
    /// </summary>
    public enum InvasionType : int
    {
        /// <summary>
        /// No invasion (use <see cref="PoroCYon.MCT.World"/>.StopInvasion to stop an invasion)
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

    /// <summary>
    /// Provides easy event invocation
    /// </summary>
    public static class World
    {
        /// <summary>
        /// Gets the current invasion type
        /// </summary>
        public static InvasionType CurrentInvasion
        {
            get
            {
                return (InvasionType)Main.invasionType;
            }
        }

        /// <summary>
        /// Sets wether the game should be in christmas state or not
        /// </summary>
        public static bool ForceChristmas
        {
            internal get;
            set;
        }
        /// <summary>
        /// Sets wether the game should be in halloween state or not
        /// </summary>
        public static bool ForceHalloween
        {
            internal get;
            set;
        }

        /// <summary>
        /// Starts an invasion.
        /// </summary>
        /// <param name="invasion">
        /// The invasion to start.
        /// If <paramref name="invasion"/> equals <see cref="PoroCYon.MCT.InvasionType"/>.None, use <see cref="PoroCYon.MCT.World"/>.StopInvasion to stop all invasions.
        /// </param>
        public static void StartInvasion(InvasionType invasion)
        {
            Main.invasionDelay = 0;
            Main.StartInvasion((int)invasion);
        }
        /// <summary>
        /// Stops all active invasions
        /// </summary>
        public static void StopInvasions()
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

        /// <summary>
        /// Spawns a custom Dust
        /// </summary>
        /// <param name="position">The position of the particle</param>
        /// <param name="size">The size of the particle</param>
        /// <param name="texture">The sprite of the particle</param>
        /// <param name="maskType">The masking type of the particle (mimic AI type)</param>
        /// <param name="velocity">The velocity of the particle; default is {0;0}</param>
        /// <param name="c">The colour (incl. alpha) of the particle; default is {255;255;255;0}</param>
        /// <param name="scale">The scale of the particle</param>
        /// <param name="noGravity">Wether the particle is bound to the laws of gravity or not; default is false (laws do apply)</param>
        /// <param name="noLight">Weter the particle can emit light or not; default is false (can emit light)</param>
        /// <returns>The ID of the newly spawned Dust instance</returns>
        public static int SpawnCustomDust(Vector2 position, Vector2 size, Texture2D texture, int maskType,
            Vector2 velocity = default(Vector2), Color? c = null, float scale = 1f, bool noGravity = false, bool noLight = false)
        {
            Color co = c ?? new Color(255, 255, 255, 0);
            int i;

            Main.dust[i = Dust.NewDust(position, (int)size.X, (int)size.Y, maskType, velocity.X, velocity.Y, co.A, co, scale)].OverrideTexture = texture;
            Main.dust[i].noGravity = noGravity;
            Main.dust[i].noLight = noLight;

            return i;
        }
        /// <summary>
        /// Spawns a custom Dust
        /// </summary>
        /// <param name="position">The position of the particle</param>
        /// <param name="size">The size of the particle</param>
        /// <param name="texture">The sprite of the particle</param>
        /// <param name="onUpdate">The behaviour of the particle</param>
        /// <param name="velocity">The velocity of the particle; default is {0;0}</param>
        /// <param name="c">The colour (incl. alpha) of the particle; default is {255;255;255;0}</param>
        /// <param name="scale">The scale of the particle</param>
        /// <param name="noGravity">Wether the particle is bound to the laws of gravity or not; default is false (laws do apply)</param>
        /// <param name="noLight">Weter the particle can emit light or not; default is false (can emit light)</param>
        /// <returns>The ID of the newly spawned Dust instance</returns>
        public static int SpawnCustomDust(Vector2 position, Vector2 size, Texture2D texture, Action onUpdate,
            Vector2 velocity = default(Vector2), Color? c = null, float scale = 1f, bool noGravity = false, bool noLight = false)
        {
            Color co = c ?? new Color(255, 255, 255, 0);
            int i;

            Main.dust[i = Dust.NewDust(position, (int)size.X, (int)size.Y, 0, velocity.X, velocity.Y, co.A, co, scale)].OverrideTexture = texture;
            Main.dust[i].OverrideUpdate += onUpdate;
            Main.dust[i].noGravity = noGravity;
            Main.dust[i].noLight = noLight;

            return i;
        }

        static void InvasionWarning()
        {
            // got from Terraria source
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
            else if (Main.invasionX < Main.spawnTileX)
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

            if (Main.netMode == 0)
                Main.NewText(text, 175, 75, 255, false);
            if (Main.netMode == 2)
                NetMessage.SendData(25, -1, -1, text, 255, 175f, 75f, 255f, 0);
        }
    }
}
