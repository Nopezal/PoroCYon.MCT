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
        /// Updates the Control
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (IsFocused && (!OldIsFocused || StayFocused))
                Click();
        }

        /// <summary>
        /// Clicks the Button
        /// </summary>
        protected virtual void Click()
        {
            Main.PlaySound("vanilla:menuTick");

            if (OnClicked != null)
                OnClicked(this);
            if (GlobalClicked != null)
                GlobalClicked(this);
        }
    }
}
