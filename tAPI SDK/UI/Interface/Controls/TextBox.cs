using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using PoroCYon.XnaExtensions.Input;
using TAPI.SDK.ObjectModel;
using TAPI.SDK.UI.Interface.Controls.Primitives;
using TAPI.SDK.Input;

namespace TAPI.SDK.UI.Interface.Controls
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
        StopWhenEnter = 1,
        /// <summary>
        /// Stop reading when shift+enter is hit, and not when enter is hit.
        /// </summary>
        StopWhenShiftEnter = 2,

        /// <summary>
        /// Stops reading when enter or shift+enter is hit
        /// </summary>
        StopWhenEnterOrShiftEnter = 3
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
        /// <summary>
        /// The behaviour of the TextBox when Enter is pressed
        /// </summary>
        public EnterMode EnterMode = EnterMode.StopWhenEnter;
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
        /// Does not include the caret.
        /// </remarks>
        public string Text
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
                return Text + (IsCaretVisible ? "_" : " ");
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
                return new Rectangle((int)Position.X, (int)Position.Y,
                    (int)(Scale.X * Font.MeasureString(TextWithCaret).X), (int)(Scale.Y * Font.MeasureString(TextWithCaret).Y));
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
            : this()
        {
            Text = defaultText;
            Font = Main.fontMouseText;
            CaretTimer = 60;
        }

        /// <summary>
        /// Updates the Control
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (IsFocused)
            {
                Main.inputTextEnter = true;
                Main.chatText = "";

                string @new = Main.GetInputText(Text);

                if (@new != Text)
                {
                    Text = @new;
                    Main.PlaySound("vanilla:menuTick");
                }

                // tabs
                if (GInput.Keyboard.IsKeyDown(Key.Tab))
                {
                    if (TabMode <= 0)
                        Text += '\t';
                    else
                        for (int i = 0; i < (int)TabMode; i++)
                            Text += ' ';

                    Main.PlaySound("vanilla:menuTick");
                }

                // enter
                if (GInput.Keyboard.IsKeyDown(Key.Enter))
                {
                    bool shift = GInput.Keyboard.IsKeyDown(Key.LeftShift) || GInput.Keyboard.IsKeyDown(Key.RightShift);

                    if (((EnterMode & EnterMode.StopWhenEnter) != 0 && !shift) || ((EnterMode & EnterMode.StopWhenShiftEnter) != 0 && shift))
                        IsFocused = false;
                    else
                        Text += '\n';

                    Main.PlaySound("vanilla:menuTick");
                }

                #region ctrl + ... // implemented in Main.GetInputText
                //if (GInput.Keyboard.IsKeyDown(Key.LeftCtrl) || GInput.Keyboard.IsKeyDown(Key.RightCtrl))
                //{
                //    // ctrl + c (copy)
                //    if (GInput.Keyboard.IsKeyDown(Key.C) && GInput.OldKeyboard.IsKeyUp(Key.C))
                //        if (Text != "")
                //            Clipboard.SetText(Text);
                //        else
                //            Clipboard.Clear();

                //    // ctrl + v (paste)
                //    if (GInput.Keyboard.IsKeyDown(Key.V) && GInput.OldKeyboard.IsKeyUp(Key.V))
                //    {
                //        string str = Clipboard.GetText();

                //        for (int i = 0; i < str.Length; i++)
                //            if ((int)str[i] < 32 || (int)str[i] == 127)
                //                str = str.Replace(str[i--].ToString(), "");

                //        Text = str;
                //    }

                //    // ctrl + x (cut)
                //    if (GInput.Keyboard.IsKeyDown(Key.X) && GInput.OldKeyboard.IsKeyUp(Key.X))
                //        if (Text != "")
                //        {
                //            Clipboard.SetText(Text);
                //            Text = "";
                //        }
                //        else
                //            Clipboard.Clear();
                //}
                #endregion
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

            DrawOutlinedString(sb, Font, TextWithCaret, Colour);
        }
    }
}
