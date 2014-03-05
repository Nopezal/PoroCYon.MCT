using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using PoroCYon.XnaExtensions.Input;
using TAPI.SDK.Input;
using TAPI.SDK.ObjectModel;
using TAPI.SDK.UI.Interface.Controls.Primitives;

namespace TAPI.SDK.UI.Interface.Controls
{
    /// <summary>
    /// A control that listens to key input
    /// </summary>
    [ComVisible(false)]
    public class KeyBox : ListeningControl<Key>, ICaretObject
    {
        Key old = Key.None;

        /// <summary>
        /// The key that was pressed in the KeyBox
        /// </summary>
        public Key Pressed
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
                return Pressed == Key.None ? (IsCaretVisible ? "_" : " ") : Pressed.ToString();
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
        /// The hitbox of the control
        /// </summary>
        public override Rectangle Hitbox
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y,
                    (int)(Scale.X * Font.MeasureString(Text).X), (int)(Scale.Y * Font.MeasureString(Text).Y));
            }
        }

        /// <summary>
        /// Creates a new instance of the KeyBox class
        /// </summary>
        public KeyBox()
            : base()
        {
            Pressed = Key.None;

            Font = Main.fontMouseText;
            CaretTimer = 60;
        }
        /// <summary>
        /// Creates a new instance of the KeyBox class
        /// </summary>
        /// <param name="defaultKey">The key that was entered in the KeyBox</param>
        public KeyBox(Key defaultKey)
            : this()
        {
            Pressed = defaultKey;
        }

        /// <summary>
        /// Gives the Focusable focus
        /// </summary>
        protected override void FocusGot()
        {
            base.FocusGot();

            old = Pressed;
            Pressed = Key.None;
        }
        /// <summary>
        /// Makes the Focusable lose its focus
        /// </summary>
        protected override void FocusLost()
        {
            base.FocusLost();

            if (Pressed == Key.None)
                Pressed = old;
        }

        /// <summary>
        /// Updates the Control
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (IsFocused)
            {
                SomethingIsListening = true;

                if (GInput.Keyboard.Keys.Length > 0)
                {
                    Pressed = GInput.Keyboard.Keys[0];

                    GotInput(Pressed);

                    Main.PlaySound("vanilla:menuTick");

                    IsFocused = false;
                }

                CaretTimer--;

                if (CaretTimer <= 0)
                    CaretTimer = 60;
            }
        }

        /// <summary>
        /// Draws the Control
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the Control</param>
        public override void Draw(SpriteBatch sb)
        {
            DrawBackground(sb);

            DrawOutlinedString(sb, Font, Text, Colour);

            base.Draw(sb);
        }
    }
}
