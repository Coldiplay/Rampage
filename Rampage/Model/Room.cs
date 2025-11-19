namespace Rampage.Model
{
    public class Room
    {
        public string Number { get; set; }
        public Dictionary<string, Player> PlayerState { get; set; } = [];
    }
}