using System;

namespace SeganX
{
    public static class AuthenService
    {
        private static Random randomer = new Random(DateTime.Now.Millisecond);

        private static Bigint Power(byte x, byte y)
        {
            Bigint res = new Bigint();
            res.Power(x, y);
            return res;
        }

        public static byte[] SecretKey(int size)
        {
            byte[] res = new byte[size];
            for (int i = 0; i < size; ++i)
                res[i] = (byte)randomer.Next(65, 90);
            return res;
        }

        public static byte[] PublicKey(byte[] secret_key, byte g, byte p)
        {
            byte[] res = new byte[secret_key.Length];
            for (int i = 0; i < secret_key.Length; ++i)
            {
                uint md = Power(g, (byte)(secret_key[i] - 65)).Mode(p);
                res[i] = (byte)(65 + md);
            }
            return res;
        }

        public static byte[] FinalKey(byte[] secret_key, byte[] public_key, uint p)
        {
            byte[] res = new byte[secret_key.Length];
            for (int i = 0; i < secret_key.Length; ++i)
            {
                uint md = Power((byte)(public_key[i] - 65), (byte)(secret_key[i] - 65)).Mode(p);
                res[i] = (byte)(65 + md);
            }
            return res;
        }

        public static byte[] Encrypt(byte[] data, byte[] key)
        {
            var res = new byte[data.Length];
            for (int i = 0; i < data.Length; ++i)
                res[i] = (byte)(data[i] + key[i % key.Length]);
            return res;
        }

        public static byte[] Decrypt(byte[] data, byte[] key)
        {
            var res = new byte[data.Length];
            for (int i = 0; i < data.Length; ++i)
                res[i] = (byte)(data[i] - key[i % key.Length]);
            return res;
        }
    }
}