using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using SolarGames.Context;

namespace SolarGames.FSM
{
    public class StateMachine
    {
        public delegate void DStateChange(string fromState, string toState, params object[] args);

        public DStateChange OnStateChange;
        public Dictionary<string, IState> States { get; protected set; }
        public EventManager Events { get; protected set; }
        public IState CurrentState { get; protected set; }
        public string StateName { get; private set; }
       

        public void Add(IState state)
        {
            this.Add(state.GetType().Name, state);
        }

        public void Add(string name, IState state)
        {
            state.Parent = this;
            this.States.Add(name, state);
        }

        public virtual void Event(string name, params object[] args)
        {
            if (CurrentState == null)
                return;

            string newState = CurrentState.Event(name, args);
            if (!string.IsNullOrEmpty(newState))
                ChangeState(newState, args);
        }

        public virtual void UIEvent(UIView sender, params object[] args)
        {
            if (CurrentState == null)
                return;

            CurrentState.UIEvent(sender, args);
        }

        public virtual void ChangeState(string newState, params object[] args)
        {
            if (!States.ContainsKey(newState))
                throw new KeyNotFoundException(string.Format("The given key({0}) was not present in the dictionary", newState));

            OnStateChange?.Invoke(StateName, newState, args);

            Exit(newState, args);
            string prevState = StateName;

            CurrentState = States[newState];
            StateName = newState;

            string _newState = CurrentState.Enter(prevState, args);
            if (!string.IsNullOrEmpty(_newState))
                ChangeState(_newState, args);
        }

        public virtual void Exit(params object[] args)
        {
            Exit(null, args);
        }

        protected virtual void Exit(string newState, params object[] args)
        {
            if (CurrentState == null)
                return;

            string _newState = CurrentState.Exit(newState, args);
            if (!string.IsNullOrEmpty(_newState))
            {
                ChangeState(_newState, args);
                return;
            }
            
            CurrentState = null;
            StateName = null;
        }

        public void Service()
        {
            if (this.CurrentState == null)
                return;
            
            this.CurrentState.Service();
        }

        public StateMachine()
        {
            States = new Dictionary<string, IState>();
            Events = new EventManager();
        }
    }
}
