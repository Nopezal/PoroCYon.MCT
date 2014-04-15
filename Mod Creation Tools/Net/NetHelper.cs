using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using Terraria;
using TAPI;

namespace PoroCYon.MCT.Net
{
    /// <summary>
    /// The values Main.netMode can have.
    /// </summary>
    public enum NetMode : int
    {
        /// <summary>
        /// The game is in singleplayer mode, or is in the menu
        /// </summary>
        Singleplayer = 0,

        /// <summary>
        /// The game is in multiplayer mode and the user joined a server
        /// </summary>
        Multiplayer_Client = 1,
        /// <summary>
        /// The game is in multiplayer mode an the user is hosting a server, or the game is running as the dedicated server
        /// </summary>
        Multiplayer_Server = 2
    }

    /// <summary>
    /// Provides helper functions for sending NetMessages
    /// </summary>
    public static class NetHelper
    {
        /// <summary>
        /// Gets the current net mode as a <see cref="PoroCYon.MCT.Net.NetMode" />
        /// </summary>
        public static NetMode CurrentMode
        {
            get
            {
                return (NetMode)Main.netMode;
            }
        }

        /// <summary>
        /// Sends data as a managed object
        /// </summary>
        /// <param name="name">The name of the mod which is sending a message</param>
        /// <param name="message">The message ID</param>
        /// <param name="toSend">The objects to send</param>
        public static void SendModData(string name, Enum message, params object[] toSend)
        {
            SendModData(name, message, -1, -1, toSend);
        }
        /// <summary>
        /// Sends data as a managed object
        /// </summary>
        /// <param name="name">The name of the mod which is sending a message</param>
        /// <param name="message">The message ID</param>
        /// <param name="remoteClient">The client ID where the message should be sent to if Main.netMode equals 2</param>
        /// <param name="ignoreClient">The client ID where the message should not be sent to if Main.netMode equals 2</param>
        /// <param name="toSend">The objects to send</param>
        public static void SendModData(string name, Enum message, int remoteClient, int ignoreClient, params object[] toSend)
        {
            if (Main.netMode == 0)
				return;

			int num = 256;
			if (Main.netMode == 2 && remoteClient >= 0)
				num = remoteClient;

            lock (NetMessage.buffer[num])
            {
                BinBuffer bb = new BinBuffer(new BinBufferByte(NetMessage.buffer[num].writeBuffer, false));
                bb.Pos = 4; //for size
                bb.WriteX((byte)100, (byte)Mods.modBases.FindIndex(0, Mods.modBases.Count, (b) => { return b.modName == name; }), (byte)Convert.ToInt32(message));

                // write stuff here

                for (int i = 0; i < toSend.Length; i++)
                {
                    Type t = toSend[i].GetType();

                    #region primitives
                    if (t == typeof(byte))
                        bb.Write((byte)toSend[i]);
                    if (t == typeof(sbyte))
                        bb.Write((sbyte)toSend[i]);

                    if (t == typeof(ushort))
                        bb.Write((ushort)toSend[i]);
                    if (t == typeof(short))
                        bb.Write((short)toSend[i]);

                    if (t == typeof(int))
                        bb.Write((int)toSend[i]);
                    if (t == typeof(uint))
                        bb.Write((uint)toSend[i]);

                    if (t == typeof(long))
                        bb.Write((long)toSend[i]);
                    if (t == typeof(ulong))
                        bb.Write((ulong)toSend[i]);

                    if (t == typeof(float))
                        bb.Write((float)toSend[i]);
                    if (t == typeof(double))
                        bb.Write((double)toSend[i]);
                    if (t == typeof(decimal))
                        bb.Write((decimal)toSend[i]);

                    if (t == typeof(DateTime))
                        bb.Write((DateTime)toSend[i]);
                    if (t == typeof(TimeSpan))
                        bb.Write((TimeSpan)toSend[i]);

                    if (t == typeof(BigInteger))
                        bb.Write((BigInteger)toSend[i]);
                    if (t == typeof(Complex))
                        bb.Write((Complex)toSend[i]);

                    if (t == typeof(MemoryStream))
                    {
                        bb.Write(((MemoryStream)toSend[i]).ToArray().Length);
                        bb.Write(((MemoryStream)toSend[i]).ToArray());
                    }
                    if (t == typeof(BinBuffer))
                    {
                        bb.Write(((BinBuffer)toSend[i]).BytesLeft());
                        bb.Write(((BinBuffer)toSend[i]));
                    }
                    if (t == typeof(BinBufferBuffer))
                    {
                        bb.Write(((BinBufferBuffer)toSend[i]).BytesLeft());
                        bb.Write((new BinBuffer((BinBufferBuffer)toSend[i])));
                    }

                    if (t == typeof(Vector2))
                        bb.Write((Vector2)toSend[i]);
                    if (t == typeof(Color))
                        bb.Write((Color)toSend[i]);
                    if (t == typeof(Item))
                        bb.Write((Item)toSend[i]);
                    #endregion

                    #region value type -> can read from memory
                    if (t.IsValueType && (t.IsExplicitLayout || t.IsLayoutSequential))
                    {
                        IntPtr ptr = IntPtr.Zero;
                        Marshal.StructureToPtr(toSend[i], ptr, false);
                        int size = Marshal.SizeOf(toSend[i]);

                        bb.Write(size);

                        for (IntPtr addr = ptr; addr.ToInt64() < ptr.ToInt64() + size; addr += 1) // read byte per byte
                            bb.Write(Marshal.ReadByte(addr));
                    }
                    #endregion

                    #region serilizable -> can use binaryformatter
                    if (t.IsSerializable)
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        MemoryStream ms = new MemoryStream();
                        bf.Serialize(ms, toSend[i]);
                        bb.Write(ms.ToArray().Length);
                        bb.Write(ms.ToArray());
                        ms.Close();
                    }
                    #endregion
                }

                #region send some other stuff
                int pos = bb.Pos;
                bb.Pos = 0;
                bb.Write((int)(pos - 4));
                bb.Pos = pos;

                if (Main.netMode == 1)
                    if (!Netplay.clientSock.tcpClient.Connected)
                        goto End;

                    try
                    {
                        NetMessage.buffer[num].spamCount++;
                        Main.txMsg++;
                        Main.txData += pos;
                        Main.txMsgType[100]++;
                        Main.txDataType[100] += pos;
                        Netplay.clientSock.networkStream.BeginWrite(NetMessage.buffer[num].writeBuffer, 0, pos, new AsyncCallback(Netplay.clientSock.ClientWriteCallBack), Netplay.clientSock.networkStream);
                    }
                    catch
                    {
                        goto End;
                    }

                if (remoteClient == -1)
                    for (int i = 0; i < 256; i++)
                        if (i != ignoreClient && NetMessage.buffer[i].broadcast && Netplay.serverSock[i].tcpClient.Connected)
                            try
                            {
                                NetMessage.buffer[i].spamCount++;
                                Main.txMsg++;
                                Main.txData += pos;
                                Main.txMsgType[100]++;
                                Main.txDataType[100] += pos;
                                Netplay.serverSock[i].networkStream.BeginWrite(NetMessage.buffer[num].writeBuffer, 0, pos, new AsyncCallback(Netplay.serverSock[i].ServerWriteCallBack), Netplay.serverSock[i].networkStream);
                            }
                            catch (Exception) { }

                else if (Netplay.serverSock[remoteClient].tcpClient.Connected)
                    try
                    {
                        NetMessage.buffer[remoteClient].spamCount++;
                        Main.txMsg++;
                        Main.txData += pos;
                        Main.txMsgType[100]++;
                        Main.txDataType[100] += pos;
                        Netplay.serverSock[remoteClient].networkStream.BeginWrite(NetMessage.buffer[num].writeBuffer, 0, pos, new AsyncCallback(Netplay.serverSock[remoteClient].ServerWriteCallBack), Netplay.serverSock[remoteClient].networkStream);
                    }
                    catch (Exception) { }

            End:
                NetMessage.buffer[num].writeLocked = false;
                #endregion
            }
        }

