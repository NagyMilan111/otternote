using System.Text;
using otternote.Services;

namespace otternote.Cryptography;

using System;

public class VaultAuthenticator
{
    private const string CheckValue = "vault_check";
    private readonly KeyDerivationService _keyService;
    private readonly EncryptionService _encryptionService;

    public VaultAuthenticator(KeyDerivationService keyService, EncryptionService encryptionService)
    {
        _keyService = keyService;
        _encryptionService = encryptionService;
    }

    public bool ValidateMasterPassword(byte[] masterPassword, byte[] salt, byte[] encryptedCheckValue, byte[] iv)
    {
        try
        {
            byte[] key = _keyService.DeriveKey(masterPassword, salt);
            byte[] decryptedBytes = _encryptionService.Decrypt(encryptedCheckValue, key, iv);
            string decrypted = Encoding.UTF8.GetString(decryptedBytes);           
            Array.Clear(key, 0, key.Length);
            return decrypted == CheckValue;
        }
        catch
        {
            return false;
        }
    }
}
