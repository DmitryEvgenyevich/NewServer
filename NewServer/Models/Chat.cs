using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using NewServer.Enums;

namespace NewServer.Models
{
    [Table("chats")]
    class Chat : BaseModel
    {
        [PrimaryKey("id", false)]
        public int id { get; set; }

        [Column("chat_type")]
        public TypesOfChat chat_type { get; set; }

        [Column("chat_name")]
        public string? chat_name { get; set; }

        [Column("avatar")]
        public string? avatar { get; set; }

    }
}