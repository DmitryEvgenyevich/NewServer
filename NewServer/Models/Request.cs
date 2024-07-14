using NewServer.Server;
using Newtonsoft.Json.Linq;

namespace NewServer.Models
{
    public class Request
    {
        public string? requestId { get; set; }
        public JObject? data { get; set; }
        public string? command { get; set; }
        public Echo? Socket { get; set; }
    }
}
