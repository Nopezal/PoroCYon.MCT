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
        public static WrapperDictionary<IControlParent, int> Hovered
        {
            get;
            internal set;
        }
        public static WrapperDictionary<IControlParent, int> Focused
        {
            get;
            internal set;
        }

		public bool CanFocus = true, StayFocused = true;
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

                Main.PlaySound("vanilla:menuTick");

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
        public bool ForceHover
        {
            private get;
            set;
        }
		public bool ForceFocus
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
            Hovered = new WrapperDictionary<IControlParent, int>();
            Focused = new WrapperDictionary<IControlParent, int>();
        }

        protected virtual void BeginHover() { }
        protected virtual void EndHover() { }
        protected virtual void FocusGot() { }
        protected virtual void FocusLost() { }

        public override void Update()
        {
			if (!CanFocus || !Parent.IsAlive)
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

                IsHovered = Main.localPlayer.mouseInterface = true;

                if (GInput.Mouse.Left || ForceFocus)
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
                else if (IsFocused && !StayFocused)
                {
                    Focused[Parent.Target] = -1;

                    FocusLost();

                    if (OnLostFocus != null)
                        OnLostFocus(this);
                    if (GlobalLostFocus != null)
                        GlobalLostFocus(this);
                }

                ForceHover = ForceFocus = false;
            }
            else
            {
                if (GInput.Mouse.Left)
                {
                    Focused[Parent.Target] = -1;

                    FocusLost();

                    if (OnLostFocus != null)
                        OnLostFocus(this);
                    if (GlobalLostFocus != null)
                        GlobalLostFocus(this);
                }
                if (IsHovered)
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
}
