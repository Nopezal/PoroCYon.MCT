using System;
using System.Collections.Generic;
using System.Linq;

namespace PoroCYon.MCT.Tools.Internal.Validation.Entities
{
    class Wall : EntityValidator
    {
        public bool house = true;
        public bool dungeon = false;
        public bool light = false;

        public int blend = 0;
        public int sound = 0;
        public int soundGroup = 0;
        public int dust = 0;

        public string drop = String.Empty;

        internal override List<CompilerError> CreateAndValidate(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            errors.AddRange(CreateAndValidateBase(json, "Wall", "Walls"));

            AddIfNotNull(SetJsonValue(json, "house",   ref house,   true ), errors);
            AddIfNotNull(SetJsonValue(json, "dungeon", ref dungeon, false), errors);
            AddIfNotNull(SetJsonValue(json, "light",   ref light,   false), errors);

            AddIfNotNull(SetJsonValue(json, "blend",      ref blend,      0), errors);
            AddIfNotNull(SetJsonValue(json, "sound",      ref sound,      0), errors);
            AddIfNotNull(SetJsonValue(json, "soundGroup", ref soundGroup, 0), errors);
            AddIfNotNull(SetJsonValue(json, "dust",       ref dust,       0), errors);

            AddIfNotNull(SetJsonValue(json, "drop", ref drop, String.Empty), errors);

            return errors;
        }
    }
    class Tile : EntityValidator
    {
        public string displayName;
        public int frameWidth = 16;
        public int frameHeight = 16;
        public int sheetColumns = 1;
        public int sheetRows = 1;
        public bool solid = false;
        public bool solidTop = false;
        public bool frameImportant = false;
        public int placementFrameX = 0;
        public int placementFrameY = 0;
        public string placementConditions = "placeTouchingSolid"; // TileDef.placementConditions[num] = ((TileDef.width[num] > 1 || TileDef.height[num] > 1) ? "flatGroundSolid" : "placeTouchingSolid");
        public int[] placementOrigin;
        public bool breaksFast, breaskByPick, breaksByAxe, breaksByHammer, breaksByCut, breaksByWater, breaksByLava;
        public int minPick, minAxe, minHammer;
        public float ratePick, rateAxe, rateHammer;
        public bool tabmle, rope, noAttach, tileDungeon, blocksLight, blocksSun, globas, shines;
        public int shineChance, frame, frameCounter;
        public int brick, moss, stone, mergeDirt, tileSand, tileFlame, alchemyFlower;
        public int sound, soundGroup, dust;
        public object drop; // int or string

        internal override List<CompilerError> CreateAndValidate(JsonFile json)
        {
            List<CompilerError> errors = new List<CompilerError>();

            errors.AddRange(CreateAndValidateBase(json, "Tile", "Tiles"));

            // see void TAPI.Defs.LoadTiles(ModBase)
            //     property5 is bool
            //     property2 is int
            //     property3 is float

            return errors;
        }
    }
}
