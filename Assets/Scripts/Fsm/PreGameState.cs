using SolarGames.Context;
using SolarGames.FSM;
using UnityEngine.UI;

namespace SolarGames.Scripts.Fsm
{
    public class PreGameState : State
    {
        public static string Name { get { return "PreGameState"; } }

        public static class Events
        {
            public const string ToGame = "to_game";
            public const string ReturnToMenu = "return_to_menu";
        }

        public static class UIViews
        {
            public const string ToGameOne = "one_player_button";
            public const string ToGameTwo = "two_players_button";
            public const string ReturnToMenuButton = "return_to_menu_button";
        }

        public override string Enter(string prevState, params object[] args)
        {
            App.Manager.Invoke("show_game_mode", this, true);
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
                case Events.ToGame:
                    return GameState.Name;
                case Events.ReturnToMenu:
                    return MenuState.Name;
                default:
                    return base.Event(name, args);
            }
        }

        [UIEventTarget(typeof(Button), UIViews.ReturnToMenuButton)]
        public void OnReturnToMenuButtonPressed(UIView view)
        {
            Parent.Event(Events.ReturnToMenu);
            App.Manager.Invoke("show_game_mode", this, false);
        }
        [UIEventTarget(typeof(Button), UIViews.ToGameOne)]
        public void OnOnePlayerGame(UIView view)
        {
            Parent.Event(Events.ToGame, "isEnemy");
        }
        [UIEventTarget(typeof(Button), UIViews.ToGameTwo)]
        public void OnTwoPlayerGame(UIView view)
        {
            Parent.Event(Events.ToGame, "isPlayer2");
        }
    }
}