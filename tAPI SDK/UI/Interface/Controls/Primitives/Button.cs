using System;
using System.Collections.Generic;
using System.Linq;
using TAPI.SDK.Input;

namespace TAPI.SDK.UI.Interface.Controls.Primitives
{
    /// <summary>
    /// A clickable button
    /// </summary>
    public abstract class Button : Focusable
    {
        /// <summary>
        /// When the Button is cliced
        /// </summary>
        public Action<Button> OnClicked = null;
        /// <summary>
        /// When a Button is clicked
        /// </summary>
        public static Action<Button> GlobalClicked = null;

        /// <summary>
        /// Creates a new instance of the Button class
        /// </summary>
        public Button()
            : base()
        {
            StayFocused = false;
        }

        /// <summary>
        /// Gives the Focusable focus
        /// </summary>
        protected override void FocusGot()
        {
            base.FocusGot();

            Click();

            Main.PlaySound("vanilla:menuTick");
        }

        /// <summary>
        /// Clicks the Button
        /// </summary>
        protected virtual void Click()
        {
            if (OnClicked != null)
                OnClicked(this);
            if (GlobalClicked != null)
                GlobalClicked(this);
        }
    }
}
