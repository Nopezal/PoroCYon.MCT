using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using TAPI.SDK.GUI.Controls;
using TAPI.SDK.GUI.Controls.Interop;
using TAPI.SDK.GUI.Controls.Primitives;
using TAPI.SDK.Input;
using TAPI.SDK.Internal;

namespace TAPI.SDK.GUI
{
	/// <summary>
	/// This class cannot be inherited.
    /// </summary>
    [GlobalMod, ComVisible(false)]
    public sealed class SdkUI : ModInterface
    {
        static SdkCustomUI customUI;
        internal static SdkCustomUI CustomUI
        {
            get
            {
                return customUI ?? (customUI = new SdkCustomUI());
            }
        }

        /// <summary>
        /// A 1-by-1 white pixel texture
        /// </summary>
        public static Texture2D WhitePixel
        {
            get;
            internal set;
        }

        /// <summary>
        /// The controls in the (global) GUI.
        /// Adding or removing a control by calling Controls.Add/Remove is not a good idea.
        /// </summary>
        public static List<CustomUI> CustomUIs
        {
            get;
            internal set;
        }
        /// <summary>
        /// The controls in the GUI.
        /// Adding or removing a control by calling Controls.Add/Remove is not a good idea.
        /// </summary>
        public static List<Control> Controls
        {
            get
            {
                return CustomUI.Controls;
            }
            internal set
            {
                CustomUI.Controls = value;
            }
        }

        /// <summary>
        /// Called when a control is added from the Controls list
        /// </summary>
        public static Action<CustomUI> OnUIAdded;
        /// <summary>
        /// Called when a control is removed from the Controls list
        /// </summary>
        public static Action<CustomUI> OnUIRemoved;
        /// <summary>
        /// Called when a control is added from the Controls list
        /// </summary>
        public static Action<Control> OnControlAdded;
        /// <summary>
        /// Called when a control is removed from the Controls list
        /// </summary>
        public static Action<Control> OnControlRemoved;
        /// <summary>
        /// The current visibility state of the game
        /// </summary>
        public static Visibility CurrentVisibility
        {
            get
            {
                Visibility ret = Visibility.None;

                if (Main.gameMenu)
                    ret = Visibility.Menu;
                else if (Main.playerInventory)
                    ret = Visibility.IngameInv;
                else
                    ret = Visibility.IngameNoInv;

                return ret;
            }
        }

        static SdkUI()
        {
            CustomUIs = new List<CustomUI>();
            CustomUIs.Add(CustomUI);
        }

        internal SdkUI(ModBase @base)
            : base(@base)
        {

        }

        internal static void Update()
        {
            for (int i = 0; i < CustomUIs.Count; i++)
                if ((CustomUIs[i].Visibility & CurrentVisibility) == CurrentVisibility)
                    CustomUIs[i].Update();
        }

        /// <summary>
        /// Draws the UI before all other UIs are drawn.
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the UI</param>
        /// <returns>Wether it should continue drawing or not</returns>
        [CallPriority(Single.PositiveInfinity)]
        public override bool PreDrawInterface(SpriteBatch sb)
        {
            Draw(sb, false);

            return base.PreDrawInterface(sb);
        }
        /// <summary>
        /// Draws the UI after all other UIs are drawn.
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the UI</param>
        [CallPriority(Single.NegativeInfinity)]
        public override void PostDrawInterface(SpriteBatch sb)
        {
            base.PostDrawInterface(sb);

            Draw(sb, true);
        }

        static void Draw(SpriteBatch sb, bool after)
        {
            for (int i = 0; i < CustomUIs.Count; i++)
                if ((CustomUIs[i].Visibility & CurrentVisibility) == CurrentVisibility && CustomUIs[i].IsDrawnAfter == after)
                    CustomUIs[i].Update();
        }

