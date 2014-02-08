using System;
using System.Collections.Generic;
using System.Linq;

namespace TAPI.SDK.GUI.Controls.Primitives
{
    public interface ICaretControl
    {
        int CaretCD
        {
            get;
        }
        bool CaretVisible
        {
            get;
        }
    }
}
