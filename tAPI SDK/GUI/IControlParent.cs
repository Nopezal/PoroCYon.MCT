using System;
using System.Collections.Generic;
using System.Linq;
using TAPI.SDK.GUI.Controls;

namespace TAPI.SDK.GUI
{
    /// <summary>
    /// An object referenced by the <see cref="TAPI.SDK.GUI.Controls.Control"/>.Parent field
    /// </summary>
    public interface IControlParent
    {
        /// <summary>
        /// The controls the parent has
        /// </summary>
        List<Control> Controls
        {
            get;
        }

        /// <summary>
        /// Adds a control to the controls list.
        /// </summary>
        /// <param name="control">The control to add</param>
        void AddControl(Control control);
        /// <summary>
        /// Removes a control from the control list
        /// </summary>
        /// <param name="control">The control to remove</param>
        void RemoveControl(Control control);
        /// <summary>
        /// Removes a control from the control list
        /// </summary>
        /// <param name="index">The index of the control to remove</param>
        void RemoveControlAt(int index);
    }
}
