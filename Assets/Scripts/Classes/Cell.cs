using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarGames.Scripts
{
    public class Cell : BaseItem
    {
        [DataName("id")]
        public TypeOfCell Id { get; set; }
        [DataName("Passability")]
        public PassabilityState Passability { get; set; }
        [DataName("Score")]
        public int Score { get; set; }

        public enum PassabilityState
        {
            total_block,
            free,
            swamp,
            high_wall
        }
        public enum TypeOfCell
        {
            empty,
            blocked_cell,
            high_wall_cell,
            rich_cell,
            score_chest,
            unit_chest
        }

        public static List<Cell> cellItems = new List<Cell>();
    }
}
