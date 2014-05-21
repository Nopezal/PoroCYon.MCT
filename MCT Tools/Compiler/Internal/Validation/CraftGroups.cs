using System;
using System.Collections.Generic;
using System.Linq;
using LitJson;

namespace PoroCYon.MCT.Tools.Internal.Validation
{
    class CraftGroups : ValidatorObject
    {
        public List<ItemCraftGroup> itemGroups = new List<ItemCraftGroup>();
        public List<TileCraftGroup> tileGroups = new List<TileCraftGroup>();

        internal override List<CompilerError> CreateAndValidate(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            #region itemGroups
            if (json.json.Has("itemGroups"))
            {
                JsonData iGroups = json.json["itemGroups"];

                if (iGroups.IsArray)
                    for (int i = 0; i < iGroups.Count; i++)
                    {
                        if (!iGroups[i].IsObject)
                        {
                            errors.Add(new CompilerError()
                            {
                                Cause = new ArrayTypeMismatchException(),
                                FilePath = json.path,
                                IsWarning = false,
                                Message = "'itemGroups[" + i + "]' is a " + iGroups[i].GetJsonType() + ", not an ItemCraftGroup."
                            });

                            continue;
                        }

                        ItemCraftGroup icg = new ItemCraftGroup();

                        errors.AddRange(icg.CreateAndValidate(new JsonFile(json.path, iGroups[i])));
                        itemGroups.Add(icg);
                    }
                else
                    errors.Add(new CompilerError()
                    {
                        Cause = new InvalidCastException(),
                        FilePath = json.path,
                        IsWarning = false,
                        Message = "Key 'itemGroups' is a " + iGroups.GetJsonType() + ", not an array of ItemCraftGroups."
                    });
            }
            #endregion

            #region tileGroups
            if (json.json.Has("tileGroups"))
            {
                JsonData tGroups = json.json["tileGroups"];

                if (tGroups.IsArray)
                    for (int i = 0; i < tGroups.Count; i++)
                    {
                        if (!tGroups[i].IsObject)
                        {
                            errors.Add(new CompilerError()
                            {
                                Cause = new ArrayTypeMismatchException(),
                                FilePath = json.path,
                                IsWarning = false,
                                Message = "'tileGroups[" + i + "]' is a " + tGroups[i].GetJsonType() + ", not an TileCraftGroup."
                            });

                            continue;
                        }

                        TileCraftGroup tcg = new TileCraftGroup();

                        errors.AddRange(tcg.CreateAndValidate(new JsonFile(json.path, tGroups[i])));
                        tileGroups.Add(tcg);
                    }
                else
                    errors.Add(new CompilerError()
                    {
                        Cause = new InvalidCastException(),
                        FilePath = json.path,
                        IsWarning = false,
                        Message = "Key 'tileGroups' is a " + tGroups.GetJsonType() + ", not an array of TileCraftGroups."
                    });
            }
            #endregion

            return errors;
        }
    }
}
