using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System.IO;
//using CsvHelper;
using System.Linq;
using System;
//using SolarGames.Poe.Stuff;
//using SolarGames.Poe.Model;

public class L
{
    public static string GetText(string key, params object[] args)
    {
        return Localization.Instance.GetText(key, args);
    }

    public static string GetText(string key)
    {
        return Localization.Instance.GetText(key);
    }

    public static void SetLanguage(string language, string fallbackLanguage = "English")
    {
        Localization.Instance.SetLanguage(language, fallbackLanguage);
    }

    internal static string GetText(object title)
    {
        throw new NotImplementedException();
    }
}

public class Localization
{
    public delegate void DOnLanguageChange(string newLanguage);
    public static Localization Instance { get { return instance; } }

    static Localization instance;

    Dictionary<string, Dictionary<string, string>> locale = new Dictionary<string, Dictionary<string, string>> { { "English", new Dictionary<string, string>() } };

    Dictionary<string, string> currentLanguageLocale;

    public string[] AvailableLanguages
    {
        get
        {
            return locale.Keys.ToArray();
        }
    }

    public string CurrentLanguage { get; private set; }
    public event DOnLanguageChange OnLanguageChange;

    public static void LoadLocale()
    {
        instance = new Localization();
        //instance.locale = new Dictionary<string, Dictionary<string, string>>();
        //instance.locale = StuffStorage.Locale;
        //LoadLocale();
        instance.SetLanguage("English");
    }

    public void SetLocale(Dictionary<string, Dictionary<string, string>> locale)
    {
        this.locale = locale;
    }

    public string GetText(string key, params object[] args)
    {
        if (currentLanguageLocale.ContainsKey(key))
            return string.Format(currentLanguageLocale[key], args);
        return "#" + key + "#";
    }

    public string GetText(string key)
    {
        if (currentLanguageLocale.ContainsKey(key))
            return currentLanguageLocale[key];
        return "#" + key + "#";
    }


    public void SetLanguage(string language, string fallbackLanguage = "English")
    {
        string oldLanguage = CurrentLanguage;
        string newLanguage = language;
        if (!locale.ContainsKey(newLanguage) && !locale.ContainsKey(fallbackLanguage))
            throw new System.InvalidOperationException(string.Format("Locale doens't contain language {0} nor {1}", language, fallbackLanguage));
        if (!locale.ContainsKey(newLanguage))
            newLanguage = fallbackLanguage;

        CurrentLanguage = newLanguage;
        currentLanguageLocale = locale[newLanguage];
        PlayerPrefs.SetString("language", newLanguage);
        PlayerPrefs.Save();
        if (oldLanguage != newLanguage && OnLanguageChange != null)
            OnLanguageChange(newLanguage);
    }

}
