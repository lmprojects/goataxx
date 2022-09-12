using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SolarGames.Scripts
{
    [ReceiveFrom("CellsParams.csv")]
    class CellsRecieve : FileReciever
    {
        public override void Recieve(DataSource dataSource)
        {
            Cell.cellItems = LoadData<Cell>(dataSource["CellsParams.csv"]);

            if (Cell.cellItems.Count == 0)
                FileLoader.exception = "File CellsParams.csv not read";
            else
                FileLoader.showGUI = false;
        }
    }
}
