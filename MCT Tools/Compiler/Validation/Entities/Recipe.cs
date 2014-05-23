using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using PoroCYon.MCT.Tools.Internal;

namespace PoroCYon.MCT.Tools.Validation.Entities
{
    /// <summary>
    /// An item recipe.
    /// </summary>
    public class Recipe : ValidatorObject
    {
#pragma warning disable 1591
        public Dictionary<string, int> items = new Dictionary<string, int>();
        public List<string> tiles = new List<string>();
        public int creates = 1;
#pragma warning restore 1591

        /// <summary>
        /// Create &amp; validate a JSON file.
        /// </summary>
        /// <param name="json">The json to validate</param>
        /// <returns>A collection of all validation errors.</returns>
        public override IEnumerable<CompilerError> CreateAndValidate(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            #region items
            if (json.Json.Has("items"))
            {
                if (!json.Json["items"].IsObject)
                    errors.Add(new CompilerError()
                    {
                        Cause = new InvalidCastException(),
                        FilePath = json.Path,
                        IsWarning = false,
                        Message = "Key 'items' is a " + json.Json["items"] + ", not an object."
                    });
                else
                {
                    JsonData its = json.Json["items"];

                    foreach (DictionaryEntry kvp in its)
                    {
                        if (kvp.Key.ToString().StartsWith("g:"))
                        {
                            string icgName = kvp.Key.ToString().Substring(2);
                            bool found = false;

                            for (int i = 0; i < ModCompiler.current.CraftGroups.itemGroups.Count; i++)
                                if (ModCompiler.current.CraftGroups.itemGroups[i].name == icgName)
                                {
                                    found = true;
                                    break;
                                }

                            if (!found)
                            {
                                errors.Add(new CompilerError()
                                {
                                    Cause = new KeyNotFoundException(),
                                    FilePath = json.Path,
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
                                FilePath = json.Path,
                                IsWarning = true,
                                Message = "The key '" + kvp.Key + " is already present in the items list, adding the two stacks..."
                            });

                            if (!(kvp.Value is int))
                                errors.Add(new CompilerError()
                                {
                                    Cause = new InvalidCastException(),
                                    FilePath = json.Path,
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
                                FilePath = json.Path,
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
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "Required key 'items' not found."
                });
            #endregion
            #region tiles
            if (json.Json.Has("tiles"))
            {
                if (!json.Json["tiles"].IsArray)
                    errors.Add(new CompilerError()
                    {
                        Cause = new InvalidCastException(),
                        FilePath = json.Path,
                        IsWarning = false,
                        Message = "Key 'tiles' is a " + json.Json["tiles"] + ", not an array of string."
                    });
                else
                {
                    JsonData tis = json.Json["tiles"];

                    for (int i = 0; i < tis.Count; i++)
                    {
                        if (!tis[i].IsString)
                        {
                            errors.Add(new CompilerError()
                            {
                                Cause = new InvalidCastException(),
                                FilePath = json.Path,
                                IsWarning = false,
                                Message = "'tiles[" + i + "]' is a " + tis[i] + ", not a string."
                            });

                            continue;
                        }

                        if (tis[i].ToString().StartsWith("g:"))
                        {
                            string icgName = tis[i].ToString().Substring(2);
                            bool found = false;

                            for (int j = 0; j < ModCompiler.current.CraftGroups.tileGroups.Count; j++)
                                if (ModCompiler.current.CraftGroups.tileGroups[j].name == icgName)
                                {
                                    found = true;
                                    break;
                                }

                            if (!found)
                            {
                                errors.Add(new CompilerError()
                                {
                                    Cause = new KeyNotFoundException(),
                                    FilePath = json.Path,
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
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "Required key 'tiles' not found."
                });
            #endregion

            AddIfNotNull(SetJsonValue(json, "creates", ref creates, 1), errors);

            return errors;
        }
    }
}
