using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace NewServer.Models
{
    [Table("user_chats")]
    class UserChats : BaseModel
    {
        [PrimaryKey("chat_id", true)]
        public int chat_id { get; set; }

        [PrimaryKey("user_id", true)]
        public int user_id { get; set; }

        [Column("last_message")]
        public int? last_message { get; set; }
    }
}