using System;
using System.Collections.Generic;
using System.Reflection;
using SolarGames.Context;
using System.Linq;

namespace SolarGames.FSM
{
    public interface IState
    {
        StateMachine Parent { get; set; }
        string Enter(string prevState, params object[] args);
        string Event(string name, params object[] args);
        void UIEvent(UIView sender, params object[] args);
        string Exit(string nextState, params object[] args);
        void Service();
    }

    public class State : IState
    {
        public StateMachine Parent { get; set; }
        Dictionary<UIEventTarget, MethodInfo> bindings;

        public virtual string Enter(string prevState, params object[] args)
        {
            //ResetTimers();
            return null;
        }

        public virtual string Event(string name, params object[] args)
        {
            return null;
        }

        public void UIEvent(UIView sender, params object[] args)
        {
            MethodInfo m = bindings.FirstOrDefault(b => b.Key.TargetType == sender.component.GetType() && b.Key.TargetName == sender.ViewName).Value;
            if (m == null)
                return;
            object[] methodArgs = new object[] { sender };
            m.Invoke(this, methodArgs);
        }

        public virtual string Exit(string nextState, params object[] args)
        {
            return null;
        }

        public virtual void Service()
        {

        }

        public State()
        {
            bindings = new Dictionary<UIEventTarget, MethodInfo>();

            foreach (var method in this.GetType().GetMethods(BindingFlags.Instance | 
                BindingFlags.NonPublic | BindingFlags.Public))
            {
                var attrs = method.GetCustomAttributes(typeof(UIEventTarget), true)
                    .Cast<UIEventTarget>().ToArray();
                
                foreach (var attr in attrs)
                {
                    bindings.Add(attr, method);
                }
            }
        }
    }
}

