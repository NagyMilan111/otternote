namespace otternote.OutgoingMessages;

public class GetResponsePayload
{
    public string UsernameBase64 { get; set; } = "";
    public string PasswordBase64 { get; set; } = "";
}