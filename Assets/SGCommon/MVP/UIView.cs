using UnityEngine;
using System.Collections;

namespace SolarGames.Context
{
    
    public class UIView : MonoBehaviour 
    {
        [SerializeField]
        protected string viewName;
        protected string viewNameArgument;
        public string ViewName
        {
            get
            {
                if (string.IsNullOrEmpty(viewNameArgument))
                {
                    if (string.IsNullOrEmpty(viewName))
                    {
                        return gameObject.name;
                    }
                    else return viewName;
                }
                else return viewNameArgument;
            }

            set
            {
                viewName = value;
            }
        }

        public Component component;

        protected virtual void Awake () 
        {
            UIContext.Register(this);
    	}

        protected virtual void OnDestroy () 
        {
            UIContext.Unregister(this);
        }
    	
        public virtual void Event()
        {
            viewNameArgument = string.Empty;
            UIContext.Event(this);
        }

        public virtual void Event(string viewName)
        {
            viewNameArgument = viewName;
            UIContext.Event(this);
        }
    }

}