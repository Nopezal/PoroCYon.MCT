using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Text;
using System.Windows.Forms;
using LitJson;
using TAPI;
using PoroCYon.MCT.Content;
using PoroCYon.MCT.Internal;
using PoroCYon.MCT.Internal.ModClasses;
using PoroCYon.MCT.ModControlling;
using PoroCYon.MCT.ObjectModel;
using PoroCYon.MCT.Net;
using PoroCYon.MCT.UI;

namespace PoroCYon.MCT
{
    /// <summary>
    /// The main MCT class.
    /// This is the entry point of the MCT.
    /// </summary>
    public static class Mct
    {
        static bool inserted = false;
        /// <summary>
        /// Wether the MCT is initalized or not
        /// </summary>
        public static bool Inited
        {
            get;
            internal set;
        }

        static void InsertMctMod()
        {
            // hacky stuff #1
            // add mod etc to list...

            ModController.LoadMod(Assembly.GetExecutingAssembly(), JsonMapper.ToObject(ReadResource("ModInfo.json")), null, new ModClasses()
            {
                Net = new MNet(),

                Interfaces  = new List<ModInterface >() { new MUI() },

                GlobalItems = new List<ModItem      >() { new MItem    () },
                GlobalNPCs  = new List<ModNPC       >() { new MNPC     () },
                GlobalProjs = new List<ModProjectile>() { new MProj    () },
                GlobalTiles = new List<ModTileType  >() { new MTileType() },

                Players  = new List<ModPlayer>() { new MPlayer() },
                Prefixes = new List<ModPrefix>() { new MPrefix() },
                Worlds   = new List<ModWorld >() { new MWorld () }
            }, new MctMod());

            inserted = true;

            //Mod m = new Mod(Assembly.GetExecutingAssembly().Location);
            //m.enabled = true;
            //m.modBase = new MctMod();
            //m.modBase.mod = m;
            //m.modInterface = new MUI();
            //typeof(Mod).GetField("_modInfo", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(m, JsonMapper.ToObject(ReadResource("ModInfo.json")));
            //m.Load();
            //Mods.mods.Add(m);
            //Setup(m);

            //m.modBase.OnLoad();
        }
        static void LoadData    ()
        {
            SyncedRandom.Reset();

            ModdableObject.Reset();

            ObjectLoader.AddInvasion(MctMod.instance, "Goblin Army", new GoblinArmyInv());
            ObjectLoader.AddInvasion(MctMod.instance, "Frost Legion", new FrostLegionInv());
            ObjectLoader.AddInvasion(MctMod.instance, "Pirates", new PiratesInv());
        }

        internal static void Uninit()
        {
            MctUI.Uninit();

            Invasion.invasions    .Clear();
            Invasion.invasionTypes.Clear();

            //ModDebugger.tempBases.Clear();
            //ModDebugger.toDebug  .Clear();

            MctMod.instance = null;

            Inited = false;
        }

        internal static string ReadResource(string resourceName)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Assembly.GetExecutingAssembly().GetManifestResourceStream("PoroCYon.MCT." + resourceName).CopyTo(ms);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        /// <summary>
        /// Initializes the MCT.
        /// Call this in OnLoad.
        /// If you do not call this, the MCT will not work (partially).
        /// </summary>
        public static void Init()
        {
            if (Inited)
                return;

            try
            {
                Inited = true; // prevent stack overflow (onload -> init -> loaddebugmod -> onload -> ...) or things getting messed up by being loaded twice.

                if (!inserted)
                    InsertMctMod();

                LoadData();
            }
            catch (Exception e)
            {
                MessageBox.Show("An unexpected exception occured in the MCT internally, please show this to PoroCYon:" + Environment.NewLine + e, "MCT internal error");
            }
        }

		/// <summary>
		/// Ensures that the MCT is installed. This method is inlined in compile-time.
		/// </summary>
		/// <param name="displayModName">The display name of the calling mod.</param>
		/// <remarks>This method is inlined in compile-time.</remarks>
		[TargetedPatchingOptOut(Consts.TPOOReason)]	// probably hacky stuff #3 (or something)
		public static void EnsureMct(string displayModName)
		{
			if (!File.Exists("PoroCYon.MCT.dll"))
			{
				API.main.Exit();

				MessageBox.Show("You must have the MCT installed in order to load " + displayModName + ".");
			}
		}
	}
}
