using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TAPI.SDK.GUI.Controls.Primitives;

namespace TAPI.SDK.GUI.Controls
{
	public class IncreaseDecreaseButton : TextBlock
	{
		TextButton incr, decr;

		public int Value;

		public IncreaseDecreaseButton()
			: base()
		{
			incr = new TextButton("+")
			{
				StaysPressed = true
			};
			incr.Click += (b) =>
			{
				Value++;
			};

			decr = new TextButton("-")
			{
				StaysPressed = true
			};
			decr.Click += (b) =>
			{
				Value--;
			};
		}

		public override Rectangle Hitbox
		{
			get
			{
				Rectangle ret = base.Hitbox;

				ret.Width += incr.Hitbox.Width + decr.Hitbox.Width + 16;

				return ret;
			}
		}

		public override void Update()
		{
			base.Update();

			incr.Position += Position + new Vector2(Hitbox.Width - decr.Hitbox.Width - 16f, 0f);
			incr.Update();
			incr.Position -= Position + new Vector2(Hitbox.Width - decr.Hitbox.Width - 16f, 0f);

			decr.Position += Position + new Vector2(Hitbox.Width - 8f, 0f);
			decr.Update();
			decr.Position -= Position + new Vector2(Hitbox.Width - 8f, 0f);
		}
		public override void Draw(SpriteBatch sb)
		{
			base.Draw(sb);

			incr.Position += Position + new Vector2(Hitbox.Width - decr.Hitbox.Width - 16f, 0f);
			incr.Draw(sb);
			incr.Position -= Position + new Vector2(Hitbox.Width - decr.Hitbox.Width - 16f, 0f);

			decr.Position += Position + new Vector2(Hitbox.Width - 8f, 0f);
			decr.Draw(sb);
			decr.Position -= Position + new Vector2(Hitbox.Width - 8f, 0f);
		}
	}
}
