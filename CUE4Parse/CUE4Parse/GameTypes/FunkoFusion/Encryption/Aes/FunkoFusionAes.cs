using System;
using System.Runtime.Intrinsics;
using CUE4Parse.UE4.VirtualFileSystem;
using static System.Runtime.Intrinsics.X86.Aes;
using static System.Runtime.Intrinsics.Vector128;

namespace CUE4Parse.GameTypes.FunkoFusion.Encryption.Aes;

/// <summary>
/// Reversed by Spiritovod
/// </summary>
public static class FunkoFusionAes
{
    // 0xFB9042D6FBAA79732196B8A058A2AE19F7075F02AA8C7EDE9EB13C410200FCEB
    private static readonly Vector128<byte>[] RoundKeys =
    [
        Create(0x4E, 0x7C, 0x8C, 0x14, 0x5B, 0x69, 0x0B, 0x66, 0xC8, 0x14, 0x6B, 0x84, 0xA7, 0x22, 0x22, 0x6C),
        Create(0x28, 0x11, 0xC1, 0x55, 0x41, 0xFA, 0x82, 0xBF, 0x8A, 0xA9, 0x8C, 0xFD, 0x6E, 0xDF, 0xBA, 0x1E),
        Create(0x64, 0x43, 0x2B, 0xBF, 0xB6, 0xA8, 0xCD, 0x3B, 0x10, 0x1A, 0xB7, 0xFA, 0x41, 0x69, 0x8A, 0xA8),
        Create(0x20, 0xF6, 0xBE, 0xA7, 0x61, 0x0C, 0x3C, 0x18, 0xEB, 0xA5, 0xB0, 0xE5, 0x85, 0x7A, 0x0A, 0xFB),
        Create(0xD3, 0x47, 0x72, 0xBF, 0x65, 0xEF, 0xBF, 0x84, 0x75, 0xF5, 0x08, 0x7E, 0x34, 0x9C, 0x82, 0xD6),
        Create(0x82, 0x28, 0x71, 0xA2, 0xE3, 0x24, 0x4D, 0xBA, 0x08, 0x81, 0xFD, 0x5F, 0x8D, 0xFB, 0xF7, 0xA4),
        Create(0xC3, 0xA5, 0x0A, 0x6A, 0xA6, 0x4A, 0xB5, 0xEE, 0xD3, 0xBF, 0xBD, 0x90, 0xE7, 0x23, 0x3F, 0x46),
        Create(0xDE, 0xF0, 0x9E, 0x5B, 0x3D, 0xD4, 0xD3, 0xE1, 0x35, 0x55, 0x2E, 0xBE, 0xB8, 0xAE, 0xD9, 0x1A),
        Create(0xD9, 0x83, 0xD2, 0xCE, 0x7F, 0xC9, 0x67, 0x20, 0xAC, 0x76, 0xDA, 0xB0, 0x4B, 0x55, 0xE5, 0xF6),
        Create(0x55, 0x34, 0xC1, 0x84, 0x68, 0xE0, 0x12, 0x65, 0x5D, 0xB5, 0x3C, 0xDB, 0xE5, 0x1B, 0xE5, 0xC1),
        Create(0x30, 0x22, 0x16, 0xDC, 0x4F, 0xEB, 0x71, 0xFC, 0xE3, 0x9D, 0xAB, 0x4C, 0xA8, 0xC8, 0x4E, 0xBA),
        Create(0x24, 0x45, 0x3C, 0xB8, 0x4C, 0xA5, 0x2E, 0xDD, 0x11, 0x10, 0x12, 0x06, 0xF4, 0x0B, 0xF7, 0xC7),
        Create(0x38, 0x5A, 0x65, 0xDB, 0x77, 0xB1, 0x14, 0x27, 0x94, 0x2C, 0xBF, 0x6B, 0x3C, 0xE4, 0xF1, 0xD1),
        Create(0xD2, 0x17, 0x43, 0x6E, 0x9E, 0xB2, 0x6D, 0xB3, 0x8F, 0xA2, 0x7F, 0xB5, 0x7B, 0xA9, 0x88, 0x72),
        Create(0xFB, 0x90, 0x42, 0xD6, 0xFB, 0xAA, 0x79, 0x73, 0x21, 0x96, 0xB8, 0xA0, 0x58, 0xA2, 0xAE, 0x19)
    ];

    private static void DecryptWithRoundKeys(byte[] input, int index, Vector128<byte>[] roundkeys)
    {
        var state = Create(input, index);
        var rounds = roundkeys.Length - 1;
        state = Xor(state, roundkeys[0]);
        for (var i = 1; i < rounds; i++)
        {
            state = Decrypt(state, roundkeys[i]);
        }

        state = DecryptLast(state, roundkeys[rounds]);
        state.CopyTo(input, index);
    }

    public static byte[] FunkoFusionDecrypt(byte[] bytes, int beginOffset, int count, bool isIndex, IAesVfsReader reader)
    {
        if (bytes.Length < beginOffset + count)
            throw new IndexOutOfRangeException("开始偏移量+计数大于字节长度");
        if (count % 16 != 0)
            throw new ArgumentException("计数必须是16的倍数");
        if (reader.AesKey == null)
            throw new NullReferenceException("reader.AesKey");

        var output = new byte[count];
        Array.Copy(bytes, beginOffset, output, 0, count);

        for (var i = 0; i < count / 16; i++)
        {
            DecryptWithRoundKeys(output, i * 16, RoundKeys);
        }

        return output;
    }
}
