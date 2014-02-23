using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using TAPI.SDK.GUI.Controls;
using TAPI.SDK.Input;

namespace TAPI.SDK.GUI
{
    /// <summary>
    /// The different buttons a <see cref="TAPI.SDK.GUI.MessageBox"/> can have.
    /// </summary>
    public enum MessageBoxButton
    {
        /// <summary>
        /// 'OK' button
        /// </summary>
        OK,
        /// <summary>
        /// 'Yes' and 'No' buttons
        /// </summary>
        YesNo
    }

    /// <summary>
    /// A message box
    /// </summary>
    public class MessageBox : Window
    {
        Action<MessageBox, bool?> Result = (mb, res) => { };

        /// <summary>
        /// Wether the text of the message box can be copied to the clipboard or not
        /// </summary>
        public bool TextIsCopyable = false;

        /// <summary>
        /// What happens when the message box is closed and the result is returned
        /// </summary>
        public Action<MessageBox, bool?> OnResult
        {
            get
            {
                return Result;
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TAPI.SDK.GUI.MessageBox"/> class.
        /// </summary>
        public MessageBox()
            : this("Looks like something wanted to get your attention,\nbut forgot to say something...\n,Calling assembly (may be useful): "
            + Assembly.GetCallingAssembly().FullName, "tAPI SDK: Weird alert: " + Assembly.GetCallingAssembly().FullName, MessageBoxButton.OK)
        {

        }

        /// <summary>
        /// Creates a new instance of the <see cref="TAPI.SDK.GUI.MessageBox"/> class.
        /// </summary>
        /// <param name="text">The text the message box contains</param>
        /// <param name="caption">The caption of the message box</param>
        /// <param name="btn">The button type of the message box</param>
        public MessageBox(string text, string caption, MessageBoxButton btn)
            : base(caption)
        {
            IsDrawnAfter = true;

            OnClosed += (w) =>
            {
                Result(this, null);
            };

            TextBlock t = new TextBlock(text);
            AddControl(t);

            if (TextIsCopyable)
                OnUpdate += (c) =>
                {
                    if (GInput.Mouse.Rectangle.Intersects(Hitbox) && GInput.Mouse.Left)
                        Clipboard.SetText(t.Text);
                };

            switch (btn)
            {
                case MessageBoxButton.OK:
                    {
                        TextButton ok = new TextButton("OK");
                        ok.Click += (b) => { Result(this, true); };
                        ok.Position = new Vector2(0f, t.Hitbox.Height);
                        AddControl(ok);
                    }
                    break;
                case MessageBoxButton.YesNo:
                    {
                        TextButton y = new TextButton("Yes");
                        y.Click += (b) => { Result(this, true); };
                        y.Position = new Vector2(0f, t.Hitbox.Height + 16f);
                        AddControl(y);

                        TextButton n = new TextButton("No");
                        n.Click += (b) => { Result(this, false); };
                        n.Position = new Vector2(0f, t.Hitbox.Height + y.Hitbox.Height + 16f);
                        AddControl(n);
                    }
                    break;
            }
        }

        /// <summary>
        /// Displays a message box
        /// </summary>
        /// <param name="text">The text the message box contains</param>
        /// <param name="caption">The title of the message box</param>
        /// <param name="btn">The type of buttons the message box contains</param>
        /// <param name="copyable">Wether the text of the message box can be copied to the clipboard or not</param>
        /// <returns>What happens when the message box is closed. Hook to this if you want something to happen when the message box is closed.</returns>
        public static Action<bool?> Show(string text, string caption = null, MessageBoxButton btn = MessageBoxButton.OK, bool copyable = false)
        {
            Assembly a = Assembly.GetCallingAssembly();
            object[] titles = a.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            string title = (titles.Length > 0 ? ((AssemblyTitleAttribute)titles[0]).Title : a.FullName.Split(',')[0]) + (caption == null ? "" : ": " + caption);

            return new MessageBox(text, title, btn)
            {
                TextIsCopyable = copyable,
                Scale = new Vector2(500f, 350f),
                Position = new Vector2(Main.screenWidth / 2f - 250f, Main.screenHeight / 2f - 175f)
            }.Show();
        }

        /// <summary>
        /// Displays the message box
        /// </summary>
        /// <returns>What happens when the message box is closed. Hook to this if you want something to happen when the message box is closed.</returns>
        public Action<bool?> Show()
        {
            Action<bool?> Return = (res) => { };

            Result += (mb, res) => { Return(res); };

            SdkUI.AddControl(this);

            return Return;
        }
    }
}
