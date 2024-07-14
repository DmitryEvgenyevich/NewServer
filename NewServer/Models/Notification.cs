using NewServer.Enums;
using NewServer.Interfaces;

namespace NewServer.Models
{
    public class Notification : IMessage
    {
        public MessageType Type { get; set; } = MessageType.Notification;
        public NotificationTypes TypeOfNotification { get; set; }
        public string? Data { get; set; }
    }
}
