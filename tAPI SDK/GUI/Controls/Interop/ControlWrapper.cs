using System;
using System.Collections.Generic;
using System.Linq;
using PoroCYon.XnaExtensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TAPI.SDK.GUI.Controls.Interop
{
    /// <summary>
	/// A Control as a MenuButton
    /// This class cannot be inherited
    /// </summary>
    public sealed class ControlWrapper : MenuButton, IDisposable, ICloneable<MenuButtonWrapper>
    {
        bool inited = false;

        public Control Control;

		public Action<ControlWrapper> OnUpdate;
		public Action<ControlWrapper, SpriteBatch, bool> OnDraw;
		public static Action<ControlWrapper> GlobalUpdate;
		public static Action<ControlWrapper, SpriteBatch, bool> GlobalDraw;

        public ControlWrapper()
            : this(Vector2.Zero, Vector2.Zero)
        {
            
        }
        public ControlWrapper(Control control)
            : this(Vector2.Zero, Vector2.Zero, control)
        {

        }
        public ControlWrapper(params Control[] controls)
            : this(Vector2.Zero, Vector2.Zero, new ControlGroup(controls))
        {

        }
        public ControlWrapper(Vector2 anchor, Vector2 position, Control control)
            : this(anchor, position)
        {
            Control = control;
        }
        public ControlWrapper(Vector2 anchor, Vector2 position, params Control[] control)
            : this(anchor, position, new ControlGroup(control))
        {

        }

		public ControlWrapper(Vector2 anchor, Vector2 offset)
			: base(anchor, offset, "", "")
		{
			Update += () =>
			{
				if (!inited)
				{
					Control.Init();

					inited = true;
				}

				Control.Update();

				if (OnUpdate != null)
					OnUpdate(this);
				if (GlobalUpdate != null)
					GlobalUpdate(this);
			};
		}

		~ControlWrapper()
        {
            Control.Dispose();
        }
        public void Dispose()
        {
            Control.Dispose();

			GC.SuppressFinalize(this);
        }

        public override void Draw(SpriteBatch sb, bool mouseOver)
        {
            Control.Draw(sb);

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
		public ControlWrapper Copy()
        {
			return (ControlWrapper)MemberwiseClone();
        }

        public static explicit operator ControlWrapper(Control control)
        {
            ControlWrapper ret = new ControlWrapper(Vector2.Zero, control.Position, control);
            control.Position = Vector2.Zero;
            return ret;
        }
        public static implicit operator Control(ControlWrapper wrapper)
        {
            Control ret = wrapper.Control.Copy();
            ret.Position += wrapper.position;
            return ret;
        }
    }
}
