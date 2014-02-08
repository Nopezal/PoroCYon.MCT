using System;
using System.Collections.Generic;
using System.Linq;

namespace TAPI.SDK.GUI.Controls.Primitives
{
    public abstract class Button : Focusable
    {
        protected override void FocusGot()
        {
            Clicked();

            if (Click != null)
                Click(this);
            if (GlobalClick != null)
                GlobalClick(this);

            Main.PlaySound("vanilla:menuTick");

            base.FocusGot();

            IsFocused = false;
        }

        public Action<Button> Click = null;
        public static Action<Button> GlobalClick = null;

        protected virtual void Clicked() { }
    }
}
