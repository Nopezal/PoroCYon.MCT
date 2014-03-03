using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using TAPI.SDK.Input;
using TAPI.SDK.Interop;
using TAPI.SDK.Net;

namespace TAPI.SDK.Internal.ModClasses
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

        public override void OnLoad()
        {
            if (code.Location.IsEmpty()) // is loaded through mod (.tapi, byte array -> no location)
            {
                // hacky stuff #2

                // remove from list, but the file still exists

                // this is so it's using the .dll (shared objects) instead from some byte array (not so shared objects),
                // but that would make the modinfo display chrash. Adding this .tapi file and removing it from the list by code fixes it.
                modInfo = new ModInfo();
                modInterfaces = new List<ModInterface>();
                modItems = new List<ModItem>();
                modNPCs = new List<ModNPC>();
                modPlayers = new List<ModPlayer>();
                modPrefixes = new List<ModPrefix>();
                modProjectiles = new List<ModProjectile>();
                modWorlds = new List<ModWorld>();

                modIndex = -1;
                modName = "";
                fileName = "";
                textures = new Dictionary<string, Texture2D>();
                files = new Dictionary<string, byte[]>();
                code = null;

                Mods.loadOrder.Remove("TAPI.SDK");

                Mods.modBases.Remove(this);

                return; // no need to continue anyway
            }

            (Sdk.WhitePixel = new Texture2D(Constants.mainInstance.GraphicsDevice, 1, 1)).SetData(new Color[1] { new Color(255, 255, 255, 0) });

            instance = this;

            base.OnLoad();
        }
        public override void OnUnload()
        {
            Sdk.Uninit();

            base.OnUnload();
        }

		//[CallPriority(Single.PositiveInfinity)]
		//public override void ReceiveMessage(int msg, params object[] data)
		//{
		//	int i = 0;

		//	Func<object> NextObject = () =>
		//	{
		//		return data[i++];
		//	};

		//	if (msg >= IEConsts.ENUM_OFFSET)
		//		switch ((InternalNetMessages)msg)
		//		{
		//			#region INIT_SDK
		//			case InternalNetMessages.INIT_SDK:
		//				Init();
		//				break;
		//			#endregion

		//			#region SyncRandom_CTOR
		//			case InternalNetMessages.SyncRandom_CTOR:
		//				{
		//					string groupName = NextObject().ToString();
		//					int seed = (int)NextObject();

		//					SyncedRandom.rands[groupName] = new Random(seed);

		//					SyncedRandom.refs[groupName]++;
		//				}
		//				break;
		//			#endregion
		//			#region SyncRandom_DTOR
		//			case InternalNetMessages.SyncRandom_DTOR:
		//				{
		//					string groupName = NextObject().ToString();

		//					SyncedRandom.refs[groupName]--;

		//					if (SyncedRandom.refs[groupName] <= 0)
		//						SyncedRandom.rands.Remove(groupName);
		//				}
		//				break;
		//			#endregion
		//			#region SyncRandom_Sync
		//			case InternalNetMessages.SyncRandom_Sync:
		//				{
		//					string groupName = NextObject().ToString();
		//					Random rand = (Random)NextObject();
		//					int refs = (int)NextObject();

		//					SyncedRandom.rands[groupName] = rand;
		//					SyncedRandom.refs[groupName] = refs;
		//				}
		//				break;
		//			#endregion
		//		}
		//	else
		//		switch ((NetMessages)msg)
		//		{

		//		}

		//	base.ReceiveMessage(msg, data);
		//}

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

        //[CallPriority(Single.PositiveInfinity)]
        //public override void OnGameUpdate()
        //{
        //    GInput.Update();

        //    SdkUI.Update();

        //    base.OnGameUpdate();
        //}

        [CallPriority(Single.PositiveInfinity)]
        public override void PostGameDraw(SpriteBatch sb)
        {
            GInput.Update();

            base.PostGameDraw(sb);
        }
    }
}
