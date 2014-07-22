using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Ionic.Zip;
using LitJson;
using Terraria;
using TAPI;

namespace PoroCYon.MCT.Tools.Internal.Porting
{
    enum LoadError
    {
        Success,

        InvalidVersion,
        InvalidChecksum,

        InvalidBufferLength = 5,
        InvalidBufferChecksum
    }

    static partial class WorldPorter
    {
        static Random backupRand = new Random();

        static string ToReadableString(LoadError err)
        {
            switch (err)
            {
                case LoadError.Success:
                    return "Success";
                case LoadError.InvalidVersion:
                    return "Invalid version";
                case LoadError.InvalidChecksum:
                    return "Invalid checksum";
                case LoadError.InvalidBufferLength:
                    return "Invalid buffer length";
                case LoadError.InvalidBufferChecksum:
                    return "Invalid buffer checksum";
            }

            throw new ArgumentOutOfRangeException("err");
        }

        internal static WorldFile ReadWorld(string path)
        {
            WorldFile ret = new WorldFile();

            BinBuffer bb = new BinBuffer(new BinBufferByte(File.ReadAllBytes(path)));

            int ver = ret.version = bb.ReadInt();

            LoadError err;
            if ((err = (ver <= 87 ? ReadV1(ref ret, bb) : ReadV2(ref ret, bb))) != LoadError.Success)
                throw new FormatException("An error occured when reading the world file: " + ToReadableString(err));

            return ret;
        }
    }
}
