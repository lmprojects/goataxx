using SolarGames.Async;
using SolarGames.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SolarGames.Scripts
{
    public class CellItem : MonoBehaviour
    {
        public Cell.TypeOfCell Id;
        public int score;
        public Cell.PassabilityState Passability;
        public CellWithEffect effectItem;
        
        public CellState state;
        public int index;
        public Vector2Int position;
        public int life_time;
        public bool isPlayer;
        public int bonusScore = 0;

        private Unit unit;
        private Unit.TypeOfCaptureRule captureRule;
        private bool isSelected;


        #region Properties
        public Sprite Sprite
        {
            get { return GetComponent<Button>().targetGraphic.GetComponent<Image>().sprite; }
            set { GetComponent<Button>().targetGraphic.GetComponent<Image>().sprite = value; }
        }

        public Sprite BackSprite
        {
            get { return GetComponent<Button>().GetComponentsInChildren<Image>().First(x => x.GetComponentInChildren<UIView>() != null && x.GetComponentInChildren<UIView>().ViewName == "player_back").sprite; }
            set { GetComponent<Button>().GetComponentsInChildren<Image>().First(x => x.GetComponentInChildren<UIView>() != null && x.GetComponentInChildren<UIView>().ViewName == "player_back").sprite = value; }
        }

        public Sprite ChestSprite
        {
            get {
                Image img = GetComponentsInChildren<Image>().First(x => x.name == "ChestCell");
                if (transform.childCount > 2 && img != null)
                    return img.sprite;
                else
                    return null;
            }
            set {
                if (transform.childCount > 2)
                    GetComponentsInChildren<Image>().First(x => x.name == "ChestCell").sprite = value;
            }
        }

        public Image Image
        {
            get { return GetComponent<Button>().GetComponentsInChildren<Image>().First(x => x.GetComponentInChildren<UIView>() != null && x.GetComponentInChildren<UIView>().ViewName == "player_back"); }
            set { }
        }

        public bool IsCellSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                if (transform.childCount > 1)
                    transform.GetChild(0).GetComponent<Image>().gameObject.SetActive(isSelected);
            }
        }
        
        public bool IsMovable
        {
            get
            {
                if (GetAllEdges().Count(x => x.state == CellState.isEmpty && x.Passability != Cell.PassabilityState.total_block) > 0)
                    return true;
                else
                    return false;
            }
        }

        public Unit Unit
        {
            get { return unit; }
            set
            {
                unit = value;
                Sprite = Unit.GetSpriteById(unit.Id);
                life_time = 0;
                string sprite_name = (state == CellState.isPlayer1) ? "pl1_back" : "pl2_back";
                BackSprite = Resources.Load<Sprite>("Icons/UnitIcons/" + sprite_name);
            }
        }

        public Unit.TypeOfCaptureRule CellCaptureRule
        {
            get { return captureRule; }
            set { captureRule = value; }
        }
        #endregion

        #region StateMethods
        public enum CellState
        {
            isEmpty,
            isPlayer1,
            isPlayer2,
            isEnemy,
            isWait
        }
        public void SetTempState(CellState new_state) //for bot search
        {
            state = new_state;
        }

        public void SetEmpty() //for empty state
        {
            state = CellState.isEmpty;
            Sprite = UI.empty;
            BackSprite = UI.empty;
            IsCellSelected = false;
            gameObject.name = state.ToString();
            isPlayer = true;
            unit = null;
        }

        public void SetState(CellState new_state)
        {
            Unit = Unit.GetUnitByType(Unit.TypeOfUnit.Base_unit);
            SetState(new_state, Unit);
        }

        public void SetState(CellState new_state, Unit new_unit)
        {
            state = new_state;
            Unit = new_unit;

            gameObject.name = state.ToString();

            if (state == CellState.isPlayer1 || state == CellState.isPlayer2 || state == CellState.isEnemy)
                isPlayer = true;
            else
                isPlayer = false;

            IsCellSelected = false;
        }

        public void SetWaitState()
        {
            state = CellState.isWait;
            IsCellSelected = true;
            BackSprite = UI.empty;
            Sprite = UI.empty;
        }

        public void SetWaitState(int radius)
        {
            foreach (CellItem item in GetEdgesByRadius(radius)) //show variants
                if (item.Passability != Cell.PassabilityState.total_block && item.state == CellState.isEmpty)
                    item.SetWaitState();

            if(radius > 1)
            {
                foreach (CellItem item in GetEdgesByRadius(2)) //delete variant under wall
                {
                    if (item.Passability == Cell.PassabilityState.high_wall)
                    {
                        int x = (item.position.x < position.x) ? -1 : 1;
                        int y = (item.position.y < position.y) ? -1 : 1;

                        if (item.position.x == position.x)
                            x = 0;
                        if (item.position.y == position.y)
                            y = 0;

                        if (x == 0 && y != 0)
                        {
                            SetEmptyHighWall(item.position.x, item.position.y + y);
                            SetEmptyHighWall(item.position.x + 1, item.position.y + y);
                            SetEmptyHighWall(item.position.x - 1, item.position.y + y);
                        }
                        else if (y == 0 && x != 0)
                        {
                            SetEmptyHighWall(item.position.x + x, item.position.y);
                            SetEmptyHighWall(item.position.x + x, item.position.y + 1);
                            SetEmptyHighWall(item.position.x + x, item.position.y - 1);
                        }
                        else
                        {
                            SetEmptyHighWall(item.position.x + x, item.position.y + y);
                            SetEmptyHighWall(item.position.x + x, item.position.y);
                            SetEmptyHighWall(item.position.x, item.position.y + y);
                        }
                    }
                }
            }
        }
        #endregion

        #region Edges
        public enum EdgePositionEnum
        {
            Top,
            Right,
            Bot,
            Left,
        }

        public Dictionary<EdgePositionEnum, List<CellItem>> Edges { get; private set; }

        public Unit.TypeOfCaptureRule CaptureRule
        {
            get
            {
                return captureRule;
            }

            set
            {
                captureRule = value;
            }
        }

        public void AddEdge(EdgePositionEnum side, CellItem item)
        {
            if (Edges.ContainsKey(side))
            {
                if (Edges[side] == null)
                    Edges[side] = new List<CellItem>();
                Edges[side].Add(item);
            }
        }

        public List<CellItem> GetEdgesByRadius(int radius)
        {
            List<CellItem> allEdges = new List<CellItem>();
            allEdges.AddRange(Edges[EdgePositionEnum.Top].Where(x => !allEdges.Contains(x)));
            allEdges.AddRange(Edges[EdgePositionEnum.Bot].Where(x => !allEdges.Contains(x)));
            allEdges.AddRange(Edges[EdgePositionEnum.Left].Where(x => !allEdges.Contains(x)));
            allEdges.AddRange(Edges[EdgePositionEnum.Right].Where(x => !allEdges.Contains(x)));

            for(int i = 0; i < allEdges.Count; i++)
            {
                Vector2 dif = allEdges[i].position - position;
                if (Mathf.Abs(dif.x) > radius || Mathf.Abs(dif.y) > radius)
                {
                    allEdges.Remove(allEdges[i]);
                    i--;
                }
            }
            return allEdges;
        }

        public List<CellItem> GetAllEdges()
        {
            List<CellItem> allEdges = new List<CellItem>();
            allEdges.AddRange(Edges[EdgePositionEnum.Top].Where(x => !allEdges.Contains(x)));
            allEdges.AddRange(Edges[EdgePositionEnum.Bot].Where(x => !allEdges.Contains(x)));
            allEdges.AddRange(Edges[EdgePositionEnum.Left].Where(x => !allEdges.Contains(x)));
            allEdges.AddRange(Edges[EdgePositionEnum.Right].Where(x => !allEdges.Contains(x)));
            return allEdges;
        }

        public void ClearEdges()
        {
            Edges = new Dictionary<EdgePositionEnum, List<CellItem>>
                {
                    { EdgePositionEnum.Top, new List<CellItem>() },
                    { EdgePositionEnum.Right, new List<CellItem>() },
                    { EdgePositionEnum.Bot, new List<CellItem>() },
                    { EdgePositionEnum.Left, new List<CellItem>() },
                };
        }
        #endregion

        #region game_methods
        public void PlayEffect()
        {
            if (effectItem == null)
                return;
            
            switch(effectItem.Effect)
            {
                case  CellWithEffect.TypeOfEffect.give_score:
                    bonusScore = Convert.ToInt32(effectItem.Value);
                    break;
                case CellWithEffect.TypeOfEffect.give_unit:
                    Unit = Unit.GetUnitByName(effectItem.Value);
                    break;
            }
        }

        public void ClearWaitCells()
        {
            foreach (CellItem item in GetAllEdges()) //clear prev variant
                if (item.state == CellState.isWait)
                    item.SetEmpty();
        }

        public void ShowVariants()
        {
            switch(Unit.MoveType)
            {
                case Unit.TypeOfMoveType.Basic:
                    SetWaitState(2);
                    break;
                case Unit.TypeOfMoveType.Neighbor:
                    SetWaitState(1);
                    break;
                case Unit.TypeOfMoveType.TwoX_Square:
                    SetWaitState(2);
                    break;
                case Unit.TypeOfMoveType.Vertical_horizontal:
                    SetWaitState(1);
                    foreach (CellItem item in GetEdgesByRadius(1)) //delete diagonal
                        if(item.state == CellState.isWait && item.position.x != position.x && item.position.y != position.y)
                            item.SetEmpty();
                    break;
                case Unit.TypeOfMoveType.Immobile:
                    break;
            }
        }

        private void SetEmptyHighWall(int x, int y)
        {
            CellItem cell = GetEdgesByRadius(2).Find((nextCell) => nextCell.position == new Vector2Int(x, y));
            if (cell != null && cell.Id != Cell.TypeOfCell.blocked_cell && cell.state == CellState.isWait)
                cell.SetEmpty();
        }

        internal void DoMove(CellItem newCell, List<CellItem> field, Action OnCompleted)
        {
            ClearWaitCells();
            newCell.SetState(state, Unit);//add new player cell
            field[newCell.index] = newCell;

            Vector2Int dif = newCell.position - position; //if this jump remove prev cell
            if (Mathf.Abs(dif.x) > 1 || Mathf.Abs(dif.y) > 1 || Unit.MoveType != Unit.TypeOfMoveType.Basic)
                SetEmpty();

            field[index] = this;

            IsCellSelected = false;
            newCell.IsCellSelected = true;


            OnCompleted.Invoke();
        }

        public List<CellItem> GetWins()
        {
            List<CellItem> result = new List<CellItem>();

            foreach (CellItem item in GetEdgesByRadius(1)) //show variants
                if (item.isPlayer && item.state != state && item.state != CellState.isEmpty && item.CellCaptureRule == Unit.TypeOfCaptureRule.Default)
                    result.Add(item);

            return result;
        }
        #endregion
    }
}
