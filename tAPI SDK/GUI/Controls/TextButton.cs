using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TAPI.SDK.GUI.Controls.Primitives;

namespace TAPI.SDK.GUI.Controls
{
    public class TextButton : Button, ITextControl
    {
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
                return new Rectangle((int)Position.X - (int)padding.X, (int)Position.Y - (int)padding.Y,
                    (int)(Scale.X * Font.MeasureString(Text).X) + (int)padding.X, (int)(Scale.Y * Font.MeasureString(Text).Y) + (int)padding.Y);
            }
        }

        public TextButton()
            : base()
        {
            Text = "Button";
            Font = Main.fontMouseText;
        }
        public TextButton(string text)
            : base()
        {
            Text = text;
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            DrawBackground(sb);

            sb.DrawString(Font, Text, Position, Colour, Rotation, Origin, Scale, SpriteEffects, LayerDepth);
        }
    }
}
