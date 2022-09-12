using SolarGames.Context;
using SolarGames.FSM;
using UnityEngine;
using UnityEngine.UI;

namespace SolarGames.Scripts.Fsm
{
    public class SettingsState : State
    {
        public static string Name { get { return "SettingsState"; } }
        protected string ShowSettingsMode = "show_settings";
        protected string IconMode = "CellPlayer1";

        public static class Events
        {
            public const string ReturnToGame = "return_to_game";
            public const string ReturnToMenu = "return_to_menu";

            public const string SoundChange = "sound_change";
            public const string MusicChange = "music_change";
        }

        public static class UIViews
        {
            public const string ReturnToGameButton = "return_to_game_button";
            public const string ReturnToMenuButton = "return_to_menu_button";
            public const string ReturnToSettingsButton = "return_to_settings_button";

            public const string SoundButton = "sound_button";
            public const string MusicButton = "music_button";
            
            public const string Player1Icon = "CellPlayer1";
            public const string Player2Icon = "CellPlayer2";
            
            public const string IconImage = "icon_select_button";
            
            public const string DifficultButton = "difficulty_button";
        }

        public override string Enter(string prevState, params object[] args)
        {
            Debug.Log("---Settings---");
            ShowSettingsMode = args[0].ToString();
            App.Manager.Invoke(ShowSettingsMode, this, true);
            App.Game.LoadSettingsValues();
            return base.Enter(prevState, args);
        }

        public override string Event(string name, params object[] args)
        {
            switch (name)
            {
                case Events.ReturnToGame:
                    return GameState.Name;
                case Events.ReturnToMenu:
                    return MenuState.Name;
                default:
                    return base.Event(name, args);
            }
        }

        public override string Exit(string nextState, params object[] args)
        {
            App.Manager.Invoke(args[0].ToString(), this, false);
            return base.Exit(nextState, args);
        }

        [UIEventTarget(typeof(Button), UIViews.ReturnToGameButton)]
        public void OnReturnToGameButtonPressed(UIView view)
        {
            Parent.Event(Events.ReturnToGame, ShowSettingsMode);
        }
        [UIEventTarget(typeof(Button), UIViews.ReturnToMenuButton)]
        public void OnReturnToMenuButtonPressed(UIView view)
        {
            Parent.Event(Events.ReturnToMenu, ShowSettingsMode);
        }
        [UIEventTarget(typeof(Button), UIViews.ReturnToSettingsButton)]
        public void OnReturnToSettingsButtonPressed(UIView view)
        {
            App.Manager.Invoke("show_icon_menu", this, false);
        }
        [UIEventTarget(typeof(ImageToggle), UIViews.SoundButton)]
        public void OnSoundChanged(UIView view)
        {
            App.Game.SettingsChanged(UIViews.SoundButton);
        }
        [UIEventTarget(typeof(ImageToggle), UIViews.MusicButton)]
        public void OnMusicChanged(UIView view)
        {
            App.Game.SettingsChanged(UIViews.MusicButton);
        }
        [UIEventTarget(typeof(ImageComponent), UIViews.Player1Icon)]
        public void OnIconPl1Changed(UIView view)
        {
            IconMode = UIViews.Player1Icon;
            App.Game.OpenIconPanel(IconMode);
        }
        [UIEventTarget(typeof(ImageComponent), UIViews.Player2Icon)]
        public void OnIconPl2Changed(UIView view)
        {
            IconMode = UIViews.Player2Icon;
            App.Game.OpenIconPanel(IconMode);
        }
        [UIEventTarget(typeof(Image), UIViews.IconImage)]
        public void OnIconSelect(UIView view)
        {
            App.Game.IconChange(IconMode, view);
        }
        [UIEventTarget(typeof(Button), UIViews.DifficultButton)]
        public void OnDifficultySelect(UIView view)
        {
            App.Game.DiffChange(view);
        }
    }
}
