using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace SolarGames.Context
{

    public static class UIContext 
    {
        static List<UIView> views = new List<UIView>();
        public static event Action<UIView> OnRegister;

        public static void Register(UIView view)
        {
            if (Exists(view.component.GetType(), view.component.name))
            {
                Debug.LogErrorFormat("Trying to register existing component type:{0}, name:{1}.", view.component.GetType(), view.component.name);
                return;
            }
            views.Add(view);
            if (OnRegister != null)
                OnRegister(view);
        }

        public static void Unregister(UIView component)
        {
            views.RemoveAll(c => c == component);
        }

        public static T GetComponent<T>(string name) where T : Component
        {
            UIView view = views.FirstOrDefault(v => v.component is T && v.ViewName == name);
            if (view == null)
                return null;
            else
                return (T)view.component;
        }
        
        public static bool Exists(Type type, string name)
        {
            return views.Any(v => v.component.GetType() == type && v.ViewName == name);
        }

        public static void Event(UIView sender, params object[] args)
        {
            App.SendUIEvent(sender, args);
        }
    }

}