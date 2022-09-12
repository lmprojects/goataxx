using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarGames.Scripts
{
    [ReceiveFrom("UnitParams.csv")]
    class UnitRecieve : FileReciever
    {
        public override void Recieve(DataSource dataSource)
        {
            Unit.unitsList = LoadData<Unit>(dataSource["UnitParams.csv"]);

            if (Unit.unitsList.Count == 0)
                FileLoader.exception = "File UnitParams.csv not read";
            else
                FileLoader.showGUI = false;
        }
    }
}
