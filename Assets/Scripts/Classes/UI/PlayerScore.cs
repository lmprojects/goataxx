using SolarGames.Context;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SolarGames.Scripts
{
    public class PlayerScore : ImageComponent
    {
        [SerializeField]
        private Text text = null;
        [SerializeField]
        private Image back = null;

        private int score;
        
        public int Score { get { return score; } set { score = value; text.text = "Score: " + value.ToString();} }
        public string Message { get { return text.text; } set { text.text = value.ToString(); } }

        public bool ActiveState
        {
            set
            {
                if (value)
                    back.color = new Color32(0, 255, 255, 168);
                else
                    back.color = new Color32(255, 255, 255, 100);
            }
        }

        public IEnumerator Animation()
        {
            float ElapsedTime = 0.0f;
            float TotalTime = 0.1f;
            while (ElapsedTime < TotalTime) //to smaller size
            {
                ElapsedTime += Time.deltaTime;
                    text.transform.localScale = Vector3.Lerp(new Vector3(1f, 1f, 1f), new Vector3(1.3f, 1.3f, 1.3f), (ElapsedTime / TotalTime));
                yield return null;
            }

            ElapsedTime = 0.0f;
            TotalTime = 0.1f;
            while (ElapsedTime < TotalTime) //to start size
            {
                ElapsedTime += Time.deltaTime;
                text.transform.localScale = Vector3.Lerp(new Vector3(1.3f, 1.3f, 1.3f), new Vector3(1f, 1f, 1f), (ElapsedTime / TotalTime));

                yield return null;
            }
        }
    }
}
