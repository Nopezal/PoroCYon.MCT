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
using PoroCYon.MCT.Internal.Diagnostics;
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
#pragma warning disable 414
        readonly static string
            fileName     = "PoroCYon.MCT.tapi",
            internalName = "PoroCYon.MCT"     ,
            displayName  = "PoroCYon.MCT"     ;
#pragma warning restore 414

        /// <summary>
        /// Wether the MCT is initalized or not
        /// </summary>
        public static bool Inited
        {
            get;
            internal set;
        }

        /// <summary>
        /// Ensures that the MCT is installed. This method is inlined in compile-time.
        /// </summary>
        /// <param name="displayModName">The display name of the calling mod.</param>
        /// <remarks>This method is inlined in compile-time.</remarks>
        [TargetedPatchingOptOut(Consts.TPOOReason)] // probably hacky stuff #3 (or something)
        public static void EnsureMct(string displayModName)
        {
            if (!File.Exists("PoroCYon.MCT.dll"))
            {
                API.main.Exit();

                MessageBox.Show("You must have the MCT installed in order to load " + displayModName + ".");
            }
        }

		static void Setup(Mod mod)
		{
			mod.modBase.modItemTemplates       = new List<ModItem      >() { new MItem    () };
			mod.modBase.modNPCTemplates        = new List<ModNPC       >() { new MNPC     () };
			mod.modBase.modPlayerTemplates     = new List<ModPlayer    >() { new MPlayer  () };
			mod.modBase.modPrefixTemplates     = new List<ModPrefix    >() { new MPrefix  () };
			mod.modBase.modProjectileTemplates = new List<ModProjectile>() { new MProj    () };
			mod.modBase.modTileTypeTemplates   = new List<ModTileType  >() { new MTileType() };
			mod.modBase.modWorldTemplates      = new List<ModWorld     >() { new MWorld   () };

			List<ModBase> mBases = new List<ModBase>();
			List<ModInterface> mUIs = new List<ModInterface>();
			List<ModWorld> mWorlds = new List<ModWorld>();
			List<ModPrefix> mPfixes = new List<ModPrefix>();

			if (mod.enabled && mod.Loaded)
			{
				mBases.Add(mod.modBase);

				if (mod.modInterface != null)
					mUIs.Add(mod.modInterface);
				if (mod.modBase.modWorldTemplates.Count > 0)
					mWorlds.AddRange(mod.modBase.modWorldTemplates);
				if (mod.modBase.modPrefixTemplates.Count > 0)
					mPfixes.AddRange(mod.modBase.modPrefixTemplates);
			}

			Hooks.Base.Setup(mBases);
			Hooks.Interface.Setup(mUIs);
			Hooks.World.Setup(mWorlds);
			Hooks.Prefixes.Setup(mPfixes);

		}

		static void InsertMctMod()
        {
			// hacky stuff #1
			// add mod etc to list...
			//ModController.LoadMod(Assembly.GetExecutingAssembly(), JsonMapper.ToObject(ReadResource("ModInfo.json")), typeof(Mod), 0, displayName);

			Mod m = new Mod(Assembly.GetExecutingAssembly().Location);
			m.enabled = true;
			m.modBase = new MctMod();
			m.modBase.mod = m;
			m.modInterface = new MUI();
			typeof(Mod).GetField("_modInfo", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(m, JsonMapper.ToObject(ReadResource("ModInfo.json")));
			m.Load();
			Mods.mods.Add(m);
			Setup(m);

			m.modBase.OnLoad();

			// the old way is redundant now, but keeping it here
			/*
            Assembly code = Assembly.GetExecutingAssembly();

            JsonData modInfo = JsonMapper.ToObject(ReadResource("ModInfo.json"));

            Mod mod = new Mod();

            mod.code     = code;
            mod.fileName = fileName;
            mod.modName  = internalName;
            mod.modInfo  = new ModInfo(modInfo);

            mod.modIndex = 0;

            mod.dlls = new List<Assembly>();
            mod.files = new Dictionary<string, byte[]>();

            foreach (ModBase @base in Mods.modBases)
                @base.modIndex++;

            Mods.loadOrder.Insert(0, internalName);
            Mods.modBases .Insert(0, mod     );

            #region instantiate mod[...]
            mod.modPlayers    .Add(new MPlayer(mod, null));
            mod.modWorlds     .Add(new MWorld (mod      ));
            mod.modItems      .Add(new MItem  (mod, null));
            mod.modNPCs       .Add(new MNPC   (mod, null));
            mod.modPrefixes   .Add(new MPrefix(mod      ));
            mod.modProjectiles.Add(new MProj  (mod, null));
            mod.modInterfaces .Add(new MUI    (mod      ));
            mod.modTileTypes  .Add(new MTile  (mod      ));
            #endregion

            ModsLoadContent.Load(code, mod);
            mod.OnLoad();

            mod.modPrefixes[0].Init(null);
            */
		}
        static void LoadData()
        {
            SyncedRandom.Reset();

            ModdableObject.Reset();

            ObjectLoader.AddInvasion(MctMod.instance, "Goblin Army", new GoblinArmyInv());
            ObjectLoader.AddInvasion(MctMod.instance, "Frost Legion", new FrostLegionInv());
            ObjectLoader.AddInvasion(MctMod.instance, "Pirates", new PiratesInv());
        }

        /// <summary>
        /// Initializes the MCT.
        /// Call this in OnLoad.
        /// If you do not call this, the MCT will not work (partially).
        /// </summary>
        public static void Init()
        {
            //if (!Inited)
            //    ModDebugger.GetModsToDebug();

            //ModDebugger.TryDebugMod(Assembly.GetCallingAssembly());

            if (Inited)
                return;

            Inited = true; // prevent stack overflow (onload -> init -> loaddebugmod -> onload -> ...) or things getting messed up by being loaded twice.

            InsertMctMod();

            LoadData();
        }

        internal static void Uninit()
        {
            MctUI.Uninit();

            Invasion.invasions.Clear();
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
    }
}
