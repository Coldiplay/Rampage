using System.Text.Json.Serialization;

namespace RampageWpf.Model
{
    public class Room
    {
        public string Number { get; set; }
        public Dictionary<string, Player> PlayerState { get; set; } = [];

        [JsonIgnore]
        public List<Player> GetPlayers => [..PlayerState.Values];

    }
}