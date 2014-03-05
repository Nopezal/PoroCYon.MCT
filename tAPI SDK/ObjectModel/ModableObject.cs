using System;
using System.Collections.Generic;
using System.Linq;
using PoroCYon.XnaExtensions;

namespace TAPI.SDK.ObjectModel
{
    /// <summary>
    /// Represents an object where fields and hooks can be added and removed (and hooks can be invoked)
    /// </summary>
    public abstract class ModableObject
    {
        /// <summary>
        /// The added fields the ModableObject has. Do not add or remove fields by calling Fields.Add/Remove
        /// </summary>
        public Dictionary<string, object> Fields = new Dictionary<string, object>();

        /// <summary>
        /// The added hooks the ModableObject has. Do not add or remove hooks by calling Hooks.Add/Remove
        /// </summary>
        public Dictionary<string, Delegate> Hooks = new Dictionary<string, Delegate>();

        /// <summary>
        /// When a field is added
        /// </summary>
        public Action<ModableObject, string, object> OnFieldAdded;
        /// <summary>
        /// When a field is added
        /// </summary>
        public static Action<ModableObject, string, object> GlobalFieldAdded;

        /// <summary>
        /// When a field is removed
        /// </summary>
        public Action<ModableObject, string> OnFieldRemoved;
        /// <summary>
        /// When a field is removed
        /// </summary>
        public static Action<ModableObject, string> GlobalFieldRemoved;

        /// <summary>
        /// Adds a field
        /// </summary>
        /// <param name="key">The key of the field to add</param>
        /// <param name="field">The field to add</param>
        public virtual void AddField(string key, object field)
        {
            Fields.Add(key, field);

            if (OnFieldAdded != null)
                OnFieldAdded(this, key, field);
            if (GlobalFieldAdded != null)
                GlobalFieldAdded(this, key, field);
        }
        /// <summary>
        /// Removes a field
        /// </summary>
        /// <param name="key">The key of the field to remove</param>
        public virtual void RemoveField(string key)
        {
            Fields.Remove(key);

            if (OnFieldRemoved != null)
                OnFieldRemoved(this, key);
            if (GlobalFieldRemoved != null)
                GlobalFieldRemoved(this, key);
        }


        /// <summary>
        /// When a hook is invoked
        /// </summary>
        public Action<ModableObject, string, object[]> OnInvoke;
        /// <summary>
        /// When a hook is invoked
        /// </summary>
        public static Action<ModableObject, string, object[]> GlobalInvoke;

        /// <summary>
        /// When a hook is added
        /// </summary>
        public Action<ModableObject, string, Delegate> OnHookAdded;
        /// <summary>
        /// When a hook is added
        /// </summary>
        public static Action<ModableObject, string, Delegate> GlobalHookAdded;

        /// <summary>
        /// When a hook is removed
        /// </summary>
        public Action<ModableObject, string> OnHookRemoved;
        /// <summary>
        /// When a hook is removed
        /// </summary>
        public static Action<ModableObject, string> GlobalHookRemoved;

        /// <summary>
        /// Invokes a hook
        /// </summary>
        /// <param name="hook">The key of the hook to invoke</param>
        /// <param name="args">The arguments to invoke the hook with</param>
        public virtual void Invoke(string hook, params object[] args)
        {
            Hooks[hook].DynamicInvoke(args);

            if (OnInvoke != null)
                OnInvoke(this, hook, args);
            if (GlobalInvoke != null)
                GlobalInvoke(this, hook, args);
        }

        /// <summary>
        /// Adds a hook
        /// </summary>
        /// <param name="key">The key of the hook to add</param>
        /// <param name="hook">The hook to add</param>
        public virtual void AddHook(string key, Delegate hook)
        {
            Hooks.Add(key, hook);

            if (OnHookAdded != null)
                OnHookAdded(this, key, hook);
            if (GlobalHookAdded != null)
                GlobalHookAdded(this, key, hook);
        }

        /// <summary>
        /// Removes a hook
        /// </summary>
        /// <param name="key">The key of the hook to remove</param>
        public virtual void RemoveHook(string key)
        {
            Hooks.Remove(key);

            if (OnHookRemoved != null)
                OnHookRemoved(this, key);
            if (GlobalHookRemoved != null)
                GlobalHookRemoved(this, key);
        }

        internal static void Reset()
        {
            GlobalFieldAdded = null;
            GlobalFieldRemoved = null;
            GlobalHookAdded = null;
            GlobalHookRemoved = null;
            GlobalInvoke = null;
        }
    }
}
