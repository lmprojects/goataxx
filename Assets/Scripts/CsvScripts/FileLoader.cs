using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace SolarGames.Scripts
{
    public class FileLoader : MonoBehaviour
    {
        public static FileLoader Instance { get { return instance; } }
        private static FileLoader instance;

        public static bool showGUI = true;
        private string address;
        public static string exception;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }

            instance = this;

            exception = "";
            address = PlayerPrefs.GetString("address", "file://" + Application.streamingAssetsPath); //"http://192.168.1.202:8080/"
            DontDestroyOnLoad(gameObject);
        }
        private void OnGUI()
        {
            if (!showGUI)
                return;

            GUI.skin.textField.fontSize = 35;
            GUI.skin.button.fontSize = 30;
            GUI.skin.textArea.fontSize = 32;

            address = GUI.TextField(ResizeGUI(0.05f, 0.03f, 0.9f, 0.06f), address);

            if(exception != "")
                GUI.TextArea(ResizeGUI(0.05f, 0.1f, 0.9f, 0.06f), exception);


            if (GUI.Button(ResizeGUI(0.4f, 0.17f, 0.2f, 0.05f), "Apply"))
            {
                if (!string.IsNullOrEmpty(address))
                    LoadData();
            }
        }

        public static Rect ResizeGUI(float x, float y, float w, float h)
        {
            int width = Screen.width;
            int height = Screen.height;

            return new Rect(width * x, height * y, width * w, height * h);
        }

        public void LoadData()
        {
            StopAllCoroutines();

            List<string> files = new List<string>();
            var receivers = System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(FileReciever)));
            foreach (Type receiverType in receivers)
            {
                ReceiveFrom[] attrs = (ReceiveFrom[])Attribute.GetCustomAttributes(receiverType, typeof(ReceiveFrom));
                files.AddRange(attrs.Select(a => a.Source));
            }

            DataSource dataSource = new WebDataSource(address, files);
            //DataSource dataSource = new ResourcesDataSource("PreviewData", files);
            StartCoroutine(dataSource.GetData(() =>
            {
                if (exception != "")
                {
                    StopAllCoroutines();
                }
                else
                {
                    Receive<MatchParamsRecieve>(dataSource);
                    Receive<UnitRecieve>(dataSource);
                    Receive<CellsRecieve>(dataSource);
                    Receive<ObjectRecieve>(dataSource);

                    PlayerPrefs.SetString("address", address);
                }
            }));

            //showGUI = false;
        }

        private void Receive<T>(DataSource dataSource) where T : FileReciever
        {
            T instance = Activator.CreateInstance<T>();
            instance.Recieve(dataSource);
        }
    }
}
