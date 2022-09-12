using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SolarGames.Scripts
{
    public class Bot : AI
    {
        public string difficulty;
        public CellItem currCell;
        private int[] botBestMove;

        public Bot()
        {
            difficulty = "Easy";
        }

        public Bot(string cur_difficulty, List<CellItem> fieldItems)
        {
            difficulty = cur_difficulty;
            Field = fieldItems;
        }

        public override CellItem GetNext()
        {
            botBestMove = new int[2];
            switch (difficulty)
            {
                case "Easy":
                    GetMaxWins(Field, CellItem.CellState.isEnemy, 1);
                    currCell = Field[botBestMove[0]];
                    return Field[botBestMove[1]];
                case "Normal":
                    break;
                case "Hard":
                    break;
                default:
                    break;
            }
            return null;
        }
        
        private int GetMaxWins(List<CellItem> currField, CellItem.CellState playerTag, int findStep)
        {
            int maxWin = -1000;
            CellItem.CellState enemyTag = (playerTag == App.Game.playerMode) ? CellItem.CellState.isPlayer1 : App.Game.playerMode; //get enemy name

            foreach (CellItem cell in currField) //pass through field
            {
                if (cell.state == playerTag) //if find cell for current player
                {
                    List<CellItem> edges = cell.GetEdgesByRadius(2); //get all edges for current cell

                    foreach (CellItem item in edges) //try all variants for move
                    {
                        if (item.state == CellItem.CellState.isEmpty && item.Id != Cell.TypeOfCell.blocked_cell)
                        {
                            List<CellItem.CellState> temp = new List<CellItem.CellState>(); //copy field tags
                            foreach (CellItem tmp in currField)
                                temp.Add(tmp.state);

                            currField[item.index].SetTempState(currField[cell.index].state); //create new cell

                            Vector2Int dif = item.position - cell.position;
                            if (Mathf.Abs(dif.x) > 1 || Mathf.Abs(dif.y) > 1)
                                currField[cell.index].SetTempState(CellItem.CellState.isEmpty); //remove prev cell

                            edges = currField[item.index].GetEdgesByRadius(1); //get edges around new cell

                            foreach (CellItem edge in edges) //get win for new cell
                                if (edge.state == enemyTag)
                                    currField[edge.index].SetTempState(playerTag);

                            int winEnemy = 0;
                            if (findStep > 0) //get max win enemy on this step
                                winEnemy = GetMaxWins(currField, enemyTag, findStep - 1);

                            if (maxWin < (currField.Count(x => x.state == playerTag) - winEnemy)) //get all current player cells
                            {
                                maxWin = currField.Count(x => x.state == playerTag) - winEnemy;
                                if (playerTag == CellItem.CellState.isEnemy)
                                {
                                    botBestMove[0] = cell.index;
                                    botBestMove[1] = item.index;
                                }
                            }

                            for (int i = 0; i < currField.Count; i++) //recover field without last moves
                                currField[i].SetTempState(temp[i]);
                        }
                    }
                }
            }

            return maxWin;
        }
    }
}
