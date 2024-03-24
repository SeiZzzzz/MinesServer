
using Newtonsoft.Json;

namespace MinesServer.GameShit.SysCraft
{
    public struct Recipie
    {
        [JsonIgnore]
        public int id { get; set; }
        public required RC result { get; init; }
        public RC[]? costres { get; init; }
        public RC[]? costcrys { get; init; }
        public required int time { get; init; }
    }
    public record struct RC(int id,int num);
}
