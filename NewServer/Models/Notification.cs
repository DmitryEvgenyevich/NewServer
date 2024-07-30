using NewServer.Enums;
using NewServer.Interfaces;

namespace NewServer.Models
{
    public class Notification : IMessage
    {
        public MessageType type { get; set; } = MessageType.Notification;
        public NotificationTypes typeOfNotification { get; set; }
        public string? data { get; set; }
    }
}
