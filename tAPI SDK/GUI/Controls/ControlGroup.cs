using System;
using System.Collections.Generic;
using System.Linq;
using TAPI.SDK.GUI.Controls.Primitives;

namespace TAPI.SDK.GUI.Controls
{
    public class ControlGroup : ControlContainer
    {
        public ControlGroup()
            : base()
        {

        }
        public ControlGroup(params Control[] controls)
            : base()
        {
            foreach (Control c in controls)
                AddControl(c);
        }
    }
}
