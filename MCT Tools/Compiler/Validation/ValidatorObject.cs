using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using PoroCYon.MCT.Internal;

namespace PoroCYon.MCT.Tools.Validation
{
    /// <summary>
    /// An object that helps with the validation of a JSON file.
    /// </summary>
    public abstract class ValidatorObject
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
        /// Sets a JSON value. If the key isn't specified, a CompilerError is returned.
        /// </summary>
        /// <typeparam name="TJsonObj">The type of the JSON value.</typeparam>
        /// <param name="json">The JSON file which contains the key/value pair to check.</param>
        /// <param name="key">The key to check.</param>
        /// <param name="value">The object to put the JSON value in.</param>
        /// <returns>null if no errors are found, not null otherwise.</returns>
        protected static CompilerError SetJsonValue<TJsonObj>(JsonFile json, string key, ref TJsonObj value)
        {
            if (!json.Json.Has(key))
                return new CompilerError()
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
        protected static CompilerError SetJsonValue<TJsonObj>(JsonFile json, string key, ref TJsonObj value, TJsonObj defaultValue)
        {
            if (!json.Json.Has(key))
            {
                value = defaultValue;

                return null;
            }

            return SetJsonValueInternal(json, key, ref value);
        }

        static CompilerError SetJsonValueInternal<TJsonObj>(JsonFile json, string key, ref TJsonObj value)
        {
            if (typeof(TJsonObj).IsArray)
            {
                dynamic arr = new dynamic[json.Json[key].Count];

                for (int i = 0; i < json.Json[key].Count; i++)
                {
                    if (typeof(TJsonObj) != typeof(object) && json.Json[key][i].GetJsonType() != CommonToolUtilities.JsonTypeFromType(typeof(TJsonObj).GetElementType()))
                        return new CompilerError()
                        {
                            Cause = new ArrayTypeMismatchException(),
                            FilePath = json.Path,
                            IsWarning = false,
                            Message = "'" + key + "[" + i + "]' is a " + json.Json.GetJsonType() +
                                      ", not a " + CommonToolUtilities.JsonTypeFromType(typeof(TJsonObj).GetElementType()) + "."
                        };

                    arr[i] = json.Json[key][i];
                }

                value = arr;

                return null;
            }

            if (typeof(TJsonObj) != typeof(object) && json.Json[key].GetJsonType() != CommonToolUtilities.JsonTypeFromType(typeof(TJsonObj)))
                return new CompilerError()
                {
                    Cause = new InvalidCastException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "'" + key + "' is a " + json.Json.GetJsonType() +
                              ", not a " + CommonToolUtilities.JsonTypeFromType(typeof(TJsonObj)) + "."
                };

            value = (dynamic)json.Json[key]; // dynamic++

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
