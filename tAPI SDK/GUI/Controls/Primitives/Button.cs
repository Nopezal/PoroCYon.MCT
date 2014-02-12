using System;
using System.Collections.Generic;
using System.Linq;
using TAPI.SDK.Input;

namespace TAPI.SDK.GUI.Controls.Primitives
{
    public abstract class Button : Focusable
	{
		/// <summary>
		/// Wether the Button keeps firing the Clicked event as long as the left mouse button is held down
		/// </summary>
		public bool StaysPressed = false;
		public Action<Button> Click = null;
		public static Action<Button> GlobalClick = null;

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

        protected virtual void Clicked() { }

		public override void Update()
		{
			if (GInput.Mouse.Left && StaysPressed)
				ForceFocus = true;

			base.Update();
		}
    }
}
