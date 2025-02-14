using System;
using System.Runtime.InteropServices;

namespace SeganX
{
    [Serializable]
    public struct CryptoFloat
    {
        public int k;
        public int v;
        public int c;

        public readonly FloatResult Result => Decrypt(v, k, c);

        public void Encrypt(float value)
        {
            FloatIntBytesUnion o;
            o.i = 0;
            o.f = value;

            var rand = new Random((int)DateTimeOffset.Now.ToUnixTimeMilliseconds());
            do { k = rand.Next(int.MinValue, int.MaxValue); } while (k == 0);
            v = (o.i - k) ^ k;
            c = o.i;
        }

        public void Save(BufferWriter buffer)
        {
            buffer.Int(k);
            buffer.Int(v);
            buffer.Int(c);
        }

        public void Load(BufferReader buffer)
        {
            k = buffer.Int();
            v = buffer.Int();
            c = buffer.Int();
        }

        public override string ToString()
        {
            return Result.value.ToString();
        }


        //////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////
        public static implicit operator CryptoFloat(float value)
        {
            var result = new CryptoFloat();
            result.Encrypt(value);
            return result;
        }

        public static FloatResult Decrypt(int v, int k)
        {
            FloatIntBytesUnion res;
            res.f = 0;
            res.i = (v ^ k) + k;
            return FloatResult.Set(res.f);
        }

        public static FloatResult Decrypt(int v, int k, int c)
        {
            FloatIntBytesUnion res;
            res.f = 0;
            res.i = (v ^ k) + k;
            return FloatResult.Set(res.i == c ? res.f : 0);
        }

        //////////////////////////////////////////////////////
        /// HELPER STRUCT
        //////////////////////////////////////////////////////
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
