using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace NewServer.Models
{
    [Table("user_chats")]
    public class UserChats : BaseModel
    {
        [Column("id")]
        public int id { get; set; }

        [PrimaryKey("chat_id", true)]
        public int chat_id { get; set; }

        [PrimaryKey("user_id", true)]
        public int user_id { get; set; }

        [Column("first_unread_message_id")]
        public int? first_unread_message_id { get; set; }
        
        [Column("is_muted")]
        public bool is_muted { get; set; }
    }
}