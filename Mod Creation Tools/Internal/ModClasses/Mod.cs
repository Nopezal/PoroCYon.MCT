using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using Terraria;
using TAPI;
using PoroCYon.MCT.Input;
using PoroCYon.MCT.Internal.Versioning;
using PoroCYon.MCT.Interop;
using PoroCYon.MCT.Net;
using PoroCYon.MCT.UI;

namespace PoroCYon.MCT.Internal.ModClasses
{
    enum UpdateMode
    {
        AudioUpdate,
        PlayerUpdate,
        PreGameDraw
    }

    [GlobalMod]
    sealed class Mod : ModBase
    {
        internal readonly static string MCTDataFile = Main.SavePath + "\\MCT_Data.sav";

        internal static Mod instance;

        internal static UpdateMode UpdateMode
        {
            get
            {
                if (AudioDef.volume > 0f)
                    return UpdateMode.AudioUpdate;
                if (!Main.gameMenu && !Main.gamePaused)
                    return UpdateMode.PlayerUpdate;

                return UpdateMode.PreGameDraw;
            }
        }

        public Mod()
            : base()
        {
            instance = this;
        }

        [CallPriority(Single.Epsilon)]
        public override void OnLoad()
        {
            base.OnLoad();

            (MctUI.WhitePixel = new Texture2D(API.main.GraphicsDevice, 1, 1)).SetData(new Color[1] { new Color(255, 255, 255, 0) });
            (MctUI.InversedWhitePixel = new Texture2D(API.main.GraphicsDevice, 1, 1)).SetData(new Color[1] { new Color(255, 255, 255, 255) });

            instance = this;

            FileStream fs = null;
            try
            {
                fs = new FileStream(MCTDataFile, FileMode.Open);
                ReadSettings(fs);
            }
            catch (IOException)
            {
                if (fs != null)
                    fs.Close();

                fs = new FileStream(MCTDataFile, FileMode.Create);

                WriteSettings(fs);
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }
        [CallPriority(Single.Epsilon)]
        public override void OnUnload()
        {
            Mct.Uninit();

            FileStream fs = new FileStream(MCTDataFile, FileMode.Create);
            WriteSettings(fs);
            fs.Close();

            instance = null;
            SettingsPage.PageInstance = null;

            base.OnUnload();
        }
        [CallPriority(Single.Epsilon)]
        public override void OnAllModsLoaded()
        {
            base.OnAllModsLoaded();

            // remove temporary modbase instances
            for (int i = 0; i < MctDebugger.tempBases.Count; i++)
                Mods.modBases.Remove(MctDebugger.tempBases[i]);

            MctDebugger.tempBases.Clear();

            // insert settings menu button in the Options menu
            Menu.menuPages.Add("MCT:Settings", new SettingsPage());

            MenuAnchor aOptions = new MenuAnchor()
            {
                anchor = new Vector2(0.5f, 0f),
                offset = new Vector2(315f, 200f),
                offset_button = new Vector2(0f, 50f)
            };

            Menu.menuPages["Options"].anchors.Add(aOptions);
            Menu.menuPages["Options"].buttons.Add(new MenuButton(0, "MCT Settings", "MCT:Settings").Where(mb => mb.SetAutomaticPosition(aOptions, 0)));

            if (UpdateChecker.IsUpdateAvailable())
                UpdateBoxInjector.Inject();
        }

        internal static void ReadSettings(Stream s)
        {
            BinBuffer bb = new BinBuffer(new BinBufferStream(s));

            UpdateChecker.CheckForUpdates = bb.ReadBool();
        }
        internal static void WriteSettings(Stream s)
        {
            BinBuffer bb = new BinBuffer(new BinBufferStream(s));

            bb.Write(UpdateChecker.CheckForUpdates);
        }

        [CallPriority(Single.PositiveInfinity)]
        public override object OnModCall(ModBase mod, params object[] arguments)
        {
            if (arguments.Length == 0)
                arguments = new object[1] { 0 };
            if (!(arguments[0] is int))
                arguments[0] = 0;

            int id = (int)arguments[0];

#pragma warning disable 1522
            //if (id >= Consts.ENUM_OFFSET)
            //    switch ((InternalModMessages)id)
            //    {

            //    }
            //else
            //    switch ((ModMessages)id)
            //    {

            //    }
#pragma warning disable 1522

            return base.OnModCall(mod, arguments);
        }
        [CallPriority(Single.PositiveInfinity)]
        public override void NetReceive(int id, BinBuffer bb)
        {
            base.NetReceive(id, bb);

            switch ((InternalNetMessages)id)
            {
                case InternalNetMessages.SyncRandom_Sync:
                    SyncedRandom.GetCached(bb.ReadInt()).NextDouble();
                    break;
                case InternalNetMessages.SyncRandom_CTOR:
                    {
                        string group = bb.ReadString();
                        int seed = bb.ReadInt();

                        SyncedRandom.rands[group] = seed;
                        SyncedRandom.refs[group]++;
                    }
                    break;
                case InternalNetMessages.SyncRandom_DTOR:
                    {
                        int seed = bb.ReadInt();
                        string group = null;

                        foreach (var kvp in SyncedRandom.rands)
                            if (kvp.Value == seed)
                                group = kvp.Key;

                        if (group == null)
                            return;

                        SyncedRandom.refs[group]--;
                        if (SyncedRandom.refs[group] <= 0)
                        {
                            SyncedRandom.rands.Remove(group);
                            SyncedRandom.refs.Remove(group);
                            SyncedRandom.RemoveCached(seed);
                        }
                    }
                    break;
            }
        }

        [CallPriority(Single.PositiveInfinity)]
        public override void PreGameDraw(SpriteBatch sb)
        {
            base.PreGameDraw(sb);

            UpdateThings(UpdateMode.PreGameDraw);
        }
        [CallPriority(Single.PositiveInfinity)]
        public override void ChooseTrack(ref string next)
        {
            base.ChooseTrack(ref next);

            UpdateThings(UpdateMode.AudioUpdate);
        }

        internal static void UpdateThings(UpdateMode mode)
        {
            if (mode != UpdateMode)
                return;

            GInput.Update();

            if (!Main.gameMenu)
                MctUI.Update();
        }
    }
}
