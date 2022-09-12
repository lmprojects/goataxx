using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SolarGames.Scripts.Cell;

namespace SolarGames.Scripts
{
    public class CellWithEffect : BaseItem
    {
        [DataName("id")]
        public TypeOfCell Id { get; private set; }
        [DataName("effect")]
        public TypeOfEffect Effect { get; private set; }
        [DataName("value1")]
        public string Value { get; private set; }

        public enum TypeOfEffect
        {
            give_score,
            give_unit
        }

        public static List<CellWithEffect> unitWithEffects = new List<CellWithEffect>();

        public static CellWithEffect GetEffectByType(TypeOfCell type)
        {
            CellWithEffect effect = unitWithEffects.Find(x => x.Id == type);
            if (effect == null)
                return null;

            CellWithEffect rezult = new CellWithEffect();
            rezult.Id = effect.Id;
            rezult.Effect = effect.Effect;
            rezult.Value = Unit.GetRandomUnit().Id.ToString();
            return rezult;
        }
    }
}
