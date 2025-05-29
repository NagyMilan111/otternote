namespace otternote;

public class CipherEntry
{
    public string Iv {get; set;}
    public string CipherText {get; set;}

    public CipherEntry(string iv, string cipherText)
    {
        this.Iv = iv;
        this.CipherText = cipherText;
    }
}