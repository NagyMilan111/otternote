using System.Text;

namespace otternote.Services;

public class SecurePasswordReaderService
{
    public char[] ReadPasswordAsCharArray()
    {
        var passwordChars = new List<char>();
        ConsoleKeyInfo key;

        Console.Write("Enter master password: ");
        while ((key = Console.ReadKey(intercept: true)).Key != ConsoleKey.Enter)
        {
            if (key.Key == ConsoleKey.Backspace && passwordChars.Count > 0)
            {
                passwordChars.RemoveAt(passwordChars.Count - 1);
                Console.Write("\b \b");
            }
            else if (!char.IsControl(key.KeyChar))
            {
                passwordChars.Add(key.KeyChar);
                Console.Write("*");
            }
        }

        Console.WriteLine();
        return passwordChars.ToArray();
    }
    
    public static byte[] GetBytes(Encoding encoding, char[] chars)
    {
        return encoding.GetBytes(chars, 0, chars.Length);
    }

}