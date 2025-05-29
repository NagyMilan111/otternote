using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
namespace encryptionTest;

public class Encryptor
{
    public JsonEntry EncryptEntry(JsonEntry plainEntry, byte[] key)
    {
        if (key.Length != 32) throw new ArgumentException("Key must be 32 bytes (256 bits)");

        // Encrypt username
        CipherEntry encryptedUsername = EncryptString(plainEntry.Username.CipherText, key);
        
        // Encrypt password
        CipherEntry encryptedPassword = EncryptString(plainEntry.Password.CipherText, key);

        // Return new JsonEntry with encrypted data
        return new JsonEntry(
            plainEntry.Site,
            new CipherEntry(encryptedPassword.Iv, encryptedPassword.CipherText),
            new CipherEntry(encryptedUsername.Iv, encryptedUsername.CipherText)
        );
    }

    private CipherEntry EncryptString(string plainText, byte[] key)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.GenerateIV();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var writer = new StreamWriter(cryptoStream))
                {
                    writer.Write(plainText);
                }

                // Return IV and ciphertext as base64 strings
                return new CipherEntry(
                    Convert.ToBase64String(aes.IV),
                    Convert.ToBase64String(ms.ToArray())
                );
            }
        }
    }
}