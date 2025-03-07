using System;
using CUE4Parse.Encryption.Aes;
using CUE4Parse.UE4.VirtualFileSystem;
using AesProvider = CUE4Parse.Encryption.Aes.Aes;

namespace CUE4Parse.GameTypes.InfinityNikki.Encryption.Aes;

public static class InfinityNikkiAes
{
    private const int AesBlockSize = 16;

    // Not used for any of the CBT packages!
    public static byte[] InfinityNikkiDecrypt(byte[] bytes, int beginOffset, int count, bool isIndex, IAesVfsReader reader)
    {
        if (bytes.Length < beginOffset + count)
            throw new IndexOutOfRangeException("开始偏移量+计数大于字节长度");
        if (count % AesBlockSize != 0)
            throw new ArgumentException("计数必须是16的倍数");
        if (reader.AesKey == null)
            throw new NullReferenceException("reader.AesKey");

        var output = bytes.Decrypt(beginOffset, count, reader.AesKey);
        return PostProcessDecryptedData(output, reader.AesKey);
    }

    public static byte[] PostProcessDecryptedData(byte[] data, FAesKey key)
    {
        for (var i = 0; i < data.Length; i += AesBlockSize)
        {
            data[i] ^= key.Key[0];
            if (data.Length >= i + AesBlockSize)
                data[i + AesBlockSize - 1] ^= key.Key[^1];
        }

        return data;
    } 
}
