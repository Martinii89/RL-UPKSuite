using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Core.RocketLeague.Decryption;

/// <summary>
/// Interface for providing decryption keys and <see cref="ICryptoTransform"/> instances for decrypting rocket league packages
/// </summary>
public interface IDecrypterProvider
{
    List<byte[]> DecryptionKeys { get; }
    ICryptoTransform GetCryptoTransform(byte[] key);
}

/// <summary>
/// Basic decryption provider. It can read a file containing Base64 encoded decryption keys. It has a thread-safe caches of decryptors for optimizing reuse of the same keys.
/// </summary>
public class DecryptionProvider : IDecrypterProvider
{
    private class ByteArrayEqualityComparer : EqualityComparer<byte[]>
    {
        public override bool Equals(byte[]? x, byte[]? y) => StructuralComparisons.StructuralEqualityComparer.Equals(x, y);

        public override int GetHashCode(byte[] obj) => StructuralComparisons.StructuralEqualityComparer.GetHashCode(obj);
    }

    private static readonly byte[] DefaultKey =
    {
        0xC7, 0xDF, 0x6B, 0x13, 0x25, 0x2A, 0xCC, 0x71,
        0x47, 0xBB, 0x51, 0xC9, 0x8A, 0xD7, 0xE3, 0x4B,
        0x7F, 0xE5, 0x00, 0xB7, 0x7F, 0xA5, 0xFA, 0xB2,
        0x93, 0xE2, 0xF2, 0x4E, 0x6B, 0x17, 0xE7, 0x79
    };

    private readonly ConcurrentDictionary<byte[], ICryptoTransform> _decryptorCache = new(new ByteArrayEqualityComparer());

    /// <summary>
    /// The list of decryption keys this provider knows about
    /// </summary>
    public List<byte[]> DecryptionKeys { get; } = new() { DefaultKey };

    /// <summary>
    /// Return a decryptor instance. The instances are cached in a thread safe container.
    /// </summary>
    /// <param name="key">The key used for decryption</param>
    /// <returns></returns>
    public ICryptoTransform GetCryptoTransform(byte[] key)
    {
        if (_decryptorCache.TryGetValue(key, out var cacheTransform))
        {
            return cacheTransform;
        }

        var decryptor = Aes.Create("AesManaged");
        Debug.Assert(decryptor != null, nameof(decryptor) + " != null");
        decryptor.KeySize = 256;
        decryptor.Key = key;
        decryptor.Mode = CipherMode.ECB;
        decryptor.Padding = PaddingMode.None;

        var cryptoTransform = decryptor.CreateDecryptor();
        _decryptorCache.TryAdd(key, cryptoTransform);
        return cryptoTransform;
    }

    /// <summary>
    /// Constructs the decryption provider. Taking in a path to a file with base64 encoded keys.
    /// </summary>
    /// <param name="keyFilePath">Path key file</param>
    public DecryptionProvider(string keyFilePath)
    {
        if (!File.Exists(keyFilePath))
        {
            Console.WriteLine("Failed to load the key file. Using only the default key");
            return;
        }
        var stringKeys = File.ReadAllLines(keyFilePath);
        foreach (var key in stringKeys)
        {
            DecryptionKeys.Add(Convert.FromBase64String(key));
        }
    }
}