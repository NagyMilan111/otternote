namespace otternote.Services;

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class EncryptionService
{
    public byte[] Encrypt(string plaintext, byte[] key)
    {
        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        byte[] plainBytes = Encoding.UTF8.GetBytes(plaintext);
        byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        byte[] combined = new byte[aes.IV.Length + cipherBytes.Length];
        Buffer.BlockCopy(aes.IV, 0, combined, 0, aes.IV.Length);
        Buffer.BlockCopy(cipherBytes, 0, combined, aes.IV.Length, cipherBytes.Length);

        return combined;
    }

    public string Decrypt(byte[] encryptedData, byte[] key)
    {
        using Aes aes = Aes.Create();
        aes.Key = key;

        byte[] iv = new byte[aes.BlockSize / 8];
        byte[] cipherText = new byte[encryptedData.Length - iv.Length];

        Buffer.BlockCopy(encryptedData, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(encryptedData, iv.Length, cipherText, 0, cipherText.Length);

        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        byte[] plainBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }
}
