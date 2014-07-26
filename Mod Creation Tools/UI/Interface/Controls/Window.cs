using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using PoroCYon.MCT.Input;
using PoroCYon.MCT.ObjectModel;
using PoroCYon.MCT.UI.Interface.Controls.Primitives;

namespace PoroCYon.MCT.UI.Interface.Controls
{
    /// <summary>
    /// A small window that contains controls
    /// </summary>
    public class Window : ControlContainer, ITextObject
    {
        readonly static Vector2 topBar = new Vector2(0f, 16f);

        TextButton close = new TextButton("X");

        Vector2 mouseRelativeTo = Vector2.Zero;

        /// <summary>
        /// The font of the Window
        /// </summary>
        public SpriteFont Font
        {
            get;
            set;
        }

        /// <summary>
        /// The title of the Window
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// Wether the window is currently being dragged or not
        /// </summary>
        public bool BeingDragged
        {
            get;
            private set;
        }

        /// <summary>
        /// When the Window is closed
        /// </summary>
        public Action<Window> OnClosed;
        /// <summary>
        /// When the Window is being dragged
        /// </summary>
        public Action<Window> OnDragging;
        /// <summary>
        /// When the user just started dragging the Window
        /// </summary>
        public Action<Window> OnDraggingStarted;
        /// <summary>
        /// When the user just stopped dragging the Window
        /// </summary>
        public Action<Window> OnDraggingStopped;

        /// <summary>
        /// When a Window is closed
        /// </summary>
        public static Action<Window> GlobalClosed;
        /// <summary>
        /// When a Window is being dragged
        /// </summary>
        public static Action<Window> GlobalDragging;
        /// <summary>
        /// When the user just started dragging a Window
        /// </summary>
        public static Action<Window> GlobalDraggingStarted;
        /// <summary>
        /// When the user just stopped dragging a Window
        /// </summary>
        public static Action<Window> GlobalDraggingStopped;

        /// <summary>
        /// Creates a new instance of the Window class
        /// </summary>
        public Window()
            : this("Window")
        {

        }
        /// <summary>
        /// Creates a new instance of the Window class
        /// </summary>
        /// <param name="title">The title of the Window</param>
        public Window(string title)
            : this()
        {
            Text = title;
            Font = Main.fontMouseText;

            BeingDragged = false;
        }

        /// <summary>
        /// Initializes the Control
        /// </summary>
        public override void Init()
        {
            Position += topBar;
            base.Init();
            Position -= topBar;

            close = new TextButton("X")
            {
                Colour = new Color(255, 255, 255, 0),
                OnClicked = (c) =>
                {
                    Destroy();
                }
            };
        }
        /// <summary>
        /// Updates the Control
        /// </summary>
        public override void Update()
        {
            if (GInput.Keyboard.IsKeyDown(Keys.Back))
            {
                Destroy();
                return;
            }

            // move window
            BeingDragged = false;
            if (GInput.Mouse.Rectangle.Intersects(new Rectangle((int)Position.X, (int)Position.Y, (int)topBar.X, (int)topBar.Y))
                && GInput.Mouse.Left)
            {
                if (!BeingDragged)
                {
                    mouseRelativeTo = GInput.Mouse.Position - Position;

                    if (OnDraggingStarted != null)
                        OnDraggingStarted(this);
                    if (GlobalDraggingStarted != null)
                        GlobalDraggingStarted(this);
                }

                BeingDragged = true;
            }
            else if (BeingDragged)
            {
                mouseRelativeTo = Vector2.Zero;
                BeingDragged = false;

                if (OnDraggingStopped != null)
                    OnDraggingStopped(this);
                if (GlobalDraggingStopped != null)
                    GlobalDraggingStopped(this);
            }

            if (BeingDragged)
            {
                Position = GInput.Mouse.Position - mouseRelativeTo;

                if (OnDragging != null)
                    OnDragging(this);
                if (GlobalDragging != null)
                    GlobalDragging(this);
            }

            // update button
            close.Position += Position;
            close.Update();
            close.Position -= Position;

            Position += topBar;
            base.Update();
            Position -= topBar;
        }
        /// <summary>
        /// Draws the Control
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the Control</param>
        public override void Draw(SpriteBatch sb)
        {
            Position += topBar;
            base.Draw(sb);
            Position -= topBar;

            // window border/bg
            if (HasBackground)
                DrawBackground(sb);

            // divider header/body
            sb.Draw(MctUI.WhitePixel, new Vector2(2f, topBar.Y), null, new Color(18, 18, 38), Rotation, Origin, new Vector2(Scale.X - 4f, 2f), SpriteEffects, LayerDepth);

            // title
            sb.DrawString(Font, Text, Position, Colour, Rotation, Origin, Scale, SpriteEffects, LayerDepth);

            // close button
            close.Position += Position;
            close.Draw(sb);
            close.Position -= Position;
        }
    }
}
