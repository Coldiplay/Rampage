using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Rampage.Model
{
    public class Player
    {
        public string Name { get; set; }
        public int HP { get; set; } = 10;

        [JsonIgnore]
        public bool IsAlive => HP > 0;
    }
}
