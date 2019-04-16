using System;
using System.Runtime.InteropServices;

namespace SeganX
{
    [Serializable]
    public struct CryptoFloat
    {
        public int key;
        public int value;

        private CryptoFloat(float a)
        {
            var rand = new Random((int)DateTime.Now.Ticks);
            do { key = rand.Next(int.MinValue, int.MaxValue); } while (key == 0);
            value = Encrypt(a, key);
        }

        public override string ToString()
        {
            return Decrypt(value, key).ToString();
        }

        public static implicit operator CryptoFloat(float value)
        {
            return new CryptoFloat(value);
        }

        public static implicit operator float(CryptoFloat value)
        {
            return Decrypt(value.value, value.key);
        }

        public static int Encrypt(float value, int key)
        {
            FloatIntBytesUnion intValue;
            intValue.i = 0;
            intValue.f = value;
            var v = (intValue.i ^ key);
            var res = v + key;
            return res;
        }

        public static float Decrypt(int value, int key)
        {
            FloatIntBytesUnion res;
            res.f = 0;
            var v = (value - key);
            res.i = v ^ key;
            return res.f;
        }


        [StructLayout(LayoutKind.Explicit)]
        internal struct FloatIntBytesUnion
        {
            [FieldOffset(0)]
            public float f;

            [FieldOffset(0)]
            public int i;
        }

    }
}
