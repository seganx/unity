using UnityEngine;

namespace SeganX
{
    public class BufferReader
    {
        private byte[] buffer = null;
        private readonly char[] charArray = new char[1];
        private readonly byte[] byteArray = new byte[1];
        private readonly sbyte[] sbyteArray = new sbyte[1];
        private readonly short[] shortArray = new short[4];
        private readonly ushort[] ushortArray = new ushort[1];
        private readonly float[] floatArray = new float[4];
        private readonly int[] intArray = new int[1];
        private readonly uint[] uintArray = new uint[1];
        private readonly long[] longArray = new long[1];
        private readonly ulong[] ulongArray = new ulong[1];

        public byte[] Bytes => buffer;
        public int Posision { get; private set; } = 0;

        public BufferReader(byte[] bytes)
        {
            Reset(bytes);
        }

        public BufferReader(int size)
        {
            buffer = new byte[size];
        }

        public BufferReader Reset(byte[] bytes)
        {
            buffer = bytes;
            Posision = 0;
            return this;
        }

        public BufferReader Reset()
        {
            Posision = 0;
            return this;
        }

        public BufferReader Read(System.Array dest, int length)
        {
            System.Buffer.BlockCopy(buffer, Posision, dest, 0, length);
            Posision += length;
            return this;
        }

        public char Char()
        {
            Read(charArray, sizeof(char));
            return charArray[0];
        }

        public byte Byte()
        {
            Read(byteArray, 1);
            return byteArray[0];
        }

        public sbyte Sbyte()
        {
            Read(sbyteArray, 1);
            return sbyteArray[0];
        }

        public bool Bool()
        {
            Read(byteArray, 1);
            return byteArray[0] > 0;
        }

        public short Short()
        {
            Read(shortArray, 2);
            return shortArray[0];
        }

        public ushort Ushort()
        {
            Read(ushortArray, 2);
            return ushortArray[0];
        }

        public float Float()
        {
            Read(floatArray, 4);
            return floatArray[0];
        }

        public int Int()
        {
            Read(intArray, 4);
            return intArray[0];
        }

        public uint Uint()
        {
            Read(uintArray, 4);
            return uintArray[0];
        }

        public long Long()
        {
            Read(longArray, sizeof(long));
            return longArray[0];
        }

        public ulong Ulong()
        {
            Read(ulongArray, sizeof(ulong));
            return ulongArray[0];
        }

        public BufferReader ReadBytes(byte[] value, int length)
        {
            return Read(value, length);
        }

        public string String()
        {
            var length = Byte();
            var res = System.Text.Encoding.UTF8.GetString(buffer, Posision, length);
            Posision += length;
            return res;
        }

        public Vector2 Vector2()
        {
            Read(floatArray, sizeof(float) * 2);
            Vector2 value;
            value.x = floatArray[0];
            value.y = floatArray[1];
            return value;
        }

        public Vector3 Vector3()
        {
            Read(floatArray, sizeof(float) * 3);
            Vector3 value;
            value.x = floatArray[0];
            value.y = floatArray[1];
            value.z = floatArray[2];
            return value;
        }

        public Vector3 Vector3(float precision)
        {
            Read(shortArray, sizeof(short) * 3);
            Vector3 value;
            value.x = shortArray[0] / precision;
            value.y = shortArray[1] / precision;
            value.z = shortArray[2] / precision;
            return value;
        }


        public Vector4 Vector4()
        {
            Read(floatArray, sizeof(float) * 4);
            Vector4 value;
            value.x = floatArray[0];
            value.y = floatArray[1];
            value.z = floatArray[2];
            value.w = floatArray[3];
            return value;
        }

        public Quaternion Quaternion()
        {
            Read(floatArray, sizeof(float) * 4);
            Quaternion value;
            value.x = floatArray[0];
            value.y = floatArray[1];
            value.z = floatArray[2];
            value.w = floatArray[3];
            return value;
        }

        public Quaternion Quaternion(float precision)
        {
            Read(shortArray, sizeof(short) * 4);
            Quaternion value;
            value.x = shortArray[0] / precision;
            value.y = shortArray[1] / precision;
            value.z = shortArray[2] / precision;
            value.w = shortArray[3] / precision;
            return value;
        }
    }
}