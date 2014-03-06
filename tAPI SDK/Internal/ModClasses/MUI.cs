using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TAPI.SDK.UI;
using TAPI.SDK.UI.Interface;
using TAPI.SDK.UI.Interface.Controls;

namespace TAPI.SDK.Internal.ModClasses
{
    [GlobalMod]
    sealed class MUI : ModInterface
    {
        public MUI(ModBase @base)
            : base(@base)
        {

        }

        [CallPriority(Single.Epsilon)]
        public override bool PreDrawInterface(SpriteBatch sb)
        {
            SdkUI.Draw(sb, DrawCalled.PreDrawInterface);

            if (!Main.playerInventory)
                PreDrawInventory(sb);

            return base.PreDrawInterface(sb);
        }

        [CallPriority(Single.Epsilon)]
        public override bool PreDrawInventory(SpriteBatch sb)
        {
            SdkUI.Draw(sb, DrawCalled.PreDrawInventory);

            return base.PreDrawInventory(sb);
        }

        [CallPriority(Single.Epsilon)]
        public override void PostDrawInventory(SpriteBatch sb)
        {
            SdkUI.Draw(sb, DrawCalled.PostDrawInventory);

            base.PostDrawInventory(sb);
        }

        [CallPriority(Single.Epsilon)]
        public override void PostDrawInterface(SpriteBatch sb)
        {
            if (!Main.playerInventory)
                PostDrawInventory(sb);

            SdkUI.Draw(sb, DrawCalled.PostDrawInterface);

            base.PostDrawInterface(sb);
        }

        public override bool KeyboardInputFocused()
        {
            return base.KeyboardInputFocused() || Control.listening;
        }
    }
}
