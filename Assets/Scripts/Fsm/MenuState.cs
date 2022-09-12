using SolarGames.Context;
using SolarGames.FSM;
using UnityEngine;
using UnityEngine.UI;

namespace SolarGames.Scripts.Fsm
{
    public class MenuState : State
    {
        public static string Name { get { return "MenuState"; } }

        public static class Events
        {
            public const string OnStartGame = "start_game";
            public const string OnSettings = "show_settings";
            public const string OnExitGame = "exit_game";
        }

        public static class UIViews
        {
            public const string StartButton = "start_button";
            public const string SettingsButton = "settings_button";
            public const string ExitButton = "exit_button";
        }

        public override string Enter(string prevState, params object[] args)
        {
            Debug.Log("---Menu game---");
            base.Enter(prevState, args);
            return string.Empty;
        }

        public override string Event(string name, params object[] args)
        {
            switch (name)
            {
                case Events.OnStartGame:
                    return PreGameState.Name;
                case Events.OnSettings:
                    return SettingsState.Name;
                case Events.OnExitGame:
                    return GameExitState.Name;
                default:
                    return base.Event(name, args);
            }
        }

        public override string Exit(string nextState, params object[] args)
        {
            return base.Exit(nextState, args);
        }

        [UIEventTarget(typeof(Button), UIViews.StartButton)]
        public void OnStartButtonPressed(UIView view)
        {
            if(!FileLoader.showGUI)
                Parent.Event(Events.OnStartGame);
        }

        [UIEventTarget(typeof(Button), UIViews.SettingsButton)]
        public void OnSettingsButtonPressed(UIView view)
        {
            Parent.Event(Events.OnSettings, "show_settings");
        }

        [UIEventTarget(typeof(Button), UIViews.ExitButton)]
        public void OnExitButtonPressed(UIView view)
        {
            Parent.Event(Events.OnExitGame);
        }
    }
}
