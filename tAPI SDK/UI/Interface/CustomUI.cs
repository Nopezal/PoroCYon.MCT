using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using TAPI.SDK.ObjectModel;
using TAPI.SDK.UI.Interface.Controls;

namespace TAPI.SDK.UI.Interface
{
    /// <summary>
    /// When to call Draw on a CustomUI
    /// </summary>
    public enum DrawCalled
    {
        /// <summary>
        /// At ModInterface.PreDrawInterface
        /// </summary>
        PreDrawInterface,
        /// <summary>
        /// At ModInterface.PreDrawInventory
        /// </summary>
        PreDrawInventory,

        /// <summary>
        /// At ModInterface.PostDrawInventory
        /// </summary>
        PostDrawInventory,
        /// <summary>
        /// At ModInterface.PostDrawInterface
        /// </summary>
        PostDrawInterface
    }

    /// <summary>
    /// The base class for all custom UIs
    /// </summary>
    public abstract class CustomUI : ModableObject, IControlParent, ICloneable<CustomUI>
    {
        List<Control> controls;

        /// <summary>
        /// The controls in the GUI.
        /// Adding or removing a control by calling Controls.Add/Remove is not a good idea.
        /// </summary>
        public ReadOnlyCollection<Control> Controls
        {
            get
            {
                return controls.AsReadOnly();
            }
        }

        /// <summary>
        /// Called when a control is added from the Controls list
        /// </summary>
        public Action<Control> OnControlAdded;
        /// <summary>
        /// Called when a control is removed from the Controls list
        /// </summary>
        public Action<Control> OnControlRemoved;
        /// <summary>
        /// Called when a control is added from the Controls list
        /// </summary>
        public static Action<Control> GlobalControlAdded;
        /// <summary>
        /// Called when a control is removed from the Controls list
        /// </summary>
        public static Action<Control> GlobalControlRemoved;

        /// <summary>
        /// The visibility of the CustomUI
        /// </summary>
        public Visibility Visibility = Visibility.Inventory;

        /// <summary>
        /// When the CustomUI is drawn. Default is PostDrawInventory.
        /// </summary>
        public DrawCalled DrawCalled = DrawCalled.PostDrawInventory;

        /// <summary>
        /// Gets wether the interface is visible or not
        /// </summary>
        public virtual bool IsVisible
        {
            get
            {
                return (Visibility & SdkUI.CurrentVisibility) != 0;
            }
        }

        /// <summary>
        /// Creates a new instance of the CustomUI class
        /// </summary>
        public CustomUI()
            : base()
        {
            controls = new List<Control>();
        }

        /// <summary>
        /// Initializes the CustomUI
        /// </summary>
        public virtual void Init() { }
        /// <summary>
        /// Updates the CustomUI
        /// </summary>
        public virtual void Update()
        {
            for (int i = 0; i < controls.Count; i++)
            {
                if (controls[i].Destroyed)
                {
                    if (OnControlRemoved != null)
                        OnControlRemoved(controls[i]);
                    if (controls[i].OnRemoved != null)
                        controls[i].OnRemoved(controls[i], null);
                    if (Control.GlobalRemoved != null)
                        Control.GlobalRemoved(controls[i], null);

                    controls.RemoveAt(i--);
                    continue;
                }

                if (controls[i].Enabled && controls[i].IsVisible)
                    controls[i].Update();
            }
        }
        /// <summary>
        /// Draws the CustomUI
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw things</param>
        public virtual void Draw(SpriteBatch sb)
        {
            for (int i = 0; i < controls.Count; i++)
            {
                if (controls[i].Destroyed)
                {
                    if (OnControlRemoved != null)
                        OnControlRemoved(controls[i]);
                    if (controls[i].OnRemoved != null)
                        controls[i].OnRemoved(controls[i], null);
                    if (Control.GlobalRemoved != null)
                        Control.GlobalRemoved(controls[i], null);

                    controls.RemoveAt(i--);
                    continue;
                }

                if (controls[i].Enabled && controls[i].IsVisible)
                    controls[i].Draw(sb);
            }
        }

        /// <summary>
        /// Adds a control to the GUI
        /// </summary>
        /// <param name="control">The control to add</param>
        public void AddControl(Control control)
        {
            if (control == null)
                return;

            controls.Add(control);

            control.Parent = new WeakReference<IControlParent>(this);

            control.ID = controls.Count;

            control.Init();

            if (OnControlAdded != null)
                OnControlAdded(control);
            if (control.OnAdded != null)
                control.OnAdded(control, this);
            if (Control.GlobalAdded != null)
                Control.GlobalAdded(control, this);
        }
        /// <summary>
        /// Removes a control from the GUI
        /// </summary>
        /// <param name="control">The control to remove</param>
        public void RemoveControl(Control control)
        {
            if (control == null)
                return;

            controls.Remove(control);

            if (OnControlRemoved != null)
                OnControlRemoved(control);
            if (control.OnRemoved != null)
                control.OnRemoved(control, this);
            if (Control.GlobalRemoved != null)
                Control.GlobalRemoved(control, this);
        }
        /// <summary>
        /// Removes a control from the GUI
        /// </summary>
        /// <param name="index">The index of the control to remove</param>
        public void RemoveControlAt(int index)
        {
            RemoveControl(controls[index]);
        }
        /// <summary>
        /// Clears the Controls list
        /// </summary>
        public void ClearControls()
        {
            while (controls.Count > 0)
                RemoveControlAt(0);
        }

        /// <summary>
        /// Creates a memberwise clone of the object
        /// </summary>
        /// <returns>A memberwise clone of the object</returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
        /// <summary>
        /// Creates a memberwise clone of the object
        /// </summary>
        /// <returns>A memberwise clone of the object</returns>
        public CustomUI Copy()
        {
            return (CustomUI)MemberwiseClone();
        }
    }
}
