using System.Collections.Generic;

namespace SolarGames.Scripts
{
    public class HistoryRecord
    {
        public List<int> Player1Indexes { get; set; }
        public List<int> Player2Indexes { get; set; }

        public HistoryRecord(IEnumerable<int> player1, IEnumerable<int> player2)
        {
            Player1Indexes = new List<int>(player1);
            Player2Indexes = new List<int>(player2);
        }

        public void ClearHistory()
        {
            Player1Indexes = new List<int>();
            Player2Indexes = new List<int>();
        }
    }
}
