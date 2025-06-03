using System.Text.Json;

namespace otternote;


public class PostPayload
{
    public string Url { get; set; } = "";
    public string MasterPassword { get; set; } = "";
    public string Password { get; set; } = "";
    public string Username { get; set; } = "";
}