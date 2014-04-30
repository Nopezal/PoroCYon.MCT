﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Text;
using System.Windows.Forms;
using LitJson;
using TAPI;
using PoroCYon.MCT.Internal.ModClasses;
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
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")] // probably hacky stuff #3 (or something)
        public static void EnsureMct(string displayModName)
        {
            if (!File.Exists("PoroCYon.MCT.dll"))
            {
                API.main.Exit();

                MessageBox.Show("You must have the MCT installed in order to load " + displayModName + ".");
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

            // hacky stuff #1
            // add mod etc to list...

            Assembly code = Assembly.GetExecutingAssembly();

            JsonData modInfo = JsonMapper.ToObject(ReadResource("ModInfo.json"));

            const string
                MODFILE = "PoroCYon.MCT.tapi",
                INTERNALNAME = "PoroCYon.MCT",
                DISPLAYNAME = "Mod Creation Tools";

            ModBase modBase = new Mod();

            modBase.code = Assembly.GetExecutingAssembly();
            modBase.fileName = MODFILE;
            modBase.modName = DISPLAYNAME;
            modBase.modInfo = new ModInfo(modInfo);

            modBase.modIndex = 0;

            foreach (ModBase @base in Mods.modBases)
                @base.modIndex++;

            Mods.loadOrder.Insert(0, INTERNALNAME);
            Mods.modBases.Insert(0, modBase);

            #region instantiate mod[...]
            modBase.modPlayers.Add(new MPlayer(modBase, null));
            modBase.modWorlds.Add(new MWorld(modBase));
            //modBase.modItems.Add(new MItem(modBase, null));
            modBase.modNPCs.Add(new MNPC(modBase, null));
            modBase.modPrefixes.Add(new MPrefix(modBase));
            modBase.modProjectiles.Add(new MProj(modBase, null));
            modBase.modInterfaces.Add(new MUI(modBase));
            #endregion

            ModsLoadContent.Load(Assembly.GetExecutingAssembly(), modBase);
            modBase.OnLoad();

            modBase.modPrefixes[0].Init(null);

            Inited = true;

            SyncedRandom.Reset();

            ModableObject.Reset();
        }

        internal static void Uninit()
        {
            MctUI.Uninit();

            Mod.instance = null;

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
