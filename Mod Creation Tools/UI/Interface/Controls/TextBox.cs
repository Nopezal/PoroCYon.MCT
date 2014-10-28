using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PoroCYon.Extensions;
using Terraria;
using PoroCYon.MCT.ObjectModel;
using PoroCYon.MCT.UI.Interface.Controls.Primitives;
using PoroCYon.MCT.Input;

namespace PoroCYon.MCT.UI.Interface.Controls
{
    /// <summary>
    /// When a TextBox should stop reading text when enter is hit
    /// </summary>
    /// <remarks>
    /// Enumeration is a set of flags
    /// </remarks>
    [Flags]
    public enum EnterMode
    {
        /// <summary>
        /// Never stop reading when enter is hit
        /// </summary>
        None = 0,

        /// <summary>
        /// Stop reading when enter is hit, and not when shift+enter is hit
        /// </summary>
        Enter = 1,
        /// <summary>
        /// Stop reading when shift+enter is hit, and not when enter is hit.
        /// </summary>
        ShiftEnter = 2,

        /// <summary>
        /// Stops reading when enter or shift+enter is hit
        /// </summary>
        EnterOrShiftEnter = 3
    }
    /// <summary>
    /// The behaviour of tabs in a TextBox
    /// </summary>
    /// <remarks>
    /// You can specify any nonnegative and non-zero number as tab amount
    /// </remarks>
    public enum TabMode : int
    {
        /// <summary>
        /// A tab (U+9) is inserted
        /// </summary>
        Tab = 0,

        /// <summary>
        /// One space is inserted
        /// </summary>
        OneSpace = 1,
        /// <summary>
        /// Two spaces are inserted
        /// </summary>
        TwoSpaces = 2,
        /// <summary>
        /// Three spaces are inserted
        /// </summary>
        ThreeSpaces = 3,
        /// <summary>
        /// Four spaces are inserted
        /// </summary>
        FourSpaces = 4
    }

    /// <summary>
    /// A box with editable text
    /// </summary>
    [ComVisible(false)]
    public class TextBox : ListeningControl<char>, ICaretObject
    {
        static int enterCount = 0;

        /// <summary>
        /// The size of the TextBox
        /// </summary>
        public Vector2? Size = null;

        /// <summary>
        /// Gets the maximum length for the input string. Negative (and nonzero) means infinite.
        /// </summary>
        /// <remarks>
        /// This is ignored if Size is smaller.
        /// </remarks>
        public int MaxLength = -1;

        /// <summary>
        /// The behaviour of the TextBox when Enter is pressed
        /// </summary>
        public EnterMode EnterMode = EnterMode.Enter;
        /// <summary>
        /// The behaviour of the TextBox when Tab is pressed
        /// </summary>
        public TabMode TabMode = TabMode.Tab;

        /// <summary>
        /// The font of the TextBox
        /// </summary>
        public SpriteFont Font
        {
            get;
            set;
        }

        /// <summary>
        /// The text of the TextBox
        /// </summary>
        /// <remarks>
        /// Does not include the ca
        /// </remarks>
        public string Text
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the default text of the <see cref="TextBox" />.
        /// </summary>
        public string DefaultText
        {
            get;
            set;
        }

        /// <summary>
        /// The text of the Textbox that does include the caret
        /// </summary>
        public string TextWithCaret
        {
            get
            {
                return Text + (IsCaretVisible && Listening ? "|" : " ");
            }
        }
        /// <summary>
        /// The caret timer of the TextBox
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
        /// The hitbox of the Control
        /// </summary>
        public override Rectangle Hitbox
        {
            get
            {
                return new Rectangle((int)Position.X - 8, (int)Position.Y - 8,
                    (int)(Scale.X * (Size.HasValue ? Size.Value.X : Font.MeasureString(Text.IsEmpty() && !IsFocused ? DefaultText : TextWithCaret).X)) + 16,
                    (int)(Scale.Y * (Size.HasValue ? Size.Value.Y : Font.MeasureString(Text.IsEmpty() && !IsFocused ? DefaultText : TextWithCaret).Y)) + 16);
            }
        }

        /// <summary>
        /// Creates a new instance of the TextBox class
        /// </summary>
        public TextBox()
            : this("")
        {

        }

        /// <summary>
        /// Creates a new instance of the TextBox class
        /// </summary>
        /// <param name="defaultText">The text of the TextBox</param>
        public TextBox(string defaultText)
            : base()
        {
            Text = defaultText;
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

            Main.clrInput();

            Listening = true;
        }

        /// <summary>
        /// Makes the Focusable lose its focus
        /// </summary>
        protected override void FocusLost()
        {
            base.FocusLost();

            Main.clrInput();

            Listening = false;
        }

        /// <summary>
        /// Updates the Control
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (Listening)
            {
                Main.inputTextEnter = true;
                Main.chatText = "";

                string @new = Main.GetInputText(Text);

                // tabs
                if (GInput.Keyboard.IsKeyDown(Keys.Tab))
                {
                    if (TabMode <= 0)
                        @new += '\t';
                    else
                        for (int i = 0; i < (int)TabMode; i++)
                            @new += ' ';
                }

                #region enter
                bool doEnter = GInput.Keyboard.KeyJustPressed(Keys.Enter);
                if (GInput.Keyboard.IsKeyDown(Keys.Enter) && GInput.OldKeyboard.IsKeyDown(Keys.Enter))
                {
                    if (enterCount == 0)
                    {
                        enterCount = 7;
                        doEnter = true;
                    }
                    enterCount--;
                }
                else
                    enterCount = 15;

                if (doEnter)
                {
                    bool shift = GInput.Keyboard.IsKeyDown(Keys.LeftShift) || GInput.Keyboard.IsKeyDown(Keys.RightShift);

                    if (((EnterMode & EnterMode.Enter) != 0 && !shift) || ((EnterMode & EnterMode.ShiftEnter) != 0 && shift))
                        IsFocused = Listening = false;
                    else
                        @new += '\n';
                }
                #endregion

                if (@new != Text)
                {
                    Text = @new;

                    if (Text.Length > MaxLength && MaxLength > 0)
                        Text = Text.Substring(0, MaxLength);

                    #region resize
                    if (Size.HasValue)
                    {
                        while (Font.MeasureString(Text).X > Size.Value.X)
                            Text = Text.Substring(0, Text.Length - 1);

                        while (Font.MeasureString(Text).Y > Size.Value.Y)
                        {
                            string old = Text;
                            Text = Text.Remove(Text.LastIndexOf("\r\n"), 1);

                            // no \r\n, \n maybe?
                            if (old == Text)
                            {
                                Text = Text.Remove(Text.LastIndexOf('\n'), 1);

                                // no \n either, \r maybe?
                                if (old == Text)
                                {
                                    Text = Text.Remove(Text.LastIndexOf('\r'), 1);

                                    // none of the above (no newlines found), prevent an infinite loop.
                                    if (old == Text)
                                        break;
                                }
                            }
                        }
                    }
                    #endregion

                    Main.PlaySound("Vanilla:menuTick");
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

            DrawBackground(sb);

            if (!Text.IsEmpty() || IsFocused)
                DrawOutlinedString(sb, Font, TextWithCaret, Position + (Size.HasValue ? Size.Value / 2f - Font.MeasureString(Text) / 2f : Vector2.Zero), Colour);
            else
                DrawOutlinedString(sb, Font, DefaultText  , Position + (Size.HasValue ? Size.Value / 2f - Font.MeasureString(Text) / 2f : Vector2.Zero), Color.Lerp(Colour, Color.Black, 0.4f));
        }
    }
}
