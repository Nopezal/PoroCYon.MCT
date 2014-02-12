using System;
using System.Collections.Generic;
using System.Linq;
using PoroCYon.XnaExtensions;
using PoroCYon.XnaExtensions.Input;

namespace TAPI.SDK.Input
{
    /// <summary>
    /// Contains global mouse/keyboard input data
    /// </summary>
    public static class GInput
    {
        /// <summary>
        /// The Keyboard state of the current frame
        /// </summary>
        public static KeyHandler Keyboard
        {
            get;
            private set;
        }
        /// <summary>
        /// The Mouse state of the current frame
        /// </summary>
        public static MouseHandler Mouse
        {
            get;
            private set;
        }
        /// <summary>
        /// The Keyboard state of the previous frame
        /// </summary>
        public static KeyHandler OldKeyboard
        {
            get;
            private set;
        }
        /// <summary>
        /// The Mouse state of the previous frame
        /// </summary>
        public static MouseHandler OldMouse
        {
            get;
            private set;
        }

        internal static void Update()
        {
            OldKeyboard = Keyboard;
            OldMouse = Mouse;

            Keyboard = KeyHandler.GetState();
            Mouse = MouseHandler.GetState();
        }
    }
}
