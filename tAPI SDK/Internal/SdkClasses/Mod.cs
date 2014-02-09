using System;
using System.Collections.Generic;
using System.Linq;
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
    sealed class Mod : ModBase
    {
        internal static bool Inited
        {
            get;
            private set;
        }
        public static bool IsAsMod
        {
            get;
            private set;
        }

        public Mod()
            : base()
        {
            
        }

        public override void OnLoad()
        {
            IsAsMod = true;

            Init();

            base.OnLoad();
        }
        public override void OnUnload()
        {
            Sdk.Inited = Inited = false;

            base.OnUnload();
        }

        static void Init()
        {
            if (Inited)
                return;

            (SdkUI.WhitePixel = new Texture2D(Constants.mainInstance.GraphicsDevice, 1, 1)).SetData(new Color[1] { Color.White });

            Inited = true;
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
            if (id >= IEConsts.ENUM_OFFSET)
                switch ((InternalModMessages)id)
                {
                    #region INIT_SDK
                    case InternalModMessages.INIT_SDK:
                        Init();
                        break;
                    #endregion
                }
            else
                switch ((ModMessages)id)
                {

                }

            return base.OnModCall(mod, arguments);
        }

        [CallPriority(Single.PositiveInfinity)]
        public override void OnGameUpdate()
        {
            GInput.Update();

            SdkUI.Update();

            base.OnGameUpdate();
        }
    }
}
