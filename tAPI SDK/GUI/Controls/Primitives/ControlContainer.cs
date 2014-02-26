using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TAPI.SDK.GUI.Controls.Primitives
{
    public abstract class ControlContainer : Control, IControlParent
    {
        /// <summary>
        /// Control positions are relative to the ControlContainer position
        /// </summary>
        public List<Control> Controls
        {
            get;
            protected set;
        }

        public ControlContainer()
            : base()
        {
            Controls = new List<Control>();
        }

        public Action<ControlContainer, Control> OnAddControl, OnRemoveControl;
        public static Action<ControlContainer, Control> GlobalAddControl, GlobalRemoveControl;

        public override void Init()
        {
            for (int i = 0; i < Controls.Count; i++)
            {
                Controls[i].Position += Position;
                Controls[i].Init();
                Controls[i].Position -= Position;
            }

            base.Init();
        }
        public override void Update()
        {
            for (int i = 0; i < Controls.Count; i++)
            {
                if (Controls[i].Destroyed)
                {
                    if (Controls[i].OnRemoved != null)
                        Controls[i].OnRemoved(Controls[i], null);
                    if (Control.GlobalRemoved != null)
                        Control.GlobalRemoved(Controls[i], null);

                    Controls[i].Dispose();

                    Controls.RemoveAt(i--);
                    continue;
                }

                if (!Controls[i].Enabled)
                    continue;

                Controls[i].Position += Position;
                Controls[i].Update();
                Controls[i].Position -= Position;
            }

            base.Update();
        }
        public override void Draw(SpriteBatch sb)
        {
            for (int i = 0; i < Controls.Count; i++)
            {
                if (Controls[i].Destroyed)
                {
                    if (Controls[i].OnRemoved != null)
                        Controls[i].OnRemoved(Controls[i], null);
                    if (Control.GlobalRemoved != null)
                        Control.GlobalRemoved(Controls[i], null);

                    Controls[i].Dispose();

                    Controls.RemoveAt(i--);
                    continue;
                }

                if (!Controls[i].Enabled)
                    continue;

                Controls[i].Position += Position;
                Controls[i].Draw(sb);
                Controls[i].Position -= Position;
            }

            base.Draw(sb);
        }

        protected override void Dispose(bool forced)
        {
            while (Controls.Count > 0)
                RemoveControl(Controls[0]);

            base.Dispose(forced);
        }

        public void AddControl(Control control)
        {
            Controls.Add(control);

            control.Init();

            if (OnAddControl != null)
                OnAddControl(this, control);
            if (GlobalAddControl != null)
                GlobalAddControl(this, control);

            if (control.OnAdded != null)
                control.OnAdded(control, null);
            if (Control.GlobalAdded != null)
                Control.GlobalAdded(control, null);
        }
        public void RemoveControl(Control control)
        {
            Controls.Remove(control);

            if (OnRemoveControl != null)
                OnRemoveControl(this, control);
            if (GlobalRemoveControl != null)
                GlobalRemoveControl(this, control);

            if (control.OnRemoved != null)
                control.OnRemoved(control, null);
            if (Control.GlobalRemoved != null)
                Control.GlobalRemoved(control, null);

            control.Dispose();
        }
        public void RemoveControlAt(int index)
        {
            RemoveControl(Controls[index]);
        }
    }
}
