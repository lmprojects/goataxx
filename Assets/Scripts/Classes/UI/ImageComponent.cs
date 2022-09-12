using SolarGames.Context;
using UnityEngine;
using UnityEngine.UI;

namespace SolarGames.Scripts
{
    public class ImageComponent : UIView
    {
        [SerializeField]
        protected Image icon;

        public Sprite Icon { get { return icon.sprite; } set { icon.sprite = value; } }

        public void LoadIcon(string name)
        {
            Icon = Resources.Load<Sprite>(name);
        }
    }
}
