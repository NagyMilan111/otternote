namespace otternote.OutgoingMessages;

public class Response<T> : BaseResponse
{
    public T Payload { get; set; }
}
