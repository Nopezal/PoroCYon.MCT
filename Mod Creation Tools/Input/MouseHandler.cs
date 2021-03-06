﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using TAPI;
using PoroCYon.MCT.Internal.Versioning;

namespace PoroCYon.MCT.Input
{
    /// <summary>
    /// Custom structure to make calculations with the Mouse easier
    /// </summary>
    public struct MouseHandler
    {
        static int currentSW, oldSW;

        Vector2 pos;

        /// <summary>
        /// Wether the left mouse button is pressed or not
        /// </summary>
        public bool Left
        {
            get;
            private set;
        }
        /// <summary>
        /// Wether the left mouse button is pressed or not (returns false when Main.blockMouse or Player.mouseInterface is true)
        /// </summary>
        public bool SafeLeft
        {
            get
            {
                return Left && !GInput.BlockMouse;
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
        /// Wether the right mouse button is pressed or not (returns false when Main.blockMouse or Player.mouseInterface is true)
        /// </summary>
        public bool SafeRight
        {
            get
            {
                return Right && !GInput.BlockMouse;
            }
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
        /// Gets or sets the position of the mouse, relative to the Terraria window
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return pos;
            }
            set
            {
                SetPosition(pos = value);
            }
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
            set
            {
                Position = value - Main.screenPosition;
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
        /// Wether the user just clicked left or not
        /// </summary>
        public bool JustClickedLeft
        {
            get
            {
                return GInput.Mouse.Left && !GInput.OldMouse.Left;
            }
        }
        /// <summary>
        /// Wether the user just clicked right or not
        /// </summary>
        public bool JustClickedRight
        {
            get
            {
                return GInput.Mouse.Right && !GInput.OldMouse.Right;
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
            MouseState xm = Mouse.GetState();

            MouseHandler ret = new MouseHandler()
            {
                Left = xm.LeftButton == ButtonState.Pressed,
                Right = xm.RightButton == ButtonState.Pressed,
                Middle = xm.MiddleButton == ButtonState.Pressed,
                XButton1 = xm.XButton1 == ButtonState.Pressed,
                XButton2 = xm.XButton2 == ButtonState.Pressed
            };

            ret.pos = xm.Position();

            oldSW = currentSW;

            currentSW = xm.ScrollWheelValue;

            ret.ScrollWheel = currentSW - oldSW;

            return ret;
        }

        /// <summary>
        /// Sets the position of the mouse
        /// </summary>
        /// <param name="position">The new position of the mouse</param>
        public static void SetPosition(Vector2 position)
        {
            Mouse.SetPosition((int)position.X, (int)position.Y);
        }
    }
}
