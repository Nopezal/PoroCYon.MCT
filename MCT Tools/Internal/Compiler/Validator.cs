using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PoroCYon.MCT.Tools.Compiler;
using PoroCYon.MCT.Tools.Compiler.Validation;
using PoroCYon.MCT.Tools.Compiler.Validation.Entities;

namespace PoroCYon.MCT.Tools.Internal.Compiler
{
    class Validator(ModCompiler mc) : CompilerPhase(mc)
    {
        internal List<CompilerError> ValidateJsons(List<JsonFile> jsons, Dictionary<string, byte[]> files, bool validateModInfo = true)
        {
            Building.jsons = jsons;
            Building.files = files;

            List<CompilerError> errors = new List<CompilerError>();

            JsonFile
                modInfoJson     = jsons[0],
                modOptionsJson  = jsons[1],
                craftGroupsJson = jsons[2];

            Building.Info = new ModInfo(Compiler);
            errors.AddRange(Building.Info.CreateAndValidate(modInfoJson));

            if (!Building.Info.validate) // HELLO, HERE AM I, I JUST WANTED TO SAY THAT THIS BLOCK CONTAINS A RETURN STATEMENT, KTHXBAI.
                return errors;

            Building.Options = new ModOptions(Compiler);
            if (modOptionsJson != null)
                errors.AddRange(Building.Options.CreateAndValidate(modOptionsJson));

            Building.CraftGroups = new CraftGroups(Compiler);
            if (craftGroupsJson != null)
                errors.AddRange(Building.CraftGroups.CreateAndValidate(modOptionsJson));

            for (int i = 3; i < errors.Count; i++)
            {
                string path = jsons[i].Path;
                int index = path.IndexOf(Path.DirectorySeparatorChar);

                if (index != -1)
                {
                    ValidatorObject obj = null;

                    switch (path.Remove(index).ToLowerInvariant())
                    {
                        case "buff":
                            obj = new Buff      (Compiler);
                            break;
                        case "item":
                            obj = new Item      (Compiler);
                            break;
                        case "npc":
                            obj = new NPC       (Compiler);
                            break;
                        case "prefix":
                            obj = new Prefix    (Compiler);
                            break;
                        case "projectile":
                            obj = new Projectile(Compiler);
                            break;
                        case "tile":
                            obj = new Tile      (Compiler);
                            break;
                        case "wall":
                            obj = new Wall      (Compiler);
                            break;

                        default:
                            errors.Add(new CompilerError()
                            {
                                Cause = new CompilerWarning(),
                                FilePath = jsons[i].Path,
                                IsWarning = true,
                                Message = "Unrecognised file '" + jsons[i].Path + "'. Are you sure it is in the right folder?"
                            });
                            break;
                    }

                    if (obj != null)
                    {
                        errors.AddRange(obj.CreateAndValidate(jsons[i])); // ACTUAL VALIDATION

                        // I'm too lazy to type casts today
                        if (obj is Buff)
                            Building.buffs.Add (obj as Buff);
                        if (obj is Item)
                            Building.items.Add (obj as Item);
                        if (obj is NPC)
                            Building.npcs.Add  (obj as NPC);
                        if (obj is Prefix)
                            Building.pfixes.Add(obj as Prefix);
                        if (obj is Projectile)
                            Building.projs.Add (obj as Projectile);
                        if (obj is Tile)
                            Building.tiles.Add (obj as Tile);
                        if (obj is Wall)
                            Building.walls.Add (obj as Wall);
                    }
                }
            }

            return errors;
        }
    }
}
