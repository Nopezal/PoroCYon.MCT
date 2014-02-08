using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TAPI.SDK.GUI.Controls.Primitives;

namespace TAPI.SDK.GUI.Controls
{
    public class TextBlock : Control, ITextControl
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
                    (int)(Scale.X * Font.MeasureString(Text).X), (int)(Scale.Y * Font.MeasureString(Text).Y));
            }
        }

        public TextBlock()
            : base()
        {
            Text = "";
            Font = Main.fontMouseText;
        }
        public TextBlock(string text)
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
