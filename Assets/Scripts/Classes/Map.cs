using System.Collections.Generic;

namespace SolarGames.Scripts
{
    public class Map
    {
        public int Id;
        public Dictionary<VariantOfCell, List<int>> mapIndexes;

        public enum VariantOfCell
        {
            player1Units,
            player2Units,
            richCells,
            blockedCells,
            highWallCells,
            scoreChests,
            unitChests
        }

        public Map()
        {
            mapIndexes = new Dictionary<VariantOfCell, List<int>>
            {
                { VariantOfCell.player1Units, new List<int>()},
                { VariantOfCell.player2Units, new List<int>()},
                { VariantOfCell.richCells, new List<int>()},
                { VariantOfCell.blockedCells, new List<int>()},
                { VariantOfCell.highWallCells, new List<int>()},
                { VariantOfCell.scoreChests, new List<int>()},
                { VariantOfCell.unitChests, new List<int>()},
            };
        }
    }
}