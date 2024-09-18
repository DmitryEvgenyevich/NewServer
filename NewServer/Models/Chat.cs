using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using NewServer.Enums;

namespace NewServer.Models
{
    [Table("chats")]
    public class Chat : BaseModel
    {
        [PrimaryKey("id", false)]
        public int id { get; set; }

        [Column("type_id")]
        public TypesOfChat type_id { get; set; }

    }
}