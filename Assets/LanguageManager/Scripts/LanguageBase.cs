using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct LanguageContent
{
    [SerializeField] private string Ids;
    [SerializeField] private string strings;

    public LanguageContent(string ids, string strings)
    {
        Ids = ids;
        this.strings = strings;
    }
    public string IDs
    {
        get => Ids;
#if UNITY_EDITOR
        set => Ids = value;
#endif

    }
    public string String
    {
        get => strings;
#if UNITY_EDITOR
        set => strings = value;
#endif

    }
}

[CreateAssetMenu(menuName = "Language")]
public class LanguageBase : ScriptableObject
{
    [SerializeField] private string languageName;
    [SerializeField]
    private List<LanguageContent> languageContent = new List<LanguageContent>
        {
            new LanguageContent("NEW_GAME", "new game"),
            new LanguageContent("LOAD_GAME", "load game"),
            new LanguageContent("EXIT_GAME", "exit game")
        };

    public string LanguageName
    {
        get => languageName;
#if UNITY_EDITOR
        set => languageName = value;
#endif

    }

    public List<LanguageContent> Content
    {
        get => languageContent;
#if UNITY_EDITOR
        set => languageContent = value;
#endif

    }

    public void ResetLanguageContent()
    {
        languageContent.Clear();
        languageContent.Add(new LanguageContent("NEW_GAME", "new game"));
        languageContent.Add(new LanguageContent("LOAD_GAME", "load game"));
        languageContent.Add(new LanguageContent("EXIT_GAME", "exit game"));
    }
}
