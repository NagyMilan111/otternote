namespace otternote;

using System.Security.Cryptography;

public class RandomGenerator
{
    public byte[] GenerateBytes(int length)
    {
        byte[] bytes = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return bytes;
    }
}
