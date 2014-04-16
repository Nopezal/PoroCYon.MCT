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
    [GlobalMod]
    sealed class Mod : ModBase
    {
        internal readonly static string MCTDataFile = Main.SavePath + "\\MCT_Data.sav";

        internal static Mod instance;

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

        [CallPriority(Single.Epsilon)]
        public override void OnAllModsLoaded()
        {
            base.OnAllModsLoaded();

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

        [CallPriority(Single.PositiveInfinity)]
        public override object OnModCall(ModBase mod, params object[] arguments)
        {
            if (arguments.Length == 0)
                arguments = new object[1] { 0 };
            if (!(arguments[0] is int))
                arguments[0] = 0;

            int id = (int)arguments[0];

            if (id >= Consts.ENUM_OFFSET)
                switch ((InternalModMessages)id)
                {

                }
            else
                switch ((ModMessages)id)
                {

                }

            return base.OnModCall(mod, arguments);
        }

        [CallPriority(Single.PositiveInfinity)]
        public override void NetReceive(int id, BinBuffer bb)
        {
            switch ((InternalNetMessages)id)
            {
                case InternalNetMessages.SyncRandom_Sync:
                    {
                        string group = bb.ReadString();
                        Random rand = (Random)NetHelper.ReadObject(typeof(Random), bb);
                        int @ref = bb.ReadInt();

                        SyncedRandom.rands[group] = rand;
                        SyncedRandom.refs[group] = @ref;
                    }
                    break;
                case InternalNetMessages.SyncRandom_CTOR:
                    {
                        string group = bb.ReadString();
                        int seed = bb.ReadInt();

                        SyncedRandom.rands[group] = new Random(seed);
                        SyncedRandom.refs[group]++;
                    }
                    break;
                case InternalNetMessages.SyncRandom_DTOR:
                    {
                        string group = bb.ReadString();

                        SyncedRandom.refs[group]--;
                        if (SyncedRandom.refs[group] <= 0)
                            SyncedRandom.rands.Remove(group);
                    }
                    break;
            }

            base.NetReceive(id, bb);
        }

        [CallPriority(Single.PositiveInfinity)]
        public override void PostGameDraw(SpriteBatch sb)
        {
            GInput.Update();

            base.PostGameDraw(sb);

            //if (UpdateChecker.LastUpdateAvailable)
            //{
            //    sb.End();
            //    sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);

            //    API.main.IsMouseVisible = true;

            //    sb.Draw(MctUI.InversedWhitePixel, Vector2.Zero, null, new Color(50, 50, 50, 150), 0f, Vector2.Zero,
            //        new Vector2(Main.screenWidth, Main.screenHeight), SpriteEffects.None, 0f);

            //    Main.mouseLeft = Main.mouseLeftRelease = Main.mouseRight = Main.mouseRightRelease = false;

            //    sb.End();
            //    sb.Begin();
            //}
        }
    }
}
