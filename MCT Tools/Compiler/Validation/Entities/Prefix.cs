using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LitJson;
using Terraria;

namespace PoroCYon.MCT.Tools.Compiler.Validation.Entities
{
    /// <summary>
    /// An item prefix.
    /// </summary>
    public class Prefix(ModCompiler mc) : ValidatorObject(mc)
    {
#pragma warning disable 1591
        public string internalName;
        public int tier;
        public float value, damage;
        public int crit;
        public float mana, shootSpeed, size, knockback, meleeDamage, rangedDamage, magicDamage, minionDamage;
        public int defense, maxLife, maxMana;
        public float moveSpeed, meleeSpeed;
        public int meleeCrit, rangedCrit, magicCrit;
        public PrefixType type;
#pragma warning restore 1591

        /// <summary>
        /// Create &amp; validate a JSON file.
        /// </summary>
        /// <param name="json">The json to validate</param>
        /// <returns>A collection of all validation errors.</returns>
        public override IEnumerable<CompilerError> CreateAndValidate(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            internalName = Building.Info.internalName + ":" + Path.GetFileNameWithoutExtension(json.Path);

            AddIfNotNull(SetJsonValue(json, "tier", ref tier, 0), errors);

            AddIfNotNull(SetJsonValue(json, "value",  ref value,  0f), errors);
            AddIfNotNull(SetJsonValue(json, "damage", ref damage, 0f), errors);

            AddIfNotNull(SetJsonValue(json, "crit", ref crit, 4), errors);

            AddIfNotNull(SetJsonValue(json, "mana",         ref mana,         0f), errors);
            AddIfNotNull(SetJsonValue(json, "shootSpeed",   ref shootSpeed,   0f), errors);
            AddIfNotNull(SetJsonValue(json, "size",         ref size,         0f), errors);
            AddIfNotNull(SetJsonValue(json, "knockback",    ref knockback,    0f), errors);

            float allDamage = 0f;
            AddIfNotNull(SetJsonValue(json, "allDamage", ref allDamage, 0f), errors);
            meleeDamage = rangedDamage = magicDamage = minionDamage = allDamage;

            AddIfNotNull(SetJsonValue(json, "meleeDamage",  ref meleeDamage,  allDamage), errors);
            AddIfNotNull(SetJsonValue(json, "rangedDamage", ref rangedDamage, allDamage), errors);
            AddIfNotNull(SetJsonValue(json, "magicDamage",  ref magicDamage,  allDamage), errors);
            AddIfNotNull(SetJsonValue(json, "minionDamage", ref minionDamage, allDamage), errors);

            AddIfNotNull(SetJsonValue(json, "defense", ref defense, 4), errors);
            AddIfNotNull(SetJsonValue(json, "maxLife", ref maxLife, 4), errors);
            AddIfNotNull(SetJsonValue(json, "maxMana", ref maxMana, 4), errors);

            AddIfNotNull(SetJsonValue(json, "moveSpeed",  ref moveSpeed,  0f), errors);
            AddIfNotNull(SetJsonValue(json, "meleeSpeed", ref meleeSpeed, 0f), errors);

            int allCrit = 4;
            AddIfNotNull(SetJsonValue(json, "allCrit", ref allCrit, 4), errors);
            meleeCrit = rangedCrit = magicCrit = allCrit;

            AddIfNotNull(SetJsonValue(json, "meleeCrit",  ref meleeCrit,  allCrit), errors);
            AddIfNotNull(SetJsonValue(json, "rangedCrit", ref rangedCrit, allCrit), errors);
            AddIfNotNull(SetJsonValue(json, "magicCrit",  ref magicCrit,  allCrit), errors);

            if (json.Json.Has("type"))
            {
                JsonData t = json.Json["type"];

                Action<JsonData> SetType = (j) =>
                {
                    if (j.IsString)
                    {
                        if (Enum.TryParse((string)j, true, out PrefixType pType))
                            type |= pType;
                    }
                    else if (j.IsInt)
                        type |= (PrefixType)(int)j;
                    else
                        errors.Add(new CompilerError(Building)
                        {
                            Cause = new InvalidCastException(),
                            FilePath = json.Path,
                            IsWarning = false,
                            Message = "'type' is a " + j.GetJsonType() + ", but should be a string or an int (or an array of one of them)."
                        });
                };

                if (t.IsArray)
                    for (int i = 0; i < t.Count; i++)
                        SetType(t[i]);
                else
                    SetType(t);
            }

            return errors;
        }
    }
}
