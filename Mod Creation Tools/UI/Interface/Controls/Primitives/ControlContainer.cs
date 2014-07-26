using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.Extensions;
using PoroCYon.MCT.ObjectModel;

namespace PoroCYon.MCT.UI.Interface.Controls.Primitives
{
    /// <summary>
    /// A Control that contains other Controls. All child controls' positions are absolute.
    /// </summary>
    public abstract class ControlContainer : Control, IControlParent
    {
        List<Control> controls = new List<Control>();

        /// <summary>
        /// Gets the child Controls of the ContainerControl
        /// </summary>
        public ReadOnlyCollection<Control> Controls
        {
            get
            {
                return controls.AsReadOnly();
            }
            protected set
            {
                controls = new List<Control>(value);
            }
        }

        /// <summary>
        /// Creates a new instance of the ControlContainer class
        /// </summary>
        public ControlContainer()
            : base()
        {

        }

        /// <summary>
        /// When a child Control is added to the ControlContainer
        /// </summary>
        public Action<ControlContainer, Control> OnAddControl;
        /// <summary>
        /// When a child Control is removed from the ControlContainer
        /// </summary>
        public Action<ControlContainer, Control> OnRemoveControl;
        /// <summary>
        /// When a child Control is added to a ControlContainer
        /// </summary>
        public static Action<ControlContainer, Control> GlobalAddControl;
        /// <summary>
        /// When a child Control is removed from a ControlContainer
        /// </summary>
        public static Action<ControlContainer, Control> GlobalRemoveControl;

        /// <summary>
        /// Initializes the Control
        /// </summary>
        public override void Init()
        {
            base.Init();

            for (int i = 0; i < controls.Count; i++)
            {
                controls[i].Parent = new WeakReference<IControlParent>(this);

                controls[i].Init();
            }
        }
        /// <summary>
        /// Updates the Control
        /// </summary>
        public override void Update()
        {
            base.Update();

            for (int i = 0; i < controls.Count; i++)
            {
                if (controls[i].Destroyed)
                {
                    if (controls[i].OnRemoved != null)
                        controls[i].OnRemoved(Controls[i], null);
                    if (Control.GlobalRemoved != null)
                        Control.GlobalRemoved(Controls[i], null);

                    controls.RemoveAt(i--);
                    continue;
                }

                if (!controls[i].Enabled)
                    continue;

                controls[i].Update();
            }
        }
        /// <summary>
        /// Draws the Control
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the Control</param>
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            for (int i = 0; i < controls.Count; i++)
            {
                if (controls[i].Destroyed)
                {
                    if (controls[i].OnRemoved != null)
                        controls[i].OnRemoved(Controls[i], null);
                    if (Control.GlobalRemoved != null)
                        Control.GlobalRemoved(Controls[i], null);

                    controls.RemoveAt(i--);
                    continue;
                }

                if (!controls[i].Enabled)
                    continue;

                controls[i].Draw(sb);
            }
        }

        /// <summary>
        /// Adds a Control to the child controls list
        /// </summary>
        /// <param name="control">The Control to add</param>
        public void AddControl(Control control)
        {
            controls.Add(control);

            control.ID = controls.Count - 1;

            control.Parent = new WeakReference<IControlParent>(this);

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
        /// <summary>
        /// Removes a Control from the child controls list
        /// </summary>
        /// <param name="control">The Control to remove</param>
        public void RemoveControl(Control control)
        {
            controls.Remove(control);

            if (OnRemoveControl != null)
                OnRemoveControl(this, control);
            if (GlobalRemoveControl != null)
                GlobalRemoveControl(this, control);

            if (control.OnRemoved != null)
                control.OnRemoved(control, null);
            if (Control.GlobalRemoved != null)
                Control.GlobalRemoved(control, null);
        }
        /// <summary>
        /// Removes a Control at the specified index from the child controls list
        /// </summary>
        /// <param name="index">The index of the Control to remove</param>
        public void RemoveControlAt(int index)
        {
            RemoveControl(controls[index]);
        }
        /// <summary>
        /// Clears the child controls list
        /// </summary>
        public void ClearControls()
        {
            controls.Clear();
        }
    }
}
