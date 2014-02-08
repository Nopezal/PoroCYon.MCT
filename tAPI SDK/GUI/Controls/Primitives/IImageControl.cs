using System;
using System.Collections.Generic;
using System.Linq;

namespace TAPI.SDK.GUI.Controls.Primitives
{
    public interface IImageControl
    {
        bool IsGif
        {
            get;
        }
        object Picture
        {
            get;
            set;
        }
    }
}
