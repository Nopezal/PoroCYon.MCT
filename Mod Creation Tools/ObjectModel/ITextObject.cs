using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PoroCYon.MCT.ObjectModel
{
    /// <summary>
    /// An object that holds text
    /// </summary>
    public interface ITextObject : IFontObject
    {
        /// <summary>
        /// The text of the ITextObject
        /// </summary>
        string Text
        {
            get;
            set;
        }
    }
}
