using NewServer.Enums;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace NewServer.Models
{
    [Table("messages")]
    public class Message : BaseModel
    {
        [PrimaryKey("id", false)]
        public int id { get; set; }

        [Column("user_chat_id")]
        public int user_chat_id { get; set; }

        [Column("text")]
        public string? text { get; set; }

        [Column("sent_at")]
        public DateTimeOffset sent_at { get; set; }

        [Column("type_id")]
        public TypesOfMessage type_id { get; set; }

    }
}