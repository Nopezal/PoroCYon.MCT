using System;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using PoroCYon.Extensions;

namespace PoroCYon.MCT.Tools.Compiler.Validation.Entities
{
    /// <summary>
    /// An item.
    /// </summary>
    public class Item : EntityValidator
    {
        #region fields
#pragma warning disable 1591
        // informative
        public int rare = 0;
        public List<string> tooltip = new List<string>();
        public int value = 0;
        public int maxStack = 1;
        public bool notMaterial = false;
        public List<Recipe> recipes = new List<Recipe>();

        // appearance
        public bool noUseGraphic = false;

        // amrour
        public bool accessory = false;
        public int defense = 0;
        public int lifeRegen = 0;
        public int manaIncrease = 0;
        public int tileBoost = 0;
        public string setName = String.Empty;
        public bool vanity = false;
        public bool armorHead = false, armorBody = false, armorLegs = false;
        public string textureHead = String.Empty, textureBody = String.Empty, textureLegs = String.Empty, textureFemale = String.Empty, textureArm = String.Empty;
        public int hairType = 0;
        public bool hasHands = false;

        // use
        public int useStyle = 0;
        public int holdStyle = 0;
        public int useTime = 100;
        public int useAnimation = 100;
        public float[] holdoutOffset = new float[2];
        public float[] holdoutOrigin = new float[2];
        public int reuseDelay = 0;
        public bool channel = false;
        public int pick = 0, axe = 0, hammer = 0;
        public bool autoReuse = false;
        public bool useTurn = false;
        public Union<string, int> useSound = 0;
        public int mana = 0;
        public bool consumable = false;

        // combat
        public bool noMelee = false;
        public bool melee = false, ranged = false, magic = false, summon = false;
        public int damage = 0;
        public int crit = 4;
        public float knockback = 0f;
        public Union<string, int> shoot = 0;
        public float shootSpeed = 0f;
        public Union<string, int> useAmmo = 0;
        public Union<string, int> ammo = 0;
        public bool notAmmo = false;

        // potion
        public bool potion = false;
        public int healLife = 0, healMana = 0;
        public Union<string, int> buff = 0;
        public int buffTime = 0;

        // tile
        public Union<string, int> createTile = -1;
        public Union<string, int> tileWand   =  0;
        public Union<string, int> createWall =  0;
        public int placeStyle = 0;
#pragma warning restore 1591
        #endregion

        /// <summary>
        /// Create &amp; validate a JSON file.
        /// </summary>
        /// <param name="json">The json to validate</param>
        /// <returns>A collection of all validation errors.</returns>
        public override IEnumerable<CompilerError> CreateAndValidate(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            errors.AddRange(CreateAndValidateBase(json, "Item", "Items"));

            #region informative
            AddIfNotNull(SetJsonValue(json, "rare", ref rare, 0), errors);
            #region tooltip
            if (json.Json.Has("tooltip"))
            {
                JsonData tt = json.Json["tooltip"];

                if (tt.IsString)
                    tooltip.Add((string)tt);
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
                                FilePath = json.Path,
                                IsWarning = false,
                                Message = "'tooltip[" + i + "]' should be a string, but is a " + tip.GetJsonType() + "."
                            });
                        else
                            tips.Add((string)tip);
                    }

