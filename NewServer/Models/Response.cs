using NewServer.Enums;
using NewServer.Interfaces;
using Newtonsoft.Json.Linq;

namespace NewServer.Models
{
    public class Response : IMessage
    {
        public string? requestId { get; set; }
        public MessageType type { get; set; } = MessageType.Response;
        public string? errorMessage { get; set; }
        public bool sendToClient { get; set; } = true;
        public JObject? data { get; set; }
    }
}
