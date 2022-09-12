using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarGames.Scripts
{
    public class MatchParams : BaseItem
    {
        [DataName("win score")]
        public int WinScore { get; set; }

        public static List<MatchParams> matchParams = new List<MatchParams>();
    }
}
