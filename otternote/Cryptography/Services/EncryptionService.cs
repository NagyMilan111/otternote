namespace otternote.Services;

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class EncryptionService
{
    public byte[] Encrypt(byte[] plaintextBytes, byte[] key)
    {
        using Aes aes = Aes.Create();
        aes.Key = key;
        Array.Clear(key, 0, key.Length);
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        byte[] cipherBytes = encryptor.TransformFinalBlock(plaintextBytes, 0, plaintextBytes.Length);

        byte[] combined = new byte[aes.IV.Length + cipherBytes.Length];
        Buffer.BlockCopy(aes.IV, 0, combined, 0, aes.IV.Length);
        Buffer.BlockCopy(cipherBytes, 0, combined, aes.IV.Length, cipherBytes.Length);

        aes.Clear();
        return combined;
    }


    public string Decrypt(byte[] encryptedData, byte[] key)
    {
        using Aes aes = Aes.Create();
        aes.Key = key;
        Array.Clear(key, 0, key.Length);

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
