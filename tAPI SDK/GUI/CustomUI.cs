using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoroCYon.XnaExtensions;
using TAPI.SDK.GUI.Controls;
using TAPI.SDK.GUI.Controls.Primitives;

namespace TAPI.SDK.GUI
{
    /// <summary>
    /// The base class for all custom UIs
    /// </summary>
    public abstract class CustomUI : ModableObject, IControlParent, IDisposable, ICloneable<CustomUI>
    {
        /// <summary>
        /// The controls in the GUI.
        /// Adding or removing a control by calling Controls.Add/Remove is not a good idea.
        /// </summary>
        public List<Control> Controls
        {
            get;
            internal set;
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
        public Visibility Visibility = Visibility.IngameInv;

        /// <summary>
        /// Wether the CustomUI is drawn at PreDrawInterface or PostDrawInterface
        /// </summary>
        public bool IsDrawnAfter = false;

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
        {
            Controls = new List<Control>();
        }

        /// <summary>
        /// Disposes the CustomUI instance
        /// </summary>
        ~CustomUI()
        {
            Dispose(false);
        }
        /// <summary>
        /// Disposes the CustomUI instance
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
            for (int i = 0; i < Controls.Count; i++)
            {
                if (Controls[i].Destroyed)
                {
                    if (OnControlRemoved != null)
                        OnControlRemoved(Controls[i]);
                    if (Controls[i].OnRemoved != null)
                        Controls[i].OnRemoved(Controls[i], null);
                    if (Control.GlobalRemoved != null)
                        Control.GlobalRemoved(Controls[i], null);

                    Controls[i].Dispose();

                    Controls.RemoveAt(i--);
                    continue;
                }

                if (Controls[i].Enabled && (Controls[i].Visibility & SdkUI.CurrentVisibility) == SdkUI.CurrentVisibility)
                    Controls[i].Update();
            }
        }
        /// <summary>
        /// Draws the CustomUI
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw things</param>
        /// <param name="after">Wether it is called in Pre- or PostDrawInterface</param>
        public virtual void Draw(SpriteBatch sb, bool after)
        {
            for (int i = 0; i < Controls.Count; i++)
            {
                if (Controls[i].Destroyed)
                {
                    if (OnControlRemoved != null)
                        OnControlRemoved(Controls[i]);
                    if (Controls[i].OnRemoved != null)
                        Controls[i].OnRemoved(Controls[i], null);
                    if (Control.GlobalRemoved != null)
                        Control.GlobalRemoved(Controls[i], null);

                    Controls[i].Dispose();

                    Controls.RemoveAt(i--);
                    continue;
                }

                if (Controls[i].IsDrawnAfter == after && Controls[i].Enabled && (Controls[i].Visibility & SdkUI.CurrentVisibility) == SdkUI.CurrentVisibility)
                    Controls[i].Draw(sb);
            }
        }

        /// <summary>
        /// When the control is disposing
        /// </summary>
        /// <param name="forced">Wether the control is disposing by IDisposable.Dispose or the constructor</param>
        protected virtual void Dispose(bool forced)
        {
            if (Controls != null)
                for (int i = 0; i < Controls.Count; i++)
                    Controls[i].Dispose();
        }

        /// <summary>
        /// Adds a control to the GUI
        /// </summary>
        /// <param name="control">The control to add</param>
        public void AddControl(Control control)
        {
            if (control == null)
                return;

            Controls.Add(control);

            control.Parent = new WeakReference<IControlParent>(this);

            control.ID = Controls.Count;

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

            Controls.Remove(control);

            if (OnControlRemoved != null)
                OnControlRemoved(control);
            if (control.OnRemoved != null)
                control.OnRemoved(control, this);
            if (Control.GlobalRemoved != null)
                Control.GlobalRemoved(control, this);

            control.Dispose();
        }
        /// <summary>
        /// Removes a control from the GUI
        /// </summary>
        /// <param name="index">The index of the control to remove</param>
        public void RemoveControlAt(int index)
        {
            RemoveControl(Controls[index]);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
        public CustomUI Copy()
        {
            return (CustomUI)MemberwiseClone();
        }
    }
}
