using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using TAPI.SDK.Internal;
using TAPI.SDK.UI.Interface;
using TAPI.SDK.UI.Interface.Controls;
using TAPI.SDK.UI.Interface.Controls.Primitives;
using TAPI.SDK.UI.MenuItems;

namespace TAPI.SDK.UI
{
    // the number of ambiguous matches is too damn high
    using IControl = TAPI.SDK.UI.Interface.Controls.Control;
    using MControl = TAPI.SDK.UI.MenuItems.Control;
    using ICheckBox = TAPI.SDK.UI.Interface.Controls.CheckBox;
    using MCheckBox = TAPI.SDK.UI.MenuItems.CheckBox;
    using IRadioButton = TAPI.SDK.UI.Interface.Controls.RadioButton;
    using MRadioButton = TAPI.SDK.UI.MenuItems.RadioButton;

    /// <summary>
    /// The visibility states of a CustomUI or Control
    /// </summary>
    [Flags]
    public enum Visibility
    {
        /// <summary>
        /// Not visible at all. Use this when you override IsVisible
        /// </summary>
        None = 0,

        /// <summary>
        /// Only visible when the inventory is opened
        /// </summary>
        Inventory = 1,
        /// <summary>
        /// Only visible when the inventory is closed
        /// </summary>
        NoInventory = 2,

        /// <summary>
        /// At all time ingame
        /// </summary>
        All = Inventory | NoInventory
    }

    /// <summary>
    /// The global UI class, provides various things regarding drawing, and holds all CustomUIs.
    /// </summary>
    public static class SdkUI
    {
        static SdkCustomUI defaultUI;

        internal static List<CustomUI> customUIs;

        internal static SdkCustomUI DefaultUI
        {
            get
            {
                return defaultUI ?? (defaultUI = new SdkCustomUI());
            }
            set
            {
                defaultUI = value;
            }
        }

        /// <summary>
        /// A ReadOnlyCollection of all CustomUI instances
        /// </summary>
        public static ReadOnlyCollection<CustomUI> CustomUIs
        {
            get
            {
                return customUIs.AsReadOnly();
            }
        }