                    for (int i = 0; i < tips.Count; i++)
                        tooltip.Add(tips[i]);
                }
                else
                    errors.Add(new CompilerError()
                    {
                        Cause = new InvalidCastException(),
                        FilePath = json.Path,
                        IsWarning = false,
                        Message = "'tooltip' should be a string or an array of strings, but is a " + tt.GetJsonType() + "."
                    });
            }
            #endregion
            #region value
            if (json.Json.Has("value"))
            {
                JsonData v = json.Json["value"];

                if (v.IsInt)
                    value = (int)v;
                else if (v.IsArray)
                {
                    if (v.Count > 4)
                        errors.Add(new CompilerError()
                        {
                            Cause = new IndexOutOfRangeException(),
                            FilePath = json.Path,
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
                                FilePath = json.Path,
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
                        FilePath = json.Path,
                        IsWarning = false,
                        Message = "'value' should be an int or an array of ints, but is a " + v.GetJsonType() + "."
                    });
            }
            #endregion
            AddIfNotNull(SetJsonValue(json, "maxStack",    ref maxStack,        1), errors);
            AddIfNotNull(SetJsonValue(json, "notMaterial", ref notMaterial, false), errors);
            #region recipes
            if (json.Json.Has("recipes"))
            {
                JsonData recs = json.Json["recipes"];

                if (recs.IsArray)
                    for (int i = 0; i < recs.Count; i++)
                    {
                        if (!recs[i].IsObject)
                        {
                            errors.Add(new CompilerError()
                            {
                                Cause = new ArrayTypeMismatchException(),
                                FilePath = json.Path,
                                IsWarning = false,
                                Message = "'recipes[" + i + "]' is a " + recs[i].GetJsonType() + ", not a Recipe."
                            });

                            continue;
                        }

                        Recipe r = new Recipe();
                        
                        errors.AddRange(r.CreateAndValidate(new JsonFile(json.Path, recs[i])));
                        recipes.Add(r);
                    }
                else
                    errors.Add(new CompilerError()
                    {
                        Cause = new InvalidCastException(),
                        FilePath = json.Path,
                        IsWarning = false,
                        Message = "Key 'recipes' is a " + recs.GetJsonType() + ", not an array of Recipes."
                    });
            }
            #endregion
            #endregion

            // appearance
            AddIfNotNull(SetJsonValue(json, "noUseGraphic", ref noUseGraphic, false), errors);

            #region armour
            AddIfNotNull(SetJsonValue(json, "accessory", ref accessory, false), errors);
            AddIfNotNull(SetJsonValue(json, "defense", ref defense, 0), errors);
            AddIfNotNull(SetJsonValue(json, "lifeRegen", ref lifeRegen, 0), errors);
            AddIfNotNull(SetJsonValue(json, "manaIncrease", ref manaIncrease, 0), errors);
            AddIfNotNull(SetJsonValue(json, "tileBoost", ref tileBoost, 0), errors);
            AddIfNotNull(SetJsonValue(json, "setName", ref setName, String.Empty), errors);
            AddIfNotNull(SetJsonValue(json, "vanity", ref vanity, false), errors);
            AddIfNotNull(SetJsonValue(json, "armorHead", ref armorHead, false), errors);
            AddIfNotNull(SetJsonValue(json, "armorBody", ref armorBody, false), errors);
            AddIfNotNull(SetJsonValue(json, "armorLegs", ref armorLegs, false), errors);
            AddIfNotNull(SetJsonValue(json, "textureHead", ref textureHead, texture + "._Head"), errors);
            AddIfNotNull(SetJsonValue(json, "textureBody", ref textureBody, texture + "._Body"), errors);
            AddIfNotNull(SetJsonValue(json, "textureLegs", ref textureLegs, texture + "._Legs"), errors);
            AddIfNotNull(SetJsonValue(json, "textureFemale", ref textureFemale, textureBody + "_Female"), errors);
            AddIfNotNull(SetJsonValue(json, "textureArm", ref textureArm, texture + "._Arm"), errors);
            AddIfNotNull(SetJsonValue(json, "hairType", ref hairType, 0), errors);
            if (hairType < 0 || hairType > 3)
                errors.Add(new CompilerError()
                {
                    Cause = new ArgumentOutOfRangeException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "'hairType': invalid value. The value must be an element of [0;3]."
                });
            AddIfNotNull(SetJsonValue(json, "hasHands", ref hasHands, false), errors);
            #endregion

            #region use
            AddIfNotNull(SetJsonValue(json, "useStyle", ref useStyle, 0), errors);
            if (useStyle < 0 || useStyle > 5)
                errors.Add(new CompilerError()
                {
                    Cause = new ArgumentOutOfRangeException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "'useStyle': invalid value. The value must be an element of [0;5]."
                });
            AddIfNotNull(SetJsonValue(json, "holdStyle", ref holdStyle, 0), errors);
            if (holdStyle < 0 || holdStyle > 2)
                errors.Add(new CompilerError()
                {
                    Cause = new ArgumentOutOfRangeException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "'holdStyle': invalid value. The value must be an element of [0;2]."
                });
            AddIfNotNull(SetJsonValue(json, "useTime", ref useTime, 100), errors);
            if (useTime <= 0)
                errors.Add(new CompilerError()
                {
                    Cause = new ArgumentOutOfRangeException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "'useTime': invalid value. The value must be grater than 0."
                });
            AddIfNotNull(SetJsonValue(json, "useAnimation", ref useAnimation, 100), errors);
            if (useTime <= 0)
                errors.Add(new CompilerError()
                {
                    Cause = new ArgumentOutOfRangeException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "'useAnimation': invalid value. The value must be grater than 0."
                });
            AddIfNotNull(SetJsonValue(json, "holdoutOffset", ref holdoutOffset, new float[2]), errors);
            if (holdoutOffset.Length != 2)
                errors.Add(new CompilerError()
                {
                    Cause = new IndexOutOfRangeException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "'holdoutOffset': array length must be 2."
                });
            AddIfNotNull(SetJsonValue(json, "holdoutOrigin", ref holdoutOrigin, new float[2]), errors);
            if (holdoutOrigin.Length != 2)
                errors.Add(new CompilerError()
                {
                    Cause = new IndexOutOfRangeException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "'holdoutOrigin': array length must be 2."
                });
            AddIfNotNull(SetJsonValue(json, "reuseDelay", ref reuseDelay, 0), errors);
            AddIfNotNull(SetJsonValue(json, "channel", ref channel, false), errors);
            AddIfNotNull(SetJsonValue(json, "pick", ref pick, 0), errors);
            AddIfNotNull(SetJsonValue(json, "axe", ref axe, 0), errors);
            AddIfNotNull(SetJsonValue(json, "hammer", ref hammer, 0), errors);
            AddIfNotNull(SetJsonValue(json, "autoReuse", ref autoReuse, false), errors);
            AddIfNotNull(SetJsonValue(json, "useTurn", ref useTurn, false), errors);
            AddIfNotNull(SetJsonValue(json, "useSound", ref useSound, useSound), errors);
            AddIfNotNull(SetJsonValue(json, "mana", ref mana, 0), errors);
            AddIfNotNull(SetJsonValue(json, "consumable", ref consumable, false), errors);
            #endregion

            #region combat
            AddIfNotNull(SetJsonValue(json, "noMelee", ref noMelee, false), errors);
            AddIfNotNull(SetJsonValue(json, "melee", ref melee, false), errors);
            AddIfNotNull(SetJsonValue(json, "ranged", ref ranged, false), errors);
            AddIfNotNull(SetJsonValue(json, "magic", ref magic, false), errors);
            AddIfNotNull(SetJsonValue(json, "summon", ref summon, false), errors);
            AddIfNotNull(SetJsonValue(json, "damage", ref damage, 0), errors);
            AddIfNotNull(SetJsonValue(json, "crit", ref crit, 4), errors);
            AddIfNotNull(SetJsonValue(json, "knockback", ref knockback, 0), errors);
            AddIfNotNull(SetJsonValue(json, "shoot", ref shoot, shoot), errors);
            AddIfNotNull(SetJsonValue(json, "shootSpeed", ref shootSpeed, 1f), errors);
            AddIfNotNull(SetJsonValue(json, "useAmmo", ref useAmmo, useAmmo), errors);
            AddIfNotNull(SetJsonValue(json, "ammo", ref ammo, ammo), errors);
            AddIfNotNull(SetJsonValue(json, "notAmmo", ref notAmmo, false), errors);
            #endregion

            #region potion
            AddIfNotNull(SetJsonValue(json, "potion",   ref potion,   false), errors);
            AddIfNotNull(SetJsonValue(json, "healLife", ref healLife, 0    ), errors);
            AddIfNotNull(SetJsonValue(json, "healMana", ref healMana, 0    ), errors);

            AddIfNotNull(SetJsonValue(json, "buff", ref buff, buff), errors);

            AddIfNotNull(SetJsonValue(json, "buffTime", ref buffTime, 0    ), errors);
            #endregion

            #region tile
            AddIfNotNull(SetJsonValue(json, "createTile", ref createTile, createTile), errors);
            AddIfNotNull(SetJsonValue(json, "tileWand"  , ref tileWand  , tileWand  ), errors);
            AddIfNotNull(SetJsonValue(json, "createWall", ref createWall, createWall), errors);

            AddIfNotNull(SetJsonValue(json, "placeStyle", ref placeStyle, 0), errors);
            #endregion

            return errors;
        }
    }
}
