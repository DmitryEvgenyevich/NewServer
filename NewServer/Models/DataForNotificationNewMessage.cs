using NewServer.Enums;

namespace NewServer.Models
{
    public class DataForNotificationNewMessage
    {
        public string text { get; set; }
        public int chat_id { get; set; }
        public string chat_title { get; set; }
        public int recipient_id { get; set; }
        public DateTimeOffset sent_at { get; set; }
        public int message_id { get; set; }
        public int type { get; set; }
        public string username { get; set; }
        public StatusesOfMessage is_read { get; set; }
    }
}
