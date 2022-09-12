using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarGames.Scripts
{
    [ReceiveFrom("ObjectParams.csv")]
    class ObjectRecieve : FileReciever
    {
        public override void Recieve(DataSource dataSource)
        {
            CellWithEffect.unitWithEffects = LoadData<CellWithEffect>(dataSource["ObjectParams.csv"]);

            if (CellWithEffect.unitWithEffects.Count == 0)
                FileLoader.exception = "File ObjectParams.csv not read";
            else
                FileLoader.showGUI = false;
        }
    }
}