        /// <summary>
        /// The current visibility state of the game
        /// </summary>
        public static Visibility CurrentVisibility
        {
            get
            {
                return Main.playerInventory ? Visibility.Inventory : Visibility.NoInventory;
            }
        }
        /// <summary>
        /// Gets or sets the DrawCalled field of the default CustomUI
        /// </summary>
        public static DrawCalled DrawCalled
        {
            get
            {
                return defaultUI.DrawCalled;
            }
            set
            {
                defaultUI.DrawCalled = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="Microsoft.Xna.Framework.Graphics.SpriteBatch"/> used by the game
        /// </summary>
        public static SpriteBatch SharedSpriteBatch
        {
            get
            {
                return Constants.mainInstance.spriteBatch;
            }
        }
        /// <summary>
        /// A 1-by-1, white pixel (#FFFFFF00)
        /// </summary>
        public static Texture2D WhitePixel
        {
            get;
            internal set;
        }


        /// <summary>
        /// Draws a string with an outline
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the string</param>
        /// <param name="font">The font of the string to draw</param>
        /// <param name="text">The text to draw</param>
        /// <param name="position">The position of the string to draw</param>
        /// <param name="foreground">The foreground colour of the string to draw</param>
        /// <param name="background">The background colour of the string to draw; default is #000000</param>
        /// <param name="offset">The offset of the outlines; default is 1</param>
        /// <param name="scale">The scale of the string to draw; default is (1, 1)</param>
        /// <param name="rotation">The rotation of the string to draw; default is 0</param>
        /// <param name="origin">The origin of the rotation of the string to draw; default is (0, 0)</param>
        /// <param name="spriteEffects">The sprite effect of the string to draw; default is None</param>
        /// <param name="layerDepth">The layer depth of the string to draw; default is 0</param>
        public static void DrawOutlinedString(SpriteBatch sb, SpriteFont font, string text, Vector2 position, Color foreground, Color? background = null, float offset = 2f,
            Vector2? scale = null, float rotation = 0f, Vector2 origin = default(Vector2), SpriteEffects spriteEffects = SpriteEffects.None, float layerDepth = 0f)
        {
            foreach (Vector2 v in new Vector2[] { new Vector2(offset, 0f), new Vector2(0f, offset), new Vector2(-offset, 0f), new Vector2(0f, -offset) })
                sb.DrawString(font, text, position + v, background ?? new Color(0, 0, 0, 0), rotation, origin, scale ?? new Vector2(1f), spriteEffects, layerDepth);

            sb.DrawString(font, text, position, foreground, rotation, origin, scale ?? new Vector2(1f), spriteEffects, layerDepth);
        }

        /// <summary>
        /// Draws text at the mouse
        /// </summary>
        /// <param name="text">The text to draw</param>
        /// <param name="rare">The colour (Item rarity) of the text to draw</param>
        /// <param name="diff">No idea</param>
        public static void MouseText(string text, int rare = 0, byte diff = 0)
        {
            Constants.mainInstance.MouseText(text, rare, diff);
        }
        /// <summary>
        /// Draws Item information at the mouse
        /// </summary>
        /// <param name="i">The Item to get the info from</param>
        public static void MouseText(Item i)
        {
            Main.mouseItem = (Item)i.Clone();

            MouseText(i.AffixName() + (i.stack > 1 ? " (" + i.stack + ")" : ""));

            Main.mouseItem = new Item();
        }

        /// <summary>
        /// Adds a CustomUI to the list
        /// </summary>
        /// <param name="customUI">The CustomUI to add</param>
        public static void AddCustomUI(CustomUI customUI)
        {
            customUIs.Add(customUI);
        }
        /// <summary>
        /// Removes a CustomUI from the list
        /// </summary>
        /// <param name="customUI">The CustomUI to remvoe</param>
        public static void RemoveCustomUI(CustomUI customUI)
        {
            customUIs.Remove(customUI);
        }
        /// <summary>
        /// Removes a CustomUI from the list 
        /// </summary>
        /// <param name="index">The index of the CustomUI to remove</param>
        public static void RemoveCustomUIAt(int index)
        {
            customUIs.RemoveAt(index);
        }
        /// <summary>
        /// Removes all CustomUIs from the list
        /// </summary>
        public static void ClearCustomUIs()
        {
            customUIs.Clear();
        }

        internal static void Uninit()
        {
            WhitePixel.Dispose();

            defaultUI = null;

            #region Interface
            CustomUI.GlobalControlAdded = null;
            CustomUI.GlobalControlRemoved = null;

            // ---

            IControl.GlobalAdded = null;
            IControl.GlobalDraw = null;
            IControl.GlobalInit = null;
            IControl.GlobalRemoved = null;
            IControl.GlobalUpdate = null;

            // ---

            Button.GlobalClicked = null;

            Checkable.GlobalChecked = null;
            Checkable.GlobalUnchecked = null;

            ControlContainer.GlobalAddControl = null;
            ControlContainer.GlobalRemoveControl = null;

            Focusable.GlobalBeginHover = null;
            Focusable.GlobalEndHover = null;
            Focusable.GlobalGotFocus = null;
            Focusable.GlobalLostFocus = null;

            ListeningControl.GlobalInputGot = null;

            // ---

            ItemContainer.GlobalCanSetItem = null;
            ItemContainer.GlobalItemChanged = null;
            ItemContainer.GlobalStackChanged = null;

            Slider.GlobalValueChanged = null;
            #endregion

            #region MenuItems
            MCheckBox.GlobalChecked = null;
            MCheckBox.GlobalUnchecked = null;

            MControl.GlobalDraw = null;
            MControl.GlobalInit = null;
            MControl.GlobalPreDraw = null;
            MControl.GlobalUpdate = null;

            Page.GlobalDraw = null;
            Page.GlobalInit = null;
            Page.GlobalUpdate = null;

            MRadioButton.groups.Clear();
            #endregion
        }

        internal static void Update()
        {
            IControl.listening = false;

            DefaultUI.Update();

            for (int i = 0; i < customUIs.Count; i++)
                if (customUIs[i].IsVisible)
                    customUIs[i].Update();
        }
        internal static void Draw(SpriteBatch sb, DrawCalled called)
        {
            if (called == DrawCalled.PostDrawInventory)
                DefaultUI.Draw(sb);

            for (int i = 0; i < customUIs.Count; i++)
                if (customUIs[i].IsVisible && customUIs[i].DrawCalled == called)
                    customUIs[i].Draw(sb);
        }
    }
}
