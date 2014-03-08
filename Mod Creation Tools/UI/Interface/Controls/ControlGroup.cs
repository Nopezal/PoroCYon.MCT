using System;
using System.Collections.Generic;
using System.Linq;
using PoroCYon.MCT.UI.Interface.Controls.Primitives;

namespace PoroCYon.MCT.UI.Interface.Controls
{
    // basic implementation for ControlContainer

    /// <summary>
    /// A control with multiple child controls
    /// </summary>
    public class ControlGroup : ControlContainer
    {
        /// <summary>
        /// Creates a new instance of the ControlGroup class
        /// </summary>
        public ControlGroup()
            : base()
        {

        }
        /// <summary>
        /// Creates a new instance of the ControlGroup class
        /// </summary>
        /// <param name="controls">The child controls of the ControlGroup</param>
        public ControlGroup(params Control[] controls)
            : base()
        {
            foreach (Control c in controls)
                AddControl(c);
        }
    }
}
