using SolarGames.FSM;
using UnityEngine;

namespace SolarGames.Scripts.Fsm
{
    public class LoadingState : State
    {
        public static string Name { get { return "LoadingState"; } }

        public static class Events
        {
            public const string OnLoadingComplete = "on_loading_complete";
        }

        public override string Enter(string prevState, params object[] args)
        {
            Debug.Log("---Loaded game---");
            Parent.Event(Events.OnLoadingComplete);
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
                case Events.OnLoadingComplete:
                    {
                        return MenuState.Name;
                    }
                default:
                    return base.Event(name, args);
            }
        }
    }
}
