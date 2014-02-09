using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TAPI.SDK.GUI.Controls.Primitives;

namespace TAPI.SDK.GUI.Controls
{
	public class RadioButton : Checkable
	{
		static bool preventStackOverflow = false;

		public string GroupName = "";

		public RadioButton()
			: this(null)
		{

		}
		public RadioButton(string groupName)
			: this(groupName, "RadionButton")
		{

		}
		public RadioButton(string groupName, string text)
			: base()
		{
			GroupName = groupName;
			Text = text;
		}

		protected override void Checked()
		{
			if (!Parent.IsAlive ||preventStackOverflow)
				return;

			if (!String.IsNullOrEmpty(GroupName))
			{
				preventStackOverflow = true;

				foreach (Control c in Parent.Target.Controls)
					if (c is RadioButton)
					{
						RadioButton rb = c as RadioButton;

						if (rb.GroupName == GroupName)
							rb.IsChecked = false;
					}

				preventStackOverflow = false;
			}

			base.Checked();
		}
		protected override void Unchecked()
		{
			if (preventStackOverflow)
				return;

			if (!String.IsNullOrEmpty(GroupName))
			{
				preventStackOverflow = true;

				IsChecked = true;

				preventStackOverflow = false;
			}

			base.Unchecked();
		}

		public override void Draw(SpriteBatch sb)
		{
			DrawBackground(sb);

			sb.DrawString(Font, Text, TextPosition, Colour, Rotation, Origin, Scale, SpriteEffects, LayerDepth);

			DrawBackground(sb, BoxHitbox);

			if (IsChecked)
				sb.DrawString(Font, "•", BoxPosition, Colour, Rotation, Origin, Scale, SpriteEffects, LayerDepth);

			base.Draw(sb);
		}
	}
}
