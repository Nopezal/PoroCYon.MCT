using System;
using System.Collections.Generic;
using System.Linq;
using PoroCYon.XnaExtensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TAPI.SDK.GUI.Controls
{
    /// <summary>
    /// This class cannot be inherited
    /// </summary>
    [CLSCompliant(false)]
    public sealed class MenuButtonWrapper : MenuButton, IDisposable, ICloneable<MenuButtonWrapper>
    {
        bool inited = false;

        public Control Control;

        public Action<MenuButtonWrapper> OnUpdate;
        public Action<MenuButtonWrapper, SpriteBatch, bool> OnDraw;
        public static Action<MenuButtonWrapper> GlobalUpdate;
        public static Action<MenuButtonWrapper, SpriteBatch, bool> GlobalDraw;

        public MenuButtonWrapper()
            : this(Vector2.Zero, Vector2.Zero)
        {
            
        }
        public MenuButtonWrapper(Control control)
            : this(Vector2.Zero, Vector2.Zero, control)
        {

        }
        public MenuButtonWrapper(params Control[] controls)
            : this(Vector2.Zero, Vector2.Zero, new ControlGroup(controls))
        {

        }

        public MenuButtonWrapper(Vector2 anchor, Vector2 offset)
            : base(anchor, offset, "", "")
        {
            Update += () =>
            {
                if (!inited)
                {
                    Control.Position += position;
                    Control.Init();
                    Control.Position -= position;

                    inited = true;
                }

                Control.Position += position;
                Control.Update();
                Control.Position -= position;

                if (OnUpdate != null)
                    OnUpdate(this);
                if (GlobalUpdate != null)
                    GlobalUpdate(this);
            };
        }
        public MenuButtonWrapper(Vector2 anchor, Vector2 position, Control control)
            : this(anchor, position)
        {
            Control = control;
        }
        public MenuButtonWrapper(Vector2 anchor, Vector2 position, params Control[] control)
            : this(anchor, position, new ControlGroup(control))
        {

        }

        ~MenuButtonWrapper()
        {
            Control.Dispose();
        }
        public void Dispose()
        {
            Control.Dispose();
        }

        public override void Draw(SpriteBatch sb, bool mouseOver)
        {
            Control.Position += position;
            Control.Draw(sb);
            Control.Position -= position;

            if (OnDraw != null)
                OnDraw(this, sb, mouseOver);
            if (GlobalDraw != null)
                GlobalDraw(this, sb, mouseOver);

            base.Draw(sb, mouseOver);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
        public MenuButtonWrapper Copy()
        {
            return (MenuButtonWrapper)MemberwiseClone();
        }

        public static explicit operator MenuButtonWrapper(Control control)
        {
            MenuButtonWrapper ret = new MenuButtonWrapper(Vector2.Zero, control.Position, control);
            control.Position = Vector2.Zero;
            return ret;
        }
        public static implicit operator Control(MenuButtonWrapper wrapper)
        {
            Control ret = wrapper.Control.Copy();
            ret.Position += wrapper.position;
            return ret;
        }
    }
}
