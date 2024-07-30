using NewServer.Enums;

namespace NewServer.Interfaces
{
    public interface IMessage
    {
        MessageType type { get; set; }
        string? data { get; set; }
    }
}
