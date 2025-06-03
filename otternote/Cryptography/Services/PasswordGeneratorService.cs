using System.Security.Cryptography;

namespace otternote.Cryptography;


public class PasswordGeneratorService
{
    // Define character sets
    private const string LowerCaseChars = "abcdefghijklmnopqrstuvwxyz";
    private const string UpperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string NumericChars = "0123456789";
    private const string SpecialChars = "!@#$%^&*()-_=+";

    public string GeneratePassword()
    {
        int length = 16;

        // Create a single character set that includes all required types
        string allChars = LowerCaseChars + UpperCaseChars + NumericChars + SpecialChars;

        // Use cryptographic random number generator
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            var passwordChars = new char[length];

            // Ensure we have at least one character from each required set
            passwordChars[0] = GetRandomChar(rng, LowerCaseChars);
            passwordChars[1] = GetRandomChar(rng, UpperCaseChars);
            passwordChars[2] = GetRandomChar(rng, NumericChars);
            passwordChars[3] = GetRandomChar(rng, SpecialChars);

            // Fill the rest of the password with random characters from all sets
            for (int i = 4; i < length; i++)
            {
                passwordChars[i] = GetRandomChar(rng, allChars);
            }

            // Shuffle the characters to mix the required characters
            return new string(passwordChars.OrderBy(c => GetRandomInt(rng)).ToArray());
        }
    }

    private char GetRandomChar(RandomNumberGenerator rng, string charSet)
    {
        return charSet[GetRandomInt(rng, 0, charSet.Length)];
    }

    private int GetRandomInt(RandomNumberGenerator rng, int minValue = 0, int maxValue = int.MaxValue)
    {
        // Generate a random integer between minValue (inclusive) and maxValue (exclusive)
        uint scale = uint.MaxValue;
        while (scale == uint.MaxValue)
        {
            byte[] fourBytes = new byte[4];
            rng.GetBytes(fourBytes);
            scale = BitConverter.ToUInt32(fourBytes, 0);
        }

        return (int)(minValue + (maxValue - minValue) * (scale / (double)uint.MaxValue));
    }
}