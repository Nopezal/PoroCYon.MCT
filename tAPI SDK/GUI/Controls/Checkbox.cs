using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TAPI.SDK.GUI.Controls.Primitives;

namespace TAPI.SDK.GUI.Controls
{
	public class CheckBox : Checkable
	{
		public CheckBox()
			: this(false, "CheckBox")
		{

		}
		public CheckBox(bool isChecked)
			: this(isChecked, "CheckBox")
		{

		}
		public CheckBox(string text)
			: this(false, text)
		{

		}
		public CheckBox(bool isChecked, string text)
			: base()
		{
			Text = text;
			IsChecked = isChecked;
		}

		public override void Draw(SpriteBatch sb)
		{
			DrawBackground(sb);

            DrawOutlinedString(sb, Font, Text, Colour);
			//sb.DrawString(Font, Text, TextPosition, Colour, Rotation, Origin, Scale, SpriteEffects, LayerDepth);

			DrawBackground(sb, BoxHitbox);

            if (IsChecked)
                SdkUI.DrawOutlinedString(sb, Font, Text, BoxPosition, Colour, null, 1f, Scale, Rotation, Origin, SpriteEffects, LayerDepth);
				//sb.DrawString(Font, "X", BoxPosition, Colour, Rotation, Origin, Scale, SpriteEffects, LayerDepth);

			base.Draw(sb);
		}
	}
}
