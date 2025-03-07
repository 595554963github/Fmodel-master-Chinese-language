using System;
using CUE4Parse.UE4.VirtualFileSystem;
using AesProvider = CUE4Parse.Encryption.Aes.Aes;

namespace CUE4Parse.GameTypes.DeltaForce.Encryption.Aes;

public static class DeltaForceAes
{
    public static byte[] DeltaForceDecrypt(byte[] bytes, int beginOffset, int count, bool isIndex, IAesVfsReader reader)
    {
        if (bytes.Length < beginOffset + count)
            throw new IndexOutOfRangeException("开始偏移量+计数大于字节长度");
        if (count % 16 != 0)
            throw new ArgumentException("计数必须是16的倍数");
        if (reader.AesKey == null)
            throw new NullReferenceException("reader.AesKey");

        var output = AesProvider.Decrypt(bytes, beginOffset, count, reader.AesKey);

        for (var i = 0; i < count; i++)
        {
            output[i] ^= 0x7F;
        }

        return output;
    }
}
