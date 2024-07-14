using NewServer.Enums;
using NewServer.Interfaces;

namespace NewServer.Models
{
    public class Response : IMessage
    {
        public string? requestId { get; set; }
        public MessageType Type { get; set; } = MessageType.Response;
        public string? ErrorMessage { get; set; }
        public bool SendToClient { get; set; } = true;
        public string? Data { get; set; }
    }
}
