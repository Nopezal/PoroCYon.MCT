using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TAPI.SDK.GUI.Controls.Primitives
{
    public interface ITextControl : IFontControl
    {
        string Text
        {
            get;
            set;
        }
    }
}
