using System;
using System.Collections.Generic;
using System.Linq;
using Ionic.Zip;
using LitJson;
using Terraria;
using TAPI;

namespace PoroCYon.MCT.Tools.Internal.Porting
{
    static partial class WorldPorter
    {
        internal static void WriteWorld(WorldFile world)
        {
            using (ZipFile zf = new ZipFile(Environment.GetFolderPath(Environment.SpecialFolder.Personal)
                + "\\My Games\\Terraria\\tAPI\\Worlds\\" + world.name + ".wld"))
            {
                #region info.json
                JsonData info = JsonMapper.ToObject("{}");

                JsonData format = JsonMapper.ToObject("{}");
                format["release"] = API.versionAssembly;
                format["subrelease"] = API.versionSubrelease;

                info["format"] = format;

                info["id"] = world.ID;

                JsonData size = JsonMapper.ToObject("[]");

                size.Add(world.size.X);
                size.Add(world.size.Y);

                JsonData borders = JsonMapper.ToObject("{}");

                borders["left"] = (int)world.bounds.X;
                borders["right"] = (int)world.bounds.Y;
                borders["up"] = (int)world.bounds.Z;
                borders["down"] = (int)world.bounds.W;

                info["borders"] = borders;

                JsonData styles = JsonMapper.ToObject("{}");

                JsonData tree = JsonMapper.ToObject("[]");

                for (int i = 0; i < world.treeStyle.Length; i++)
                    tree.Add(world.treeStyle[i]);

                styles["tree"] = tree;

                JsonData caveBack = JsonMapper.ToObject("[]");

                for (int i = 0; i < world.caveBackStyle.Length; i++)
                    caveBack.Add(world.caveBackStyle[i]);

                styles["caveBack"] = caveBack;

                styles["iceBack"] = world.iceBackStyle;
                styles["jungleBack"] = world.jungleBackStyle;
                styles["hellBack"] = world.hellBackStyle;

                info["styles"] = styles;

                JsonData treeX = JsonMapper.ToObject("[]");

                for (int i = 0; i < world.treeX.Length; i++)
                    treeX.Add(world.treeX[i]);

                info["treeX"] = treeX;

                JsonData caveBackX = JsonMapper.ToObject("[]");

                for (int i = 0; i < world.caveBackX.Length; i++)
                    caveBackX.Add(world.caveBackX[i]);

                info["caveBackX"] = caveBackX;

                JsonData spawn = JsonMapper.ToObject("[]");

                spawn.Add(world.spawn.X);
                spawn.Add(world.spawn.Y);

                info["spawn"] = spawn;

                JsonData layers = JsonMapper.ToObject("{}");

                layers["worldSurface"] = world.surface;
                layers["rock"] = world.rockLayer;
                layers["hell"] = world.size.Y - 200d;

                info["layers"] = layers;

                JsonData time = JsonMapper.ToObject("{}");

                time["day"] = world.day;
                time["time"] = world.time;

                JsonData moon = JsonMapper.ToObject("{}");

                moon["type"] = world.moonType;
                moon["phase"] = world.moonPhase;

                time["moon"] = moon;

                time["bloodMoon"] = world.bloodMoon;
                time["eclipse"] = world.eclipse;

                info["time"] = time;

                JsonData dungeon = JsonMapper.ToObject("[]");

                dungeon.Add(world.dungeon.X);
                dungeon.Add(world.dungeon.Y);

                info["dungeon"] = dungeon;

                info["crimson"] = world.crimson;
                info["corruption"] = !world.crimson;

                JsonData progress = JsonMapper.ToObject("{}");

                JsonData bosses = JsonMapper.ToObject("{}");

                bosses["Eye of Ctulhu"] = world.DefeatedEoC;
                bosses["Eater of Worlds"] = world.DefeatedEoW;
                bosses["Skeletron"] = world.DefeatedSkeletron;
                bosses["Queen Bee"] = world.DefeatedQueenBee;

                bosses["The Destroyer"] = world.DefeatedDestroyer;
                bosses["The Twins"] = world.DefeatedTwins;
                bosses["Skeletron Prime"] = world.DefeatedSkeletronPrime;

                bosses["Plantera"] = world.DefeatedPlantera;
                bosses["Golem"] = world.DefeatedGolem;

                progress["bosses"] = bosses;

                JsonData townNPCs = JsonMapper.ToObject("{}");

                townNPCs["Goblin Tinkerer"] = world.SavedGoblin;
                townNPCs["Wizard"] = world.SavedWizard;
                townNPCs["Mechanic"] = world.SavedMechanic;

                progress["npcs"] = townNPCs;

                JsonData invasions = JsonMapper.ToObject("{}");

                JsonData currentInvasion = JsonMapper.ToObject("{}");

                currentInvasion["delay"] = world.invasionDelay;
                currentInvasion["size"] = world.invasionSize;
                currentInvasion["type"] = world.invasionType;
                currentInvasion["x"] = world.invasionX;

                invasions["current"] = currentInvasion;

                invasions["Goblin Army"] = world.DefeatedGoblins;
                invasions["Frost Legion"] = world.DefeatedFrostLegion;
                invasions["Pirates"] = world.DefeatedPirates;

                progress["invasions"] = invasions;

                JsonData shadowOrb = JsonMapper.ToObject("{}");

                shadowOrb["smashed"] = world.SmashedShadowOrb;
                shadowOrb["counter"] = world.shadowOrbsSmashed;

                progress["shadowOrb"] = shadowOrb;

                progress["spawnMeteor"] = world.MeteorHasLanded;
                progress["hardmode"] = world.Hardmode;
                progress["altarCounter"] = world.altarsSmashed;

                info["progress"] = progress;

                JsonData sky = JsonMapper.ToObject("{}");

                sky["raining"] = world.Raining;
                sky["rainTime"] = world.rainTime;
                sky["maxRain"] = world.maxRain;
                sky["cloudBGActive"] = world.cloudBgActive;
                sky["clouds"] = world.cloudAmt;
                sky["windSpeed"] = world.windSpeed;

                info["sky"] = sky;

                JsonData hmOres = JsonMapper.ToObject("[]");

                hmOres.Add(world.ore1Type);
                hmOres.Add(world.ore2Type);
                hmOres.Add(world.ore3Type);

                info["hardmodeOres"] = hmOres;

                JsonData bgs = JsonMapper.ToObject("{}");

                bgs["tree"] = WorldGen.treeBG;
                bgs["corrupt"] = WorldGen.corruptBG;
                bgs["jungle"] = WorldGen.jungleBG;
                bgs["snow"] = WorldGen.snowBG;
                bgs["hallow"] = WorldGen.hallowBG;
                bgs["crimson"] = WorldGen.crimsonBG;
                bgs["desert"] = WorldGen.desertBG;
                bgs["ocean"] = WorldGen.oceanBG;

                info["bgs"] = bgs;

                zf.AddEntry("Info.json", JsonMapper.ToJson(info));
                #endregion

                BinBuffer bb = new BinBuffer();

                #region tiles.dat
                for (int y = 0; y < world.size.Y; y++)
                    for (int x = 0; x < world.size.X; x++)
                    {
                        Tile t = world.tiles[x, y];

                        bb.Write(t.Active || t.type > Main.maxTileSets ? t.type + 1 : 0);
                        bb.Write(t.wall);
                        bb.Write(t.frameX);
                        bb.Write(t.frameY);
                        bb.Write(t.Colour);
                        bb.Write(t.WallColour);
                        bb.Write(t.LiquidType);
                        bb.Write(t.liquid);
                        bb.Write(new BitsByte(t.Wire, t.Wire2, t.Wire3, t.Actuator, t.Inactive, t.HalfBrick, (t.bb3 & 16) == 16, (t.bb3 & 32) == 32));
                    }

                bb.Pos = 0;
                zf.AddEntry("Tiles.dat", bb.ReadBytes());
                #endregion

                bb.Clear();

                #region chests.dat

                for (int i = 0; i < world.chests.Length; i++)
                {
                    Chest c = world.chests[i];

                    if (c == null || c.items.All(it => it.netID == 0 || it.stack <= 0))
                        bb.Write(false);
                    else
                    {
                        bb.Write(true);
                        bb.WriteX(c.position.X, c.position.Y);

                        for (int j = 0; j < c.items.Length; i++)
                        {
                            if (c.items[i] == null)
                                c.items[i] = new Item();

                            PlayerPorter.WriteItem(bb, c.items[i]);
                        }
                    }
                }

                bb.Pos = 0;
                zf.AddEntry("Chests.dat", bb.ReadBytes());
                #endregion

                bb.Clear();

                #region itemmappings.dat
                bb.Write(0);
                bb.Pos = 0;
                zf.AddEntry("ItemMappings.dat", bb.ReadBytes());
                #endregion

                bb.Clear();

                #region signs.dat
                for (int i = 0; i < world.signs.Length; i++)
                {
                    if (world.signs[i] == null || world.signs[i].text == null ||
                            (String.IsNullOrEmpty(world.signs[i].text) && world.signs[i].position.X == 0 && world.signs[i].position.Y == 0))
                        bb.Write(false);
                    else
                    {
                        bb.Write(true);

                        bb.WriteX(world.signs[i].position.X, world.signs[i].position.Y);

                        bb.WriteX(world.signs[i].text == null ? String.Empty : world.signs[i].text);
                    }
                }
                #endregion

                bb.Clear();

                #region npcs.dat
                bb.Write(world.townNPCs.Count);
                for (int i = 0; i < world.townNPCs.Count; i++)
                {
                    TownNPC n = world.townNPCs[i];

                    bb.Write(n.occupation);
                    bb.Write(n.position);
                    bb.Write(n.homeless);
                    bb.WriteX(n.homeTile.X, n.homeTile.Y);
                }

                bb.Write(world.townNPCs.Count);
                for (int i = 0; i < world.townNPCs.Count; i++)
                {
                    TownNPC n = world.townNPCs[i];

                    bb.Write(n.type > Main.maxNPCTypes ? 0 : n.type);
                    bb.Write(n.name);
                }
                #endregion

                //bb.Clear();

                #region tilecodes.dat
                zf.AddEntry("TileCodes.dat", new byte[0]);
                #endregion

                zf.Save();
            }
        }
    }
}
