namespace Rampage.Model
{
    public class Room
    {
        public string Number { get; set; }
        public Dictionary<Player, int> Players { get; set; } = [];
    }
}