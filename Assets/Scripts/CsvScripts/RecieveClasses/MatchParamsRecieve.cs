using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarGames.Scripts
{
    [ReceiveFrom("MatchParams.csv")]
    class MatchParamsRecieve : FileReciever
    {
        public override void Recieve(DataSource dataSource)
        {
            MatchParams.matchParams = LoadData<MatchParams>(dataSource["MatchParams.csv"]);

            if (MatchParams.matchParams.Count == 0)
                FileLoader.exception = "File MatchParams.csv not read";
            else
                FileLoader.showGUI = false;
        }
    }
}
