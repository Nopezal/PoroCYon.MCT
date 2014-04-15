using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using TAPI;

namespace PoroCYon.MCT.UI.Interface.Controls.Primitives
{
    /// <summary>
    /// A clickable button
    /// </summary>
    public abstract class Button : Focusable
    {
        int fireCD = 0;

        /// <summary>
        /// When the Button is cliced
        /// </summary>
        public Action<Button> OnClicked = null;
        /// <summary>
        /// When a Button is clicked
        /// </summary>
        public static Action<Button> GlobalClicked = null;

        /// <summary>
        /// Wether the Button should keep firing the Click effect as long as its hovered and focused. Default is false.
        /// </summary>
        public bool KeepFiring = false;

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

            if (IsFocused && IsHovered)
                if (KeepFiring)
                {
                    if (--fireCD <= 0)
                    {
                        Click();
                        fireCD = 5;
                    }
                }
                else if (!OldIsFocused)
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
