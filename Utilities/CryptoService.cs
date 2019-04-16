using System;
using System.IO;

public static class CryptoService
{
    public static string ComputeMD5(byte[] src, string salt)
    {
        var md5 = System.Security.Cryptography.MD5.Create();

        //  create bytes
        var saltBytes = System.Text.Encoding.ASCII.GetBytes(salt);
        var inputBytes = new byte[saltBytes.Length + src.Length];

        // copy bytes
        System.Buffer.BlockCopy(src, 0, inputBytes, 0, src.Length);
        System.Buffer.BlockCopy(saltBytes, 0, inputBytes, src.Length, saltBytes.Length);

        // compute hash
        byte[] hashBytes = md5.ComputeHash(inputBytes);
        var res = new System.Text.StringBuilder();
        for (int i = 0; i < hashBytes.Length; i++)
            res.Append(hashBytes[i].ToString("X2"));
        return res.ToString();
    }

    public static void CopyBytes(byte[] dest, int destIndex, byte[] src, int srcIndex, int count)
    {
        Buffer.BlockCopy(src, srcIndex, dest, destIndex, count);
    }

    public static bool CompareBytes(byte[] b1, int b1index, byte[] b2, int b2index, int count = int.MaxValue)
    {
        if (b1 == b2) return true;
        var len = Math.Min(Math.Min(b1.Length - b1index, b2.Length - b2index), count);
        for (int i = 0; i < len; i++)
            if (b1[i + b1index] != b2[i + b2index])
                return false;
        return true;
    }

    public static byte[] EncryptBytes(byte[] data, byte[] key, int index = 0, int count = int.MaxValue)
    {
        var len = Math.Min(data.Length - index, count);
        var res = new byte[len];
        for (int i = 0; i < len; ++i)
            res[i] = (byte)(data[i + index] + key[i % key.Length]);
        return res;
    }

    public static byte[] DecryptBytes(byte[] data, byte[] key, int index = 0, int count = int.MaxValue)
    {
        var len = Math.Min(data.Length - index, count);
        var res = new byte[len];
        for (int i = 0; i < len; ++i)
            res[i] = (byte)(data[i + index] - key[i % key.Length]);
        return res;
    }

    public static byte[] EncryptWithMac(byte[] src, byte[] key, string salt)
    {
        var md5 = ComputeMD5(src, salt).GetBytes();
        var data = EncryptBytes(src, key);
        var res = new byte[md5.Length + data.Length];
        CopyBytes(res, 0, md5, 0, md5.Length);
        CopyBytes(res, md5.Length, data, 0, data.Length);
        return res;
    }

    public static byte[] DecryptWithMac(byte[] src, byte[] key, string salt)
    {
        var res = DecryptBytes(src, key, 32);
        var md5 = ComputeMD5(res, salt).GetBytes();
        if (CompareBytes(md5, 0, src, 0, 32))
            return res;
        else
            return null;
    }
}
