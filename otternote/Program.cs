using System.Text;
using otternote.Json;
using otternote.Services;

namespace otternote;


class Program
{
    static void Main(string[] args)
    {

        SecurePasswordReaderService securePasswordReaderService = new SecurePasswordReaderService();
        
        char[] masterPasswordChars = securePasswordReaderService.ReadPasswordAsCharArray();        
        byte[] masterPasswordBytes = SecurePasswordReaderService.GetBytes(Encoding.UTF8, masterPasswordChars);
        Array.Clear(masterPasswordChars, 0, masterPasswordChars.Length);

        
        JsonHandler handler = new JsonHandler();
        try
        {
            JsonFile file = handler.Load("jsonexample.json");
        }
        catch (FileNotFoundException)
        {
            NewFileInitializer.Initialize(masterPasswordBytes);
            
            Array.Clear(masterPasswordBytes, 0, masterPasswordBytes.Length);
            
            Console.WriteLine("Initialization complete, you will need to sign in again.");
            Thread.Sleep(5000);
            Environment.Exit(0);
        }


        
        Console.ReadLine();
    }
    
}