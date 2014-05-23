using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PoroCYon.MCT.Tools.Validation;
using PoroCYon.MCT.Tools.Validation.Entities;

namespace PoroCYon.MCT.Tools.Internal
{
    static class Validator
    {
        internal static List<CompilerError> ValidateJsons(List<JsonFile> jsons, Dictionary<string, byte[]> files, bool validateModInfo = true)
        {
            ModCompiler.current = new ModData();

            ModCompiler.current.jsons = jsons;
            ModCompiler.current.files = files;

            List<CompilerError> errors = new List<CompilerError>();

            JsonFile
                modInfoJson     = jsons[0],
                modOptionsJson  = jsons[1],
                craftGroupsJson = jsons[2];

            ModCompiler.current.Info = new ModInfo();
            errors.AddRange(ModCompiler.current.Info.CreateAndValidate(modInfoJson));

            if (!ModCompiler.current.Info.validate) // HELLO, HERE AM I, I JUST WANTED TO SAY THAT THIS BLOCK CONTAINS A RETURN STATEMENT, KTHXBAI.
                return errors;

            ModCompiler.current.Options = new ModOptions();
            if (modOptionsJson != null)
                errors.AddRange(ModCompiler.current.Options.CreateAndValidate(modOptionsJson));

            ModCompiler.current.CraftGroups = new CraftGroups();
            if (craftGroupsJson != null)
                errors.AddRange(ModCompiler.current.CraftGroups.CreateAndValidate(modOptionsJson));

            for (int i = 3; i < errors.Count; i++)
            {
                string path = jsons[i].Path;
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
                                FilePath = jsons[i].Path,
                                IsWarning = true,
                                Message = "Unrecognised file '" + jsons[i].Path + "'. Are you sure it is in the right folder?"
                            });

                            obj = null;
                            break;
                    }

                    if (obj != null)
                    {
                        errors.AddRange(obj.CreateAndValidate(jsons[i])); // ACTUAL VALIDATION

                        // I'm too lazy to type casts today
                        if (obj is Item)
                            ModCompiler.current.items.Add(obj as Item);
                        if (obj is NPC)
                            ModCompiler.current.npcs.Add(obj as NPC);
                        if (obj is Projectile)
                            ModCompiler.current.projs.Add(obj as Projectile);
                        if (obj is Tile)
                            ModCompiler.current.tiles.Add(obj as Tile);
                        if (obj is Wall)
                            ModCompiler.current.walls.Add(obj as Wall);
                    }
                }
            }

            return errors;
        }
    }
}
