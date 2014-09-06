using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using LitJson;
using PoroCYon.Extensions;
using PoroCYon.MCT.Internal;

namespace PoroCYon.MCT.Tools.Compiler.Validation
{
    // this is where the ACTUAL checking occurs

    /// <summary>
    /// An object that helps with the validation of a JSON file.
    /// </summary>
    public abstract class ValidatorObject(ModCompiler mc) : CompilerPhase(mc)
    {
        /// <summary>
        /// Adds a CompilerError to a List of CompilerErrors if the value is not null.
        /// </summary>
        /// <param name="err">The error to add.</param>
        /// <param name="list">The list of errors.</param>
        [TargetedPatchingOptOut(Consts.TPOOReason)]
        protected static void AddIfNotNull(CompilerError err, List<CompilerError> list)
        {
            if (err != null)
                list.Add(err);
        }
        /// <summary>
        /// Adds a collection of CompilerErrors to a List of CompilerErrors if the value is not null.
        /// </summary>
        /// <param name="coll">The collection of errors to add.</param>
        /// <param name="list">The list of errors.</param>
        [TargetedPatchingOptOut(Consts.TPOOReason)]
        protected static void AddIfNotNull(IEnumerable<CompilerError> coll, List<CompilerError> list)
        {
            if (coll != null && coll.Count() != 0)
                foreach (CompilerError e in coll)
                    AddIfNotNull(e, list);
        }

        /// <summary>
        /// Sets a JSON value. If the key isn't specified, a CompilerError is returned.
        /// </summary>
        /// <typeparam name="TJsonObj">The type of the JSON value.</typeparam>
        /// <param name="json">The JSON file which contains the key/value pair to check.</param>
        /// <param name="key">The key to check.</param>
        /// <param name="value">The object to put the JSON value in.</param>
        /// <returns>null if no errors are found, not null otherwise.</returns>
        protected CompilerError SetJsonValue<TJsonObj>(JsonFile json, string key, ref TJsonObj value)
        {
            if (!json.Json.Has(key))
                return new CompilerError(Building)
                {
                    Cause = new KeyNotFoundException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "Required key '" + key + "' not found."
                };

            return SetJsonValueInternal(json, key, ref value);
        }
        /// <summary>
        /// Sets a JSON value. If the key isn't specified, <paramref name="defaultValue" /> is used.
        /// </summary>
        /// <typeparam name="TJsonObj">The type of the JSON value.</typeparam>
        /// <param name="json">The JSON file which contains the key/value pair to check.</param>
        /// <param name="key">The key to check.</param>
        /// <param name="value">The object to put the JSON value in.</param>
        /// <param name="defaultValue">The default value of the key/value pair.</param>
        /// <returns>null if no errors are found, not null otherwise.</returns>
        protected CompilerError SetJsonValue<TJsonObj>(JsonFile json, string key, ref TJsonObj value, TJsonObj defaultValue)
        {
            if (!json.Json.Has(key))
            {
                value = defaultValue;

                return null;
            }

            return SetJsonValueInternal(json, key, ref value);
        }
        /// <summary>
        /// Sets a JSON value. If the key isn't specified, a CompilerError is returned.
        /// </summary>
        /// <typeparam name="T1">The first type of the union.</typeparam>
        /// <typeparam name="T2">The second type of the union.</typeparam>
        /// <param name="json">The JSON file which contains the key/value pair to check.</param>
        /// <param name="key">The key to check.</param>
        /// <param name="value">The object to put the JSON value in.</param>
        /// <returns>null if no errors are found, not null otherwise.</returns>
        protected CompilerError SetJsonValue<T1, T2>(JsonFile json, string key, ref Union<T1, T2> value)
        {
            if (!json.Json.Has(key))
                return new CompilerError(Building)
                {
                    Cause = new KeyNotFoundException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "Required key '" + key + "' not found."
                };

            JsonData j = json.Json[key];

            if (typeof(T1) == typeof(object))
            {
                if (typeof(T2) == typeof(object))
                    return new CompilerError(Building)
                    {
                        Cause = new InvalidCastException(),
                        FilePath = json.Path,
                        IsWarning = false,
                        Message = "Cannot cast to a union of two blank System.Objects."
                    };

                if (j.GetJsonType() != CommonToolUtilities.JsonTypeFromType(typeof(T2)))
                {
                    value = (T1)(dynamic)j;

                    return null;
                }
            }
            if (typeof(T2) == typeof(object) && j.GetJsonType() != CommonToolUtilities.JsonTypeFromType(typeof(T1)))
            {
                value = (T1)(dynamic)j;

                return null;
            }

            // praise dynamic
            if (j.GetJsonType() == CommonToolUtilities.JsonTypeFromType(typeof(T1)))
            {
                value = (T1)(dynamic)j;

                return null;
            }
            if (j.GetJsonType() == CommonToolUtilities.JsonTypeFromType(typeof(T2)))
            {
                value = (T2)(dynamic)j;

                return null;
            }

            return new CompilerError(Building)
            {
                Cause = new InvalidCastException(),
                FilePath = json.Path,
                IsWarning = false,
                Message = "'" + key + "': Expected a " + CommonToolUtilities.JsonTypeFromType(typeof(T1)) + " or "
                    + CommonToolUtilities.JsonTypeFromType(typeof(T2)) + ", but got a " + j.GetJsonType() + "."
            };
        }
        /// <summary>
        /// Sets a JSON value. If the key isn't specified, <paramref name="defaultValue" /> is used.
        /// </summary>
        /// <typeparam name="T1">The first type of the union.</typeparam>
        /// <typeparam name="T2">The second type of the union.</typeparam>
        /// <param name="json">The JSON file which contains the key/value pair to check.</param>
        /// <param name="key">The key to check.</param>
        /// <param name="value">The object to put the JSON value in.</param>
        /// <param name="defaultValue">The default value of the key/value pair.</param>
        /// <returns>null if no errors are found, not null otherwise.</returns>
        protected CompilerError SetJsonValue<T1, T2>(JsonFile json, string key, ref Union<T1, T2> value, Union<T1, T2> defaultValue)
        {
            if (!json.Json.Has(key))
            {
                value = defaultValue;

                return null;
            }

            return SetJsonValue(json, key, ref value);
        }

