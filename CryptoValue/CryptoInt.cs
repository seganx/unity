using System;

namespace SeganX
{
    [Serializable]
    public struct CryptoInt
    {
        public int k;
        public int v;

        public int Value { get { return Decrypt(v, k); } }

        private CryptoInt(int a)
        {
            var rand = new Random((int)DateTime.Now.Ticks);
            do { k = rand.Next(int.MinValue, int.MaxValue); } while (k == 0);
            v = Encrypt(a, k);
        }

        public override string ToString()
        {
            return Decrypt(v, k).ToString();
        }

        public static implicit operator CryptoInt(int value)
        {
            return new CryptoInt(value);
        }

        public static implicit operator int(CryptoInt value)
        {
            return Decrypt(value.v, value.k);
        }

        public static implicit operator string(CryptoInt value)
        {
            return Decrypt(value.v, value.k).ToString();
        }

        public static CryptoInt operator ++(CryptoInt input)
        {
            input = Decrypt(input.v, input.k) + 1;
            return input;
        }

        public static CryptoInt operator --(CryptoInt input)
        {
            input = Decrypt(input.v, input.k) - 1;
            return input;
        }

        private static int Encrypt(int value, int key)
        {
            var v = (value ^ key);
            var res = v + key;
            return res;
        }

        private static int Decrypt(int value, int key)
        {
            var v = (value - key);
            var res = v ^ key;
            return res;
        }
    }
}
