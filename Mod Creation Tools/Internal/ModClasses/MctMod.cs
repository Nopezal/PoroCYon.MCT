using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.Extensions;
using Terraria;
using TAPI;
using PoroCYon.MCT.Input;
using PoroCYon.MCT.Internal.Diagnostics;
using PoroCYon.MCT.Internal.Versioning;
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
    sealed class MctMod : ModBase
    {
        internal readonly static string MCTDataFile = Main.SavePath + "\\MCT_Data.sav";

        internal static MctMod instance;

        internal static UpdateMode UpdateMode
        {
            get
            {
                if (Main.musicVolume > 0f)
                    return UpdateMode.AudioUpdate;
                if (!Main.gameMenu && !Main.gamePaused)
                    return UpdateMode.PlayerUpdate;

                return UpdateMode.PreGameDraw;
            }
        }

        public MctMod()
            : base()
        {
            instance = this;
        }

        [CallPriority(Single.Epsilon)]
        public override void OnLoad()
        {
            base.OnLoad();

            if (!Main.dedServ)
            {
                (MctUI.WhitePixel = new Texture2D(API.main.GraphicsDevice, 1, 1)).SetData(new Color[1] { new Color(255, 255, 255, 0) });
                (MctUI.InversedWhitePixel = new Texture2D(API.main.GraphicsDevice, 1, 1)).SetData(new Color[1] { new Color(255, 255, 255, 255) });
            }

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

            try
            {
                if (ModDebugger.ShouldDebug)
                    ModDebugger.DebugMods();

                if (Main.dedServ)
                    return;

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

                if (UpdateChecker.GetIsUpdateAvailable())
                    UpdateBoxInjector.Inject();
            }
            catch (Exception e)
            {
                MessageBox.Show("An unexpected exception occured in the MCT internally, please show this to PoroCYon:" + Environment.NewLine + e, "MCT internal error");
            }
        }

        internal static void  ReadSettings(Stream s)
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
