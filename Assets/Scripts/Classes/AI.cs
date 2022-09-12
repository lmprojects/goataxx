using System.Collections.Generic;

namespace SolarGames.Scripts
{
    public abstract class AI
    {
        public List<CellItem> Field { get; set; }

        public virtual CellItem GetNext()
        {
            return Field[0];
        }
    }
}
