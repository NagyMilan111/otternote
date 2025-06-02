using System.Text.Json;

namespace otternote;


public class NativeMessage
{
    public string Type { get; set; } = "";
    public JsonElement Payload { get; set; }
}
