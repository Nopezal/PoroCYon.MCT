using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using TAPI;
using PoroCYon.MCT.UI;
using PoroCYon.MCT.UI.Interface;
using PoroCYon.MCT.UI.Interface.Controls;

namespace PoroCYon.MCT.Internal.ModClasses
{
    [GlobalMod]
    sealed class MUI : ModInterface
    {
        sealed class PostInventoryLayer : InterfaceLayer
        {
            internal PostInventoryLayer()
                : base("PoroCYon.MCT:PostDrawInventoryUI")
            {

            }

            protected override void OnDraw(SpriteBatch sb)
            {
                MctUI.Draw(sb, DrawCalled.PostDrawInventory);
            }
        }

        public MUI(ModBase @base)
            : base(@base)
        {

        }

        [CallPriority(Single.Epsilon)]
        public override bool PreDrawInterface(SpriteBatch sb)
        {
            MctUI.Draw(sb, DrawCalled.PreDrawInterface);

            if (!Main.playerInventory)
                PreDrawInventory(sb);

            return base.PreDrawInterface(sb);
        }

        [CallPriority(Single.Epsilon)]
        public override bool PreDrawInventory(SpriteBatch sb)
        {
            MctUI.Draw(sb, DrawCalled.PreDrawInventory);

            return base.PreDrawInventory(sb);
        }

        [CallPriority(Single.Epsilon)]
        public override void PostDrawInterface(SpriteBatch sb)
        {
            if (!Main.playerInventory)
                PostDrawInventory(sb);

            MctUI.Draw(sb, DrawCalled.PostDrawInterface);

            base.PostDrawInterface(sb);
        }

        public override bool KeyboardInputFocused()
        {
            return base.KeyboardInputFocused() || Control.listening;
        }

        public override void ModifyInterfaceLayerList(List<InterfaceLayer> list)
        {
            // putting it here so it is also drawn when the inventory is closed
            list.Insert(list.IndexOf(InterfaceLayer.LayerInventory) + 1, new PostInventoryLayer());
        }
    }
}
