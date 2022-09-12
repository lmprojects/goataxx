using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
//using UniLinq;

namespace SolarGames.FSM
{
    public delegate void DEvent(EventArgs args);

    public class EventArgs
    {
        public string EventName { get; set; }
        public object Sender { get; set; }
        public object[] Args { get; set; }
        public EventManager Manager { get; set; }
        public override string ToString()
        {
            return string.Format("[EventArgs: EventName={0}, Sender={1}, Args={2}]", EventName, Sender, Args);
        }

        public EventArgs() : this("") { }
        public EventArgs(string eventName) : this(eventName, null) { }

        public EventArgs(string eventName, object sender, params object[] args)
        {
            this.EventName = eventName;
            this.Sender = sender;
            this.Args = args;
        }
    }

    public class EventManager
    {
        public class EventTree
        {
            public EventManager Manager { get; protected set; }
            public HashSet<DEvent> Listeners { get; protected set; }

            public EventTree(EventManager parent)
            {
                this.Listeners = new HashSet<DEvent>();
                this.Manager = parent;
            }
        }

        private Dictionary<string, EventTree> _events;

        public void Invoke(string eventName)
        {
            this.Invoke(new EventArgs(eventName));
        }

        public void Invoke(string eventName, object sender, params object[] args)
        {
            this.Invoke(new EventArgs(eventName, sender, args));
        }

        public void Invoke(EventArgs args)
        {
            if (!_events.ContainsKey(args.EventName))
                return;
            
            HashSet<DEvent> _listeners = _events[args.EventName].Listeners;
            DEvent[] listeners = new DEvent[_listeners.Count];
            _listeners.CopyTo(listeners);

            if (listeners.Length == 0)
                return;

            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();

            foreach (DEvent listener in listeners)
            {
                if (listener != null)
                {
                    if (listener.Method.IsStatic || listener.Target != null)
                    {
                        try
                        {
                            listener.Invoke(args);
                        }
                        catch (Exception ex)
                        {
                            UnityEngine.Debug.LogException(ex);
                        }
                    }
                    else
                        this.RemoveEvent(listener);
                }
            }

            if (timer.ElapsedMilliseconds > 1000)
                UnityEngine.Debug.LogWarningFormat("Invoke {1} took time {0}ms", timer.ElapsedMilliseconds, args.EventName);
            timer.Stop();
        }

        public void AddEvent(string eventName, DEvent listener)
        {
            if (!this._events.ContainsKey(eventName))
                this._events.Add(eventName, new EventTree(this));

            this._events[eventName].Listeners.Add(listener);
        }

        public void RemoveAll(string eventName)
        {
            if (!this._events.ContainsKey(eventName))
                return;
            this._events.Remove(eventName);
        }

        public void RemoveEvent(DEvent listener)
        {
            foreach (var pair in new Dictionary<string, EventTree>(_events))
            {
                if (pair.Value.Listeners.Contains(listener))
                    pair.Value.Listeners.Remove(listener);

                if (pair.Value.Listeners.Count == 0)
                    _events.Remove(pair.Key);
            }
        }
            

        public EventManager()
        {
            _events = new Dictionary<string, EventTree>();
        }

        public override string ToString()
        {
            return string.Format("[EventManager] Keys={0}, Listeners={1}", _events.Keys.Count, _events.Values.Sum(v => v.Listeners.Count));
        }
    }



}

