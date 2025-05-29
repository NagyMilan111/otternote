using System.Text;
using System.Text.Json;

namespace encryptionTest;

public class NativeMessageSender
{
    private readonly Stream output;

    public NativeMessageSender(Stream output)
    {
        this.output = output;
    }

    public void SendMessage<T>(T message)
    {
        string json = JsonSerializer.Serialize(message);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
        byte[] lengthPrefix = BitConverter.GetBytes(jsonBytes.Length);

        output.Write(lengthPrefix, 0, 4);
        output.Write(jsonBytes, 0, jsonBytes.Length);
        output.Flush();
    }
}