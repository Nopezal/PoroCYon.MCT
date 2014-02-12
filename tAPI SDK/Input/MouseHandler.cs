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

    /// <summary>
    /// Custom structure to make calculations with the Mouse easier
    /// </summary>
    public struct MouseHandler
    {
        bool l;

        /// <summary>
        /// Wether the left mouse button is pressed or not (returns false when Main.blockMouse or Player.mouseInterface is true)
        /// </summary>
        public bool Left
        {
            get
            {
                return l && !Constants.mainInstance.blockMouse && !Main.localPlayer.mouseInterface;
            }
        }

        /// <summary>
        /// Wether the right mouse button is pressed or not
        /// </summary>
        public bool Right
        {
            get;
            private set;
        }
        /// <summary>
        /// Wether the middle button (scroll wheel) is pressed or not
        /// </summary>
        public bool Middle
        {
            get;
            private set;
        }
        /// <summary>
        /// Wether the first X button is pressed or not
        /// </summary>
        public bool XButton1
        {
            get;
            private set;
        }
        /// <summary>
        /// Wether the second X button is pressed or not
        /// </summary>
        public bool XButton2
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the position of the mouse, relative to the Terraria window
        /// </summary>
        public Vector2 Position
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets the hitbox of the mouse, relative to the Terraria window
        /// </summary>
        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y,
                    (int)(Main.cursorTexture.Width * Main.cursorScale), (int)(Main.cursorTexture.Height * Main.cursorScale));
            }
        }
        /// <summary>
        /// Gets the position of the mouse, relative to the Terraria world
        /// </summary>
        public Vector2 WorldPosition
        {
            get
            {
                return Position + Main.screenPosition;
            }
        }
        /// <summary>
        /// Gets the hitbox of the mouse, relative to the Terraria world
        /// </summary>
        public Rectangle WorldRectangle
        {
            get
            {
                return new Rectangle((int)(Position.X + Main.screenPosition.X), (int)(Position.Y + Main.screenPosition.Y),
                    (int)(Main.cursorTexture.Width * Main.cursorScale), (int)(Main.cursorTexture.Height * Main.cursorScale));
            }
        }

        /// <summary>
        /// Gets the current scroll wheel value
        /// </summary>
        public int ScrollWheel
        {
            get;
            private set;
        }

        /// <summary>
        /// Retrieves the current mouse state
        /// </summary>
        /// <returns>The current mouse state</returns>
        public static MouseHandler GetState()
        {
            MouseHandler ret = new MouseHandler()
            {
                l = AvalonMouse.LeftButton == MouseButtonState.Pressed,
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

        /// <summary>
        /// Sets the position of the mouse
        /// </summary>
        /// <param name="position">The new position of the mouse</param>
        public static void SetPosition(Vector2 position)
        {
            XnaMouse.SetPosition((int)position.X, (int)position.Y);
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out Point lpPoint);
    }
}
