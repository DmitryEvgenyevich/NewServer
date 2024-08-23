using NewServer.Enums;
using NewServer.Interfaces;
using Newtonsoft.Json.Linq;

namespace NewServer.Models
{
    public class Notification : IMessage
    {
        public MessageType type { get; set; } = MessageType.Notification;
        public NotificationTypes typeOfNotification { get; set; }
        public JToken? data { get; set; }
    }
}
