using System.Text;
using System.Text.Json;
using otternote.Cryptography;
using otternote.Json;
using otternote.OutgoingMessages;
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
        byte[] vaultCheckIV = Convert.FromBase64String(file.Header["vault_check_iv"]);

        while (true)
        {
            NativeMessage msg = receiver.ReadMessage<NativeMessage>();
            if (msg == null) break;

            switch (msg.Type)
            {
                case "get":
                    GetPayload getPayload = JsonSerializer.Deserialize<GetPayload>(msg.Payload);
                    
                    //Maybe the storing of the master password could be better
                    byte[] masterPassword = Encoding.UTF8.GetBytes(getPayload.MasterPassword);
                    if (vaultAuthenticator.ValidateMasterPassword(masterPassword, salt, encryptedVaultCheck, vaultCheckIV))
                    {
                        foreach (JsonEntry entry in file.Entries)
                        {
                            if (entry.Site.Equals(getPayload.Url))
                            {
                                byte[] key = keyDerivationService.DeriveKey(masterPassword, salt);
                                byte[] passwordText = Encoding.UTF8.GetBytes(entry.Password.CipherText);
                                byte[] passwordIv = Encoding.UTF8.GetBytes(entry.Password.Iv);
                                
                                byte[] usernameText = Encoding.UTF8.GetBytes(entry.Username.CipherText);
                                byte[] usernameIv = Encoding.UTF8.GetBytes(entry.Username.Iv);
                                
                                string base64Password = Convert.ToBase64String(encryptionService.Decrypt(passwordText, key, passwordIv));
                                string base64Username = Convert.ToBase64String(encryptionService.Decrypt(usernameText, key, usernameIv));
                                
                                Response<GetResponsePayload> getResponsePayload = new Response<GetResponsePayload>();
                                getResponsePayload.Type = "getResponse";
                                getResponsePayload.Status = "ok";
                                getResponsePayload.Payload.PasswordBase64 = base64Password;
                                getResponsePayload.Payload.UsernameBase64 = base64Username;
                                
                                //Untested!!!!
                                sender.SendMessage(getResponsePayload);
                            }
                        }
                    }
                    break;
                
                case "post":

                    
                    break;
            }

        }
    }
}