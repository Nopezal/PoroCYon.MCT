using System;
using System.Collections.Generic;
using System.Linq;

namespace TAPI.SDK.UI.Interface.Controls.Primitives
{
    /// <summary>
    /// A Focusable that listens to an input
    /// </summary>
    public abstract class ListeningControl : Focusable
    {
        /// <summary>
        /// When a ListeningControl got input
        /// </summary>
        public static Action<ListeningControl, object> GlobalInputGot;

        /// <summary>
        /// Wether the ListeningControl is listening or not
        /// </summary>
        protected bool Listening = false;

        /// <summary>
        /// Creates a new instance of the ListeningControl class
        /// </summary>
        public ListeningControl()
            : base()
        {

        }
    }

    /// <summary>
    /// A Focusable that listens to a certain type of input
    /// </summary>
    /// <typeparam name="T">The type of the input</typeparam>
    public abstract class ListeningControl<T> : ListeningControl
    {
        /// <summary>
        /// When the ListeningControl got input
        /// </summary>
        public Action<ListeningControl<T>, T> OnInputGot;

        /// <summary>
        /// Creates a new instance of the ListeningControl class
        /// </summary>
        public ListeningControl()
            : base()
        {

        }

        /// <summary>
        /// Gets input. Must be called manually.
        /// </summary>
        /// <param name="got">The input that the ListeningControl got</param>
        protected virtual void GotInput(T got)
        {
            if (OnInputGot != null)
                OnInputGot(this, got);

            if (GlobalInputGot != null)
                GlobalInputGot(this, got as object);
        }
    }
}
