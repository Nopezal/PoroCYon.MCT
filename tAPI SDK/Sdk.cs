using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using TAPI.SDK.Input;
using TAPI.SDK.Internal;
using TAPI.SDK.Internal.ModClasses;
using TAPI.SDK.Interop;
using TAPI.SDK.ObjectModel;
using TAPI.SDK.Net;
using TAPI.SDK.UI;
using TAPI.SDK.UI.MenuItems;

namespace TAPI.SDK
{
    /// <summary>
    /// The main SDK class.
    /// This is the entry point of the SDK.
    /// </summary>
    public static class Sdk
    {
        /// <summary>
        /// Wether the SDK is initalized or not
        /// </summary>
        public static bool Inited
        {
            get;
            internal set;
        }

        /// <summary>
        /// Initializes the SDK.
        /// Call this in OnLoad.
        /// If you do not call this, the SDK will not work (partially).
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
                MODFILE = "TAPI.SDK.tapi",
                INTERNALNAME = "TAPI.SDK",
                DISPLAYNAME = "tAPI SDK";

            ModBase modBase = new Mod();

            modBase.fileName = MODFILE;
            modBase.modName = DISPLAYNAME;
            modBase.modInfo = new ModInfo(modInfo);
            modBase.modIndex = 0;
            modBase.code = Assembly.GetExecutingAssembly();

            foreach (ModBase @base in Mods.modBases)
                @base.modIndex++;

            Mods.modBases.Insert(0, modBase);

            Mods.loadOrder.Insert(0, INTERNALNAME);

            #region instantiate mod[...]
            modBase.modPlayers.Add(new MPlayer(modBase, null));
            modBase.modWorlds.Add(new MWorld(modBase));
            modBase.modItems.Add(new MItem(modBase, null));
            modBase.modNPCs.Add(new MNPC(modBase, null));
            modBase.modPrefixes.Add(new MPrefix(modBase, null));
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
            SdkUI.Uninit();

            Mod.instance = null;

            Inited = false;
        }

        internal static string ReadResource(string resourceName)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Assembly.GetExecutingAssembly().GetManifestResourceStream("TAPI.SDK." + resourceName).CopyTo(ms);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}
