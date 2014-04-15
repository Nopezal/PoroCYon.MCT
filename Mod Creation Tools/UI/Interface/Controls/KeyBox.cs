using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using TAPI;
using PoroCYon.MCT.Input;
using PoroCYon.MCT.ObjectModel;
using PoroCYon.MCT.UI.Interface.Controls.Primitives;

namespace PoroCYon.MCT.UI.Interface.Controls
{
    /// <summary>
    /// A control that listens to key input
    /// </summary>
    [ComVisible(false)]
    public class KeyBox : ListeningControl<Keys>, ICaretObject
    {
        Keys old = Keys.None;

        /// <summary>
        /// The key that was pressed in the KeyBox
        /// </summary>
        public Keys Pressed
        {
            get;
            private set;
        }

        /// <summary>
        /// The SpriteFont of the KeyBox
        /// </summary>
        public SpriteFont Font
        {
            get;
            set;
        }

        /// <summary>
        /// The text of the KeyBox.
        /// Setting it will thow a NotSupportedException.
        /// Includes the caret.
        /// </summary>
        public string Text
        {
            get
            {
                return Pressed == Keys.None ? (IsCaretVisible && Listening ? "_" : " ") : Pressed.ToString();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// The timer of the caret of the KeyBox
        /// </summary>
        public int CaretTimer
        {
            get;
            private set;
        }
        /// <summary>
        /// Wether the caret is visible or not
        /// </summary>
        public bool IsCaretVisible
        {
            get
            {
                return CaretTimer >= 30;
            }
        }

        /// <summary>
        /// The text of the KeyBox. Includes the caret.
        /// </summary>
        public string TextWithCaret
        {
            get
            {
                return Text;
            }
        }

        /// <summary>
        /// The hitbox of the control
        /// </summary>
        public override Rectangle Hitbox
        {
            get
            {
                return new Rectangle((int)Position.X - 8, (int)Position.Y - 8,
                    (int)(Scale.X * Font.MeasureString(Text).X) + 16, (int)(Scale.Y * Font.MeasureString(Text).Y) + 16);
            }
        }

        /// <summary>
        /// Creates a new instance of the KeyBox class
        /// </summary>
        public KeyBox()
            : this(Keys.None)
        {

        }
        /// <summary>
        /// Creates a new instance of the KeyBox class
        /// </summary>
        /// <param name="defaultKey">The key that was entered in the KeyBox</param>
        public KeyBox(Keys defaultKey)
            : base()
        {
            Pressed = defaultKey;

            Font = Main.fontMouseText;
            CaretTimer = 60;

            StayFocused = true;

            ListensToKeyboard = true;
        }

        /// <summary>
        /// Gives the Focusable focus
        /// </summary>
        protected override void FocusGot()
        {
            base.FocusGot();

            Listening = true;

            old = Pressed;
            Pressed = Keys.None;
        }
        /// <summary>
        /// Makes the Focusable lose its focus
        /// </summary>
        protected override void FocusLost()
        {
            base.FocusLost();

            Listening = false;

            if (Pressed == Keys.None)
                Pressed = old;
        }

        /// <summary>
        /// Updates the Control
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (Listening)
            {
                if (GInput.Keyboard.Keys.Length > 0)
                {
                    Pressed = GInput.Keyboard.Keys[0];

                    GotInput(Pressed);

                    Main.PlaySound("vanilla:menuTick");

                    IsFocused = Listening = false;
                }

                if (--CaretTimer <= 0)
                    CaretTimer = 60;
            }
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
