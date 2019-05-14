using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance {get; private set;}

    [Serializable]
    public class LanguageEvent : UnityEvent<int> {}

    public List<string> languages;
    public int defaultLanguage;
    public int currentLanguage;

    public LanguageEvent LanguageChanged;


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SetLanguage(PlayerPrefs.GetInt("language", defaultLanguage));
    }

    public void SetLanguage(int index)
    {
        currentLanguage = index;
        PlayerPrefs.SetInt("language", currentLanguage);
        LanguageChanged.Invoke(currentLanguage);
    }

    public void ToggleLanguage()
    {
        SetLanguage((currentLanguage + 1) % languages.Count);
    }

}
