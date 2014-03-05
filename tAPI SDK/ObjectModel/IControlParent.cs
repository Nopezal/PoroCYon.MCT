﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TAPI.SDK.UI.Interface.Controls;

namespace TAPI.SDK.ObjectModel
{
    /// <summary>
    /// An object that contains Controls
    /// </summary>
    public interface IControlParent
    {
        /// <summary>
        /// The controls of the IControlParent
        /// </summary>
        ReadOnlyCollection<Control> Controls
        {
            get;
        }

        /// <summary>
        /// Adds a control to the collection of controls
        /// </summary>
        /// <param name="control">The control to add</param>
        void AddControl(Control control);
        /// <summary>
        /// Removes a control from the collection of controls
        /// </summary>
        /// <param name="control">The control to remove</param>
        void RemoveControl(Control control);
        /// <summary>
        /// Removes a control from the collection of controls from the given ID
        /// </summary>
        /// <param name="control">The ID of the control to remove</param>
        void RemoveControlAt(int id);
        /// <summary>
        /// Clears the controls list
        /// </summary>
        void ClearControls();
    }
}
