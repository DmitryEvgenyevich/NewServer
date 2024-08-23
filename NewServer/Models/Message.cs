using Supabase.Postgrest.Attributes;
using NewServer.Enums;

namespace NewServer.Models
{
    [Table("messages")]
    internal class Message
    {
        [PrimaryKey("id", false)]
        public int id { get; set; }

        [Column("sender_id")]
        public int sender_id { get; set; }

        [Column("message")]
        public string? message { get; set; }

        [Column("time")]
        public DateTime time { get; set; }

        [Column("user_chat_id")]
        public int user_chat_id { get; set; }

        [Column("status_of_message")]
        public StatusesOfMessage status_of_message { get; set; }

        [Column("type_of_message")]
        public TypesOfMessage type_of_message { get; set; }

        [Column("file_id")]
        public int? file_id { get; set; }
    }
}
