using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace SolarGames.Scripts
{
    public class WebDataSource : DataSource
    {
        private string address;
        private List<string> files;

        public WebDataSource(string address, List<string> files)
        {
            this.address = address;
            this.files = files;
        }

        public override IEnumerator GetData(Action callback)
        {
            foreach (string file in files)
            {
                string exc = "";
                try
                {
                    UnityWebRequest.Get(System.IO.Path.Combine(address, file));
                }
                catch (Exception ex)
                {
                    exc = ex.Message;
                }

                if(exc != "")
                {
                    FileLoader.exception = exc;
                    callback?.Invoke();
                    yield return "";
                }

                using (UnityWebRequest www = UnityWebRequest.Get(System.IO.Path.Combine(address, file)))
                {
                    yield return www.SendWebRequest();

                    if (www.isNetworkError || www.isHttpError)
                    {
                        Debug.LogWarningFormat("Can't get data from '{0}': {1}", address, www.error);
                        FileLoader.exception = "Can't get data from " + address + " : " + www.error;
                        callback?.Invoke();
                        yield return "";
                    }
                    else
                    {
                        // Show results as text
                        Debug.LogFormat("Web data from '{0}':\n{1}", address, www.downloadHandler.text);
                        this[file] = www.downloadHandler.text;
                        FileLoader.exception = "";
                    }
                }
            }

            callback?.Invoke();
        }
    }
}
