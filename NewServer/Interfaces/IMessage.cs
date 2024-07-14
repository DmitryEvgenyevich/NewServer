using NewServer.Enums;

namespace NewServer.Interfaces
{
    public interface IMessage
    {
        MessageType Type { get; set; }
        string? Data { get; set; }
    }
}
