using System;

namespace SeganX
{
    [Serializable]
    public struct CryptoInt
    {
        public int key;
        public int value;

        private CryptoInt(int a)
        {
            var rand = new Random((int)DateTime.Now.Ticks);
            do { key = rand.Next(int.MinValue, int.MaxValue); } while (key == 0);
            value = Encrypt(a, key);
        }

        public override string ToString()
        {
            return Decrypt(value, key).ToString();
        }

        public static implicit operator CryptoInt(int value)
        {
            return new CryptoInt(value);
        }

        public static implicit operator int(CryptoInt value)
        {
            return Decrypt(value.value, value.key);
        }

        public static CryptoInt operator ++(CryptoInt input)
        {
            input = Decrypt(input.value, input.key) + 1;
            return input;
        }

        public static CryptoInt operator --(CryptoInt input)
        {
            input = Decrypt(input.value, input.key) - 1;
            return input;
        }

        public static int Encrypt(int value, int key)
        {
            var v = (value ^ key);
            var res = v + key;
            return res;
        }

        public static int Decrypt(int value, int key)
        {
            var v = (value - key);
            var res = v ^ key;
            return res;
        }
    }
}
