using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace PoroCYon.MCT.Tools.Internal.Porting
{
    enum Difficulty : byte
    {
        Softcore,
        Mediumcore,
        Hardcore
    }
    enum Gender : byte
    {
        Female,
        Male
    }

    class Item
    {
        public int  netID;
        public byte prefix;
        public int  stack = 1;
    }

    class PlayerFile
    {
        public int version;
        public string name;
        public Difficulty difficulty;
        public int hair;
        public byte hairDye;
        public byte hideVisual;
        public Gender gender;
        public int life, lifeMax;
        public int mana, manaMax;
        public Color
            hairColour,
            skinColour,
            eyeColour,
            shirtColour,
            undershirtColour,
            pantsColour,
            shoeColour;

        public Item[]
            inventory = new Item[59],
            armour    = new Item[16],
            dye       = new Item[3 ],
            piggyBank = new Item[40],
            safe      = new Item[40];

        public int[]
            buffType = new int[22],
            buffTime = new int[22];

        public int[]
            spX = new int[200],
            spY = new int[200],
            spI = new int[200];
        public string[] spN = new string[200];

        public bool hbLocked;
        public int anglerQuestsFinished;

        public PlayerFile()
        {
            for (int i = 0; i < 58; i++)
            {
                inventory[i] = new Item();

                if (i < 16)
                    armour[i] = new Item();
                if (i < 3)
                    dye[i] = new Item();
                if (i < 40)
                {
                    piggyBank[i] = new Item();
                    safe[i] = new Item();
                }
            }
        }
    }
}
