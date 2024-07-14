using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace NewServer.Models
{
    [Table("users")]
    public class User : BaseModel
    {
        [PrimaryKey("id", false)]
        public int id { get; set; }
        
        [Column("username")]
        public string? username { get; set; }

        [Column("email")]
        public string? email { get; set; }

        [Column("password")]
        public string? password { get; set; }
        
        [Column("last_login")]
        public DateTimeOffset last_login { get; set; }

        [Column("created_at")]
        public DateTimeOffset created_at { get; set; }

        [Column("description")]
        public string? description { get; set; }

        [Column("avatar_id")]
        public int avatar_id { get; set; }
    }
}
