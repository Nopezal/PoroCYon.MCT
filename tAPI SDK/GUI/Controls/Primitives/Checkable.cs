using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TAPI.SDK.GUI.Controls.Primitives
{
	public enum BoxLocation
	{
		Left,
		Right,
		Top,
		Bottom
	}
	public abstract class Checkable : Focusable, ITextControl
	{
		public bool IsChecked = false;

		public BoxLocation Location = BoxLocation.Left;

		public Action<Checkable> Check = null, Uncheck;
		public static Action<Checkable> GlobalChecked = null, GlobalUnchecked;

		public string Text
		{
			get;
			set;
		}
		public SpriteFont Font
		{
			get;
			set;
		}

		public override Rectangle Hitbox
		{
			get
			{
				Rectangle ret = new Rectangle((int)Position.X, (int)Position.Y, (int)(Font.MeasureString(Text).X * Scale.X), (int)(Font.MeasureString(Text).Y * Scale.Y));

				switch (Location)
				{
					case BoxLocation.Left:
					case BoxLocation.Right:
						ret.Width += (int)(16f * Scale.X) + 8;
						break;
					case BoxLocation.Bottom:
					case BoxLocation.Top:
						ret.Height += (int)(16f * Scale.Y) + 8;
						break;
				}

				return ret;
			}
		}

		public virtual Vector2 BoxPosition
		{
			get
			{
				Vector2 pos = Position;

				switch (Location)
				{
					case BoxLocation.Left:
						// nothing
						break;
					case BoxLocation.Right:
						pos.X += Font.MeasureString(Text).X * Scale.X;
						break;
					case BoxLocation.Top:
					case BoxLocation.Bottom:
						pos.X += Hitbox.Width / 2f - 8f;

						if (Location == BoxLocation.Bottom)
							pos.Y += Hitbox.Height - 24f;
						break;
				}

				return pos;
			}
		}
		public virtual Vector2 TextPosition
		{
			get
			{
				Vector2 pos = Position;

				switch (Location)
				{
					case BoxLocation.Left:
						pos.Y += 16f * Scale.X + 8f;
						break;
					case BoxLocation.Bottom:
					case BoxLocation.Right:
						// nothing
						break;
					case BoxLocation.Top:
						pos.Y += 16f * Scale.Y + 8f;
						break;
				}

				return pos;
			}
		}
		public virtual Rectangle BoxHitbox
		{
			get
			{
				return new Rectangle((int)BoxPosition.X, (int)BoxPosition.Y, (int)(16 * Scale.X), (int)(16 * Scale.X));
			}
		}

		protected override void FocusGot()
		{
			if (IsChecked = !IsChecked)
			{
				Checked();

				if (Check != null)
					Check(this);
				if (GlobalChecked != null)
					GlobalChecked(this);
			}
			else
			{
				Unchecked();

				if (Uncheck != null)
					Uncheck(this);
				if (GlobalUnchecked != null)
					GlobalUnchecked(this);
			}

			Main.PlaySound("vanilla:menuTick");

			base.FocusGot();

			IsFocused = false;
		}

		protected virtual void Checked() { }
		protected virtual void Unchecked() { }
	}
}
