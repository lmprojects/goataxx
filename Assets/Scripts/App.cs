using SolarGames.Context;
using SolarGames.FSM;
using SolarGames.Scripts;
using SolarGames.Scripts.Fsm;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SolarGames
{
    public class App : MonoBehaviour
    {
        public static StateMachine FSM { get; private set; }
        public static Game Game { get; set; }
        public static EventManager Manager { get; set; }
        public static List<Map> maps;

        public void Awake()
        {
            if (FSM != null) return;
            
            Game = new Game();
            Manager = new EventManager();

            FSM = new StateMachine();
            
            FSM.Add(new LoadingState());
            FSM.Add(new MenuState());
            FSM.Add(new PreGameState());
            FSM.Add(new GameState());
            FSM.Add(new SettingsState());
            FSM.Add(new GameExitState());
            FSM.Add(new OfflineState());
        }

        private void Start()
        {
            FSM.ChangeState("LoadingState");
            
            Manager.AddEvent("destroy", DestroyObj);
            Manager.AddEvent("instantiate", InstantiateObj);

            CreateMaps();
        }

        static public void CreateMaps()
        {
            maps = new List<Map>
            {
                new Map //1
                {
                    Id = 1,
                    mapIndexes = new Dictionary<Map.VariantOfCell, List<int>>
                    {
                        { Map.VariantOfCell.player1Units, new List<int>{0, 48} },
                        { Map.VariantOfCell.player2Units, new List<int>{6, 42} },
                        { Map.VariantOfCell.richCells, new List<int>{3, 17, 23, 25,31, 45} },
                        { Map.VariantOfCell.blockedCells, new List<int>{16, 18, 30, 32} },
                        { Map.VariantOfCell.highWallCells, new List<int>{21, 22, 26, 27} },
                        { Map.VariantOfCell.unitChests, new List<int>{3, 21, 24, 27, 45} },
                        { Map.VariantOfCell.scoreChests, new List<int>{17, 23, 25, 31} }
                    }
                },
                new Map //2
                {
                    Id = 2,
                    mapIndexes = new Dictionary<Map.VariantOfCell, List<int>>
                    {
                        { Map.VariantOfCell.player1Units, new List<int>{0, 6} },
                        { Map.VariantOfCell.player2Units, new List<int>{42, 48} },
                        { Map.VariantOfCell.richCells, new List<int>{3, 17, 23, 25,31, 45} },
                        { Map.VariantOfCell.blockedCells, new List<int>{16, 18, 30, 32} },
                        { Map.VariantOfCell.highWallCells, new List<int>{21, 22, 26, 27} },
                        { Map.VariantOfCell.unitChests, new List<int>{3, 21, 24, 27, 45} },
                        { Map.VariantOfCell.scoreChests, new List<int>{17, 23, 25, 31} }
                    }
                },
                new Map //3
                {
                    Id = 3,
                    mapIndexes = new Dictionary<Map.VariantOfCell, List<int>>
                    {
                        { Map.VariantOfCell.player1Units, new List<int>{21, 23} },
                        { Map.VariantOfCell.player2Units, new List<int>{25, 27} },
                        { Map.VariantOfCell.richCells, new List<int>{0, 6, 10, 38, 42, 48} },
                        { Map.VariantOfCell.blockedCells, new List<int>{8, 12, 17, 24, 31, 36, 40} },
                        { Map.VariantOfCell.highWallCells, new List<int>() },
                        { Map.VariantOfCell.unitChests, new List<int>{0, 3, 6, 42, 45, 48} },
                        { Map.VariantOfCell.scoreChests, new List<int>{7, 10, 13, 35, 38, 41} }
                    }
                },
                new Map //4
                {
                    Id = 4,
                    mapIndexes = new Dictionary<Map.VariantOfCell, List<int>>
                    {
                        { Map.VariantOfCell.player1Units, new List<int>{35, 37, 39, 41, 43, 45, 47} },
                        { Map.VariantOfCell.player2Units, new List<int>{1, 3, 5, 7, 9, 11, 13} },
                        { Map.VariantOfCell.richCells, new List<int>{21, 22, 23, 24, 25, 26, 27} },
                        { Map.VariantOfCell.blockedCells, new List<int>() },
                        { Map.VariantOfCell.highWallCells, new List<int>{ 8, 10, 12, 36, 38, 40} },
                        { Map.VariantOfCell.unitChests, new List<int>{22, 24, 26} },
                        { Map.VariantOfCell.scoreChests, new List<int>{21, 23, 25, 27} }
                    }
                },
                new Map //5
                {
                    Id = 5,
                    mapIndexes = new Dictionary<Map.VariantOfCell, List<int>>
                    {
                        { Map.VariantOfCell.player1Units, new List<int>{42, 48} },
                        { Map.VariantOfCell.player2Units, new List<int>{0, 6} },
                        { Map.VariantOfCell.richCells, new List<int>{8, 12, 14, 20, 24, 28, 34, 36, 40} },
                        { Map.VariantOfCell.blockedCells, new List<int>{ 17, 21, 27, 31} },
                        { Map.VariantOfCell.highWallCells, new List<int>{16, 18, 22, 26, 30, 32} },
                        { Map.VariantOfCell.unitChests, new List<int>{3, 15, 19, 29, 33, 45} },
                        { Map.VariantOfCell.scoreChests, new List<int>{23, 24, 25} }
                    }
                }
            };
            ShuffleMaps();
        }

        static private void ShuffleMaps()
        {
            List<Map> temp = maps;
            maps = new List<Map>();
            while(temp.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, temp.Count);
                Map m = temp[index];
                temp.Remove(m);
                maps.Add(m);
            }
        }

        private void Update()
        {
            FSM.Service();
        }

        private void OnDestroy()
        {
            Manager.RemoveEvent(DestroyObj);
            Manager.RemoveEvent(InstantiateObj);
        }

        public static void SendEvent(string name, params object[] args)
        {
            FSM.Event(name, args);
        }

        public static void SendUIEvent(UIView sender, params object[] args)
        {
            FSM.UIEvent(sender, args);
        }
        
        private void InstantiateObj(FSM.EventArgs args)
        {
            if (args.Args == null)
                return;
            try
            {
                Instantiate((GameObject)args.Args[0], (Transform)args.Args[1]);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message + " args[0] " + args.Args[0] + " args[1] " + args.Args[1]);
            }
        }

        private void DestroyObj(FSM.EventArgs args)
        {
            if (args.Args == null)
                return;

            Destroy((GameObject)args.Args[0]);
        }
    }
}