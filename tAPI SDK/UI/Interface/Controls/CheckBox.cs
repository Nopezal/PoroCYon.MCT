using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TAPI.SDK.UI.Interface.Controls.Primitives;

namespace TAPI.SDK.UI.Interface.Controls
{
    /// <summary>
    /// A box that can be checked and unchecked
    /// </summary>
    public class CheckBox : Checkable
    {
        /// <summary>
        /// Creates a new instance of the CheckBox class
        /// </summary>
        public CheckBox()
            : this(false, "CheckBox")
        {

        }
        /// <summary>
        /// Creates a new instance of the CheckBox class
        /// </summary>
        /// <param name="isChecked">Wether the checkbox is checked or not</param>
        public CheckBox(bool isChecked)
            : this(isChecked, "CheckBox")
        {

        }
        /// <summary>
        /// Creates a new instance of the CheckBox class
        /// </summary>
        /// <param name="text">The text of the checkbox</param>
        public CheckBox(string text)
            : this(false, text)
        {

        }
        /// <summary>
        /// Creates a new instance of the CheckBox class
        /// </summary>
        /// <param name="isChecked">Wether the checkbox is checked or not</param>
        /// <param name="text">The text of the checkbox</param>
        public CheckBox(bool isChecked, string text)
            : base()
        {
            Text = text;
            Font = Main.fontMouseText;
            IsChecked = isChecked;
        }

        /// <summary>
        /// Draws the Control
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the Control</param>
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            Vector2 charSize = Font.MeasureString(DisplayChar.ToString());

            DrawBackground(sb);

            DrawOutlinedString(sb, Font, Text, TextPosition, Colour);

            // draw box

            // top, horizontal
            sb.Draw(SdkUI.WhitePixel, BoxPosition, null, Colour, Rotation, Origin,
                new Vector2(charSize.X + 6f, 2f) * Scale, SpriteEffects, LayerDepth);
            // bottom, horizontal
            sb.Draw(SdkUI.WhitePixel, BoxPosition + new Vector2(0f, charSize.Y - 4f) * Scale, null, Colour, Rotation, Origin,
                new Vector2(charSize.X + 8f, 2f) * Scale, SpriteEffects, LayerDepth);

            // left, vertical
            sb.Draw(SdkUI.WhitePixel, BoxPosition, null, Colour, Rotation, Origin,
                new Vector2(2f, charSize.Y - 4f) * Scale, SpriteEffects, LayerDepth);
            // right, vertical
            sb.Draw(SdkUI.WhitePixel, BoxPosition + new Vector2(charSize.X + 6f, 0f) * Scale, null, Colour, Rotation, Origin,
                new Vector2(2f, charSize.Y - 4f) * Scale, SpriteEffects, LayerDepth);

            if (IsChecked)
                SdkUI.DrawOutlinedString(sb, Font, DisplayChar.ToString(), BoxPosition + new Vector2(4f), Colour, null, 1f, Scale, Rotation, Origin, SpriteEffects, LayerDepth);
        }
    }
}
