using System.Text.Json.Serialization;

namespace RampageWpf.Model
{
    public class RoomAction
    {
        public string Actor {  get; set; }
        public string GroupId { get; set; }
        public int ActionType { get; set; }
        public string Target { get; set; }

        [JsonIgnore]
        public string GetActionName => ActionType switch
        {
            1 => "Атака",
            2 => "Защита",
            _ => "Ошибка"
        };
    }
}