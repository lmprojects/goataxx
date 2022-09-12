
using UnityEngine;

namespace SolarGames.Scripts.Fsm
{
    public class GameExitState : OfflineState
    {
        public static string Name { get { return "GameExitState"; } }

        public new static class Events
        {
            public const string GameExit = "exit";
        }

        public override string Enter(string prevState, params object[] args)
        {
            Debug.Log("---Exit---");
            //some save and quit in app.game
                    Application.Quit();
            return base.Enter(prevState, args);
        }

        public override string Event(string name, params object[] args)
        {
            switch (name)
            {
                case Events.GameExit:
                    return string.Empty;
                default:
                    return base.Event(name, args);
            }
        }
    }
}
