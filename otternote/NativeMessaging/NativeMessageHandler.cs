using System.Text.Json;

namespace otternote;


public class NativeMessageHandler
{
    NativeMessageReceiver receiver = new NativeMessageReceiver(Console.OpenStandardInput());
    NativeMessageSender sender = new NativeMessageSender(Console.OpenStandardOutput());
    
    public void HandleMessages()
    {
        while (true)
        {
            NativeMessage msg = receiver.ReadMessage<NativeMessage>();
            if (msg == null) break;

            switch (msg.Type)
            {
                case "get":
                    GetPayload getPayload = JsonSerializer.Deserialize<GetPayload>(msg.Payload);
                    
                    
                    
                    break;
            }

        }
    }
}