using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SolarGames.Scripts
{
    public class UnitCell : ImageComponent
    {
        public Unit unit;
        private bool isSelected;

        public bool IsCellSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                if (transform.childCount > 1)
                    transform.GetChild(0).GetComponent<Image>().gameObject.SetActive(isSelected);
            }
        }

        public void SetIcon()
        {
            Icon = Resources.Load<Sprite>("Icons/UnitIcons/" + unit.Id);
        }

        public void SetEmpty()
        {
            unit = null;
            Icon = UI.empty;
            IsCellSelected = false;
        }
    }
}
