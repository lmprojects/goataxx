using SolarGames.FSM;
using UnityEngine;

namespace SolarGames.Scripts
{
    public class ShowHide : MonoBehaviour
    {
        public string dataName = "show_";
        public bool defaultHide = false;
        
        private void Start()
        {
            App.Manager.AddEvent(dataName, Change);

            if (defaultHide)
                gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            App.Manager.RemoveEvent(Change);
        }

        void Change(EventArgs args)
        {
            gameObject.SetActive((bool)args.Args[0]);
        }
    }
}
