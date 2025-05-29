using System.Text.Json;

namespace encryptionTest;

public class NativeMessage
{
    public string Type { get; set; } = "";
    public JsonElement Payload { get; set; }
}