        /// <summary>
        /// Retrieves a managed object sent by <see cref="PoroCYon.MCT.Net.NetHelper"/>.SendModData
        /// </summary>
        /// <param name="t">The type of the sent object to read. It must have a parameterless constructor defined.</param>
        /// <param name="bb">The data that is received from the network</param>
        /// <returns>The data as a managed object</returns>
        public static object ReadObject(Type t, BinBuffer bb)
        {
            object ret = Activator.CreateInstance(t);

            #region primitives
            if (t == typeof(byte))
                ret = bb.ReadByte();
            if (t == typeof(sbyte))
                ret = bb.ReadSByte();

            if (t == typeof(ushort))
                ret = bb.ReadUShort();
            if (t == typeof(short))
                ret = bb.ReadShort();

            if (t == typeof(int))
                ret = bb.ReadInt();
            if (t == typeof(uint))
                ret = bb.ReadUInt();

            if (t == typeof(long))
                ret = bb.ReadLong();
            if (t == typeof(ulong))
                ret = bb.ReadULong();

            if (t == typeof(float))
                ret = bb.ReadFloat();
            if (t == typeof(double))
                ret = bb.ReadDouble();
            if (t == typeof(decimal))
                ret = bb.ReadDecimal();

            if (t == typeof(DateTime))
                ret = bb.ReadDateTime();
            if (t == typeof(TimeSpan))
                ret = bb.ReadTimeSpan();

