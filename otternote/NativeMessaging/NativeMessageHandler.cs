using System.Security.Cryptography;
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
    
    public void HandleMessages(JsonFile file, bool _keepRuning)
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
            if (msg == null || !_keepRuning) break;

            switch (msg.Type)
            {
                case "get":
                {
                    GetPayload getPayload = JsonSerializer.Deserialize<GetPayload>(msg.Payload);

                    //Maybe the storing of the master password could be better
                    byte[] masterPassword = Encoding.UTF8.GetBytes(getPayload.MasterPassword);
                    if (vaultAuthenticator.ValidateMasterPassword(masterPassword, salt, encryptedVaultCheck,
                            vaultCheckIV))
                    {
                        foreach (JsonEntry entry in file.Entries)
                        {
                            if (entry.Site.Equals(getPayload.Url))
                            {
                                byte[] key = keyDerivationService.DeriveKey(masterPassword, salt);
                                Array.Clear(masterPassword, 0, masterPassword.Length);
                                byte[] passwordText = Encoding.UTF8.GetBytes(entry.Password.CipherText);
                                byte[] passwordIv = Encoding.UTF8.GetBytes(entry.Password.Iv);

                                byte[] usernameText = Encoding.UTF8.GetBytes(entry.Username.CipherText);
                                byte[] usernameIv = Encoding.UTF8.GetBytes(entry.Username.Iv);

                                string base64Password =
                                    Convert.ToBase64String(encryptionService.Decrypt(passwordText, 
                                        key, passwordIv));
                                string base64Username =
                                    Convert.ToBase64String(encryptionService.Decrypt(usernameText, 
                                        key, usernameIv));

                                Array.Clear(key, 0, key.Length);

                                Response<GetResponsePayload> getResponsePayload = new Response<GetResponsePayload>();
                                getResponsePayload.Type = "getResponse";
                                getResponsePayload.Status = "ok";
                                getResponsePayload.Payload.PasswordBase64 = base64Password;
                                getResponsePayload.Payload.UsernameBase64 = base64Username;

                                base64Password = "";
                                base64Username = "";

                                //Untested!!!!
                                sender.SendMessage(getResponsePayload);
                            }
                        }
                    }

                    break;
                }

                case "post":
                {
                    PostPayload postPayload = JsonSerializer.Deserialize<PostPayload>(msg.Payload);
                    byte[] masterPassword = Encoding.UTF8.GetBytes(postPayload.MasterPassword);
                    if (vaultAuthenticator.ValidateMasterPassword(masterPassword, salt, encryptedVaultCheck,
                            vaultCheckIV))
                    {
                        byte[] key = keyDerivationService.DeriveKey(masterPassword, salt);
                        Array.Clear(masterPassword, 0, masterPassword.Length);
                        EntryEncryptor entryEncryptor = new EntryEncryptor();
                        JsonEntry encryptedEntry = entryEncryptor.EncryptEntry(postPayload.Username, 
                            postPayload.Password, postPayload.Url, key);
                        Array.Clear(key, 0, key.Length);
                        JsonHandler handler = new JsonHandler();

                        var existingIndex = file.Entries.FindIndex(e => e.Site == postPayload.Url);
                        if (existingIndex != -1)
                        {
                            file.Entries[existingIndex] = encryptedEntry;
                        }
                        else
                        {
                            file.Entries.Add(encryptedEntry);
                        }

                        BaseResponse response = new BaseResponse();
                        response.Type = "post";
                        response.Status = "ok";
                        
                        sender.SendMessage(response);

                    }
                    
                    break;
                }

                case "delete":
                {
                    DeletePayload deletePayload = JsonSerializer.Deserialize<DeletePayload>(msg.Payload);
                    byte[] masterPassword = Encoding.UTF8.GetBytes(deletePayload.MasterPassword);
                    if (vaultAuthenticator.ValidateMasterPassword(masterPassword, salt, encryptedVaultCheck,
                            vaultCheckIV))
                    {
                        Array.Clear(masterPassword, 0, masterPassword.Length);
                        
                        JsonHandler handler = new JsonHandler();
                        
                        handler.Delete(deletePayload.Url, file);

                        BaseResponse response = new BaseResponse();
                        response.Type = "delete";
                        response.Status = "ok";
                        
                        sender.SendMessage(response);
                    }


                    break;
                }

                case "get/generate-password":
                {
                    GetPayload getPayload = JsonSerializer.Deserialize<GetPayload>(msg.Payload);
                    byte[] masterPassword = Encoding.UTF8.GetBytes(getPayload.MasterPassword);
                    if (vaultAuthenticator.ValidateMasterPassword(masterPassword, salt, encryptedVaultCheck,
                            vaultCheckIV))
                    {
                        Array.Clear(masterPassword, 0, masterPassword.Length);
                        PasswordGeneratorService passwordGeneratorService = new PasswordGeneratorService();

                        Response<GeneratePasswordResponse> response = new Response<GeneratePasswordResponse>();
                        response.Type = "get/generate-password";
                        response.Status = "ok";
                        response.Payload.password = passwordGeneratorService.GeneratePassword();
                        
                        sender.SendMessage(response);
                    }


                    break;
                }

            }

        }
    }
}