using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TAPI.SDK.UI.MenuItems
{
    /// <summary>
    /// A MenuPage that provides basic initialization, updating and drawing, as well as calling initialize events on Control objects
    /// </summary>
    public class Page : MenuPage
    {
        /// <summary>
        /// Called when the Page is initialized
        /// </summary>
        public Action<Page> OnInit;
        /// <summary>
        /// Called when the Page is updated
        /// </summary>
        public Action<Page> OnUpdate;
        /// <summary>
        /// Called when the Page is drawn
        /// </summary>
        public Action<Page, SpriteBatch> OnDraw;
        /// <summary>
        /// Called when a Page is initialized
        /// </summary>
        public static Action<Page> GlobalInit;
        /// <summary>
        /// Called when a Page is updated
        /// </summary>
        public static Action<Page> GlobalUpdate;
        /// <summary>
        /// Called whan a Page is drawn
        /// </summary>
        public static Action<Page, SpriteBatch> GlobalDraw;

        /// <summary>
        /// Creates a new instance of the Page class
        /// </summary>
        public Page()
            : base()
        {
            OnEntry += () => Init();
            base.Update += () => Update();
        }

        /// <summary>
        /// Initializes the Page
        /// </summary>
        protected virtual void Init()
        {
            if (OnInit != null)
                OnInit(this);
            if (GlobalInit != null)
                GlobalInit(this);

            foreach (MenuButton b in buttons)
                if (b is Control)
                    ((Control)b).CallInit();
        }
        /// <summary>
        /// Updates the Page
        /// </summary>
        protected new virtual void Update()
        {
            if (OnUpdate != null)
                OnUpdate(this);
            if (GlobalUpdate != null)
                GlobalUpdate(this);
        }

        /// <summary>
        /// Draws the Page
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the Page</param>
        public override void Draw(SpriteBatch sb)
        {
            if (OnDraw != null)
                OnDraw(this, sb);
            if (GlobalDraw != null)
                GlobalDraw(this, sb);

            base.Draw(sb);
        }
    }
}
