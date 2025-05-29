namespace otternote.Services;

using System.Security.Cryptography;

public class KeyDerivationService
{
    private readonly int _iterations;
    private readonly int _keySize;

    public KeyDerivationService(int iterations = 100_000, int keySize = 32)
    {
        _iterations = iterations;
        _keySize = keySize;
    }

    public byte[] DeriveKey(string password, byte[] salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, _iterations, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(_keySize);
    }
}
