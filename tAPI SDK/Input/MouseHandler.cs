using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TAPI.SDK.Input
{
    using AvalonMouse = System.Windows.Input.Mouse;
    using XnaMouse = Microsoft.Xna.Framework.Input.Mouse;

    public struct MouseHandler
    {
        public bool Left
        {
            get;
            private set;
        }
        public bool Right
        {
            get;
            private set;
        }
        public bool Middle
        {
            get;
            private set;
        }
        public bool XButton1
        {
            get;
            private set;
        }
        public bool XButton2
        {
            get;
            private set;
        }

        public Vector2 Position
        {
            get;
            private set;
        }
        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y,
                    (int)(Main.cursorTexture.Width * Main.cursorScale), (int)(Main.cursorTexture.Height * Main.cursorScale));
            }
        }
        public Vector2 WorldPosition
        {
            get
            {
                return Position + Main.screenPosition;
            }
        }
        public Rectangle WorldRectangle
        {
            get
            {
                return new Rectangle((int)(Position.X + Main.screenPosition.X), (int)(Position.Y + Main.screenPosition.Y),
                    (int)(Main.cursorTexture.Width * Main.cursorScale), (int)(Main.cursorTexture.Height * Main.cursorScale));
            }
        }

        public int ScrollWheel
        {
            get;
            private set;
        }

        public static MouseHandler GetState()
        {
            MouseHandler ret = new MouseHandler()
            {
                Left = AvalonMouse.LeftButton == MouseButtonState.Pressed,
                Right = AvalonMouse.RightButton == MouseButtonState.Pressed,
                Middle = AvalonMouse.RightButton == MouseButtonState.Pressed,
                XButton1 = AvalonMouse.XButton1 == MouseButtonState.Pressed,
                XButton2 = AvalonMouse.XButton2 == MouseButtonState.Pressed
            };

            ret.ScrollWheel = XnaMouse.GetState().ScrollWheelValue;

            Point pt;
            GetCursorPos(out pt);

            ret.Position = new Vector2(pt.X, pt.Y);

            return ret;
        }

        public static void SetPosition(Vector2 position)
        {
            XnaMouse.SetPosition((int)position.X, (int)position.Y);
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out Point lpPoint);
    }
}
