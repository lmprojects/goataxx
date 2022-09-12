using SolarGames.Context;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SolarGames.Scripts.Fsm
{
    public class GameState : OfflineState
    {
        public static string Name { get { return "GameState"; } }
        public static UnitCell currUnit = null;

        public new static class Events
        {
            public const string ShowSettings = "show_settings";
        }

        public static class UIViews
        {
            public const string NextStepButton = "nextStep_button";
            public const string PrevStepButton = "prevStep_button";
            public const string SettingsButton = "settings_button";
            public const string RestartButton = "restart_button";
            public const string SmileButton = "smile_button";
            public const string Cell = "cell";
            public const string Unit = "unit";
        }

        public override string Enter(string prevState, params object[] args)
        {
            if (SceneManager.GetActiveScene().name != "Game")
            {
                Debug.Log("---Start game---");
                App.Game.LoadScene("Game", args[0].ToString());
            }

            return base.Enter(prevState, args);
        }

        public override string Exit(string nextState, params object[] args)
        {
            return base.Exit(nextState, args);
        }

        public override string Event(string name, params object[] args)
        {
            switch (name)
            {
                case Events.ShowSettings:
                    return SettingsState.Name;
                default:
                    return base.Event(name, args);
            }
        }

        [UIEventTarget(typeof(Button), UIViews.SettingsButton)]
        public void OnSettingsButtonPressed(UIView view)
        {
            Parent.Event(Events.ShowSettings, "show_settings_in_game");
        }
        [UIEventTarget(typeof(Button), UIViews.RestartButton)]
        public void OnRestartButtonPressed(UIView view)
        {
            App.Game.lockMove = false;
            currUnit = null;
            App.Manager.Invoke("restart");
        }
        [UIEventTarget(typeof(Button), UIViews.SmileButton)]
        public void OnsmileButtonPressed(UIView view)
        {
            App.Game.lockMove = false;
            currUnit = null;
            App.Manager.Invoke("show_smile");
        }
        [UIEventTarget(typeof(Button), UIViews.Cell)]
        public void OnCellPressed(UIView view)
        {
            CellItem item = view.GetComponent<CellItem>();
            
            if(currUnit != null)
            {
                App.Manager.Invoke("stand_unit", this, view.GetComponent<CellItem>(), currUnit);
                currUnit = null;
            }
            else if (App.Game.lockMove == false && currUnit == null)
            {
                if (item.state == CellItem.CellState.isEnemy)
                    return;
                if(item.state == CellItem.CellState.isWait)
                    App.Manager.Invoke("player_move", this, view);
                else
                    App.Manager.Invoke("show_variants", this, view);
            }
        }
        [UIEventTarget(typeof(Button), UIViews.NextStepButton)]
        public void ToNextStep(UIView view)
        {
            App.Manager.Invoke("change_step");
        }
        [UIEventTarget(typeof(Button), UIViews.PrevStepButton)]
        public void ToPrevStep(UIView view)
        {
            App.Manager.Invoke("change_step");
        }
        [UIEventTarget(typeof(Button), UIViews.Unit)]
        public void OnUnitPress(UIView view)
        {
            if (currUnit != null)
                currUnit.IsCellSelected = false;

            currUnit = view.GetComponent<UnitCell>();
            if(currUnit.unit != null)
                App.Manager.Invoke("show_unit_variants", this, currUnit);
        }
    }
}
