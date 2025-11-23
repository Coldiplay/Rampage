using System.Text.Json.Serialization;

namespace RampageWpf.Model
{
    public class Player
    {
        public string Name { get; set; }
        public int HP { get; set; } = 10;

        [JsonIgnore]
        public bool IsAlive => HP > 0;
    }
}
