namespace encryptionTest;

public class JsonEntry
{
    public string Site {get; set;}
    public CipherEntry Password {get; set;}
    public CipherEntry Username {get; set;}

    public JsonEntry(string site, CipherEntry password, CipherEntry username)
    {
        this.Site = site;
        this.Password = password;
        this.Username = username;
    }
    
}