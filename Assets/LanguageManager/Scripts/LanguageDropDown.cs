using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;

public class LanguageDropDown : MonoBehaviour
{
    private List<LanguageBase> languages = new List<LanguageBase>();
    private TMP_Dropdown dropdown;
    


    private void Awake()
    {
         dropdown = GetComponent<TMP_Dropdown>();
    }
    private void Start()
    {        
        SearchForCustomLanguage();    
    }
    public void DropDownChangeValue(int index)
    {
        string stringaDaStampare = dropdown.options[index].text;
       
        foreach (LanguageTextMeshPro textToUpdate in SceneView.FindObjectsOfType<LanguageTextMeshPro>())
        {
            textToUpdate.SelectedLanguage = GetLanguageBaseFromString(stringaDaStampare);
            textToUpdate.ChangeText();
        }

    }

    private void SearchForCustomLanguage()
    {
        dropdown.ClearOptions();        
        string[] languageIds = AssetDatabase.FindAssets("l:Language");
        string[] languagePaths = new string[languageIds.Length];
        string[] languageNames = new string[languageIds.Length];
       
        for (int i = 0; i < languageIds.Length; i++)
        {
            languagePaths[i] = AssetDatabase.GUIDToAssetPath(languageIds[i]);
            languageNames[i] = Path.GetFileNameWithoutExtension(languagePaths[i]);
            TMP_Dropdown.OptionData newOption = new TMP_Dropdown.OptionData();
            newOption.text = languageNames[i];

            languages.Add(GetLanguageBaseFromString(languageNames[i]));
            dropdown.options.Add(newOption);
        }
        if (dropdown.options.Count > 0)
        {


            string stringaDaStampare = dropdown.options[0].text;

            foreach (LanguageTextMeshPro textToUpdate in SceneView.FindObjectsOfType<LanguageTextMeshPro>())
            {
                textToUpdate.SelectedLanguage = GetLanguageBaseFromString(stringaDaStampare);
                textToUpdate.ChangeText();
            }
        }

    }
    private LanguageBase GetLanguageBaseFromString(string languageName)
    {
        string[] tempArray = AssetDatabase.FindAssets(languageName);
        string tempPath = AssetDatabase.GUIDToAssetPath(tempArray[0]);
        return (LanguageBase)AssetDatabase.LoadAssetAtPath(tempPath, typeof(LanguageBase));
    }
}
