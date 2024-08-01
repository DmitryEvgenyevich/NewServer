using NewServer.Enums;
using Newtonsoft.Json.Linq;

namespace NewServer.Interfaces
{
    public interface IMessage
    {
        MessageType type { get; set; }
        JObject? data { get; set; }
    }
}
