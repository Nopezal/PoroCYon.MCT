using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using TAPI.SDK.GUI;
using TAPI.SDK.Input;
using TAPI.SDK.Internal;
using TAPI.SDK.Internal.SdkClasses;
using TAPI.SDK.Interop;
using TAPI.SDK.Net;

namespace TAPI.SDK
{
    public static class Sdk
    {
        static string ReadResource(string resourceName)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Assembly.GetExecutingAssembly().GetManifestResourceStream("TAPI.SDK." + resourceName).CopyTo(ms);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

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

            // Adding the current assembly to the mods... *puts on sunglasses*

            // because of this, all mods will load the same assembly instance (hehehe), so the data can be accessible for all mods

            Assembly code = Assembly.GetExecutingAssembly();

            JsonData modInfo = JsonMapper.ToObject(ReadResource("ModInfo.json"));

            const string
                MODFILE = "TAPI.SDK.tapimod",
                MODNAME = "TAPI.SDK",
                DISPLAYNAME = "tAPI SDK";

            ModBase modBase = new Mod();

            modBase.fileName = MODFILE;
            modBase.modName = MODNAME;
            modBase.modInfo = new ModInfo(modInfo);

            Mods.loadOrder.Add(MODNAME);
            Mods.modBases.Add(modBase);

			//// r2?
			//File.Copy(code.Location, code.Location + ".tmp");
			//Mods.modAssemblyData.Add(DISPLAYNAME, File.ReadAllBytes(code.Location + ".tmp"));
			//File.Delete(code.Location + ".tmp");

            #region instantiate mod[...]
            modBase.modPlayer = new MPlayer(modBase, null);
            modBase.modWorld = new MWorld(modBase);
            modBase.modItem = new MItem(modBase, null);
            modBase.modNPC = new MNPC(modBase, null);
            modBase.modProjectile = new MProj(modBase, null);
            modBase.modInterface = new SdkUI(modBase);
            modBase.modPrefix = new MPrefix(null, modBase);

            if (modBase.modPlayer != null)
                modBase.modPlayers.Add(modBase.modPlayer);
            if (modBase.modWorld != null)
                modBase.modWorlds.Add(modBase.modWorld);
            if (modBase.modItem != null)
                modBase.modItems.Add(modBase.modItem);
            if (modBase.modNPC != null)
                modBase.modNPCs.Add(modBase.modNPC);
            if (modBase.modProjectile != null)
                modBase.modProjectiles.Add(modBase.modProjectile);
            if (modBase.modInterface != null)
                modBase.modInterfaces.Add(modBase.modInterface);
            if (modBase.modPrefix != null)
                modBase.modPrefixes.Add(modBase.modPrefix);
            #endregion

            modBase.OnLoad();

            Inited = true;
        }
    }
}
