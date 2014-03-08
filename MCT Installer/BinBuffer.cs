using System;
using System.IO;
using System.Text;

namespace TAPI.SDK.Installer
{
    public class BinBuffer
    {
        protected byte[] buffer;
        protected int _pos = 0, bytes = 0;

        public BinBuffer() : this(8192) { }
        public BinBuffer(int startBytes)
        {
            buffer = new byte[startBytes];
        }
        public BinBuffer(byte[] bytes) : this(bytes, true) { }
        public BinBuffer(byte[] bytes, bool filled)
        {
            if (filled) this.bytes = bytes.Length;
            buffer = bytes;
        }

        protected void IncreaseBufferSize()
        {
            Array.Resize(ref buffer, buffer.Length * 2);
        }

        public int Pos
        {
            get
            {
                return _pos;
            }
            set
            {
                _pos = value;
                while (_pos > buffer.Length - 1) IncreaseBufferSize();
                while (_pos > bytes - 1) bytes++;
            }
        }

        public bool IsEmpty() { return GetSize() == 0; }
        public int GetSize() { return bytes; }
        public int GetSizeReal() { return buffer.Length; }
        public int BytesLeft() { return bytes - _pos; }

        public void clear()
        {
            _pos = 0;
            bytes = 0;
        }

        public BinBuffer Write(byte value)
        {
            if (_pos > buffer.Length - 1) IncreaseBufferSize();
            if (_pos > bytes - 1) bytes++;
            buffer[_pos++] = value;
            return this;
        }
        public BinBuffer Write(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++) Write(bytes[i]);
            return this;
        }
        public BinBuffer Write(short value) { return Write(BitConverter.GetBytes(value)); }
        public BinBuffer Write(ushort value) { return Write(BitConverter.GetBytes(value)); }
        public BinBuffer Write(int value) { return Write(BitConverter.GetBytes(value)); }
        public BinBuffer Write(uint value) { return Write(BitConverter.GetBytes(value)); }
        public BinBuffer Write(long value) { return Write(BitConverter.GetBytes(value)); }
        public BinBuffer Write(ulong value) { return Write(BitConverter.GetBytes(value)); }
        public BinBuffer Write(float value) { return Write(BitConverter.GetBytes(value)); }
        public BinBuffer Write(double value) { return Write(BitConverter.GetBytes(value)); }
        public BinBuffer Write(bool value) { return Write((byte)(value ? 1 : 0)); }
        public BinBuffer Write(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            Write(bytes.Length);
            Write(bytes);
            return this;
        }

        public BinBuffer WriteX(params byte[] values) { for (int i = 0; i < values.Length; i++) Write(values[i]); return this; }
        public BinBuffer WriteX(params short[] values) { for (int i = 0; i < values.Length; i++) Write(values[i]); return this; }
        public BinBuffer WriteX(params ushort[] values) { for (int i = 0; i < values.Length; i++) Write(values[i]); return this; }
        public BinBuffer WriteX(params int[] values) { for (int i = 0; i < values.Length; i++) Write(values[i]); return this; }
        public BinBuffer WriteX(params uint[] values) { for (int i = 0; i < values.Length; i++) Write(values[i]); return this; }
        public BinBuffer WriteX(params long[] values) { for (int i = 0; i < values.Length; i++) Write(values[i]); return this; }
        public BinBuffer WriteX(params ulong[] values) { for (int i = 0; i < values.Length; i++) Write(values[i]); return this; }
        public BinBuffer WriteX(params float[] values) { for (int i = 0; i < values.Length; i++) Write(values[i]); return this; }
        public BinBuffer WriteX(params double[] values) { for (int i = 0; i < values.Length; i++) Write(values[i]); return this; }
        public BinBuffer WriteX(params bool[] values) { for (int i = 0; i < values.Length; i++) Write(values[i]); return this; }
        public BinBuffer WriteX(params string[] values) { for (int i = 0; i < values.Length; i++) Write(values[i]); return this; }

        public byte ReadByte()
        {
            if (_pos > buffer.Length - 1) throw new IndexOutOfRangeException();
            return buffer[_pos++];
        }
        public byte[] ReadBytes(int count)
        {
            byte[] bytes = new byte[count];
            for (int i = 0; i < count; i++) bytes[i] = ReadByte();
            return bytes;
        }
        public short ReadShort() { return BitConverter.ToInt16(ReadBytes(2), 0); }
        public ushort ReadUShort() { return BitConverter.ToUInt16(ReadBytes(2), 0); }
        public int ReadInt() { return BitConverter.ToInt32(ReadBytes(4), 0); }
        public uint ReadUInt() { return BitConverter.ToUInt32(ReadBytes(4), 0); }
        public long ReadLong() { return BitConverter.ToInt64(ReadBytes(8), 0); }
        public ulong ReadULong() { return BitConverter.ToUInt64(ReadBytes(8), 0); }
        public float ReadFloat() { return BitConverter.ToSingle(ReadBytes(4), 0); }
        public double ReadDouble() { return BitConverter.ToDouble(ReadBytes(8), 0); }
        public bool ReadBool() { return ReadByte() == 1; }
        public string ReadString() { return Encoding.UTF8.GetString(ReadBytes(ReadInt())); }

        public void FillByte(byte value)
        {
            for (int i = 0; i < GetSizeReal(); i++) buffer[i] = value;
        }

        public BinBuffer Copy() { return Copy(BytesLeft()); }
        public BinBuffer Copy(int bytes)
        {
            BinBuffer binb = new BinBuffer();
            for (int i = 0; i < bytes; i++) binb.Write(ReadByte());
            return binb;
        }

        public void Write(BinBuffer binb) { Write(binb, binb.BytesLeft()); }
        public void Write(BinBuffer binb, int bytes)
        {
            for (int i = 0; i < bytes; i++) Write(binb.ReadByte());
        }
    }
}
