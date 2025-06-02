using System.Text;
using otternote.Cryptography;
using otternote.Json;
using otternote.Services;

namespace otternote;


class Program
{
    static void Main(string[] args)
    {

        JsonFile? file = null;
        KeyDerivationService keyDerivationService = new KeyDerivationService();
        SecurePasswordReaderService securePasswordReaderService = new SecurePasswordReaderService();
        EncryptionService encryptionService = new EncryptionService();
        VaultAuthenticator vaultAuthenticator = new VaultAuthenticator(keyDerivationService, encryptionService);
        char[] masterPasswordChars = securePasswordReaderService.ReadPasswordAsCharArray();        
        byte[] masterPasswordBytes = SecurePasswordReaderService.GetBytes(Encoding.UTF8, masterPasswordChars);
        Array.Clear(masterPasswordChars, 0, masterPasswordChars.Length);
        
        
        JsonHandler handler = new JsonHandler();
        try
        {
            file = handler.Load("vault.json");
        }
        catch (FileNotFoundException)
        {
            NewFileInitializer.Initialize(masterPasswordBytes);
            
            Array.Clear(masterPasswordBytes, 0, masterPasswordBytes.Length);
            
            Console.WriteLine("Initialization complete, you will need to sign in again.");
            Thread.Sleep(5000);
            Environment.Exit(0);
        }

        byte[] salt = Convert.FromBase64String(file.Header["salt"]);
        byte[] encryptedVaultCheck = Convert.FromBase64String(file.Header["vault_check"]);

        if (vaultAuthenticator.ValidateMasterPassword(masterPasswordBytes, salt, encryptedVaultCheck))
        {
            Console.WriteLine("Master password verified.");
        }
        else
        {
            Console.WriteLine("Master password verification failed, exiting...");
            Thread.Sleep(5000);
            Environment.Exit(0);
        }



        Console.ReadLine();
    }
    
}