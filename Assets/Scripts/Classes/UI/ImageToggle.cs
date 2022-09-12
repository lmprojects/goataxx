using SolarGames.Context;
using UnityEngine;
using UnityEngine.UI;

namespace SolarGames.Scripts
{
    public class ImageToggle : ImageComponent
    {
        private bool isActive;

        public bool IsActive {
            set { isActive = value; LoadIcon(); }
            get { return isActive; }
        }

        private void LoadIcon()
        {
            string img = (isActive) ? viewName + "_on" : viewName + "_off";
            Icon = Resources.Load<Sprite>("Icons/Interface/" + img);
        }
    }
}
