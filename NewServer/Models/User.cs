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
        public DateTimeOffset? last_login { get; set; }

        [Column("created_at")]
        public DateTimeOffset created_at { get; set; }

        [Column("description")]
        public string? description { get; set; }

        [Column("avatar_id")]
        public int? avatar_id { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is User))
                return false;

            User other = (User)obj;
            return this.email == other.email && this.username == other.username;
        }

        public override int GetHashCode()
        {
            // Используем простую комбинацию хэш-кодов полей
            return HashCode.Combine(username, email);
        }
    }
}
