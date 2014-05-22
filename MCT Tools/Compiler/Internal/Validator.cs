using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TAPI;
using PoroCYon.MCT.Tools.Internal.Validation;
using PoroCYon.MCT.Tools.Internal.Validation.Entities;

namespace PoroCYon.MCT.Tools.Internal
{
    using ModInfo = Validation.ModInfo;

    class ModData
    {
        internal ModInfo info;
        internal ModOptions options;
        internal CraftGroups craftGroups;

        internal List<Item> items = new List<Item>();
        internal List<NPC> npcs = new List<NPC>();
        internal List<Projectile> projs = new List<Projectile>();
        internal List<Tile> tiles = new List<Tile>();
        internal List<Wall> walls = new List<Wall>();

        internal List<JsonFile> jsons = new List<JsonFile>();
        internal Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();
    }

    static class Validator
    {
        internal static Dictionary<string, string> modDict;
        internal static ModData current;

        internal static List<CompilerError> ValidateJsons(List<JsonFile> jsons, Dictionary<string, byte[]> files, bool validateModInfo = true)
        {
            current = new ModData();

            current.jsons = jsons;
            current.files = files;

            modDict = Mods.GetInternalNameToPathDictionary(); // dat name

            List<CompilerError> errors = new List<CompilerError>();

            JsonFile
                modInfoJson     = jsons[0],
                modOptionsJson  = jsons[1],
                craftGroupsJson = jsons[2];

            current.info = new ModInfo();
            errors.AddRange(current.info.CreateAndValidate(modInfoJson));

            if (!current.info.validate) // HELLO HERE I AM, I JUST WANTED TO SAY THAT THIS BLOCK CONTAINS A RETURN STATEMENT. KTHXBAI.
                return errors;

            current.options = new ModOptions();
            if (modOptionsJson != null)
                errors.AddRange(current.options.CreateAndValidate(modOptionsJson));

            current.craftGroups = new CraftGroups();
            if (craftGroupsJson != null)
                errors.AddRange(current.craftGroups.CreateAndValidate(modOptionsJson));

            for (int i = 3; i < errors.Count; i++)
            {
                string path = jsons[i].path;
                int index = path.IndexOf(Path.DirectorySeparatorChar);

                if (index != -1)
                {
                    ValidatorObject obj;

                    switch (path.Remove(index).ToLowerInvariant())
                    {
                        case "item":
                            obj = new Item();
                            break;
                        case "npc":
                            obj = new NPC();
                            break;
                        case "projectile":
                            obj = new Projectile();
                            break;
                        case "tile":
                            obj = new Tile();
                            break;
                        case "wall":
                            obj = new Wall();
                            break;

                        default:
                            errors.Add(new CompilerError()
                            {
                                Cause = new CompilerWarning(),
                                FilePath = jsons[i].path,
                                IsWarning = true,
                                Message = "Unrecognised file '" + jsons[i].path + "'. Are you sure it is in the right folder?"
                            });

                            obj = null;
                            break;
                    }

                    if (obj != null)
                    {
                        errors.AddRange(obj.CreateAndValidate(jsons[i])); // ACTUAL VALIDATION

                        // I'm too lazy to type casts today
                        if (obj is Item)
                            current.items.Add(obj as Item);
                        if (obj is NPC)
                            current.npcs.Add(obj as NPC);
                        if (obj is Projectile)
                            current.projs.Add(obj as Projectile);
                        if (obj is Tile)
                            current.tiles.Add(obj as Tile);
                        if (obj is Wall)
                            current.walls.Add(obj as Wall);
                    }
                }
            }

            return errors;
        }
    }
}
