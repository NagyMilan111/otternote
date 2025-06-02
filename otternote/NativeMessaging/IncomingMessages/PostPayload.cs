using System.Text.Json;

namespace otternote;


public class PostPayload
{
    private string Url { get; set; } = "";
    private byte[] MasterPassword { get; set; } = [];
    private string Password { get; set; } = "";
    private string Username { get; set; } = "";
}