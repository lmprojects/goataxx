using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//using TMPro;

public class LocalizationBehavior : MonoBehaviour {

    public string key;
    public bool upper;

    //TextMeshProUGUI text;

    void Start()
    {
        //text = GetComponent<TextMeshProUGUI>();
        Instance_OnLanguageChange(null);
        Localization.Instance.OnLanguageChange += Instance_OnLanguageChange; 
    }

    void OnDestroy()
    {
        Localization.Instance.OnLanguageChange -= Instance_OnLanguageChange;
    }

    private void Instance_OnLanguageChange(string newLanguage)
    {
        //text.text = L.GetText(key);
        //if (upper)
        //    text.text = text.text.ToUpper();
    }
}
