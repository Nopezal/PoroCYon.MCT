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
using TAPI.SDK.GUI.Controls.Primitives;
using TAPI.SDK.Input;

namespace TAPI.SDK.GUI.Controls
{
    /// <summary>
    /// Enumeration is a set of flags
    /// </summary>
    [Flags]
    public enum EnterMode
    {
        None = 0,
        StopWhenEnter = 1,
        StopWhenShiftEnter = 2,
        StopWhenEnterOrShiftEnter = 3
    }
    /// <summary>
    /// You can specify any nonnegative and non-zero number as tab amount
    /// </summary>
    public enum TabMode : int
    {
        Tab = 0,
        OneSpace = 1,
        TwoSpaces = 2,
        ThreeSpaces = 3,
        FourSpaces = 4
    }

    [ComVisible(false)]
    public class TextBox : ListeningControl<char>, ITextControl, ICaretControl
    {
        public EnterMode EnterMode = EnterMode.StopWhenEnter;
        public TabMode TabMode = TabMode.Tab;

        /// <summary>
        /// Does not include the caret.
        /// </summary>
        public string Text
        {
            get;
            set;
        }
        public string TextWithCaret
        {
            get
            {
                return Text + (CaretVisible ? "_" : " ");
            }
        }
        public SpriteFont Font
        {
            get;
            set;
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

        public override Rectangle Hitbox
        {
            get
            {
                return new Rectangle((int)Position.X - (int)padding.X, (int)Position.Y - (int)padding.Y,
                    (int)(Scale.X * Font.MeasureString(TextWithCaret).X) + (int)padding.X, (int)(Scale.Y * Font.MeasureString(TextWithCaret).Y) + (int)padding.Y);
            }
        }

        public KeyboardText KeyboardText
        {
            get;
            private set;
        }

        public TextBox()
            : base()
        {
            Text = "";
            Font = Main.fontMouseText;
            CaretCD = 60;

            KeyboardText = new KeyboardText(Form.FromHandle(Constants.mainInstance.Window.Handle) as Form);
        }
        public TextBox(string defaultText)
            : this()
        {
            Text = defaultText;
        }

        public override void Init()
        {
            base.Init();

            KeyboardText.StartReading();
        }
        public override void Update()
        {
            base.Update();

            if (Listening)
            {
                Main.inputTextEnter = true;
                Main.chatText = "";

                string add = KeyboardText.ReadToEnd();
                if (add != "")
                {
                    Text += add;
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
                if (EnterMode != EnterMode.None && GInput.Keyboard.IsKeyDown(Key.Enter))
                {
                    if ((EnterMode & EnterMode.StopWhenEnterOrShiftEnter) == EnterMode.StopWhenEnterOrShiftEnter)
                        Listening = false;
                    else if ((EnterMode & EnterMode.StopWhenEnter) == EnterMode.StopWhenEnter)
                    {
                        if (GInput.Keyboard.IsKeyUp(Key.LeftShift) || GInput.Keyboard.IsKeyUp(Key.LeftShift))
                            Text += '\n';
                        else
                            Listening = false;
                    }
                    else if ((EnterMode & EnterMode.StopWhenShiftEnter) == EnterMode.StopWhenShiftEnter)
                    {
                        if (GInput.Keyboard.IsKeyUp(Key.LeftShift) || GInput.Keyboard.IsKeyUp(Key.LeftShift))
                            Listening = false;
                        else
                            Text += '\n';
                    }

                    Main.PlaySound("vanilla:menuTick");
                }

                // ctrl + ...
                if (GInput.Keyboard.IsKeyDown(Key.LeftCtrl) || GInput.Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    // ctrl + c (copy)
                    if (GInput.Keyboard.IsKeyDown(Key.C) && GInput.OldKeyboard.IsKeyUp(Key.C))
                        if (Text != "")
                            Clipboard.SetText(Text);
                        else
                            Clipboard.Clear();

                    // ctrl + v (paste)
                    if (GInput.Keyboard.IsKeyDown(Key.V) && GInput.OldKeyboard.IsKeyUp(Key.V))
                    {
                        string str = Clipboard.GetText();

                        for (int i = 0; i < str.Length; i++)
                            if ((int)str[i] < 32 || (int)str[i] == 127)
                                str = str.Replace(str[i--].ToString(), "");

                        Text = str;
                    }

                    // ctrl + x (cut)
                    if (GInput.Keyboard.IsKeyDown(Key.X) && GInput.OldKeyboard.IsKeyUp(Key.X))
                        if (Text != "")
                        {
                            Clipboard.SetText(Text);
                            Text = "";
                        }
                        else
                            Clipboard.Clear();
                }
            }
        }
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            DrawBackground(sb);

            sb.DrawString(Font, TextWithCaret, Position, Colour, Rotation, Origin, Scale, SpriteEffects, LayerDepth);
        }

        protected override void Dispose(bool forced)
        {
            KeyboardText.StopReading();
        }
    }
}
