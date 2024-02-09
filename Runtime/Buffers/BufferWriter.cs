using UnityEngine;

namespace SeganX
{
    public class BufferWriter
    {
        private readonly byte[] buffer = null;
        private readonly char[] charArray = new char[1];
        private readonly byte[] byteArray = new byte[1];
        private readonly sbyte[] sbyteArray = new sbyte[1];
        private readonly short[] shortArray = new short[4];
        private readonly ushort[] ushortArray = new ushort[1];
        private readonly float[] floatArray = new float[4];
        private readonly int[] intArray = new int[4];
        private readonly uint[] uintArray = new uint[1];
        private readonly long[] longArray = new long[1];
        private readonly ulong[] ulongArray = new ulong[1];

        public int Length { get; private set; } = 0;

        public BufferWriter(int size)
        {
            buffer = new byte[size];
        }

        public byte[] GetBytes()
        {
            var result = new byte[Length];
            System.Buffer.BlockCopy(buffer, 0, result, 0, Length);
            return result;
        }

        public BufferWriter Reset()
        {
            Length = 0;
            return this;
        }

        public BufferWriter Append(System.Array src, int length)
        {
            System.Buffer.BlockCopy(src, 0, buffer, Length, length);
            Length += length;
            return this;
        }

        public BufferWriter Char(char value)
        {
            charArray[0] = value;
            return Append(charArray, sizeof(char));
        }

        public BufferWriter Byte(byte value)
        {
            byteArray[0] = value;
            return Append(byteArray, 1);
        }

        public BufferWriter Sbyte(sbyte value)
        {
            sbyteArray[0] = value;
            return Append(sbyteArray, 1);
        }

        public BufferWriter Bool(bool value)
        {
            byteArray[0] = value ? (byte)1 : (byte)0;
            return Append(byteArray, sizeof(byte));
        }

        public BufferWriter Short(short value)
        {
            shortArray[0] = value;
            return Append(shortArray, sizeof(short));
        }

        public BufferWriter Ushort(ushort value)
        {
            ushortArray[0] = value;
            return Append(ushortArray, sizeof(ushort));
        }

        public BufferWriter Float(float value)
        {
            floatArray[0] = value;
            return Append(floatArray, sizeof(float));
        }

        public BufferWriter Int(int value)
        {
            intArray[0] = value;
            return Append(intArray, sizeof(int));
        }

        public BufferWriter Uint(uint value)
        {
            uintArray[0] = value;
            return Append(uintArray, sizeof(uint));
        }

        public BufferWriter Long(long value)
        {
            longArray[0] = value;
            return Append(longArray, sizeof(long));
        }

        public BufferWriter Ulong(ulong value)
        {
            ulongArray[0] = value;
            return Append(ulongArray, sizeof(ulong));
        }

        public BufferWriter AppendBytes(byte[] value, int length)
        {
            return Append(value, length);
        }

        public BufferWriter String(string value)
        {
            var length = (byte)System.Text.Encoding.UTF8.GetBytes(value, 0, value.Length, buffer, Length + 1);
            buffer[Length] = length;
            Length += length + 1;
            return this;
        }

        public BufferWriter Vector2(Vector2 value)
        {
            floatArray[0] = value.x;
            floatArray[1] = value.y;
            return Append(floatArray, sizeof(float) * 2);
        }

        public BufferWriter Vector3(Vector3 value)
        {
            floatArray[0] = value.x;
            floatArray[1] = value.y;
            floatArray[2] = value.z;
            return Append(floatArray, sizeof(float) * 3);
        }

        public BufferWriter Vector3(Vector3 value, float precision)
        {
            shortArray[0] = (short)Mathf.RoundToInt(value.x * precision);
            shortArray[1] = (short)Mathf.RoundToInt(value.y * precision);
            shortArray[2] = (short)Mathf.RoundToInt(value.z * precision);
            return Append(shortArray, sizeof(short) * 3);
        }

        public BufferWriter Vector4(Vector4 value)
        {
            floatArray[0] = value.x;
            floatArray[1] = value.y;
            floatArray[2] = value.z;
            floatArray[3] = value.w;
            return Append(floatArray, sizeof(float) * 4);
        }

        public BufferWriter Quaternion(Quaternion value)
        {
            floatArray[0] = value.x;
            floatArray[1] = value.y;
            floatArray[2] = value.z;
            floatArray[3] = value.w;
            return Append(floatArray, sizeof(float) * 4);
        }

        public BufferWriter Quaternion(Quaternion value, float precision)
        {
            shortArray[0] = (short)Mathf.RoundToInt(value.x * precision);
            shortArray[1] = (short)Mathf.RoundToInt(value.y * precision);
            shortArray[2] = (short)Mathf.RoundToInt(value.z * precision);
            shortArray[3] = (short)Mathf.RoundToInt(value.w * precision);
            return Append(shortArray, sizeof(short) * 4);
        }
    }
}