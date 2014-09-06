using System;
using System.Collections.Generic;
using System.Linq;
using PoroCYon.Extensions;

namespace PoroCYon.MCT.Tools.Compiler.Validation.Entities
{
    /// <summary>
    /// A tile.
    /// </summary>
    public class Tile(ModCompiler mc) : EntityValidator(mc)
    {
#pragma warning disable 1591
        public int frameWidth = 16;
        public int frameHeight = 16;
        public int sheetColumns = 1;
        public int sheetRows = 1;
        public bool solid = false;
        public bool solidTop = false;
        public bool frameImportant = false;
        public int placementFrameX = 0;
        public int placementFrameY = 0;
        public string placementConditions = "placeTouchingSolid"; 
        public int[] placementOrigin;
        public bool breaksFast, breaskByPick, breaksByAxe, breaksByHammer, breaksByCut, breaksByWater, breaksByLava;
        public int minPick, minAxe, minHammer;
        public float ratePick, rateAxe, rateHammer;
        public bool table, rope, noAttach, tileDungeon, blocksLight, blocksSun, glows, shines;
        public int shineChance, frame, frameCounter;
        public bool brick, moss, stone, mergeDirt, tileSand, tileFlame, alchemyFlower;
        public int sound, soundGroup, dust;
        public Union<string, int> drop =01;
#pragma warning restore 1591

        /// <summary>
        /// Create &amp; validate a JSON file.
        /// </summary>
        /// <param name="json">The json to validate</param>
        /// <returns>A collection of all validation errors.</returns>
        public override IEnumerable<CompilerError> CreateAndValidate(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            errors.AddRange(CreateAndValidateBase(json, "Tile", "Tiles"));

            AddIfNotNull(SetJsonValue(json, "frameWidth",     ref frameWidth,     16   ), errors);
            AddIfNotNull(SetJsonValue(json, "frameHeight",    ref frameHeight,    16   ), errors);
            AddIfNotNull(SetJsonValue(json, "sheetColumns",   ref sheetColumns,   1    ), errors);
            AddIfNotNull(SetJsonValue(json, "sheetRows",      ref sheetRows,      1    ), errors);
            AddIfNotNull(SetJsonValue(json, "solid",          ref solid,          false), errors);
            AddIfNotNull(SetJsonValue(json, "solidTop",       ref solidTop,       false), errors);
            AddIfNotNull(SetJsonValue(json, "frameImportant", ref frameImportant, false), errors);
            if (frameImportant)
            {
                AddIfNotNull(SetJsonValue(json, "placementFrameX", ref placementFrameX, 0), errors);
                AddIfNotNull(SetJsonValue(json, "placementFrameY", ref placementFrameY, 0), errors);
            }
            AddIfNotNull(SetJsonValue(json, "placementConditions", ref placementConditions, null), errors);
            if (String.IsNullOrEmpty(placementConditions))
                placementConditions = size[0] > 1 || size[1] > 1 ? "flatGroundSolid" : "placeTouchingSolid";
            AddIfNotNull(SetJsonValue(json, "placementOrigin", ref placementOrigin, new int[2] { 0, 0 }), errors);
            if (placementOrigin.Length == 0 || placementOrigin.Length > 2)
                errors.Add(new CompilerError(Building)
                {
                    Cause = new IndexOutOfRangeException(),
                    FilePath = json.Path,
                    IsWarning = false,
                    Message = "'placementOrigin': length must be 1 or 2."
                });
            if (placementOrigin.Length == 1)
            {
                Array.Resize(ref placementOrigin, 2);
                placementOrigin[1] = placementOrigin[0];
            }
            AddIfNotNull(SetJsonValue(json, "breaksFast",     ref breaksFast,     false), errors);
            AddIfNotNull(SetJsonValue(json, "breaksByPick",   ref breaksFast,     false), errors);
            AddIfNotNull(SetJsonValue(json, "breaksByAxe",    ref breaksByAxe,    false), errors);
            AddIfNotNull(SetJsonValue(json, "breaksByHammer", ref breaksByHammer, false), errors);
            AddIfNotNull(SetJsonValue(json, "breaksByCut",    ref breaksByCut,    false), errors);
            AddIfNotNull(SetJsonValue(json, "breaksByWater",  ref breaksByWater,  false), errors);
            AddIfNotNull(SetJsonValue(json, "breaksByLava",   ref breaksByLava,   false), errors);
            AddIfNotNull(SetJsonValue(json, "minPick",    ref minPick,    0), errors);
            AddIfNotNull(SetJsonValue(json, "minAxe",     ref minAxe,     0), errors);
            AddIfNotNull(SetJsonValue(json, "minHammer",  ref minHammer,  0), errors);
            AddIfNotNull(SetJsonValue(json, "ratePick",   ref ratePick,   0), errors);
            AddIfNotNull(SetJsonValue(json, "rateAxe",    ref rateAxe,    0), errors);
            AddIfNotNull(SetJsonValue(json, "rateHammer", ref rateHammer, 0), errors);
            AddIfNotNull(SetJsonValue(json, "table",       ref table,       false), errors);
            AddIfNotNull(SetJsonValue(json, "rope",        ref rope,        false), errors);
            AddIfNotNull(SetJsonValue(json, "noAttach",    ref noAttach,    false), errors);
            AddIfNotNull(SetJsonValue(json, "tileDungeon", ref tileDungeon, false), errors);
            AddIfNotNull(SetJsonValue(json, "blocksLight", ref blocksLight, false), errors);
            AddIfNotNull(SetJsonValue(json, "blocksSun",   ref blocksSun,   false), errors);
            AddIfNotNull(SetJsonValue(json, "glows",       ref glows,       false), errors);
            AddIfNotNull(SetJsonValue(json, "shines",      ref shines,      false), errors);
            AddIfNotNull(SetJsonValue(json, "shineChance",  ref shineChance,  0), errors);
            AddIfNotNull(SetJsonValue(json, "frame",        ref frame,        0), errors);
            AddIfNotNull(SetJsonValue(json, "frameCounter", ref frameCounter, 0), errors);
            AddIfNotNull(SetJsonValue(json, "brick",         ref brick,         false), errors);
            AddIfNotNull(SetJsonValue(json, "moss",          ref moss,          false), errors);
            AddIfNotNull(SetJsonValue(json, "stone",         ref stone,         false), errors);
            AddIfNotNull(SetJsonValue(json, "mergeDirt",     ref blocksLight,   false), errors);
            AddIfNotNull(SetJsonValue(json, "tileSand",      ref tileSand,      false), errors);
            AddIfNotNull(SetJsonValue(json, "tileFlame",     ref tileFlame,     false), errors);
            AddIfNotNull(SetJsonValue(json, "alchemyFlower", ref alchemyFlower, false), errors);
            AddIfNotNull(SetJsonValue(json, "sound",      ref sound,      0), errors);
            AddIfNotNull(SetJsonValue(json, "soundGroup", ref soundGroup, 0), errors);
            AddIfNotNull(SetJsonValue(json, "dust",       ref dust,       0), errors);

            AddIfNotNull(SetJsonValue(json, "drop", ref drop, drop), errors);

            return errors;
        }
    }
}