        /// <summary>
        /// Adds a control to the GUI
        /// </summary>
        /// <param name="ui">The control to add</param>
        public static void AddUI(CustomUI ui)
        {
            CustomUIs.Add(ui);

            ui.Init();

            if (OnUIAdded != null)
                OnUIAdded(ui);
        }
        /// <summary>
        /// Removes a control from the GUI
        /// </summary>
        /// <param name="ui">The CustomUI to remove</param>
        public static void RemoveUI(CustomUI ui)
        {
            CustomUIs.Remove(ui);

            if (OnUIRemoved != null)
                OnUIRemoved(ui);

            ui.Dispose();
        }
        /// <summary>
        /// Removes a CustomUI from the GUI
        /// </summary>
        /// <param name="index">The index of the CustomUI to remove</param>
        public static void RemoveUIAt(int index)
        {
            RemoveUI(CustomUIs[index]);
        }

        /// <summary>
        /// Adds a control to the GUI
        /// </summary>
        /// <param name="control">The control to add</param>
        public static void AddControl(Control control)
        {
            control.ID = CustomUI.Controls.Count;

            CustomUI.Controls.Add(control);

            control.Init();

            if (OnControlAdded != null)
                OnControlAdded(control);
            if (control.OnAdded != null)
                control.OnAdded(control, null);
            if (Control.GlobalAdded != null)
                Control.GlobalAdded(control, null);
        }
        /// <summary>
        /// Removes a control from the GUI
        /// </summary>
        /// <param name="control">The control to remove</param>
        public static void RemoveControl(Control control)
        {
            CustomUI.Controls.Remove(control);

            if (OnControlRemoved != null)
                OnControlRemoved(control);
            if (control.OnRemoved != null)
                control.OnRemoved(control, null);
            if (Control.GlobalRemoved != null)
                Control.GlobalRemoved(control, null);

            control.Dispose();
        }
        /// <summary>
        /// Removes a control from the GUI
        /// </summary>
        /// <param name="index">The index of the control to remove</param>
        public static void RemoveControlAt(int index)
        {
            RemoveControl(CustomUI.Controls[index]);
        }

        internal static void Reset()
        {
            Controls.Clear();
            CustomUIs.Clear();

            WhitePixel.Dispose();

            OnControlAdded = null;
            OnControlRemoved = null;
            OnUIAdded = null;
            OnUIRemoved = null;

            GUI.CustomUI.GlobalControlAdded = null;
            GUI.CustomUI.GlobalControlRemoved = null;

            // ---

            Control.GlobalAdded = null;
            Control.GlobalDraw = null;
            Control.GlobalInit = null;
            Control.GlobalRemoved = null;
            Control.GlobalUpdate = null;

            // ---

            ControlWrapper.GlobalDraw = null;
            ControlWrapper.GlobalUpdate = null;

            // ---

            Button.GlobalClick = null;

            Checkable.GlobalChecked = null;
            Checkable.GlobalUnchecked = null;

            ControlContainer.GlobalAddControl = null;
            ControlContainer.GlobalRemoveControl = null;

            Focusable.GlobalGotFocus = null;
            Focusable.GlobalLostFocus = null;
            Focusable.GlobalBeginHover = null;
            Focusable.GlobalEndHover = null;

            ListeningControl.GlobalInputGot = null;

            // ---

            ItemContainer.GlobalItemChanged = null;
            ItemContainer.GlobalTrySetItem = null;

            Slider.GlobalValueChanged = null;

            Window.GlobalClosed = null;
            Window.GlobalDragging = null;
            Window.GlobalDraggingStarted = null;
            Window.GlobalDraggingStopped = null;
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
        public static void DrawOutlinedString(SpriteBatch sb, SpriteFont font, string text, Vector2 position, Color foreground, Color? background = null, float offset = 1f,
            Vector2? scale = null, float rotation = 0f, Vector2 origin = default(Vector2), SpriteEffects spriteEffects = SpriteEffects.None, float layerDepth = 0f)
        {
            foreach (Vector2 v in new Vector2[] { new Vector2(offset, 0f), new Vector2(0f, offset), new Vector2(-offset, 0f), new Vector2(0f, -offset) })
                sb.DrawString(font, text, position, background ?? Color.Black, rotation, origin, scale ?? new Vector2(1f), spriteEffects, layerDepth);

            sb.DrawString(font, text, position, foreground, rotation, origin, scale ?? new Vector2(1f), spriteEffects, layerDepth);
        }
    }
}
