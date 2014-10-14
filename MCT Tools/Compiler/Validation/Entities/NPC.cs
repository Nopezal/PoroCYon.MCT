using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LitJson;
using PoroCYon.Extensions;

namespace PoroCYon.MCT.Tools.Compiler.Validation.Entities
{
    /// <summary>
    /// An NPC.
    /// </summary>
    public class NPC(ModCompiler mc) : EntityValidator(mc)
    {
        #region fields
#pragma warning disable 1591
        // internal
        public bool netAlways = false;

        // informative
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
        public List<Union<string, int>> buffImmune = new List<Union<string, int>>();
        public Union<string, int> soundHit = 0;
        public Union<string, int> soundKilled = 0;
        public string music = String.Empty;
        public List<Drop> drops = new List<Drop>();
		public Union<string, int> catchItem = 0;
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
                        errors.Add(new CompilerError(Building)
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
                            errors.Add(new CompilerError(Building)
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
                    errors.Add(new CompilerError(Building)
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

            // gameplay
            AddIfNotNull(SetJsonValue(json, "npcSlots", ref npcSlots, 1f), errors);
            AddIfNotNull(SetJsonValue(json, "aiStyle", ref aiStyle, 0), errors);
            AddIfNotNull(SetJsonValue(json, "friendly", ref friendly, false), errors);
            AddIfNotNull(SetJsonValue(json, "townNPC", ref townNPC, false), errors);
            AddIfNotNull(SetJsonValue(json, "male", ref male, true /* based on the vanilla male/female raito + default value of the field, don't sue me because of this */), errors);
            AddIfNotNull(SetJsonValue(json, "boss", ref boss, false), errors);
            AddIfNotNull(SetJsonValue(json, "dontTakeDamage", ref dontTakeDamage, false), errors);
            AddIfNotNull(SetJsonValue(json, "lavaImmune", ref lavaImmune, false), errors);
            AddIfNotNull(SetJsonValue(json, "noGravity", ref noGravity, false), errors);
            AddIfNotNull(SetJsonValue(json, "noTileCollide", ref noTileCollide, false), errors);
			AddIfNotNull(SetJsonValue(json, "catchItem", ref catchItem, catchItem), errors);

			// appearance
			AddIfNotNull(SetJsonValue(json, "frameCount", ref frameCount, 1), errors);
            AddIfNotNull(SetJsonValue(json, "animationStyle", ref animationStyle, 0), errors);
            AddIfNotNull(SetJsonValue(json, "behindTiles", ref behindTiles, false), errors);
            AddIfNotNull(SetJsonValue(json, "alpha", ref alpha, 0), errors);

            // interface
            AddIfNotNull(SetJsonValue(json, "textureHead", ref textureHead, texture + "._Head"), errors);
            if (townNPC && !Building.files.ContainsKey(textureHead))
                errors.Add(new CompilerError(Building)
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

            if (json.Json.Has("buffImmune"))
            {
                JsonData j = json.Json["buffImmune"];

                if (!j.IsArray)
                    errors.Add(new CompilerError(Building)
                    {
                        Cause = new ArrayTypeMismatchException(),
                        FilePath = json.Path,
                        IsWarning = false,
                        Message = "'buffImmune must be an array, but is a " + j.GetType()
                    });
                else
                    for (int i = 0; i < j.Count; i++)
                        if (j[i].IsString)
                            buffImmune.Add((string)j[i]);
                        else if (j[i].IsInt)
                            buffImmune.Add((int   )j[i]);
                        else
                            errors.Add(new CompilerError(Building)
                            {
                                Cause = new ArrayTypeMismatchException(),
                                FilePath = json.Path,
                                IsWarning = false,
                                Message = "'buffImmune['" + i + "]' must be an int or a string, but is a " + buffImmune[i].GetType()
                            });
            }

            AddIfNotNull(SetJsonValue(json, "soundHit", ref soundHit, soundHit), errors);
            AddIfNotNull(SetJsonValue(json, "soundKilled", ref soundKilled, soundKilled), errors);
            AddIfNotNull(SetJsonValue(json, "music", ref music, String.Empty), errors);

            #region drops
            if (json.Json.Has("drops"))
            {
                JsonData drps = json.Json["drops"]; // I read this as 'derps' instead of 'drops' .__.

                if (drps.IsObject)
                    drps = JsonMapper.ToObject("[" + drps.ToJson() + "]");

                if (drps.IsArray)
                    for (int i = 0; i < drps.Count; i++)
                    {
                        JsonData drop = drps[i];

                        if (drop.IsObject)
                        {
                            Drop d = new Drop(Compiler);

                            errors.AddRange(d.CreateAndValidate(new JsonFile(json.Path, drop)));
                            drops.Add(d);
                        }
                        else
                            errors.Add(new CompilerError(Building)
                            {
                                Cause = new ArrayTypeMismatchException(),
                                FilePath = json.Path,
                                IsWarning = false,
                                Message = "'drops[" + i + "]' must be a Drop, but is a " + drop.GetJsonType() + "."
                            });
                    }
                else
                    errors.Add(new CompilerError(Building)
                    {
                        Cause = new InvalidCastException(),
                        FilePath = json.Path,
                        IsWarning = false,
                        Message = "'drops' must be a Drop or an array of drops, but is a " + drps.GetJsonType() + "."
                    });
            }
            #endregion

            return errors;
        }
    }
}
