using System.Collections.Generic;
using UnityEngine;

namespace SolarGames.Scripts
{
    public class Unit : BaseItem
    {
        [DataName("id")]
        public TypeOfUnit Id { get; private set; }
        [DataName("deployment")]
        public TypeOfDeployment Deployment { get; private set; }
        [DataName("moveType")]
        public TypeOfMoveType MoveType { get; private set; }
        [DataName("deployEffect")]
        public TypeOfDeployEffect DeployEffect { get; private set; }
        [DataName("value1")]
        public int Value1 { get; private set; }
        [DataName("decay_time")]
        public int DecayTime { get; private set; }
        [DataName("lifeEffect")]
        public TypeOfLifeEffect LifeEffect { get; private set; }
        [DataName("captureRule")]
        public TypeOfCaptureRule CaptureRule { get; private set; }

        #region Enums
        public enum TypeOfUnit
        {
            Base_unit,
            General,
            Enforcer,
            Rich_guy,
            Slime
        }

        public enum TypeOfDeployment
        {
            No_restriction,
            Foes_welcome,
            Friends_welcome,
            None
        }

        public enum TypeOfMoveType
        {
            Basic,
            Neighbor,
            TwoX_Square,
            Vertical_horizontal,
            Immobile
        }

        public enum TypeOfDeployEffect
        {
            Default,
            Pacifist,
            Unibomb,
            Slime_bomb,
            None
        }

        public enum TypeOfLifeEffect
        {
            Create_fortress,
            Boost_points,
            Decay,
            None
        }

        public enum TypeOfCaptureRule
        {
            Default,
            Untouchable
        }
        #endregion

        public static List<Unit> unitsList = new List<Unit>();

        public static Unit GetUnitByType(TypeOfUnit type)
        {
            return unitsList.Find(x => x.Id == type);
        }

        public static Unit GetRandomUnit()
        {
            Unit randomUnit = null;
            while (randomUnit == null)
            {
                randomUnit = unitsList[Random.Range(0, unitsList.Count)];
                if (randomUnit.Deployment == TypeOfDeployment.None)
                    randomUnit = null;
            }
            return randomUnit;
        }

        public static Sprite GetSpriteById(TypeOfUnit Id)
        {
            Sprite sprite = null;
            switch (Id)
            {
                case TypeOfUnit.Base_unit:
                    sprite = Resources.Load<Sprite>("Icons/FieldIcons/empty");
                    break;
                case TypeOfUnit.Enforcer:
                    sprite = Resources.Load<Sprite>("Icons/UnitIcons/enforcer");
                    break;
                case TypeOfUnit.General:
                    sprite = Resources.Load<Sprite>("Icons/UnitIcons/general");
                    break;
                case TypeOfUnit.Rich_guy:
                    sprite = Resources.Load<Sprite>("Icons/UnitIcons/rich_guy");
                    break;
                case TypeOfUnit.Slime:
                    sprite = Resources.Load<Sprite>("Icons/UnitIcons/slime");
                    break;
                default:
                    sprite = Resources.Load<Sprite>("Icons/FieldIcons/empty");
                    break;
            }
            return sprite;
        }

        public static Unit GetUnitByName(string Id)
        {
            switch (Id)
            {
                case "Base_unit":
                    return GetUnitByType(TypeOfUnit.Base_unit);
                case "Enforcer":
                    return GetUnitByType(TypeOfUnit.Enforcer);
                case "General":
                    return GetUnitByType(TypeOfUnit.General);
                case "Rich_guy":
                    return GetUnitByType(TypeOfUnit.Rich_guy);
                case "Slime":
                    return GetUnitByType(TypeOfUnit.Slime);
                default:
                    return GetUnitByType(TypeOfUnit.Base_unit);
            }
        }
    }
}