        CompilerError SetJsonValueInternal<TJsonObj>(JsonFile json, string key, ref TJsonObj value)
        {
            if (typeof(TJsonObj).IsArray)
            {
                dynamic arr = new dynamic[json.Json[key].Count];
                
                for (int i = 0; i < json.Json[key].Count; i++)
                {
                    if (typeof(TJsonObj) != typeof(object) && json.Json[key][i].GetJsonType() != CommonToolUtilities.JsonTypeFromType(typeof(TJsonObj).GetElementType()))
                        return new CompilerError(Building)
                        {
                            Cause = new ArrayTypeMismatchException(),
                            FilePath = json.Path,
                            IsWarning = false,
                            Message = "'" + key + "[" + i + "]' is a " + json.Json.GetJsonType() +
                                      ", not a " + CommonToolUtilities.JsonTypeFromType(typeof(TJsonObj).GetElementType()) + "."
                        };

                    TJsonObj j = (TJsonObj)arr[i];
                    SetJsonValueInternal(new JsonFile(json.Path, json.Json[key][i]), ref j);
                    arr[i] = j;
                }
                
                value = arr;

                return null;
            }

            if (typeof(TJsonObj) != typeof(object) && json.Json[key].GetJsonType() != CommonToolUtilities.JsonTypeFromType(typeof(TJsonObj)))
                return new CompilerError(Building)
                {
                    Cause = new InvalidCastException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "'" + key + "' is a " + json.Json.GetJsonType() +
                              ", not a " + CommonToolUtilities.JsonTypeFromType(typeof(TJsonObj)) + "."
                };

            value = (TJsonObj)(dynamic)json.Json[key]; // dynamic++

            return null;
        }
        CompilerError SetJsonValueInternal<TJsonObj>(JsonFile json, ref TJsonObj value)
        {
            if (typeof(TJsonObj).IsArray)
            {
                dynamic arr = new dynamic[json.Json.Count];

                for (int i = 0; i < json.Json.Count; i++)
                {
                    if (typeof(TJsonObj) != typeof(object) && json.Json[i].GetJsonType() != CommonToolUtilities.JsonTypeFromType(typeof(TJsonObj).GetElementType()))
                        return new CompilerError(Building)
                        {
                            Cause = new ArrayTypeMismatchException(),
                            FilePath = json.Path,
                            IsWarning = false,
                            Message = "JSON value is a " + json.Json.GetJsonType() +
                                      ", not a " + CommonToolUtilities.JsonTypeFromType(typeof(TJsonObj).GetElementType()) + "."
                        };

                    SetJsonValueInternal(new JsonFile(json.Path, json.Json[i]), null, ref arr[i]);
                }

                value = arr;

                return null;
            }

            if (typeof(TJsonObj) != typeof(object) && json.Json.GetJsonType() != CommonToolUtilities.JsonTypeFromType(typeof(TJsonObj)))
                return new CompilerError(Building)
                {
                    Cause = new InvalidCastException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "JSON value is a " + json.Json.GetJsonType() +
                              ", not a " + CommonToolUtilities.JsonTypeFromType(typeof(TJsonObj)) + "."
                };

            value = (TJsonObj)(dynamic)json.Json; // dynamic++

            return null;
        }

        /// <summary>
        /// Create &amp; validate a JSON file.
        /// </summary>
        /// <param name="json">The json to validate</param>
        /// <returns>A collection of all validation errors.</returns>
        public abstract IEnumerable<CompilerError> CreateAndValidate(JsonFile json);
    }
}
