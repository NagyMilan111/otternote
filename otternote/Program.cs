using System.Text;
using otternote.Cryptography;
using otternote.Json;
using otternote.Services;

namespace otternote;


class Program
{
    static volatile bool _keepRunning = true;
    
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
        byte[] vaultCheckIv = Convert.FromBase64String(file.Header["vault_check_iv"]);

        if (vaultAuthenticator.ValidateMasterPassword(masterPasswordBytes, salt, encryptedVaultCheck, vaultCheckIv))
        {
            Console.WriteLine("Master password verified. Press Ctrl+C to exit.");
            
            // Subscribe to Ctrl+C (SIGINT)
            Console.CancelKeyPress += (sender, e) =>
            {
                Console.WriteLine("\nCtrl+C detected. Exiting...");
                e.Cancel = true; // Prevent immediate process termination
                _keepRunning = false; // Set flag to exit loop
            };
            
            NativeMessageHandler nativeMessageHandler = new NativeMessageHandler();

            while (_keepRunning)
            {
                    nativeMessageHandler.HandleMessages(file, _keepRunning);
            }

            handler.Save(file, "vault.json");
            Thread.Sleep(5000);
            Environment.Exit(0);
            
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