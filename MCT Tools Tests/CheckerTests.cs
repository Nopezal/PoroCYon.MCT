using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TAPI;
using PoroCYon.MCT.Tools.Compiler;
using PoroCYon.MCT.Tools.Compiler.Validation;
using PoroCYon.MCT.Tools.Compiler.Validation.Entities;
using PoroCYon.MCT.Tools.Internal.Compiler;
using PoroCYon.MCT.Tools.ModCompiler;

namespace PoroCYon.MCT.Tools.Tests
{
    using ModInfo = Compiler.Validation.ModInfo;

    public sealed class Mod : ModBase { }

    [TestClass]
    public class CheckerTests
    {
        static ModData CreateMod()
        {
            ModData md = new ModData();

            md.Assembly = Assembly.GetExecutingAssembly();
            md.Info = new ModInfo();
            md.Info.author = "PoroCYon";
            md.Info.displayName = "Test Mod";
            md.Info.includePDB = true;
            md.Info.internalName = "TestMod";
            md.Info.info = "Mod meant to test the MCT Tools";
            md.OriginName = "TestMod";
            md.OriginPath = Directory.GetCurrentDirectory() + "\\TestMod";

            md.items.Add(new Item()
            {
                internalName = "TestMod:TestItem",
                displayName = "Test Item",
                maxStack = 999,
                rare = 2,
                recipes = new List<Recipe>()
                {
                    new Recipe()
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

            return md;
        }

        [TestMethod]
        public void TestLoadDefs()
        {
            Checker.LoadDefs();
        }

        [TestMethod]
        public void TestCheckItem()
        {
            current = CreateMod();

            Assert.IsTrue(CreateOutput(Checker.Check().ToList()).Succeeded);
        }
    }
}
