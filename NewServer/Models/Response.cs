using NewServer.Enums;
using NewServer.Interfaces;

namespace NewServer.Models
{
    public class Response : IMessage
    {
        public string? requestId { get; set; }
        public MessageType type { get; set; } = MessageType.Response;
        public string? errorMessage { get; set; }
        public bool sendToClient { get; set; } = true;
        public string? data { get; set; }
    }
}
