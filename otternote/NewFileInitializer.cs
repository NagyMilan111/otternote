using System.Security.Cryptography;
using System.Text;
using otternote.Json;
using otternote.Services;

namespace otternote;

public class NewFileInitializer
{
    public static void Initialize(byte[] masterPassword)
    {
        SaltGeneratorService saltGeneratorService = new SaltGeneratorService();
        JsonHandler handler = new JsonHandler();
        KeyDerivationService keyDerivationService = new KeyDerivationService();
        EncryptionService encryptionService = new EncryptionService();
        Dictionary<string, string> fileHeader = new Dictionary<string, string>();
        byte[] salt = saltGeneratorService.GenerateBytes(16);
        byte[] key = keyDerivationService.DeriveKey(masterPassword, salt);
        
        byte[] plainTextVaultCheck = Encoding.UTF8.GetBytes("vault_check");
        
        byte[] encryptedVaultCheck = encryptionService.Encrypt(plainTextVaultCheck, key);
        
        Array.Clear(key, 0, key.Length);
        
        fileHeader.Add("version", "1");
        fileHeader.Add("algorithm", "AES-256-CBC");
        fileHeader.Add("salt", Convert.ToBase64String(salt));
        fileHeader.Add("vault_check", Convert.ToBase64String(encryptedVaultCheck));
        Aes aes = Aes.Create();
        aes.GenerateIV();
        fileHeader.Add("vault_check_iv", Convert.ToBase64String(aes.IV));

        
        JsonFile file = new JsonFile(fileHeader);
        handler.Save(file, "vault.json");

    }
}