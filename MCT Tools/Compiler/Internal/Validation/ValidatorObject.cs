using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using LitJson;
using PoroCYon.MCT.Internal;

namespace PoroCYon.MCT.Tools.Internal.Validation
{
    abstract class ValidatorObject
    {
        [TargetedPatchingOptOut(Consts.TPOOReason)]
        protected static void AddIfNotNull(CompilerError err, List<CompilerError> list)
        {
            if (err != null)
                list.Add(err);
        }

        protected static CompilerError SetJsonValue<TJsonObj>(JsonFile json, string key, ref TJsonObj value)
        {
            if (!json.json.Has(key))
                return new CompilerError()
                {
                    Cause = new KeyNotFoundException(),
                    FilePath = json.path,
                    IsWarning = false,
                    Message = "Key '" + key + "' not found."
                };

            return SetJsonValueInternal(json, key, ref value);
        }
        protected static CompilerError SetJsonValue<TJsonObj>(JsonFile json, string key, ref TJsonObj value, TJsonObj defaultValue)
        {
            if (!json.json.Has(key))
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
                dynamic arr = new dynamic[json.json[key].Count];

                for (int i = 0; i < json.json[key].Count; i++)
                {
                    if (json.json[key][i].GetJsonType() != CommonToolUtilities.JsonTypeFromType(typeof(TJsonObj)))
                        return new CompilerError()
                        {
                            Cause = new InvalidCastException(),
                            FilePath = json.path,
                            IsWarning = false,
                            Message = "'" + key + "[" + i + "]' is a " + json.json.GetJsonType() +
                                      ", not a " + CommonToolUtilities.JsonTypeFromType(typeof(TJsonObj)) + "."
                        };

                    arr[i] = json.json[key][i];
                }

                value = arr;

                return null;
            }

            if (json.json[key].GetJsonType() != CommonToolUtilities.JsonTypeFromType(typeof(TJsonObj)))
                return new CompilerError()
                {
                    Cause = new InvalidCastException(),
                    FilePath = json.path,
                    IsWarning = false,
                    Message = "'" + key + "' is a " + json.json.GetJsonType() +
                              ", not a " + CommonToolUtilities.JsonTypeFromType(typeof(TJsonObj)) + "."
                };

            value = (dynamic)json.json[key]; // dynamic++

            return null;
        }

        internal abstract List<CompilerError> CreateAndValidate(JsonFile json);
    }
}
