using System;
using System.Collections.Generic;
using System.Linq;

namespace TAPI.SDK.GUI.Controls.Primitives
{
    public abstract class ListeningControl<T> : Focusable
    {
        bool listening;

        public bool Listening
        {
            get
            {
                return listening && IsFocused;
            }
            protected set
            {
                listening = IsFocused = value;
            }
        }

        public Action<ListeningControl<T>, T> OnInputGot;
        public static Action<ListeningControl<T>, T> GlobalInputGot;

        public ListeningControl()
            : base()
        {

        }

        protected override void FocusGot()
        {
            Listening = true;
        }
        protected override void FocusLost()
        {
            Listening = false;
        }

        protected virtual void GotInput(T got)
        {
            if (OnInputGot != null)
                OnInputGot(this, got);
            if (GlobalInputGot != null)
                GlobalInputGot(this, got);
        }
    }
}
