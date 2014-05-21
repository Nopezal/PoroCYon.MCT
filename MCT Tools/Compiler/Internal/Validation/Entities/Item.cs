using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LitJson;

namespace PoroCYon.MCT.Tools.Internal.Validation.Entities
{
    class Item : EntityValidator
    {
        public int rare = 0;
        public string tooltip = String.Empty;
        public int value = 0;
        public int maxStack = 1;
        public bool notMaterial = false;
        public List<Recipe> recipes = new List<Recipe>();

        internal Item(ModInfo info)
            : base(info)
        {

        }

        internal override List<CompilerError> CreateAndValidate(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            AddIfNotNull(SetJsonValue(json, "code", ref code, modInfo.internalName + ".Items." + Path.GetFileNameWithoutExtension(json.path)), errors);

            AddIfNotNull(SetJsonValue(json, "rare", ref rare, 0), errors);
            #region tooltip
            if (json.json.Has("tooltip"))
            {
                JsonData tt = json.json["tooltip"];

                if (tt.IsString)
                    tooltip = (string)tt;
                else if (tt.IsArray)
                {
                    List<string> tips = new List<string>();

                    for (int i = 0; i < tt.Count; i++)
                    {
                        JsonData tip = tt[i];

                        if (!tip.IsString)
                            errors.Add(new CompilerError()
                            {
                                Cause = new ArrayTypeMismatchException(),
                                FilePath = json.path,
                                IsWarning = false,
                                Message = "'tooltip[" + i + "]' should be a string, but is a " + tip.GetJsonType() + "."
                            });
                        else
                            tips.Add((string)tip);
                    }

                    for (int i = 0; i < tips.Count; i++)
                    {
                        tooltip += tips[i];

                        if (i < tips.Count - 1)
                            tooltip += Environment.NewLine;
                    }
                }
                else
                    errors.Add(new CompilerError()
                    {
                        Cause = new InvalidCastException(),
                        FilePath = json.path,
                        IsWarning = false,
                        Message = "'tooltip' should be a string or an array of strings, but is a " + tt.GetJsonType() + "."
                    });
            }
            #endregion
            #region value
            if (json.json.Has("value"))
            {
                JsonData v = json.json["value"];

                if (v.IsInt)
                    value = (int)v;
                else if (v.IsArray)
                {
                    if (v.Count > 4)
                        errors.Add(new CompilerError()
                        {
                            Cause = new IndexOutOfRangeException(),
                            FilePath = json.path,
                            IsWarning = false,
                            Message = "'value' array's length should be ranging from 0 to 4."
                        });

                    int[] values  = new int[4]; // structs, no init needed

                    for (int i = 0; i < v.Count; i++)
                    {
                        JsonData val = v[i];

                        if (!val.IsInt)
                            errors.Add(new CompilerError()
                            {
                                Cause = new ArrayTypeMismatchException(),
                                FilePath = json.path,
                                IsWarning = false,
                                Message = "'value[" + i + "]' should be an int, but is a " + val.GetJsonType() + "."
                            });
                        else
                            values[i] = (int)val;
                    }

                    value += values[0] * 1000000; // p
                    value += values[1] * 10000;   // g
                    value += values[2] * 100;     // g
                    value += values[3] * 1;       // g
                }
                else
                    errors.Add(new CompilerError()
                    {
                        Cause = new InvalidCastException(),
                        FilePath = json.path,
                        IsWarning = false,
                        Message = "'value' should be an int or an array of ints, but is a " + v.GetJsonType() + "."
                    });
            }
            #endregion
            AddIfNotNull(SetJsonValue(json, "maxStack",    ref maxStack,        1), errors);
            AddIfNotNull(SetJsonValue(json, "notMaterial", ref notMaterial, false), errors);
            #region recipes
            if (json.json.Has("recipes"))
            {
                JsonData recs = json.json["recipes"];

                if (recs.IsArray)
                    for (int i = 0; i < recs.Count; i++)
                    {
                        if (!recs[i].IsObject)
                        {
                            errors.Add(new CompilerError()
                            {
                                Cause = new ArrayTypeMismatchException(),
                                FilePath = json.path,
                                IsWarning = false,
                                Message = "'recipes[" + i + "]' is a " + recs[i].GetJsonType() + ", not a Recipe."
                            });

                            continue;
                        }

                        Recipe r = new Recipe();
                        
                        errors.AddRange(r.CreateAndValidate(new JsonFile(json.path, recs[i])));
                        recipes.Add(r);
                    }
                else
                    errors.Add(new CompilerError()
                    {
                        Cause = new InvalidCastException(),
                        FilePath = json.path,
                        IsWarning = false,
                        Message = "Key 'recipes' is a " + recs.GetJsonType() + ", not an array of Recipes."
                    });
            }
            #endregion

            return errors;
        }
    }
}
