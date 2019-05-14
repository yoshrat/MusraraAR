using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageElement : MonoBehaviour
{
    List<GameObject> languageParents = new List<GameObject>();

    void Awake()
    {
        LanguageManager.Instance.LanguageChanged.AddListener(OnLanguageChanged);
        foreach (string language in LanguageManager.Instance.languages)
        {
            languageParents.Add(transform.Find(language).gameObject);
        }
    }

    public void SetLanguage(int languageIndex)
    {
        foreach (GameObject gameObject in languageParents)
        {
            gameObject.SetActive(false);
        }
        languageParents[languageIndex].SetActive(true);
    }

    private void OnLanguageChanged(int languageIndex)
    {
        SetLanguage(languageIndex);
    }
}
