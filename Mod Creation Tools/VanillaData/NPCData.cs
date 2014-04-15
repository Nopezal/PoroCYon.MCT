﻿using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using TAPI;

namespace PoroCYon.MCT.VanillaData
{
    /// <summary>
    /// Contains vanilla NPC data
    /// </summary>
    public static class NPCData
    {
        /// <summary>
        /// All vanilla NPC types as an enumeration (derives from System.Int32)
        /// </summary>
        public enum NetIDs : int
        {
            // fml not summary-ing this
#pragma warning disable 1591
            BigHornetStingy = -65,
            LittleHornetStingy = -64,
            BigHornetSpikey = -63,
            LittleHornetSpikey = -62,
            BigHornetLeafy = -61,
            LittleHornetLeafy = -60,
            BigHornetHoney = -59,
            LittleHornetHoney = -58,
            BigHornetFatty = -57,
            LittleHornetFatty = -56,
            BigRainZombie = -55,
            SmallRainZombie = -54,
            BigPantlessSkeleton = -53,
            SmallPantlessSkeleton = -52,
            BigMisassembledSkeleton = -51,
            SmallMisassembledSkeleton = -50,
            BigHeadacheSkeleton = -49,
            SmallHeadacheSkeleton = -48,
            BigSkeleton = -47,
            SmallSkeleton = -46,
            BigFemaleZombie = -45,
            SmallFemaleZombie = -44,
            DemonEye2 = -43,
            PurpleEye2 = -42,
            GreenEye2 = -41,
            DialatedEye2 = -40,
            SleepyEye2 = -39,
            CataractEye2 = -38,
            BigTwiggyZombie = -37,
            SmallTwiggyZombie = -36,
            BigSwampZombie = -35,
            SmallSwampZombie = -34,
            BigSlimedZombie = -33,
            SmallSlimedZombie = -32,
            BigPincushionZombie = -31,
            SmallPincushionZombie = -30,
            BigBaldZombie = -29,
            SmallBaldZombie = -28,
            BigZombie = -27,
            SmallZombie = -26,
            BigCrimslime = -25,
            LittleCrimslime = -24,
            BigCrimera = -23,
            LittleCrimera = -22,
            GiantMossHornet = -21,
            BigMossHornet = -20,
            LittleMossHornet = -19,
            TinyMossHornet = -18,
            BigStinger = -17,
            LittleStinger = -16,
            HeavySkeleton = -15,
            BigBoned = -14,
            ShortBones = -13,
            BigEater = -12,
            LittleEater = -11,
            JungleSlime = -10,
            YellowSlime = -9,
            RedSlime = -8,
            PurpleSlime = -7,
            BlackSlime = -6,
            BabySlime = -5,
            Pinky = -4,
            GreenSlime = -3,
            Slimer2 = -2,
            Slimeling = -1,
            BlueSlime = 1,
            DemonEye = 2,
            Zombie = 3,
            EyeofCthulhu = 4,
            ServantofCthulhu = 5,
            EaterofSouls = 6,
            DevourerHead = 7,
            DevourerBody = 8,
            DevourerTail = 9,
            GiantWormHead = 10,
            GiantWormBody = 11,
            GiantWormTail = 12,
            EaterofWorldsHead = 13,
            EaterofWorldsBody = 14,
            EaterofWorldsTail = 15,
            MotherSlime = 16,
            Merchant = 17,
            Nurse = 18,
            ArmsDealer = 19,
            Dryad = 20,
            Skeleton = 21,
            Guide = 22,
            MeteorHead = 23,
            FireImp = 24,
            BurningSphere = 25,
            GoblinPeon = 26,
            GoblinThief = 27,
            GoblinWarrior = 28,
            GoblinSorcerer = 29,
            ChaosBall = 30,
            AngryBones = 31,
            DarkCaster = 32,
            WaterSphere = 33,
            CursedSkull = 34,
            SkeletronHead = 35,
            SkeletronHand = 36,
            OldMan = 37,
            Demolitionist = 38,
            BoneSerpentHead = 39,
            BoneSerpentBody = 40,
            BoneSerpentTail = 41,
            Hornet = 42,
            ManEater = 43,
            UndeadMiner = 44,
            Tim = 45,
            Bunny = 46,
            CorruptBunny = 47,
            Harpy = 48,
            CaveBat = 49,
            KingSlime = 50,
            JungleBat = 51,
            DoctorBones = 52,
            TheGroom = 53,
            Clothier = 54,
            Goldfish = 55,
            Snatcher = 56,
            CorruptGoldfish = 57,
            Piranha = 58,
            LavaSlime = 59,
            Hellbat = 60,
            Vulture = 61,
            Demon = 62,
            BlueJellyfish = 63,
            PinkJellyfish = 64,
            Shark = 65,
            VoodooDemon = 66,
            Crab = 67,
            DungeonGuardian = 68,
            Antlion = 69,
            SpikeBall = 70,
            DungeonSlime = 71,
            BlazingWheel = 72,
            GoblinScout = 73,
            Bird = 74,
            Pixie = 75,
            ArmoredSkeleton = 77,
            Mummy = 78,
            DarkMummy = 79,
            LightMummy = 80,
            CorruptSlime = 81,
            Wraith = 82,
            CursedHammer = 83,
            EnchantedSword = 84,
            Mimic = 85,
            Unicorn = 86,
            WyvernHead = 87,
            WyvernLegs = 88,
            WyvernBody = 89,
            WyvernBody2 = 90,
            WyvernBody3 = 91,
            WyvernTail = 92,
            GiantBat = 93,
            Corruptor = 94,
            DiggerHead = 95,
            DiggerBody = 96,
            DiggerTail = 97,
            SeekerHead = 98,
            SeekerBody = 99,
            SeekerTail = 100,
            Clinger = 101,
            AnglerFish = 102,
            GreenJellyfish = 103,
            Werewolf = 104,
            BoundGoblin = 105,
            BoundWizard = 106,
            GoblinTinkerer = 107,
            Wizard = 108,
            Clown = 109,
            SkeletonArcher = 110,
            GoblinArcher = 111,
            VileSpit = 112,
            WallofFlesh = 113,
            WallofFleshEye = 114,
            TheHungry = 115,
            TheHungryII = 116,
            LeechHead = 117,
            LeechBody = 118,
            LeechTail = 119,
            ChaosElemental = 120,
            Slimer = 121,
            Gastropod = 122,
            BoundMechanic = 123,
            Mechanic = 124,
            Retinazer = 125,
            Spazmatism = 126,
            SkeletronPrime = 127,
            PrimeCannon = 128,
            PrimeSaw = 129,
            PrimeVice = 130,
            PrimeLaser = 131,
            BaldZombie = 132,
            WanderingEye = 133,
            TheDestroyer = 134,
            TheDestroyerBody = 135,
            TheDestroyerTail = 136,
            IlluminantBat = 137,
            IlluminantSlime = 138,
            Probe = 139,
            PossessedArmor = 140,
            ToxicSludge = 141,
            SantaClaus = 142,
            SnowmanGangsta = 143,
            MisterStabby = 144,
            SnowBalla = 145,
            IceSlime = 147,
            Penguin = 148,
            Penguin2 = 149,
            IceBat = 150,
            Lavabat = 151,
            GiantFlyingFox = 152,
            GiantTortoise = 153,
            IceTortoise = 154,
            Wolf = 155,
            RedDevil = 156,
            Arapaima = 157,
            Vampire = 158,
            Vampire2 = 159,
            Truffle = 160,
            ZombieEskimo = 161,
            Frankenstein = 162,
            BlackRecluse = 163,
            WallCreeper = 164,
            WallCreeper2 = 165,
            SwampThing = 166,
            UndeadViking = 167,
            CorruptPenguin = 168,
            IceElemental = 169,
            Pigron = 170,
            Pigron2 = 171,
            RuneWizard = 172,
            Crimera = 173,
            Herpling = 174,
            AngryTrapper = 175,
            MossHornet = 176,
            Derpling = 177,
            Steampunker = 178,
            CrimsonAxe = 179,
            Pigron3 = 180,
            FaceMonster = 181,
            FloatyGross = 182,
            Crimslime = 183,
            SpikedIceSlime = 184,
            SnowFlinx = 185,
            PincushionZombie = 186,
            SlimedZombie = 187,
            SwampZombie = 188,
            TwiggyZombie = 189,
            CataractEye = 190,
            SleepyEye = 191,
            DialatedEye = 192,
            GreenEye = 193,
            PurpleEye = 194,
            LostGirl = 195,
            Nymph = 196,
            ArmoredViking = 197,
            Lihzahrd = 198,
            Lihzahrd2 = 199,
            FemaleZombie = 200,
            HeadacheSkeleton = 201,
            MisassembledSkeleton = 202,
            PantlessSkeleton = 203,
            SpikedJungleSlime = 204,
            Moth = 205,
            IcyMerman = 206,
            DyeTrader = 207,
            PartyGirl = 208,
            Cyborg = 209,
            Bee = 210,
            Bee2 = 211,
            PirateDeckhand = 212,
            PirateCorsair = 213,
            PirateDeadeye = 214,
            PirateCrossbower = 215,
            PirateCaptain = 216,
            CochinealBeetle = 217,
            CyanBeetle = 218,
            LacBeetle = 219,
            SeaSnail = 220,
            Squid = 221,
            QueenBee = 222,
            Zombie2 = 223,
            FlyingFish = 224,
            UmbrellaSlime = 225,
            FlyingSnake = 226,
            Painter = 227,
            WitchDoctor = 228,
            Pirate = 229,
            Goldfish2 = 230,
            HornetFatty = 231,
            HornetHoney = 232,
            HornetLeafy = 233,
            HornetSpikey = 234,
            HornetStingy = 235,
            JungleCreeper = 236,
            JungleCreeper2 = 237,
            BlackRecluse2 = 238,
            BloodCrawler = 239,
            BloodCrawler2 = 240,
            BloodFeeder = 241,
            BloodJelly = 242,
            IceGolem = 243,
            RainbowSlime = 244,
            Golem = 245,
            GolemHead = 246,
            GolemFist = 247,
            GolemFist2 = 248,
            GolemHead2 = 249,
            AngryNimbus = 250,
            Eyezor = 251,
            Parrot = 252,
            Reaper = 253,
            Zombie3 = 254,
            Zombie4 = 255,
            FungoFish = 256,
            AnomuraFungus = 257,
            MushiLadybug = 258,
            FungiBulb = 259,
            GiantFungiBulb = 260,
            FungiSpore = 261,
            Plantera = 262,
            PlanterasHook = 263,
            PlanterasTentacle = 264,
            Spore = 265,
            BrainofCthulhu = 266,
            Creeper = 267,
            IchorSticker = 268,
            RustyArmoredBones = 269,
            RustyArmoredBones2 = 270,
            RustyArmoredBones3 = 271,
            RustyArmoredBones4 = 272,
            BlueArmoredBones = 273,
            BlueArmoredBones2 = 274,
            BlueArmoredBones3 = 275,
            BlueArmoredBones4 = 276,
            HellArmoredBones5 = 277,
            HellArmoredBones6 = 278,
            HellArmoredBones7 = 279,
            HellArmoredBones8 = 280,
            RaggedCaster = 281,
            RaggedCaster2 = 282,
            Necromancer = 283,
            Necromancer2 = 284,
            Diabolist = 285,
            Diabolist2 = 286,
            BoneLee = 287,
            DungeonSpirit = 288,
            GiantCursedSkull = 289,
            Paladin = 290,
            SkeletonSniper = 291,
            TacticalSkeleton = 292,
            SkeletonCommando = 293,
            AngryBones2 = 294,
            AngryBones3 = 295,
            AngryBones4 = 296,
            Bird2 = 297,
            Bird3 = 298,
            Squirrel = 299,
            Mouse = 300,
            Raven = 301,
            Slime = 302,
            Bunny2 = 303,
            HoppinJack = 304,
            Scarecrow = 305,
            Scarecrow2 = 306,
            Scarecrow3 = 307,
            Scarecrow4 = 308,
            Scarecrow5 = 309,
            Scarecrow6 = 310,
            Scarecrow7 = 311,
            Scarecrow8 = 312,
            Scarecrow9 = 313,
            Scarecrow10 = 314,
            HeadlessHorseman = 315,
            Ghost = 316,
            DemonEye3 = 317,
            DemonEye4 = 318,
            Zombie5 = 319,
            Zombie6 = 320,
            Zombie7 = 321,
            Skeleton2 = 322,
            Skeleton3 = 323,
            Skeleton4 = 324,
            MourningWood = 325,
            Splinterling = 326,
            Pumpking = 327,
            Pumpking2 = 328,
            Hellhound = 329,
            Poltergeist = 330,
            Zombie8 = 331,
            Zombie9 = 332,
            Slime2 = 333,
            Slime3 = 334,
            Slime4 = 335,
            Slime5 = 336,
            Bunny3 = 337,
            ZombieElf = 338,
            ZombieElf2 = 339,
            ZombieElf3 = 340,
            PresentMimic = 341,
            GingerbreadMan = 342,
            Yeti = 343,
            Everscream = 344,
            IceQueen = 345,
            SantaNK1 = 346,
            ElfCopter = 347,
            Nutcracker = 348,
            Nutcracker3 = 349,
            ElfArcher = 350,
            Krampus = 351,
            Flocko = 352,
#pragma warning restore 1591
        }

        /// <summary>
        /// The default AI codes as an Action
        /// </summary>
        public static Dictionary<int, Action<NPC, object[]>> AICode = new Dictionary<int, Action<NPC, object[]>>();
    }
}
