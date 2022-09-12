using SolarGames.Context;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SolarGames.Scripts
{
    public class Game
    {
        public CellItem.CellState playerMode;
        public string difficultyLevel = "Easy";
        public bool lockMove = false;
        public bool soundEnable = true;
        public bool musicEnable = true;

        private List<string> diffLevel = new List<string> { "Easy", "Normal", "Hard" };
        private CellItem player1Pref, player2Pref;

        public void LoadSettingsValues()
        {
            UIContext.GetComponent<ImageToggle>("sound_button").IsActive = soundEnable;
            UIContext.GetComponent<ImageToggle>("music_button").IsActive = musicEnable;

            UIContext.GetComponent<Text>("difficulty_text").text = difficultyLevel;
            
            player1Pref = Resources.Load<GameObject>("Prefabs/Units/CellPlayer1").GetComponent<CellItem>();
            player2Pref = Resources.Load<GameObject>("Prefabs/Units/CellPlayer2").GetComponent<CellItem>();
            
            if (UIContext.GetComponent<ImageComponent>("CellPlayer1") != null)
            {
                UIContext.GetComponent<ImageComponent>("CellPlayer1").Icon = player1Pref.Sprite;
                UIContext.GetComponent<ImageComponent>("CellPlayer2").Icon = player2Pref.Sprite;
            }
        }

        public void SettingsChanged(string btn)
        {
            switch (btn)
            {
                case "sound_button":
                    soundEnable = !soundEnable;
                    UIContext.GetComponent<ImageToggle>(btn).IsActive = soundEnable;
                    break;
                case "music_button":
                    musicEnable = !musicEnable;
                    UIContext.GetComponent<ImageToggle>(btn).IsActive = musicEnable;
                    break;
            }
        }

        public void LoadScene(string name, string mode)
        {
            Debug.Log("---Load game scene---");
            playerMode = (mode == "isEnemy") ? CellItem.CellState.isEnemy : CellItem.CellState.isPlayer2;
            SceneManager.LoadScene(name);
        }

        public void OpenIconPanel(string selectedPlayer)
        {
            App.Manager.Invoke("show_icon_menu", this, true);
            
            GridLayoutGroup grid = UIContext.GetComponent<GridLayoutGroup>("icon_grid");
            Image imgPref = Resources.Load<GameObject>("Prefabs/Other/ImageIconMenu").GetComponent<Image>(); //get prefab image icon

            if (imgPref == null) //dont search prefab image
            {
                App.Manager.Invoke("show_icon_menu", this, false);
                return;
            }

            foreach (Image child in grid.transform.GetComponentsInChildren<Image>()) //clear grid
                App.Manager.Invoke("destroy", this, child.gameObject);

            Object[] icons = Resources.LoadAll("Icons", typeof(Sprite)); //get all sprites
            
            foreach (Object sprite in icons) //create icon images
            {
                Sprite enemyName = (selectedPlayer == "CellPlayer1") ? player2Pref.Sprite : player1Pref.Sprite;
                if (sprite != enemyName) //get icon to other player and dont show
                {
                    imgPref.sprite = (Sprite)sprite;
                    App.Manager.Invoke("instantiate", this, imgPref.gameObject, grid.transform);
                }
            }
        }

        public void IconChange(string btn, UIView selectIcon)
        {
            Sprite btnSprite = selectIcon.gameObject.GetComponent<Image>().sprite; //get selected icon sprite
            UIContext.GetComponent<ImageComponent>(btn).Icon = btnSprite; //set sprite to player button in settings

            CellItem item = (btn == "CellPlayer1") ? player1Pref : player2Pref;
            item.Sprite = btnSprite; //set sprite to prefab

            GridLayoutGroup grid = UIContext.GetComponent<GridLayoutGroup>("icon_grid");
            foreach (Image cell in grid.GetComponentsInChildren<Image>()) //clear grid
                App.Manager.Invoke("destroy", this, cell.gameObject);

            App.Manager.Invoke("show_icon_menu", this, false);
        }

        public void DiffChange(UIView view)
        {
            string currLevel = view.GetComponentInChildren<Text>().text; //get current level name

            int currIndex = diffLevel.IndexOf(currLevel); //get index level in list
            if (currIndex >= 0) //if level in list
            {
                if (currIndex == diffLevel.Count - 1) //if last level get first
                    currIndex = -1;
                difficultyLevel = diffLevel[currIndex + 1];
                view.GetComponentInChildren<Text>().text = difficultyLevel; //change level name
            }
        }
    }
}