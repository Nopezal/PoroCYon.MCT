﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using TAPI.SDK.GUI.Controls;

namespace TAPI.SDK.GUI
{
	/// <summary>
	/// This class cannot be inherited.
    /// </summary>
    [GlobalMod]
    public sealed class SdkUI : ModInterface
    {
        /// <summary>
        /// A 1-by-1 white pixel texture
        /// </summary>
        public static Texture2D WhitePixel
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
            get;
            internal set;
        }

        /// <summary>
        /// Called when a control is added from the Controls list
        /// </summary>
        public static Action<Control> OnControlAdded;
        /// <summary>
        /// Called when a control is removed from the Controls list
        /// </summary>
        public static Action<Control> OnControlRemoved;
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
            Controls = new List<Control>();
        }

        internal SdkUI(ModBase @base)
            : base(@base)
        {
            Controls = new List<Control>();
        }

        internal static void Update()
        {
            for (int i = 0; i < Controls.Count; i++)
            {
                if (Controls[i].Destroyed)
                {
                    if (OnControlRemoved != null)
                        OnControlRemoved(Controls[i]);
                    if (Controls[i].OnRemoved != null)
                        Controls[i].OnRemoved(Controls[i], null);
                    if (Control.GlobalRemoved != null)
                        Control.GlobalRemoved(Controls[i], null);

                    Controls[i].Dispose();

                    Controls.RemoveAt(i--);
                    continue;
                }

                if (Controls[i].Enabled && (Controls[i].Visibility & CurrentVisibility) == CurrentVisibility)
                    Controls[i].Update();
            }
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
            for (int i = 0; i < Controls.Count; i++)
            {
                if (Controls[i].Destroyed)
                {
                    if (OnControlRemoved != null)
                        OnControlRemoved(Controls[i]);
                    if (Controls[i].OnRemoved != null)
                        Controls[i].OnRemoved(Controls[i], null);
                    if (Control.GlobalRemoved != null)
                        Control.GlobalRemoved(Controls[i], null);

                    Controls[i].Dispose();

                    Controls.RemoveAt(i--);
                    continue;
                }

                if (Controls[i].IsDrawnAfter == after && Controls[i].Enabled && (Controls[i].Visibility & CurrentVisibility) == CurrentVisibility)
                    Controls[i].Draw(sb);
            }
        }


        /// <summary>
        /// Adds a control to the GUI
        /// </summary>
        /// <param name="control">The control to add</param>
        public static void AddControl(Control control)
        {
            control.ID = Controls.Count;

            Controls.Add(control);

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
            Controls.Remove(control);

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
            RemoveControl(Controls[index]);
        }
    }
}
