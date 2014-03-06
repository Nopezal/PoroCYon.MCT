using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TAPI.SDK.Input;
using TAPI.SDK.ObjectModel;

namespace TAPI.SDK.UI.Interface.Controls.Primitives
{
    /// <summary>
    /// A Control that can be focused
    /// </summary>
    public abstract class Focusable : Control
    {
        /// <summary>
        /// The hovered Focusable for each IControlParent
        /// </summary>
        public static WrapperDictionary<IControlParent, int> Hovered
        {
            get;
            internal set;
        }
        /// <summary>
        /// The focused Focusable for each IControlParent
        /// </summary>
        public static WrapperDictionary<IControlParent, int> Focused
        {
            get;
            internal set;
        }

        /// <summary>
        /// Wether the Focusable can be focused or not. Default is true.
        /// </summary>
        public bool CanFocus = true;
        /// <summary>
        /// Wether the Focusable will stay focused or not. Default is true.
        /// </summary>
        public bool StayFocused = true;

        /// <summary>
        /// Wether the Focusable is hovered or not.
        /// </summary>
        public bool IsHovered
        {
            get
            {
                if (!Parent.IsAlive)
                    return false;

                if (!Hovered.ContainsKey(Parent.Target))
                {
                    Hovered.Add(Parent.Target, -1);
                    return false;
                }
                return ID == Hovered[Parent.Target];
            }
            private set
            {
                if (!Parent.IsAlive)
                    return;

                Hovered[Parent.Target] = value ? ID : -1;

                for (int i = 0; i < Parent.Target.Controls.Count; i++)
                    if (Parent.Target.Controls[i] is Focusable)
                    {
                        Focusable f = (Focusable)Parent.Target.Controls[i];

                        if (f.ID == Hovered[Parent.Target])
                            f.BeginHover();
                        else
                            f.EndHover();
                    }
            }
        }
        /// <summary>
        /// Wether the Focusable is focused or not.
        /// </summary>
        public bool IsFocused
        {
            get
            {
                if (!Parent.IsAlive)
                    return false;

                if (!Focused.ContainsKey(Parent.Target))
                {
                    Focused.Add(Parent.Target, -1);
                    return false;
                }
                return ID == Focused[Parent.Target];
            }
            set
            {
                if (!Parent.IsAlive)
                    return;

                Focused[Parent.Target] = value ? ID : -1;

                for (int i = 0; i < Parent.Target.Controls.Count; i++)
                    if (Parent.Target.Controls[i] is Focusable)
                    {
                        Focusable f = (Focusable)Parent.Target.Controls[i];

                        if (f.ID == Focused[Parent.Target])
                            f.FocusGot();
                        else
                            f.FocusLost();
                    }
            }
        }
        /// <summary>
        /// Wether the Focusable was focused in the previous tick or not
        /// </summary>
        public bool OldIsFocused
        {
            get;
            private set;
        }

        /// <summary>
        /// Sets wether the Focusable will automatically be hovered or not.
        /// </summary>
        public bool ForceHover
        {
            private get;
            set;
        }
        /// <summary>
        /// Sets wether the Focusable will automatically be focused or not.
        /// </summary>
        public bool ForceFocus
        {
            private get;
            set;
        }

        /// <summary>
        /// When the Focusable becomes hovered
        /// </summary>
        public Action<Focusable> OnBeginHover;
        /// <summary>
        /// When the Focusable isn't hovered anymore
        /// </summary>
        public Action<Focusable> OnEndHover;
        /// <summary>
        /// When the Focusable becomes focused
        /// </summary>
        public Action<Focusable> OnGotFocus;
        /// <summary>
        /// When the Focusable lost its focus
        /// </summary>
        public Action<Focusable> OnLostFocus;

        /// <summary>
        /// When a Focusable becomes hovered
        /// </summary>
        public static Action<Focusable> GlobalGotFocus;
        /// <summary>
        /// When a Focusable isn't hovered anymore
        /// </summary>
        public static Action<Focusable> GlobalLostFocus;
        /// <summary>
        /// When a Focusable becomes focused
        /// </summary>
        public static Action<Focusable> GlobalBeginHover;
        /// <summary>
        /// When a Focusable lost its focus
        /// </summary>
        public static Action<Focusable> GlobalEndHover;

        /// <summary>
        /// Creates a new instance of the Focusable class
        /// </summary>
        public Focusable()
            : base()
        {
            ForceHover = false;
        }

        static Focusable()
        {
            Hovered = new WrapperDictionary<IControlParent, int>();
            Focused = new WrapperDictionary<IControlParent, int>();
        }

        /// <summary>
        /// Begins hovering the Focusable
        /// </summary>
        protected virtual void BeginHover()
        {
            if (OnBeginHover != null)
                OnBeginHover(this);
            if (GlobalBeginHover != null)
                GlobalBeginHover(this);
        }
        /// <summary>
        /// Stops hovering the Focusable
        /// </summary>
        protected virtual void EndHover()
        {
            if (OnEndHover != null)
                OnEndHover(this);
            if (GlobalEndHover != null)
                GlobalEndHover(this);
        }

        /// <summary>
        /// Gives the Focusable focus
        /// </summary>
        protected virtual void FocusGot()
        {
            if (OnGotFocus != null)
                OnGotFocus(this);
            if (GlobalGotFocus != null)
                GlobalGotFocus(this);
        }
        /// <summary>
        /// Makes the Focusable lose its focus
        /// </summary>
        protected virtual void FocusLost()
        {
            if (OnLostFocus != null)
                OnLostFocus(this);
            if (GlobalLostFocus != null)
                GlobalLostFocus(this);
        }

        /// <summary>
        /// Updates the Control
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (!CanFocus || !Parent.IsAlive)
                return;

            OldIsFocused = IsFocused;

            if (GInput.Mouse.Rectangle.Intersects(Hitbox) || ForceHover)
            {
                if (!IsHovered)
                    BeginHover();

                IsHovered = Main.localPlayer.mouseInterface = true;
            }
            else if (IsHovered)
            {
                EndHover();

                IsHovered = false;
            }

            if ((GInput.Mouse.Left && IsHovered) || ForceFocus)
            {
                if (!IsFocused)
                    FocusGot();

                IsFocused = Main.localPlayer.mouseInterface = true;
            }
            else if (IsFocused && !StayFocused)
            {
                IsFocused = false;

                FocusLost();
            }

            ForceHover = ForceFocus = false;
        }

        /// <summary>
        /// Draws a blue background behind the control
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the background</param>
        protected new void DrawBackground(SpriteBatch sb)
        {
            if (!HasBackground)
                return;

            DrawBackground(sb, Hitbox);
        }
        /// <summary>
        /// Draws a blue background behind the control
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the background</param>
        /// <param name="hb">The bounds of the background to draw</param>
        protected new void DrawBackground(SpriteBatch sb, Rectangle hb)
        {
            if (!HasBackground)
                return;

            Drawing.DrawBlueBox(sb, hb.X, hb.Y, hb.Width, hb.Height, IsHovered ? 0.85f : 0.75f);
        }
    }
}
