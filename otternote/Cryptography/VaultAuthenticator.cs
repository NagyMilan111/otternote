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

    public bool ValidateMasterPassword(byte[] masterPassword, byte[] salt, byte[] encryptedCheckValue)
    {
        try
        {
            byte[] key = _keyService.DeriveKey(masterPassword, salt);
            string decrypted = _encryptionService.Decrypt(encryptedCheckValue, key);
            Array.Clear(key, 0, key.Length);
            return decrypted == CheckValue;
        }
        catch
        {
            return false;
        }
    }
}
