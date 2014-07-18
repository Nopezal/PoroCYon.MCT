using System;
using System.Collections.Generic;
using System.Linq;
using LitJson;

namespace PoroCYon.MCT.Tools.Compiler.Validation
{
    /// <summary>
    /// A CraftGroup JSON file (CraftGroups.json)
    /// </summary>
    public class CraftGroups : ValidatorObject
    {
#pragma warning disable 1591
        public List<ItemCraftGroup> itemGroups = new List<ItemCraftGroup>();
        public List<TileCraftGroup> tileGroups = new List<TileCraftGroup>();
#pragma warning restore 1591

        /// <summary>
        /// Create &amp; validate a JSON file.
        /// </summary>
        /// <param name="json">The json to validate</param>
        /// <returns>A collection of all validation errors.</returns>
        public override IEnumerable<CompilerError> CreateAndValidate(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            #region itemGroups
            if (json.Json.Has("itemGroups"))
            {
                JsonData iGroups = json.Json["itemGroups"];

                if (iGroups.IsArray)
                    for (int i = 0; i < iGroups.Count; i++)
                    {
                        if (!iGroups[i].IsObject)
                        {
                            errors.Add(new CompilerError()
                            {
                                Cause = new ArrayTypeMismatchException(),
                                FilePath = json.Path,
                                IsWarning = false,
                                Message = "'itemGroups[" + i + "]' is a " + iGroups[i].GetJsonType() + ", not an ItemCraftGroup."
                            });

                            continue;
                        }

                        ItemCraftGroup icg = new ItemCraftGroup();

                        errors.AddRange(icg.CreateAndValidate(new JsonFile(json.Path, iGroups[i])));
                        itemGroups.Add(icg);
                    }
                else
                    errors.Add(new CompilerError()
                    {
                        Cause = new InvalidCastException(),
                        FilePath = json.Path,
                        IsWarning = false,
                        Message = "Key 'itemGroups' is a " + iGroups.GetJsonType() + ", not an array of ItemCraftGroups."
                    });
            }
            #endregion

            #region tileGroups
            if (json.Json.Has("tileGroups"))
            {
                JsonData tGroups = json.Json["tileGroups"];

                if (tGroups.IsArray)
                    for (int i = 0; i < tGroups.Count; i++)
                    {
                        if (!tGroups[i].IsObject)
                        {
                            errors.Add(new CompilerError()
                            {
                                Cause = new ArrayTypeMismatchException(),
                                FilePath = json.Path,
                                IsWarning = false,
                                Message = "'tileGroups[" + i + "]' is a " + tGroups[i].GetJsonType() + ", not an TileCraftGroup."
                            });

                            continue;
                        }

                        TileCraftGroup tcg = new TileCraftGroup();

                        errors.AddRange(tcg.CreateAndValidate(new JsonFile(json.Path, tGroups[i])));
                        tileGroups.Add(tcg);
                    }
                else
                    errors.Add(new CompilerError()
                    {
                        Cause = new InvalidCastException(),
                        FilePath = json.Path,
                        IsWarning = false,
                        Message = "Key 'tileGroups' is a " + tGroups.GetJsonType() + ", not an array of TileCraftGroups."
                    });
            }
            #endregion

            return errors;
        }
    }
}
