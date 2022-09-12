using SolarGames.Async;
using SolarGames.Context;
using SolarGames.FSM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SolarGames.Scripts
{
    public class UI : MonoBehaviour
    {
        #region serializeFields
        [SerializeField]
        protected int sizeField = 7;
        [SerializeField]
        protected GameObject field;
        [SerializeField]
        protected CellItem cellPref;
        [SerializeField]
        protected CellItem blockedPref;
        [SerializeField]
        protected CellItem richPref;
        [SerializeField]
        protected CellItem highWallPref;
        [SerializeField]
        protected CellItem scoreChestPref;
        [SerializeField]
        protected CellItem unitChestPref;
        [SerializeField]
        protected CellItem player1Pref;
        [SerializeField]
        protected CellItem player2Pref;
        [SerializeField]
        protected CellItem enemyPref;
        #endregion

        #region privateFields
        private Bot bot;
        private CellItem curplayer;
        private List<CellItem> allItems;
        private List<CellItem> newItems;
        private List<HistoryRecord> history;
        private PlayerScore playerScore1, playerScore2, gameOverPanel;
        private CellItem.CellState currentMoveIs;
        private CellItem.CellState playerMode;
        private bool isGameOver;
        private List<UnitCell> unitCells;
        private List<Unit.TypeOfUnit> units_player1, units_player2;

        private int win_score;
        private float timer;
        #endregion

        public static Sprite empty;

        private void Start()
        {
            App.Manager.AddEvent("restart", Restart);
            App.Manager.AddEvent("show_variants", ShowPlayerVariants);
            App.Manager.AddEvent("player_move", PlayerMove);
            App.Manager.AddEvent("change_step", ChangeStep);
            App.Manager.AddEvent("stand_unit", StandUnit);
            App.Manager.AddEvent("show_unit_variants", ShowUnitVariants);

            empty = cellPref.Sprite;

            Restart(new FSM.EventArgs());
        }
        private void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
                SceneManager.LoadScene("Menu");
            else if(Input.GetKeyUp(KeyCode.F5))
            {
                App.Game.lockMove = false;
                Fsm.GameState.currUnit = null;
                App.Manager.Invoke("restart");
            }

            #region TestUnit
            Unit.TypeOfUnit add_unit = Unit.TypeOfUnit.Base_unit;
            if (Input.GetKeyUp(KeyCode.F1))
                add_unit = Unit.TypeOfUnit.Enforcer;
            else if (Input.GetKeyUp(KeyCode.F2))
                add_unit = Unit.TypeOfUnit.General;
            else if (Input.GetKeyUp(KeyCode.F3))
                add_unit = Unit.TypeOfUnit.Rich_guy;
            else if (Input.GetKeyUp(KeyCode.F4))
                add_unit = Unit.TypeOfUnit.Slime;

            if (add_unit != Unit.TypeOfUnit.Base_unit)
            {
                List<Unit.TypeOfUnit> currUnits = (currentMoveIs == CellItem.CellState.isPlayer1) ? units_player1 : units_player2;

                if (currUnits.Count < 5)
                    currUnits.Add(add_unit);

                for (int i = 0; i < unitCells.Count; i++)
                {
                    if (i < currUnits.Count)
                    {
                        unitCells[i].unit = Unit.GetUnitByType(currUnits[i]);
                        unitCells[i].SetIcon();
                    }
                    else
                        unitCells[i].SetEmpty();
                }
            }
            
            #endregion

            App.Manager.Invoke("show_game_over", this, isGameOver);

            if (isGameOver)
            {
                timer += Time.deltaTime;
                //UIContext.GetComponent<Button>("prevStep_button").interactable = false;
                //UIContext.GetComponent<Button>("nextStep_button").interactable = false;

                if (timer > 0.8f && Input.touchCount > 0)
                    Restart(null);
            }
        }

        private void OnDestroy()
        {
            App.Manager.RemoveEvent(ShowPlayerVariants);
            App.Manager.RemoveEvent(PlayerMove);
            App.Manager.RemoveEvent(Restart);
            App.Manager.RemoveEvent(ChangeStep);
            App.Manager.RemoveEvent(StandUnit);
            App.Manager.RemoveEvent(ShowUnitVariants);
        }

        private void Restart(FSM.EventArgs args)
        {
            StopAllCoroutines();

            win_score = MatchParams.matchParams[0].WinScore;
            timer = 0;
            bot = new Bot();
            playerMode = App.Game.playerMode;
            currentMoveIs = CellItem.CellState.isPlayer1;
            allItems = new List<CellItem>();
            newItems = new List<CellItem>();
            history = new List<HistoryRecord>();

            enemyPref.Sprite = player2Pref.Sprite;

            playerScore1 = UIContext.GetComponent<PlayerScore>("player1_score");
            playerScore2 = UIContext.GetComponent<PlayerScore>("player2_score");
            gameOverPanel = UIContext.GetComponent<PlayerScore>("GO_panel");
            Text goal = UIContext.GetComponent<Text>("goal");
            goal.text = "Current goal: " + win_score; //get from csv

            Transform unitPanel = UIContext.GetComponent<HorizontalLayoutGroup>("unit_panel").transform;
            unitCells = unitPanel.GetComponentsInChildren<UnitCell>().ToList();
            units_player1 = new List<Unit.TypeOfUnit>();
            units_player2 = new List<Unit.TypeOfUnit>();

            foreach (UnitCell unit in unitCells)
                unit.SetEmpty();

       //     playerScore1.Icon = player1Pref.Sprite;
        //    playerScore2.Icon = player2Pref.Sprite;
            playerScore1.ActiveState = true;
            playerScore2.ActiveState = false;
            playerScore1.Score = 0;
            playerScore2.Score = 0;

            //UIContext.GetComponent<Button>("prevStep_button").interactable = false;
            //UIContext.GetComponent<Button>("nextStep_button").interactable = false;

            isGameOver = false;
            App.Game.lockMove = false;

            if (field.transform.childCount > 0)
                foreach (CellItem item in field.GetComponentsInChildren<CellItem>())
                    Destroy(item.gameObject);

            CreateField();
        }
        
        private void AddHistoryRecord()
        {
            history.Insert(0, new HistoryRecord(
            allItems.Where(o => o.state == CellItem.CellState.isPlayer1).Select(o => o.index),
            allItems.Where(o => o.state == playerMode).Select(o => o.index)));
        }

        private void RewriteHistoryRecord(int index)
        {
            history[index] = new HistoryRecord(
            allItems.Where(o => o.state == CellItem.CellState.isPlayer1).Select(o => o.index),
            allItems.Where(o => o.state == playerMode).Select(o => o.index));
        }

        private void ChangeStep(FSM.EventArgs args)
        {
            HistoryRecord tmp = history[1];
            history[1] = history[0];
            history[0] = tmp;

            foreach (CellItem item in allItems)
            {
                if (history[0].Player1Indexes.Contains(item.index))
                    item.SetState(CellItem.CellState.isPlayer1);
                else if (history[0].Player2Indexes.Contains(item.index))
                    item.SetState(playerMode);
                else // fix it, dont save field cell by type
                    item.SetEmpty();
            }

            UpdateScorePanel();

            if (playerMode == CellItem.CellState.isPlayer2)
                ChangePlayer();

            bool active = UIContext.GetComponent<Button>("prevStep_button").interactable;
            //UIContext.GetComponent<Button>("prevStep_button").interactable = !active; //set step to prev move not active
            //UIContext.GetComponent<Button>("nextStep_button").interactable = active; //set step to next move active
        }

        private void CreateCell(Cell.TypeOfCell Id, int index)
        {
            CellItem item = null;

            switch(Id) //DO: from csv with normal id
            {
                case Cell.TypeOfCell.high_wall_cell:
                    item = Instantiate(highWallPref.gameObject, field.transform).GetComponent<CellItem>();
                    break;
                case Cell.TypeOfCell.rich_cell:
                    item = Instantiate(richPref.gameObject, field.transform).GetComponent<CellItem>();
                    break;
                case Cell.TypeOfCell.blocked_cell:
                    item = Instantiate(blockedPref.gameObject, field.transform).GetComponent<CellItem>();
                    break;
                case Cell.TypeOfCell.score_chest:
                    item = Instantiate(scoreChestPref.gameObject, field.transform).GetComponent<CellItem>();
                    break;
                case Cell.TypeOfCell.unit_chest:
                    item = Instantiate(unitChestPref.gameObject, field.transform).GetComponent<CellItem>();
                    break;
                default:
                    item = Instantiate(cellPref.gameObject, field.transform).GetComponent<CellItem>();
                    break;
            }
            item.index = index;
            item.IsCellSelected = false;

            allItems.Add(item);
        }

        public void CreateField()
        {
            Map currentMap = App.maps[0]; //get first map

            int prev = currentMap.Id; //get last map
            App.maps.Remove(currentMap); //clear list
            if (App.maps.Count == 1) //refresh maps list
            {
                App.CreateMaps(); //recreate maps list

                if (App.maps[0].Id == prev) //if first map is current, move to end of list
                {
                    Map temp = App.maps[0];
                    App.maps.Remove(temp);
                    App.maps.Add(temp);
                }
            }

            for (int i = 0; i < sizeField * sizeField; i++) //create field
            {
                if(currentMap.mapIndexes[Map.VariantOfCell.player1Units].Contains(i))
                {
                    CreateCell(Cell.TypeOfCell.empty, i);
                    allItems[i].SetState(CellItem.CellState.isPlayer1);
                    playerScore1.Score++;
                }
                else if (currentMap.mapIndexes[Map.VariantOfCell.player2Units].Contains(i))
                {
                    CreateCell(Cell.TypeOfCell.empty, i);
                    allItems[i].SetState(playerMode);
                    playerScore2.Score++;
                }
                else if (currentMap.mapIndexes[Map.VariantOfCell.richCells].Contains(i))
                    CreateCell(Cell.TypeOfCell.rich_cell, i);
                else if (currentMap.mapIndexes[Map.VariantOfCell.blockedCells].Contains(i))
                    CreateCell(Cell.TypeOfCell.blocked_cell, i);
                else if (currentMap.mapIndexes[Map.VariantOfCell.highWallCells].Contains(i))
                    CreateCell(Cell.TypeOfCell.high_wall_cell, i);
                else if (currentMap.mapIndexes[Map.VariantOfCell.unitChests].Contains(i))
                    CreateCell(Cell.TypeOfCell.unit_chest, i);
                else if (currentMap.mapIndexes[Map.VariantOfCell.scoreChests].Contains(i))
                    CreateCell(Cell.TypeOfCell.score_chest, i);
                else
                    CreateCell(Cell.TypeOfCell.empty, i);

                if (currentMap.mapIndexes[Map.VariantOfCell.unitChests].Contains(i) && allItems[i].Passability != Cell.PassabilityState.total_block)
                {
                    allItems[i].ChestSprite = unitChestPref.ChestSprite;
                    allItems[i].effectItem = CellWithEffect.GetEffectByType(Cell.TypeOfCell.unit_chest);
                }
                else if (currentMap.mapIndexes[Map.VariantOfCell.scoreChests].Contains(i) && allItems[i].Passability != Cell.PassabilityState.total_block)
                {
                    allItems[i].ChestSprite = scoreChestPref.ChestSprite;
                    allItems[i].effectItem = CellWithEffect.unitWithEffects.FirstOrDefault(x => x.Id == Cell.TypeOfCell.score_chest);
                }
            }

            foreach (CellItem cellForDetect in allItems) //detect edges for all cells
            {
                CellItem[,] cells = new CellItem[sizeField, sizeField];
                int playerRow = cellForDetect.index / sizeField;
                int playerCol = cellForDetect.index - (playerRow * sizeField);
                int radius = 2;

                int k = 0, l = 0;
                foreach (CellItem item in allItems) //to 2d array
                {
                    if (k == sizeField)
                    {
                        k = 0;
                        l++;
                    }
                    cells[l, k] = item;
                    cells[l, k].position = new Vector2Int(l, k);
                    k++;
                }

                cellForDetect.position = new Vector2Int(playerRow, playerCol);
                cellForDetect.ClearEdges();
                for (int i = playerRow - radius; i < sizeField; i++)
                    for (int j = playerCol - radius; j < sizeField; j++)
                    {
                        if (i >= 0 && j >= 0)
                        {
                            if (i < playerRow && j >= playerCol - radius && j <= playerCol + radius) //top
                                cellForDetect.AddEdge(CellItem.EdgePositionEnum.Top, cells[i, j]);
                            if (i > playerRow && i <= playerRow + radius && j >= playerCol - radius && j <= playerCol + radius) //bot
                                cellForDetect.AddEdge(CellItem.EdgePositionEnum.Bot, cells[i, j]);
                            if (j < playerCol && i >= playerRow - radius && i <= playerRow + radius) //left
                                cellForDetect.AddEdge(CellItem.EdgePositionEnum.Left, cells[i, j]);
                            if (j > playerCol && j <= playerCol + radius && i >= playerRow - radius && i <= playerRow + radius) //right
                                cellForDetect.AddEdge(CellItem.EdgePositionEnum.Right, cells[i, j]);
                        }
                    }
            }
            
            AddHistoryRecord();
        }

        private void ClearAllWaitCells()
        {
            foreach (CellItem item in allItems)
                if (item.state == CellItem.CellState.isWait)
                    item.SetEmpty();
        }

        private void ShowUnitVariants(FSM.EventArgs args)
        {
            if (curplayer != null) //clear wait cells prev player
                curplayer.ClearWaitCells();

            UnitCell curUnit = (UnitCell)args.Args[0]; //get unit
            if (curUnit == null)
                return;

            ClearAllWaitCells(); //clear field
            curUnit.IsCellSelected = true;
            App.Game.lockMove = true;

            switch (curUnit.unit.Deployment) //show variants
            {
                case Unit.TypeOfDeployment.No_restriction:
                    foreach(CellItem item in allItems)
                    {
                        if (item.state == CellItem.CellState.isEmpty && item.Passability != Cell.PassabilityState.total_block)
                            item.SetWaitState();
                    }
                    break;
                case Unit.TypeOfDeployment.Foes_welcome:
                    {
                        CellItem.CellState enemy = (currentMoveIs == CellItem.CellState.isPlayer1) ? playerMode : CellItem.CellState.isPlayer1;
                        foreach(CellItem item in allItems)
                        {
                            if (item.state == enemy)
                                item.SetWaitState(1);
                        }
                        break;
                    }
                case Unit.TypeOfDeployment.Friends_welcome:
                    {
                        foreach (CellItem item in allItems)
                        {
                            if (item.state == currentMoveIs)
                                item.SetWaitState(1);
                        }
                        break;
                    }
                case Unit.TypeOfDeployment.None:
                    return;
            }
        }
        
        private void StandUnit(FSM.EventArgs args)
        {
            CellItem item = args.Args[0] as CellItem;
            UnitCell curUnit = args.Args[1] as UnitCell;

            curUnit.IsCellSelected = false;
            App.Game.lockMove = false;

            if (item.state != CellItem.CellState.isWait)
            {
                ClearAllWaitCells();
                return;
            }

            ClearAllWaitCells();

            allItems[item.index].SetState(currentMoveIs);
            allItems[item.index].Unit = curUnit.unit;
            curplayer = allItems[item.index];

            if (curplayer.effectItem != null && curUnit.unit.Id != Unit.TypeOfUnit.Slime) //if chest
                PlayChestEffect(curplayer);

            UnitCell selectedUnit = unitCells.First(x => x.name == curUnit.name); //delete unit from table
            selectedUnit.SetEmpty();

            List<Unit.TypeOfUnit> currUnits = (currentMoveIs == CellItem.CellState.isPlayer1) ? units_player1 : units_player2;
            int index = unitCells.IndexOf(selectedUnit);
            currUnits.RemoveAt(index); //delete unit from list

            PlayDeployEffect();
        }
        
        private void ShowPlayerVariants(FSM.EventArgs args)
        {
            if (curplayer != null) //clear prev variants
            {
                curplayer.IsCellSelected = false;
                curplayer.ClearWaitCells();
            }

            UIView view = (UIView)args.Args[0];
            curplayer = view.gameObject.GetComponent<CellItem>(); //get player
            
            if (curplayer.state != currentMoveIs) //if click to other player return
                return;

            if (curplayer.Unit != null && curplayer.Unit.MoveType == Unit.TypeOfMoveType.Immobile)
                return;

            StartCoroutine(SelectCell(curplayer)); //animation player
            curplayer.ShowVariants();
        }

        private bool PlayerIsMovable()
        {
            List<int> curPlayerList = (currentMoveIs == CellItem.CellState.isPlayer1) ? 
                history[0].Player1Indexes : history[0].Player2Indexes;
            
            foreach (int index in curPlayerList) //check all player cells
                if (allItems[index].IsMovable)
                    return true;

            return false;
        }

        private void PlayerMove(FSM.EventArgs args)
        {
            StopAllCoroutines();

            CellItem item = curplayer;
            if (currentMoveIs == CellItem.CellState.isEnemy) //get new selected cell
                item = (CellItem)args.Args[0];
            else
            {
                UIView view = (UIView)args.Args[0];
                item = view.gameObject.GetComponent<CellItem>();
            }
            
            if (curplayer.state != CellItem.CellState.isEnemy) //create new history if not bot
                AddHistoryRecord();
            
            curplayer.DoMove(item, allItems, () =>
            {
                curplayer = item; //set player as new cell

                CheckWins();

                if (item.effectItem != null) //if chest
                    PlayChestEffect(item);

                AfterMove();
            });
        }

        private void PlayChestEffect(CellItem item)
        {
            item.PlayEffect();

            if (item.effectItem.Effect == CellWithEffect.TypeOfEffect.give_score)
            {
                item.effectItem = null;
                item.ChestSprite = cellPref.Sprite;
            }
            else if (item.effectItem.Effect == CellWithEffect.TypeOfEffect.give_unit && currentMoveIs != CellItem.CellState.isEnemy)
            {
                List<Unit.TypeOfUnit> currUnits = (currentMoveIs == CellItem.CellState.isPlayer1) ? units_player1 : units_player2;

                if (currUnits.Count < 5)
                {
                    currUnits.Add(item.Unit.Id);

                    item.Unit = Unit.GetUnitByType(Unit.TypeOfUnit.Base_unit);
                    item.effectItem = null;
                    item.ChestSprite = cellPref.Sprite;
                }

                for (int i = 0; i < unitCells.Count; i++)
                {
                    if (i < currUnits.Count)
                    {
                        unitCells[i].unit = Unit.GetUnitByType(currUnits[i]);
                        unitCells[i].SetIcon();
                    }
                    else
                        unitCells[i].SetEmpty();
                }
            }
        }

        private void PlayDeployEffect()
        {
            switch(curplayer.Unit.DeployEffect)
            {
                case Unit.TypeOfDeployEffect.Default:
                    CheckWins(); //check win cells
                    AfterMove();
                    break;
                case Unit.TypeOfDeployEffect.Unibomb:
                    StartCoroutine(AnimExplosion(curplayer));
                    break;
                case Unit.TypeOfDeployEffect.Slime_bomb:
                    int count = curplayer.Unit.Value1;
                    foreach (CellItem edge in curplayer.GetEdgesByRadius(1))
                    {
                        if (edge.Passability != Cell.PassabilityState.total_block && edge.state == CellItem.CellState.isEmpty && count > 0)
                        {
                            edge.SetState(curplayer.state, curplayer.Unit);
                            count--;
                        }
                    }
                    AfterMove();
                    break;
                case Unit.TypeOfDeployEffect.Pacifist:
                    AfterMove();
                    break;
                case Unit.TypeOfDeployEffect.None:
                    AfterMove();
                    break;
            }
        }

        private void PlayLifeEffect()
        {
            foreach(CellItem item  in allItems)
            {
                if (item.Unit != null)
                {
                    item.life_time++;
                    item.score = Cell.cellItems.FirstOrDefault(x => x.Id == item.Id).Score + item.bonusScore; //get default score
                    item.bonusScore = 0;
                    item.CellCaptureRule = Unit.GetUnitByType(item.Unit.Id).CaptureRule;//get default captureRule

                    List<CellItem> edges = item.GetEdgesByRadius(1);
                    foreach (CellItem edge in edges)
                    {
                        if (edge.state == item.state && edge.Unit != null && edge.Unit.LifeEffect == Unit.TypeOfLifeEffect.Boost_points) //if near rich guy
                            item.score *= 2;
                        if (edge.state == item.state && edge.Unit != null && edge.Unit.LifeEffect == Unit.TypeOfLifeEffect.Create_fortress) //if near general
                            item.CellCaptureRule = Unit.TypeOfCaptureRule.Untouchable;
                    }

                    switch (item.Unit.LifeEffect)
                    {
                        case Unit.TypeOfLifeEffect.None:
                            break;
                        case Unit.TypeOfLifeEffect.Boost_points:
                            break;
                        case Unit.TypeOfLifeEffect.Create_fortress:
                            break;
                        case Unit.TypeOfLifeEffect.Decay:
                            if (item.life_time == item.Unit.DecayTime + 1)
                                item.SetEmpty();
                            break;
                    }
                }
            }
        }

        private void AfterMove()
        {
            PlayLifeEffect();
            RewriteHistoryRecord(0);
            ChangePlayerWithResetStep();
            UpdateScorePanel();

            if (isGameOver)
                return;

            if (playerMode == CellItem.CellState.isEnemy && currentMoveIs == playerMode) //start bot
            {
                App.Game.lockMove = true; //lock player move

                bot = new Bot(App.Game.difficultyLevel, allItems);

                switch (App.Game.difficultyLevel)
                {
                    case "Easy":
                        StartCoroutine(EasyBot());
                        break;
                    default:
                        StartCoroutine(EasyBot());
                        break;
                }
            }
        }

        private void CheckWins()
        {
            List<CellItem> wins = curplayer.GetWins();
            foreach (CellItem itm in wins) //change cells around player cell
                allItems[itm.index].SetState(curplayer.state);

            StartCoroutine(AnimWin(wins, curplayer)); //start win animation
        }

        private void ChangePlayerWithResetStep()
        {
            ChangePlayer();
            
            if (currentMoveIs != CellItem.CellState.isEnemy)
            {
                //UIContext.GetComponent<Button>("prevStep_button").interactable = true; //set step to prev move active
                //UIContext.GetComponent<Button>("nextStep_button").interactable = false; //set step to next move not active
            }
        }

        private void ChangePlayer()
        {
            currentMoveIs = (currentMoveIs == CellItem.CellState.isPlayer1) ? playerMode : CellItem.CellState.isPlayer1;
            
            if (currentMoveIs == CellItem.CellState.isPlayer1) //player1 move
            {
                App.Game.lockMove = false; //unlock player move
                playerScore1.ActiveState = true;
                playerScore2.ActiveState = false;
            }
            else //player2 move
            {
                playerScore1.ActiveState = false;
                playerScore2.ActiveState = true;
            }

            if (currentMoveIs != CellItem.CellState.isEnemy)
            {
                List<Unit.TypeOfUnit> currUnits = (currentMoveIs == CellItem.CellState.isPlayer1) ? units_player1 : units_player2;

                for (int i = 0; i < unitCells.Count; i++)
                {
                    if (i < currUnits.Count)
                    {
                        unitCells[i].unit = Unit.GetUnitByType(currUnits[i]);
                        unitCells[i].SetIcon();
                    }
                    else
                        unitCells[i].SetEmpty();
                }
            }
        }

        private void UpdateScorePanel()
        {
            if (currentMoveIs == CellItem.CellState.isPlayer1)
                foreach (CellItem item in allItems.FindAll(i => i.state == playerMode && i.Unit.Id != Unit.TypeOfUnit.Slime))
                {
                    Debug.Log(item.position + " " + item.score);
                    playerScore2.Score += item.score;
                    StartCoroutine(playerScore2.Animation());
                }
            else
                foreach (CellItem item in allItems.FindAll(i => i.state == CellItem.CellState.isPlayer1 && i.Unit.Id != Unit.TypeOfUnit.Slime))
                {
                    Debug.Log(item.position + " " + item.score);
                    playerScore1.Score += item.score;
                    StartCoroutine(playerScore1.Animation());
                }
            
            Debug.Log("Player1 = " + playerScore1.Score + " Player2 = " + playerScore2.Score);

            if (isGameOver)
                return;

            if (playerScore1.Score >= win_score || history[0].Player2Indexes.Count == 0)
            {
                Debug.Log("Win player1");

                StopAllCoroutines();

                isGameOver = true;
                gameOverPanel.Icon = playerScore1.Icon;
                gameOverPanel.Message = playerScore1.Score.ToString();
            }
            else if (playerScore2.Score >= win_score || history[0].Player1Indexes.Count == 0)
            {
                Debug.Log("Win player2");

                StopAllCoroutines();

                isGameOver = true;
                gameOverPanel.Icon = playerScore2.Icon;
                gameOverPanel.Message = playerScore2.Score.ToString();
            }
            else if (playerScore2.Score >= win_score && playerScore1.Score >= win_score)
            {
                Debug.Log("Draw");

                StopAllCoroutines();

                isGameOver = true;
                gameOverPanel.Icon = playerScore1.Icon;
                gameOverPanel.Message = "Draw";
            }
            else if (!PlayerIsMovable()) //if dont have variant for curr player change player
            {
                if (playerMode == CellItem.CellState.isEnemy && currentMoveIs == CellItem.CellState.isPlayer1) //if player1 dont move and is game with bot gameover
                {
                    Debug.Log("Player1 can not move");

                    StopAllCoroutines();

                    isGameOver = true;
                    gameOverPanel.Icon = playerScore2.Icon;
                    gameOverPanel.Message = "Cannot move";

                   // ChangePlayerWithResetStep();
                    UpdateScorePanel();
                }
                else
                    ChangePlayerWithResetStep();
            }
        }
        
        private IEnumerator EasyBot()
        {
            yield return new WaitForSeconds(0.6f);

            CellItem nextCell = bot.GetNext();
            curplayer = bot.currCell;

            StartCoroutine(SelectCell(bot.currCell)); //start anim selected bot cell
            yield return new WaitForSeconds(0.6f);

            PlayerMove(new FSM.EventArgs("", this, nextCell)); //move to new bot cell

            yield return null;

        }

        private IEnumerator NormalBot()
        {
            yield return null;
        }

        private IEnumerator HardBot()
        {
            yield return new WaitForSeconds(0.5f);
        }

        private IEnumerator SelectCell(CellItem cell)
        {
            cell.IsCellSelected = true;
            yield return null;
        }

        private IEnumerator MoveCell(CellItem currCell, CellItem newCell) //do: add normal animation
        {
            currCell.IsCellSelected = false;
            newCell.IsCellSelected = true;

            yield return null;
        }

        private IEnumerator AnimWin(List<CellItem> items, CellItem winner)
        {
            yield return new WaitForSeconds(0.2f);

            float ElapsedTime = 0.0f;
            float TotalTime = 0.1f;
            while (ElapsedTime < TotalTime) //to smaller size
            {
                ElapsedTime += Time.deltaTime;
                foreach(CellItem item in items)
                    item.Image.transform.localScale = Vector3.Lerp(new Vector3(1f, 1f, 1f), new Vector3(0.8f, 0.8f, 0.8f), (ElapsedTime / TotalTime));

                yield return null;
            }

            ElapsedTime = 0.0f;
            TotalTime = 0.1f;
            while (ElapsedTime < TotalTime) //to start size
            {
                ElapsedTime += Time.deltaTime;
                foreach (CellItem item in items)
                    item.Image.transform.localScale = Vector3.Lerp(new Vector3(0.8f, 0.8f, 0.8f), new Vector3(1f, 1f, 1f), (ElapsedTime / TotalTime));

                yield return null;
            }

            curplayer.IsCellSelected = false;
        }
        
        private IEnumerator AnimExplosion(CellItem bomber)
        {
            yield return new WaitForSeconds(0.2f);

            CellItem.CellState enemy = (bomber.state == CellItem.CellState.isPlayer1) ? playerMode : CellItem.CellState.isPlayer1;
            List<CellItem> bomb_items = bomber.GetEdgesByRadius(1).FindAll(x => x.state == enemy); //get all cells enemy
            if(bomb_items.Count > bomber.Unit.Value1)
                bomb_items.RemoveAt(bomber.Unit.Value1);

            foreach (CellItem item in bomb_items)
                item.Sprite = Resources.Load<Sprite>("Icons/UnitIcons/bomb_effect");

            float ElapsedTime = 0.0f;
            float TotalTime = 0.4f;
            while (ElapsedTime < TotalTime) //to smaller size
            {
                ElapsedTime += Time.deltaTime;
                foreach (CellItem item in bomb_items)
                {
                    float alpha = Mathf.Lerp(1f, 0f, (ElapsedTime / TotalTime));
                    item.Image.color = new Color(item.Image.color.r, item.Image.color.g, item.Image.color.b, alpha);
                }

                yield return null;
            }

            foreach (CellItem item in bomb_items)
            {
                item.SetEmpty();
                item.Image.color = new Color(item.Image.color.r, item.Image.color.g, item.Image.color.b, 1f);
            }
            
            AfterMove();
        }

        private IEnumerator FastAnimWin()
        {
            if (newItems.Count > 0)
            {
                Sprite sprite = null;
                if (allItems[newItems[0].index].state == CellItem.CellState.isPlayer1)
                    sprite = player1Pref.Sprite;
                else
                    sprite = player2Pref.Sprite;

                foreach (CellItem item in newItems)
                    allItems[item.index].SetState(curplayer.state, curplayer.Unit);

                curplayer.IsCellSelected = false;
            }
            yield return null;
        }
    }
}
