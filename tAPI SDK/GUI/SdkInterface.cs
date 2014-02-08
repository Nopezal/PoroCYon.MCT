using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using TAPI.SDK.GUI.Controls;

namespace TAPI.SDK.GUI
{
    [CLSCompliant(false)]
    public sealed class SdkInterface : ModInterface
    {
        public static Texture2D WhitePixel
        {
            get;
            internal set;
        }

        public static List<Control> Controls
        {
            get;
            internal set;
        }

        public static Action<Control> OnControlAdded, OnControlRemoved;

        static SdkInterface()
        {
            Controls = new List<Control>();
        }

        public SdkInterface(ModBase @base)
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

                if (Controls[i].Enabled)
                    Controls[i].Update();
            }
        }

        [CallPriority(Single.PositiveInfinity)]
        public override bool PreDrawInterface(SpriteBatch sb)
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

                if (!Controls[i].IsDrawnAfter && Controls[i].Enabled)
                    Controls[i].Draw(sb);
            }

            return base.PreDrawInterface(sb);
        }
        [CallPriority(Single.NegativeInfinity)]
        public override void PostDrawInterface(SpriteBatch sb)
        {
            base.PostDrawInterface(sb);

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

                if (Controls[i].IsDrawnAfter && Controls[i].Enabled)
                    Controls[i].Draw(sb);
            }
        }

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
    }
}
