namespace otternote.OutgoingMessages;

public class BaseResponse
{
    public string Type { get; set; } = "";
    public string Status { get; set; } = "ok"; // or "error"
}

