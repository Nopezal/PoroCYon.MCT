using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using PoroCYon.XnaExtensions;
using PoroCYon.XnaExtensions.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TAPI.SDK.GUI.Controls.Primitives;
using TAPI.SDK.Input;

namespace TAPI.SDK.GUI.Controls
{
    public class Window : ControlContainer, ITextControl
    {
        readonly static Vector2 topBar = new Vector2(0f, 16f);

        TextButton close = new TextButton("X");

        Vector2 mouseRelativeTo = Vector2.Zero;

        /// <summary>
        /// Window title
        /// </summary>
        public string Text
        {
            get;
            set;
        }
        public SpriteFont Font
        {
            get;
            set;
        }
        public bool BeingDragged
        {
            get;
            private set;
        }

        public Action<Window> OnClosed, OnDraggingStarted, OnDragging, OnDraggingStopped;
        public static Action<Window> GlobalClosed, GlobalDraggingStarted, GlobalDragging, GlobalDraggingStopped;

        public Window()
            : base()
        {
            Text = "Window";
            Font = Main.fontMouseText;

            BeingDragged = false;
        }
        public Window(string title)
            : this()
        {
            Text = title;
        }

        public override void Init()
        {
            Position += topBar;
            base.Init();
            Position -= topBar;

            close = new TextButton("X")
            {
                Colour = Color.Red,
                Click = (c) =>
                {
                    Destroy();
                }
            };
        }
        public override void Update()
        {
            if (GInput.Keyboard.IsKeyDown(Key.Back))
            {
                Destroy();
                return;
            }

            // move window
            BeingDragged = false;
            if (GInput.Mouse.Rectangle.Intersects(new Rectangle((int)Position.X - (int)padding.X, (int)Position.Y - (int)padding.Y, (int)topBar.X, (int)topBar.Y))
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
        public override void Draw(SpriteBatch sb)
        {
            // window border/bg
            DrawBackground(sb);

            sb.Draw(SdkInterface.WhitePixel, new Vector2(2f, topBar.Y), null, new Color(18, 18, 38), Rotation, Origin, new Vector2(Scale.X - 4f, 2f), SpriteEffects, LayerDepth);

            // title
            sb.DrawString(Font, Text, Position + padding, Colour, Rotation, Origin, Scale, SpriteEffects, LayerDepth);

            // close button
            close.Position += Position;
            close.Draw(sb);
            close.Position -= Position;

            Position += topBar;
            base.Draw(sb);
            Position -= topBar;
        }
    }
}
