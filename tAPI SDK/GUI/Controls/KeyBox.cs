using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using PoroCYon.XnaExtensions.Input;
using TAPI.SDK.Input;
using TAPI.SDK.GUI.Controls.Primitives;

namespace TAPI.SDK.GUI.Controls
{
    public class KeyBox : ListeningControl<Key>, ITextControl, ICaretControl
    {
        Key old = Key.None;

        public SpriteFont Font
        {
            get;
            set;
        }
        public Key Pressed
        {
            get;
            private set;
        }
        public int CaretCD
        {
            get;
            private set;
        }
        public bool CaretVisible
        {
            get
            {
                return CaretCD >= 30;
            }
        }
        /// <summary>
        /// Setting it will thow a NotSupportedException. Includes the caret.
        /// </summary>
        public string Text
        {
            get
            {
                return Pressed == Key.None ? (CaretVisible ? "_" : " ") : Pressed.ToString();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override Rectangle Hitbox
        {
            get
            {
                return new Rectangle((int)Position.X - (int)padding.X, (int)Position.Y - (int)padding.Y,
                    (int)(Scale.X * Font.MeasureString(Text).X) + (int)padding.X, (int)(Scale.Y * Font.MeasureString(Text).Y) + (int)padding.Y);
            }
        }

        public KeyBox()
            : base()
        {
            Pressed = Key.None;

            Font = Main.fontMouseText;
            CaretCD = 60;
        }
        public KeyBox(Key defaultKey)
            : this()
        {
            Pressed = defaultKey;
        }

        protected override void FocusGot()
        {
            base.FocusGot();

            old = Pressed;
            Pressed = Key.None;
        }
        protected override void FocusLost()
        {
            base.FocusLost();

            if (Pressed == Key.None)
                Pressed = old;
        }

        public override void Update()
        {
            base.Update();

            if (Listening)
            {
                foreach (Key k in GInput.Keyboard.Keys)
                {
                    Pressed = k;

                    Main.PlaySound("vanilla:menuTick");

                    GotInput(k);

                    Listening = false;

                    break;
                }

                CaretCD--;

                if (CaretCD <= 0)
                    CaretCD = 60;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            DrawBackground(sb);

            sb.DrawString(Font, Text, Position, Colour, Rotation, Origin, Scale, SpriteEffects, LayerDepth);

            base.Draw(sb);
        }
    }
}
