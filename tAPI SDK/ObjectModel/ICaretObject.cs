﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TAPI.SDK.ObjectModel
{
    /// <summary>
    /// an ITextObject with a caret, usually used for editable text
    /// </summary>
    public interface ICaretObject : ITextObject
    {
        /// <summary>
        /// The timer value of the caret blinking, in ticks (60/sec)
        /// </summary>
        int CaretTimer
        {
            get;
        }
        /// <summary>
        /// Wether the caret is visible or not
        /// </summary>
        bool IsCaretVisible
        {
            get;
        }
    }
}
