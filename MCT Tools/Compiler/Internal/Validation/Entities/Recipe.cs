using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitJson;

namespace PoroCYon.MCT.Tools.Internal.Validation.Entities
{
    class Recipe : ValidatorObject
    {
        public Dictionary<string, int> items = new Dictionary<string, int>();
        public List<string> tiles = new List<string>();
        public int creates = 1;

        internal override List<CompilerError> CreateAndValidate(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            #region items
            if (json.json.Has("items"))
            {
                if (!json.json["items"].IsObject)
                    errors.Add(new CompilerError()
                    {
                        Cause = new InvalidCastException(),
                        FilePath = json.path,
                        IsWarning = false,
                        Message = "Key 'items' is a " + json.json["items"] + ", not an object."
                    });
                else
                {
                    JsonData its = json.json["items"];

                    foreach (DictionaryEntry kvp in its)
                    {
                        if (kvp.Key.ToString().StartsWith("g:"))
                        {
                            string icgName = kvp.Key.ToString().Substring(2);
                            bool found = false;

                            for (int i = 0; i < Validator.current.craftGroups.itemGroups.Count; i++)
                                if (Validator.current.craftGroups.itemGroups[i].name == icgName)
                                {
                                    found = true;
                                    break;
                                }

                            if (!found)
                            {
                                errors.Add(new CompilerError()
                                {
                                    Cause = new KeyNotFoundException(),
                                    FilePath = json.path,
                                    IsWarning = false,
                                    Message = "CraftGroup " + kvp.Key + " not found."
                                });

                                continue;
                            }
                        }

                        if (items.ContainsKey(kvp.Key.ToString()))
                        {
                            errors.Add(new CompilerError()
                            {
                                Cause = new ArgumentException(),
                                FilePath = json.path,
                                IsWarning = true,
                                Message = "The key '" + kvp.Key + " is already present in the items list, adding the two stacks..."
                            });

                            if (!(kvp.Value is int))
                                errors.Add(new CompilerError()
                                {
                                    Cause = new InvalidCastException(),
                                    FilePath = json.path,
                                    IsWarning = false,
                                    Message = "The key '" + kvp.Key + " 's value should be an int, not a "
                                              + (kvp.Value is JsonData ? ((JsonData)kvp.Value).GetJsonType() : (object)kvp.Value.GetType()) + "."
                                });
                            else
                                items[kvp.Key.ToString()] += (int)kvp.Value;
                        }
                        else if (!(kvp.Value is int))
                            errors.Add(new CompilerError()
                            {
                                Cause = new InvalidCastException(),
                                FilePath = json.path,
                                IsWarning = false,
                                Message = "The key '" + kvp.Key + " 's value should be an int, not a "
                                          + (kvp.Value is JsonData ? ((JsonData)kvp.Value).GetJsonType() : (object)kvp.Value.GetType()) + "."
                            });
                        else
                            items.Add(kvp.Key.ToString(), (int)kvp.Value);
                    }
                }
            }
            else
                errors.Add(new CompilerError()
                {
                    Cause = new KeyNotFoundException(),
                    FilePath = json.path,
                    IsWarning = false,
                    Message = "Required key 'items' not found."
                });
            #endregion
            #region tiles
            if (json.json.Has("tiles"))
            {
                if (!json.json["tiles"].IsArray)
                    errors.Add(new CompilerError()
                    {
                        Cause = new InvalidCastException(),
                        FilePath = json.path,
                        IsWarning = false,
                        Message = "Key 'tiles' is a " + json.json["tiles"] + ", not an array of string."
                    });
                else
                {
                    JsonData tis = json.json["tiles"];

                    for (int i = 0; i < tis.Count; i++)
                    {
                        if (!tis[i].IsString)
                        {
                            errors.Add(new CompilerError()
                            {
                                Cause = new InvalidCastException(),
                                FilePath = json.path,
                                IsWarning = false,
                                Message = "'tiles[" + i + "]' is a " + tis[i] + ", not a string."
                            });

                            continue;
                        }

                        if (tis[i].ToString().StartsWith("g:"))
                        {
                            string icgName = tis[i].ToString().Substring(2);
                            bool found = false;

                            for (int j = 0; j < Validator.current.craftGroups.tileGroups.Count; j++)
                                if (Validator.current.craftGroups.tileGroups[j].name == icgName)
                                {
                                    found = true;
                                    break;
                                }

                            if (!found)
                            {
                                errors.Add(new CompilerError()
                                {
                                    Cause = new KeyNotFoundException(),
                                    FilePath = json.path,
                                    IsWarning = false,
                                    Message = "CraftGroup " + tis[i] + " not found."
                                });

                                continue;
                            }
                        }


                        if (!tiles.Contains((string)tis[i]))
                            tiles.Add((string)tis[i]);
                    }
                }
            }
            else
                errors.Add(new CompilerError()
                {
                    Cause = new KeyNotFoundException(),
                    FilePath = json.path,
                    IsWarning = false,
                    Message = "Required key 'tiles' not found."
                });
            #endregion

            AddIfNotNull(SetJsonValue(json, "creates", ref creates, 1), errors);

            return errors;
        }
    }
}
