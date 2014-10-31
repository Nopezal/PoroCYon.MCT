using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using LitJson;
using PoroCYon.Extensions;
using PoroCYon.Extensions.Collections;
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

            return SetJsonValueInternal_Generic(json, key, ref value);
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

            return SetJsonValueInternal_Generic(json, key, ref value);
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
        /// <summary>
        /// Sets a JSON value from an array.
        /// </summary>
        /// <typeparam name="TJsonElement">The type of the elements in the array.</typeparam>
        /// <param name="json">The JSON file which contains the key/value pair to check.</param>
        /// <param name="key">The key to check.</param>
        /// <param name="value">The object to put the JSON value in.</param>
        /// <param name="hasDefault">Whether to return an empty array when the key is not found. If false and the key does not exist, a CompilerError is returned.</param>
        /// <param name="default">The default value for the array.</param>
        /// <returns>null if no errors are found, not null otherwise.</returns>
        protected CompilerError SetJsonValue<TJsonElement>(JsonFile json, string key, ref TJsonElement[] value, bool hasDefault, TJsonElement[] @default = null)
        {
            if (!json.Json.Has(key))
            {
                if (hasDefault)
                {
                    value = @default ?? new TJsonElement[0];

                    return null;
                }

                return new CompilerError(Building)
                {
                    Cause = new KeyNotFoundException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "Required key '" + key + "' not found."
                };
            }

            return SetJsonValueInternal_GenericArr(json, key, ref value);
        }

        CompilerError SetJsonValueInternal_GenericArr<TJsonElement>(JsonFile json, string key, ref TJsonElement[] value)
        {
            if (value == null)
                value = new TJsonElement[json.Json[key].Count];
            if (value.Length != json.Json[key].Count)
                Array.Resize(ref value, json.Json[key].Count);

            for (int i = 0; i < json.Json[key].Count; i++)
            {
                if (json.Json[key][i].GetJsonType() != CommonToolUtilities.JsonTypeFromType(typeof(TJsonElement)))
                    return new CompilerError(Building)
                    {
                        Cause = new ArrayTypeMismatchException(),
                        FilePath = json.Path,
                        IsWarning = false,
                        Message = "'" + key + "[" + i + "]' is a " + json.Json.GetJsonType() +
                                  ", not a " + CommonToolUtilities.JsonTypeFromType(typeof(TJsonElement)) + "."
                    };

                if ((CompilerError err = DeserializeJsonPrimitive(new JsonFile(json.Path, json.Json[key][i]), ref value[i])) != null)
                    return err;
            }

            return null;
        }
        /*CompilerError SetJsonValueInternal_Arr(JsonFile json, string key, Type elemType, ref object[] value)
        {
            for (int i = 0; i < json.Json[key].Count; i++)
            {
                if (json.Json[key][i].GetJsonType() != CommonToolUtilities.JsonTypeFromType(elemType))
                    return new CompilerError(Building)
                    {
                        Cause = new ArrayTypeMismatchException(),
                        FilePath = json.Path,
                        IsWarning = false,
                        Message = "'" + key + "[" + i + "]' is a " + json.Json.GetJsonType() +
                                  ", not a " + CommonToolUtilities.JsonTypeFromType(elemType) + "."
                    };

                if ((CompilerError err = DeserializeJsonPrimitive(new JsonFile(json.Path, json.Json[key][i]), elemType, ref value[i])) != null)
                    return err;
            }

            return null;
        }*/

        CompilerError SetJsonValueInternal_Generic<TJsonObj>(JsonFile json, string key, ref TJsonObj value)
        {
            object o = value;
            CompilerError ret = SetJsonValueInternal(json, key, typeof(TJsonObj), ref o);
            if (o != null)
                value = typeof(TJsonObj) == o.GetType() ? (TJsonObj)o : (TJsonObj)Convert.ChangeType(o, typeof(TJsonObj));

            return ret;
        }

        CompilerError SetJsonValueInternal(JsonFile json, string key, Type jsonType, ref object value)
        {
            if (jsonType.IsArray)
                return new CompilerError(Building)
                {
                    Cause = new ArgumentException("The type cannot be an array, use the proper SetJsonValue overload. This indicates a bug in the compiler.", "jsonType"),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "Wrong overload used. There is a bug in the compiler, please file it. Stack trace: " + Environment.StackTrace
                };

            if (jsonType != typeof(object) && json.Json[key].GetJsonType() != CommonToolUtilities.JsonTypeFromType(jsonType)
                    && ImplicitelyConvert(new JsonFile(json.Path, json.Json[key]), jsonType) == null)
                return new CompilerError(Building)
                {
                    Cause = new InvalidCastException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "'" + key + "' is a " + json.Json.GetJsonType() +
                              ", not a " + CommonToolUtilities.JsonTypeFromType(jsonType) + "."
                };

            return DeserializeJsonPrimitive(new JsonFile(json.Path, json.Json[key]), jsonType, ref value);
        }

        /*CompilerError SetJsonValueInternal(JsonFile json, Type jsonType, ref object value)
        {
            if (jsonType.IsArray)
                return new CompilerError(Building)
                {
                    Cause = new ArgumentException("The type cannot be an array, use the proper overload.", "jsonType"),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "Wrong overload used."
                };

            if (jsonType != typeof(object) && json.Json.GetJsonType() != CommonToolUtilities.JsonTypeFromType(jsonType)
                    && ImplicitelyConvert(new JsonFile(json.Path, json.Json), jsonType) == null)
                return new CompilerError(Building)
                {
                    Cause = new InvalidCastException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "JSON value is a " + json.Json.GetJsonType() +
                              ", not a " + CommonToolUtilities.JsonTypeFromType(jsonType) + "."
                };

            return DeserializeJsonPrimitive(json, jsonType, ref value);
        }*/
        /*CompilerError SetJsonValueInternal<TJsonObj>(JsonFile json, ref TJsonObj value)
        {
            if (typeof(TJsonObj).IsArray)
                return new CompilerError(Building)
                {
                    Cause = new ArgumentException("The type cannot be an array, use the proper overload.", "TJsonObj"),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "Wrong overload used."
                };

            if (typeof(TJsonObj) != typeof(object) && json.Json.GetJsonType() != CommonToolUtilities.JsonTypeFromType(typeof(TJsonObj))
                    && ImplicitelyConvert(new JsonFile(json.Path, json.Json), typeof(TJsonObj)) == null)
                return new CompilerError(Building)
                {
                    Cause = new InvalidCastException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "JSON value is a " + json.Json.GetJsonType() +
                              ", not a " + CommonToolUtilities.JsonTypeFromType(typeof(TJsonObj)) + "."
                };

            return DeserializeJsonPrimitive(json, ref value);
        }*/

        CompilerError DeserializeJsonPrimitive<TJson>(JsonFile jPrimitive, ref TJson outp)
        {
            dynamic o = outp;
            CompilerError ret = DeserializeJsonPrimitive(jPrimitive, typeof(TJson), ref o);
            if (o != null)
                outp = o;
            return ret;
        }
        CompilerError DeserializeJsonPrimitive(JsonFile jPrimitive, Type toType, ref object outp)
        {
            if ((object i = ImplicitelyConvert(jPrimitive, toType)) != null)
            {
                outp = i;

                return null;
            }

            if (jPrimitive.Json.GetJsonType() != CommonToolUtilities.JsonTypeFromType(toType))
                return new CompilerError(Building)
                {
                    Cause = new InvalidCastException(),
                    FilePath = jPrimitive.Path,
                    IsWarning = false,
                    Message = "JSON value is a " + jPrimitive.Json.GetJsonType() +
                              ", not a " + CommonToolUtilities.JsonTypeFromType(toType) + "."
                };

            if (toType.IsEnum)
            {
                if (jPrimitive.Json.IsInt)
                    outp = Convert.ChangeType((int)jPrimitive.Json, toType);
                if (jPrimitive.Json.IsLong)
                    outp = Convert.ChangeType((long)jPrimitive.Json, toType);
                if (jPrimitive.Json.IsString)
                    try
                    {
                        outp = Enum.Parse(toType, (string)jPrimitive.Json, true);
                    }
                    catch (Exception e)
                    {
                        return new CompilerError(Building)
                        {
                            Cause = e,
                            FilePath = jPrimitive.Path,
                            IsWarning = false,
                            Message = "Invalid enum value " + jPrimitive.Json + " in JSON file " + jPrimitive.Path
                        };
                    }
            }

            switch (jPrimitive.Json.GetJsonType())
            {
                case JsonType.Boolean:
                    outp = (bool)jPrimitive.Json;
                    break;
                case JsonType.Double:
                    outp = (double)jPrimitive.Json;
                    break;
                case JsonType.Int:
                    outp = (int)jPrimitive.Json;
                    break;
                case JsonType.Long:
                    outp = (long)jPrimitive.Json;
                    break;
                case JsonType.None: // JSON null
                    outp = null;
                    break;
                case JsonType.String:
                    outp = (string)jPrimitive.Json;
                    break;
                default:
                    return new CompilerError(Building)
                    {
                        Cause = new InvalidCastException(),
                        FilePath = jPrimitive.Path,
                        IsWarning = false,
                        Message = "JSON value is not a primitive."
                    };
            }

            return null;
        }

        object ImplicitelyConvert(JsonFile jPrimitive, Type toType)
        {
            JsonType
                from = jPrimitive.Json.GetJsonType(),
                to   = CommonToolUtilities.JsonTypeFromType(toType);

            if (to == JsonType.Double)
            {
                if (from == JsonType.Long)
                    return (double)(long)jPrimitive.Json;
                if (from == JsonType.Int)
                    return (double)(int )jPrimitive.Json;
            }
            if (to == JsonType.Long)
            {
                if (from == JsonType.Int)
                    return (long)(int)jPrimitive.Json;
            }

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
