using System.Text;
using System.Text.Json;

namespace otternote;

public class NativeMessageReceiver
{
    private readonly Stream input;

    public NativeMessageReceiver(Stream input)
    {
        this.input = input;
    }
    
    public T? ReadMessage<T>()
    {
        var lengthBytes = new byte[4];
        int bytesRead = input.Read(lengthBytes, 0, 4);
        if (bytesRead == 0) return default;

        int messageLength = BitConverter.ToInt32(lengthBytes, 0);
        var buffer = new byte[messageLength];
        input.Read(buffer, 0, messageLength);

        string json = Encoding.UTF8.GetString(buffer);
        return JsonSerializer.Deserialize<T>(json);
    }
}