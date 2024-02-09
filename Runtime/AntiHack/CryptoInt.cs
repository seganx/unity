using System;

namespace SeganX
{
    [Serializable]
    public struct CryptoInt
    {
        public int k;
        public int v;
        public int c;

        public IntResult Get
        {
            get
            {
                var res = (v ^ k) + k;
                return IntResult.Set(res == c ? res : 0);
            }
        }

        public void Encrypt(int value)
        {
            var rand = new Random((int)DateTimeOffset.Now.ToUnixTimeMilliseconds());
            do { k = rand.Next(int.MinValue, int.MaxValue); } while (k == 0);
            v = (value - k) ^ k;
            c = value;
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
            return Get.value.ToString();
        }

        //////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////
        public static implicit operator CryptoInt(int value)
        {
            var result = new CryptoInt();
            result.Encrypt(value);
            return result;
        }
    }
}
