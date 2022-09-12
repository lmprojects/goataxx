using SolarGames.FSM;

namespace SolarGames.Scripts.Fsm
{
    public class OfflineState : State
    {

        public static class Events
        {
            public const string OnDisconnect = "on_disconnect";
            public const string OnPlayerDeath = "on_player_death";
            public const string LoadLocation = "load_location";
            public const string OnBossDefeated = "on_boss_defeated";
            public const string OnBossWon = "on_boss_won";
        }

        public override string Enter(string prevState, params object[] args)
        {
            SwitchOfflineMode();
            return base.Enter(prevState, args);
        }

        public override string Event(string name, params object[] args)
        {
            switch (name)
            {
                //case Events.OnDisconnect:
                //    return LoginState.Name;
                //case Events.OnPlayerDeath:
                //    return DeathState.Name;
                //case Events.LoadLocation:
                //    return LocationLoadingState.Name;
                //case Events.OnBossDefeated:
                //    return BossDefeatedState.Name;
                //case Events.OnBossWon:
                //    return BossWonState.Name;
                default:
                    return base.Event(name, args);
            }
        }

        public virtual void SwitchOfflineMode()
        {
            //if (App.Game.OfflineMode.IsEnabled)
            //    App.Game.OfflineMode.Disable();
        }
    }
}
