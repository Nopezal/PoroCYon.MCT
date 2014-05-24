using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LitJson;
using PoroCYon.MCT.Tools.Internal;

namespace PoroCYon.MCT.Tools.Validation.Entities
{
    /// <summary>
    /// An NPC.
    /// </summary>
    public class NPC : EntityValidator
    {
        #region fields
#pragma warning disable 1591
        // internal
        public bool netAlways = false;

        // informative
        public string displayName;
        public string occupation = "The Town NPC";
        public int value = 0;

        // stats
        public int lifeMax;
        public int damage = 0;
        public int defense = 0;
        public float knockbackResist = 1f;
        public int critChance = 4;
        public float critMult = 2f;

        // appearance
        public int frameCount = 1;
        public int animationStyle = 0;
        public bool behindTiles = false;
        public int alpha = 0;

        // interface
        public string textureHead;
        public bool shop = false;
        public bool showHealthBar = true;
        public float lifeBarScale = 1f;
        public bool realLifeHealthBar = false;

        // gameplay
        public float npcSlots = 1f;
        public int aiStyle;
        public bool friendly = false;
        public bool townNPC = false;
        public bool male = true;
        public bool boss = false;
        public bool dontTakeDamage = false;
        public bool lavaImmune = false;
        public bool noGravity = false;
        public bool noTileCollide = false;
        public object[] buffImmune;
        public object soundHit = 0;
        public object soundKilled = 0;
        public string music = String.Empty;
        public List<Drop> drops = new List<Drop>();
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

            errors.AddRange(CreateAndValidateBase(json, "NPC", "NPCs"));

            AddIfNotNull(SetJsonValue(json, "netAlways", ref netAlways, false), errors);

            // informative
            AddIfNotNull(SetJsonValue(json, "displayName", ref displayName, Path.GetFileNameWithoutExtension(json.Path)), errors);
            AddIfNotNull(SetJsonValue(json, "occupation",  ref occupation,  Path.GetFileNameWithoutExtension(json.Path)), errors);
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

            // stats
            AddIfNotNull(SetJsonValue(json, "lifeMax", ref lifeMax), errors);
            AddIfNotNull(SetJsonValue(json, "damage", ref damage, 0), errors);
            AddIfNotNull(SetJsonValue(json, "defense", ref defense, 0), errors);
            AddIfNotNull(SetJsonValue(json, "knockbackResist", ref knockbackResist, 1f), errors);
            AddIfNotNull(SetJsonValue(json, "critChance", ref critChance, 4), errors);
            AddIfNotNull(SetJsonValue(json, "critMult", ref critMult, 4), errors);

            // appearance
            AddIfNotNull(SetJsonValue(json, "frameCount", ref frameCount, 1), errors);
            AddIfNotNull(SetJsonValue(json, "animationStyle", ref animationStyle, 0), errors);
            AddIfNotNull(SetJsonValue(json, "behindTiles", ref behindTiles, false), errors);
            AddIfNotNull(SetJsonValue(json, "alpha", ref alpha, 0), errors);

            // interface
            AddIfNotNull(SetJsonValue(json, "textureHead", ref textureHead, texture + "._Head"), errors);
            if (!ModCompiler.current.files.ContainsKey(textureHead))
                errors.Add(new CompilerError()
                {
                    Cause = new FileNotFoundException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "'textureHead': file " + textureHead + " not found."
                });
            AddIfNotNull(SetJsonValue(json, "shop", ref shop, false), errors);
            AddIfNotNull(SetJsonValue(json, "showHealthBar", ref showHealthBar, true), errors);
            AddIfNotNull(SetJsonValue(json, "lifeBarScale", ref lifeBarScale, 1f), errors);
            AddIfNotNull(SetJsonValue(json, "realLifeHealthBar", ref realLifeHealthBar, false), errors);

            // gameplay
            AddIfNotNull(SetJsonValue(json, "npcSlots", ref npcSlots, 1f), errors);
            AddIfNotNull(SetJsonValue(json, "aiStyle", ref aiStyle, 0), errors);
            AddIfNotNull(SetJsonValue(json, "friendly", ref friendly, false), errors);
            AddIfNotNull(SetJsonValue(json, "townNPC", ref townNPC, false), errors);
            AddIfNotNull(SetJsonValue(json, "male", ref male, true /* based on the vanilla male/female raito, don't sue me because of this */), errors);
            AddIfNotNull(SetJsonValue(json, "boss", ref boss, false), errors);
            AddIfNotNull(SetJsonValue(json, "dontTakeDamage", ref dontTakeDamage, false), errors);
            AddIfNotNull(SetJsonValue(json, "lavaImmune", ref lavaImmune, false), errors);
            AddIfNotNull(SetJsonValue(json, "noGravity", ref noGravity, false), errors);
            AddIfNotNull(SetJsonValue(json, "noTileCollide", ref noTileCollide, false), errors);
            AddIfNotNull(SetJsonValue(json, "buffImmune", ref buffImmune, new object[0]), errors);
            for (int i = 0; i < buffImmune.Length; i++)
                if (!(buffImmune[i] is int) && !(buffImmune[i] is string))
                    errors.Add(new CompilerError()
                    {
                        Cause = new ArrayTypeMismatchException(),
                        FilePath = json.Path,
                        IsWarning = false,
                        Message = "'buffImmune['" + i + "]' must be an int or a string, but is a " + buffImmune[i].GetType()
                    });
            AddIfNotNull(SetJsonValue(json, "soundHit", ref soundHit, 0), errors);
            if (!(soundHit is int) && !(soundHit is string))
                errors.Add(new CompilerError()
                {
                    Cause = new ArrayTypeMismatchException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "'soundHit' must be an int or a string, but is a " + soundHit.GetType()
                });
            AddIfNotNull(SetJsonValue(json, "soundKilled", ref soundKilled, 0), errors);
            if (!(soundKilled is int) && !(soundKilled is string))
                errors.Add(new CompilerError()
                {
                    Cause = new ArrayTypeMismatchException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "'soundKilled' must be an int or a string, but is a " + soundKilled.GetType()
                });
            AddIfNotNull(SetJsonValue(json, "music", ref music, String.Empty), errors);

            #region drops
            if (json.Json.Has("drops"))
            {
                JsonData drs = json.Json["drops"];

                if (drs.IsObject)
                    drs = JsonMapper.ToObject("[" + drs.ToJson() + "]");

                if (drs.IsArray)
                    for (int i = 0; i < drs.Count; i++)
                    {
                        JsonData drop = drs[i];

                        if (drs.IsObject)
                        {
                            Drop d = new Drop();

                            errors.AddRange(d.CreateAndValidate(new JsonFile(json.Path, drop)));
                            drops.Add(d);
                        }
                        else
                            errors.Add(new CompilerError()
                            {
                                Cause = new ArrayTypeMismatchException(),
                                FilePath = json.Path,
                                IsWarning = false,
                                Message = "'drops[" + i + "]' must be a Drop, but is a " + drop.GetJsonType() + "."
                            });
                    }
                else
                    errors.Add(new CompilerError()
                    {
                        Cause = new InvalidCastException(),
                        FilePath = json.Path,
                        IsWarning = false,
                        Message = "'drops' must be a Drop or an array of drops, but is a " + drs.GetJsonType() + "."
                    });
            }
            #endregion

            return errors;
        }
    }
}
