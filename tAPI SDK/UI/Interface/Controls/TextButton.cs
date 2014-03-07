using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TAPI.SDK.ObjectModel;
using TAPI.SDK.UI.Interface.Controls.Primitives;

namespace TAPI.SDK.UI.Interface
{
    /// <summary>
    /// A button with text
    /// </summary>
    public class TextButton : Button, ITextObject
    {
        /// <summary>
        /// The font of the TextButton
        /// </summary>
        public SpriteFont Font
        {
            get;
            set;
        }

        /// <summary>
        /// The text drawn by the TextButton
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// The hitbox of the TextButton
        /// </summary>
        public override Rectangle Hitbox
        {
            get
            {
                return new Rectangle((int)Position.X - 16, (int)Position.Y - 8,
                    (int)(Scale.X * Font.MeasureString(Text).X) + 32, (int)(Scale.Y * Font.MeasureString(Text).Y) + 16);
            }
        }

        /// <summary>
        /// Creates a new instance of the TextButton class
        /// </summary>
        public TextButton()
            : this("Button")
        {

        }
        /// <summary>
        /// Creates a new instance of the TextButton class
        /// </summary>
        /// <param name="text">The text of the TextButton</param>
        public TextButton(string text)
            : base()
        {
            Text = text;
            Font = Main.fontMouseText;
        }

        /// <summary>
        /// Draws the Control
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the Control</param>
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            if (HasBackground)
                DrawBackground(sb);

            DrawOutlinedString(sb, Font, Text, Colour);
        }
    }
}