            if (t == typeof(BigInteger))
                ret = bb.ReadBigInt();
            if (t == typeof(Complex))
                ret = bb.ReadComplex();

            if (t == typeof(MemoryStream))
                ret = new MemoryStream(bb.ReadBytes(bb.ReadInt()));
            if (t == typeof(BinBuffer))
                ret = new BinBuffer(new BinBufferByte(bb.ReadBytes(bb.ReadInt())));

            if (t == typeof(Vector2))
                ret = bb.ReadVector2();
            if (t == typeof(Color))
                ret = bb.ReadColor();
            if (t == typeof(Item))
                ret = bb.ReadItem();
            #endregion

            #region value type -> can read from memory
            if (t.IsValueType && (t.IsExplicitLayout || t.IsLayoutSequential))
            {
                int size = bb.ReadInt();
                IntPtr ptr = Marshal.AllocHGlobal(size); // malloc

                for (IntPtr addr = ptr; addr.ToInt64() < ptr.ToInt64() + size; addr += 1)
                    Marshal.WriteByte(addr, bb.ReadByte());

                ret = Marshal.PtrToStructure(ptr, t);

                Marshal.FreeHGlobal(ptr); // free
            }
            #endregion

            #region serilizable -> can use binaryformatter
            if (t.IsSerializable)
            {
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(bb.ReadBytes(bb.ReadInt()));
                ret = bf.Deserialize(ms);
                ms.Close();
            }
            #endregion

            return ret;
        }

        /// <summary>
        /// Sends a message to the chat lines.
        /// If Main.netMode equals 0, the message is sent with Main.NewText.
        /// If Main.netMode equals 1, nothing will happen.
        /// If Main.netMode equals 2, the message is sent as NetMessage 25.
        /// </summary>
        /// <param name="text">The text to send</param>
        public static void SendText(string text)
        {
            SendText(text, Color.White);
        }
        /// <summary>
        /// Sends a message to the chat lines.
        /// If Main.netMode equals 0, the message is sent with Main.NewText.
        /// If Main.netMode equals 1, nothing will happen.
        /// If Main.netMode equals 2, the message is sent as NetMessage 25.
        /// </summary>
        /// <param name="text">The text to send</param>
        /// <param name="c">The colour of the text to send; default is White</param>
        public static void SendText(string text, Color c)
        {
            if (Main.netMode == 0)
                Main.NewText(text, c.R, c.G, c.B);
            if (Main.netMode == 2)
                NetMessage.SendData(25, -1, -1, text, 255, c.R, c.G, c.B, 0);
        }
    }
}
