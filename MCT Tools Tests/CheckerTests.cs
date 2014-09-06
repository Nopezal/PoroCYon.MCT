using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TAPI;
using PoroCYon.MCT.Tools.Compiler;
using PoroCYon.MCT.Tools.Compiler.Validation.Entities;
using PoroCYon.MCT.Tools.Internal.Compiler;

namespace PoroCYon.MCT.Tools.Tests
{
    using ModInfo = Compiler.Validation.ModInfo;

    public sealed class Mod : ModBase { }

    [TestClass]
    public class CheckerTests
    {
        static ModCompiler CreateMod()
        {
            ModCompiler mc = new ModCompiler();

            ModData md = new ModData(mc);

            md.Assembly = Assembly.GetExecutingAssembly();
            md.Info = new ModInfo(mc);
            md.Info.author = "PoroCYon";
            md.Info.displayName = "Test Mod";
            md.Info.includePDB = true;
            md.Info.internalName = "TestMod";
            md.Info.info = "Mod meant to test the MCT Tools";
            md.OriginName = "TestMod";
            md.OriginPath = Directory.GetCurrentDirectory() + "\\TestMod";

            md.items.Add(new Item(mc)
            {
                internalName = "TestMod:TestItem",
                displayName = "Test Item",
                maxStack = 999,
                rare = 2,
                recipes = new List<Recipe>()
                {
                    new Recipe(mc)
                    {
                        creates = 3,
                        items = new Dictionary<string, int>()
                        {
                            { "g:Wood"          , 5 },
                            { "TestMod:TestItem", 1 }
                        },
                        tiles = new List<string>()
                        {
                            "Work Bench"
                        }
                    }
                },
                value = 50
            });

            return mc;
        }

        [TestMethod]
        public void TestLoadDefs()
        {
            Checker.LoadDefs();
        }

        [TestMethod]
        public void TestCheckItem()
        {
            var mc = CreateMod();

            Assert.IsTrue(mc.CreateOutput(new Checker(mc).Check().ToList()).Succeeded);
        }
    }
}
