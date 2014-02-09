using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TAPI.SDK.Input;

namespace TAPI.SDK.GUI.Controls.Primitives
{
    public abstract class Focusable : Control
    {
        public static int Hovered
        {
            get;
            private set;
        }
        public static int Focused
        {
            get;
            private set;
        }

		public bool CanFocus = true;
        public bool IsHovered
        {
            get
            {
                return ID == Hovered;
            }
            private set
            {
                Hovered = value ? ID : -1;

                for (int i = 0; i < SdkUI.Controls.Count; i++)
                    if (SdkUI.Controls[i] is Focusable)
                    {
                        Focusable f = (Focusable)SdkUI.Controls[i];

                        if (f.ID == Hovered)
                            f.BeginHover();
                        else
                            f.EndHover();
                    }
            }
        }
        public bool IsFocused
        {
            get
            {
                return ID == Focused;
            }
            set
            {
                Focused = value ? ID : -1;

                Main.PlaySound("vanilla:menuTick");

                for (int i = 0; i < SdkUI.Controls.Count; i++)
                    if (SdkUI.Controls[i] is Focusable)
                    {
                        Focusable f = (Focusable)SdkUI.Controls[i];

                        if (f.ID == Focused)
                            f.FocusGot();
                        else
                            f.FocusLost();
                    }
            }
        }
        public bool ForceHover
        {
            private get;
            set;
        }

        public Action<Focusable> OnGotFocus, OnLostFocus, OnBeginHover, OnEndHover;
        public static Action<Focusable> GlobalGotFocus, GlobalLostFocus, GlobalBeginHover, GlobalEndHover;

        public Focusable()
            : base()
        {
            ForceHover = false;
        }

        static Focusable()
        {
            Hovered = Focused = -1;
        }

        protected virtual void BeginHover() { }
        protected virtual void EndHover() { }
        protected virtual void FocusGot() { }
        protected virtual void FocusLost() { }

        public override void Update()
        {
			if (!CanFocus)
				return;

            if (GInput.Mouse.Rectangle.Intersects(Hitbox) || ForceHover)
            {
                if (!IsHovered)
                {
                    BeginHover();

                    if (OnBeginHover != null)
                        OnBeginHover(this);
                    if (GlobalBeginHover != null)
                        GlobalBeginHover(this);
                }

                IsHovered = true;

                if (GInput.Mouse.Left)
                {
                    if (!IsFocused)
                    {
                        FocusGot();

                        if (OnGotFocus != null)
                            OnGotFocus(this);
                        if (GlobalGotFocus != null)
                            GlobalGotFocus(this);
                    }

                    IsFocused = true;
                }
                else if (IsFocused)
                {
                    FocusLost();

                    if (OnLostFocus != null)
                        OnLostFocus(this);
                    if (GlobalLostFocus != null)
                        GlobalLostFocus(this);
                }

                ForceHover = false;
            }
            else if (IsHovered)
            {
                EndHover();

                if (OnEndHover != null)
                    OnEndHover(this);
                if (GlobalEndHover != null)
                    GlobalEndHover(this);

                IsHovered = false;
            }
        }
    }
}
