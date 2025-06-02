using System.Text.Json;
using otternote.Cryptography;
using otternote.Json;
using otternote.Services;

namespace otternote;


public class NativeMessageHandler
{
    NativeMessageReceiver receiver = new NativeMessageReceiver(Console.OpenStandardInput());
    NativeMessageSender sender = new NativeMessageSender(Console.OpenStandardOutput());
    
    public void HandleMessages(JsonFile file)
    {
        KeyDerivationService keyDerivationService = new KeyDerivationService();
        EncryptionService encryptionService = new EncryptionService();
        VaultAuthenticator vaultAuthenticator = new VaultAuthenticator(keyDerivationService, encryptionService);
        
        byte[] salt = Convert.FromBase64String(file.Header["salt"]);
        byte[] encryptedVaultCheck = Convert.FromBase64String(file.Header["vault_check"]);
        
        while (true)
        {
            NativeMessage msg = receiver.ReadMessage<NativeMessage>();
            if (msg == null) break;

            switch (msg.Type)
            {
                case "get":
                    GetPayload getPayload = JsonSerializer.Deserialize<GetPayload>(msg.Payload);
                    if (vaultAuthenticator.ValidateMasterPassword(getPayload.MasterPassword, salt, encryptedVaultCheck))
                    {
                        foreach (JsonEntry entry in file.Entries)
                        {
                            if (entry.Site.Equals(getPayload.Url))
                            {
                                sender.SendMessage(msg);
                            }
                        }
                    }
                    else
                    {
                        break;
                    }

                    break;
            }

        }
    }
}