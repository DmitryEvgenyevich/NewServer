using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace NewServer.Models
{
    [Table("files")]
    public class Files : BaseModel
    {
        [PrimaryKey("id", false)]
        public int id { get; set; }

        [Column("path")]
        public string? path { get; set; }
    }
}
