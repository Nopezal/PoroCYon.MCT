using System;
using System.Collections.Generic;
using System.Linq;
using PoroCYon.XnaExtensions;
using PoroCYon.XnaExtensions.Input;

namespace TAPI.SDK.Input
{
    public static class GInput
    {
        public static KeyHandler Keyboard
        {
            get;
            private set;
        }
        public static MouseHandler Mouse
        {
            get;
            private set;
        }
        public static KeyHandler OldKeyboard
        {
            get;
            private set;
        }
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
