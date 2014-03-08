using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using TAPI;
using PoroCYon.MCT.Input;
using PoroCYon.MCT.Interop;
using PoroCYon.MCT.Net;
using PoroCYon.MCT.UI;

namespace PoroCYon.MCT.Internal.ModClasses
{
    [GlobalMod]
    sealed class Mod : ModBase
    {
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

            (MctUI.WhitePixel = new Texture2D(Constants.mainInstance.GraphicsDevice, 1, 1)).SetData(new Color[1] { new Color(255, 255, 255, 0) });

            instance = this;
        }
        [CallPriority(Single.Epsilon)]
        public override void OnUnload()
        {
            Mct.Uninit();

            base.OnUnload();
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
        }
    }
}
