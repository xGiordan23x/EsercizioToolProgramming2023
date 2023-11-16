using Codice.CM.Common;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

public class LanguageManager : EditorWindow
{

    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;


    private LanguageBase selectedLanguage;


    [MenuItem("Tools/LanguageManager")]
    public static void ShowExample()
    {
        LanguageManager wnd = GetWindow<LanguageManager>();
        wnd.titleContent = new GUIContent("LanguageManager");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);


        //Variabili UI-Builder
        DropdownField languageDropdownField = rootVisualElement.Q<DropdownField>("LanguageDropdown");
        TextField languageNameTextField = rootVisualElement.Q<TextField>("LanguageNameTextField");
        Button languageAddButton = rootVisualElement.Q<Button>("AddLanguageButton");
        ScrollView languageContentScrollView = rootVisualElement.Q<ScrollView>("LanguageContentScrollView");
        Foldout languageContentFoldout = rootVisualElement.Q<Foldout>("LanguageContentFoldout");
        Button exportButton = rootVisualElement.Q<Button>("ExportButton");
        Button importButton = rootVisualElement.Q<Button>("ImportButton");

        //Ricerca tutte le lingue custom in assets
        SearchForCustomLanguage(languageDropdownField);

        //Bottone Aggiungi Lingua
        languageAddButton.clicked += () =>
        {
            if (string.IsNullOrWhiteSpace(languageNameTextField.text))
            {
                EditorUtility.DisplayDialog("Error", "Enter a valid Language name", "Ok");
                return;
            }

            //Creazione lingua
            LanguageBase newLanguage = ScriptableObject.CreateInstance<LanguageBase>();
            newLanguage.name = languageNameTextField.text;
            newLanguage.LanguageName = languageNameTextField.text;
            AssetDatabase.CreateAsset(newLanguage, "Assets/LanguageManager/Languages/" + newLanguage.name + ".asset");
            var asset = AssetDatabase.LoadMainAssetAtPath("Assets/LanguageManager/Languages/" + newLanguage.name + ".asset");
            AssetDatabase.SetLabels(asset, new string[] { "Language" });



            //Aggiungo alla dropdown
            languageDropdownField.choices.Add(newLanguage.name);
            languageDropdownField.value = newLanguage.name;
            selectedLanguage = GetLanguageBaseFromString(languageDropdownField.value);

            EditorUtility.SetDirty(newLanguage);
            //EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        };

        //Cambia lingua selezionata in base a dropDown
        languageDropdownField.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            string selectedLanguageName = evt.newValue;
            selectedLanguage = GetLanguageBaseFromString(selectedLanguageName);
            languageContentFoldout.value = false;
            languageContentFoldout.value = true;

        });

        //Mostra Content se foldout aperto
        languageContentFoldout.RegisterCallback<ChangeEvent<bool>>(evt =>
        {
            languageContentFoldout.value = evt.newValue;
            if (languageContentFoldout.value == true && selectedLanguage!= null)
            {
                languageContentScrollView.Clear();
                for (int i = 0; i < selectedLanguage.Content.Count; i++)
                {
                    int currentIndex = i;

                    VisualElement addedVisualElement = new VisualElement();
                    addedVisualElement.AddToClassList("VerticalTextFields");
                    TextField IdsTextField, stringTextField;

                    CreateIDsString(i, out IdsTextField, out stringTextField);

                    addedVisualElement.Add(IdsTextField);
                    addedVisualElement.Add(stringTextField);

                    languageContentScrollView.Add(addedVisualElement);


                    //salvataggio cambio testo Ids
                    IdsTextField.RegisterCallback<ChangeEvent<string>>(evt =>
                    {
                        IdsTextField.value = evt.newValue;
                        LanguageContent lc = selectedLanguage.Content[currentIndex];
                        lc.IDs = IdsTextField.value;
                        selectedLanguage.Content[currentIndex] = lc;

                        EditorUtility.SetDirty(selectedLanguage);
                        //EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());


                    });

                    //salvatagggio cambio Testo string
                    stringTextField.RegisterCallback<ChangeEvent<string>>(evt =>
                    {
                        stringTextField.value = evt.newValue;
                        LanguageContent lc = selectedLanguage.Content[currentIndex];
                        lc.String = stringTextField.value;
                        selectedLanguage.Content[currentIndex] = lc;

                        EditorUtility.SetDirty(selectedLanguage);
                        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
                    });




                    //creo bottone rimuovi
                    Button deleteButton = new Button();
                    deleteButton.text = "Delete";
                    addedVisualElement.Add(deleteButton);

                    //rimuovi Language Content  se premuto
                    deleteButton.clicked += () =>
                    {
                        if (selectedLanguage.Content.Count - 1 <= 0)
                        {
                            EditorUtility.DisplayDialog("Error", "Can't delete or " + selectedLanguage.LanguageName + " Content will be empty", "ok");
                            return;
                        }
                        bool deleteConfirmation = EditorUtility.DisplayDialog("Delete IDs", "this will Delete: " + selectedLanguage.Content[currentIndex] + " the list will shift Up .Are you sure?", "Yes", "Cancel");
                        if (deleteConfirmation)
                        {
                            languageContentFoldout.value = false;
                            languageContentScrollView.RemoveAt(currentIndex);
                            selectedLanguage.Content.RemoveAt(currentIndex);
                            languageContentFoldout.value = true;

                        }

                        EditorUtility.SetDirty(selectedLanguage);
                        //EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

                    };


                    //Creo Bottone Reset e Add
                    if (currentIndex == selectedLanguage.Content.Count - 1)
                    {
                        VisualElement resetAddVisual = new VisualElement();
                        resetAddVisual.AddToClassList("VerticalTextFields");
                        languageContentScrollView.Add(resetAddVisual);

                        //Creo Reset
                        Button resetButton = new Button();
                        resetButton.text = "Reset";
                        resetAddVisual.Add(resetButton);
                        //reset Content se premuto
                        resetButton.clicked += () =>
                        {
                            bool resetConfirmation = EditorUtility.DisplayDialog("Reset IDs", "this will reset all IDs to .Are you sure?", "Yes", "Cancel");
                            if (resetConfirmation)
                            {
                                languageContentFoldout.value = false;
                                selectedLanguage.ResetLanguageContent();
                                languageContentFoldout.value = true;
                            }
                            EditorUtility.SetDirty(selectedLanguage);
                            //EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
                        };

                        //Creo Bottone Aggiungi
                        Button addButton = new Button();
                        addButton.text = "Add";
                        resetAddVisual.Add(addButton);
                        addButton.clicked += () =>
                        {
                            languageContentFoldout.value = false;
                            selectedLanguage.Content.Add(new LanguageContent("EMPTY_GAME", "empty game"));
                            CreateIDsString(i, out IdsTextField, out stringTextField);
                            languageContentFoldout.value = true;

                            EditorUtility.SetDirty(selectedLanguage);
                            // EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

                        };
                    }
                }
            }


        });

        //Bottone Esporta lingua
        exportButton.clicked += () =>
        {
            if (selectedLanguage != null)
            {
                string filePath = EditorUtility.SaveFilePanel("Save Language", "Export current Language in CSV", selectedLanguage.LanguageName + ".CSV", "CSV");
                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    string fileContent = "";
                    foreach (LanguageContent content in selectedLanguage.Content)
                    {
                        fileContent += $"'{selectedLanguage.name}','{content.IDs}','{content.String}'\n";
                    }
                    File.WriteAllText(filePath, fileContent);
                }
            }
        };

        //Bottone Importa lingua
        importButton.clicked += () =>
        {
            string filePath = EditorUtility.OpenFilePanel("Import", "This will import a custom language of .CSV", "CSV");

            if (!string.IsNullOrWhiteSpace(filePath))
            {
                LanguageBase nuovaLingua = ScriptableObject.CreateInstance<LanguageBase>();
                nuovaLingua.Content.Clear();
                selectedLanguage = null;
                languageContentFoldout.value = false;
                languageDropdownField.value = null;

                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    bool wordStarted = false;
                    List<string> words = new List<string>();
                    string word = null;

                    foreach (char c in line)
                    {
                        if ((c == '\u0027') && wordStarted == false)
                        {
                            wordStarted = true;
                        }
                        else if ((c == '\u0027') && wordStarted == true)
                        {
                            wordStarted = false;
                            words.Add(word);
                            word = null;
                        }

                        if (wordStarted && c != '\u0027')
                        {
                            word += c;
                        }
                    }


                    nuovaLingua.Content.Add(new LanguageContent() { IDs = words[1], String = words[2] });
                    nuovaLingua.name = words[0];
                    nuovaLingua.LanguageName = words[0];
                    words.Clear();
                }

                //Creazione lingua
                AssetDatabase.CreateAsset(nuovaLingua, "Assets/LanguageManager/Languages/" + nuovaLingua.name + ".asset");
                var asset = AssetDatabase.LoadMainAssetAtPath("Assets/LanguageManager/Languages/" + nuovaLingua.name + ".asset");
                AssetDatabase.SetLabels(asset, new string[] { "Language" });

                //Aggiungo alla dropDown
                languageDropdownField.choices.Add(nuovaLingua.name);
                languageDropdownField.value = nuovaLingua.name;
                selectedLanguage = nuovaLingua;

            }
        };
    }


    private void CreateIDsString(int i, out TextField IdsTextField, out TextField stringTextField)
    {
        IdsTextField = new TextField("ID[" + i + "]");
        IdsTextField.AddToClassList("IDsTextField");
        IdsTextField.AddToClassList("ContentInputField");
        IdsTextField.value = selectedLanguage.Content[i].IDs;

        stringTextField = new TextField("String[" + i + "]");
        stringTextField.AddToClassList("StringTextField");
        stringTextField.AddToClassList("ContentInputField");
        stringTextField.value = selectedLanguage.Content[i].String;


    }
    private static void SearchForCustomLanguage(DropdownField languageDropdownField)
    {
        string[] languageIds = AssetDatabase.FindAssets("l:Language");
        string[] languagePaths = new string[languageIds.Length];
        string[] languageNames = new string[languageIds.Length];


        for (int i = 0; i < languageIds.Length; i++)
        {
            languagePaths[i] = AssetDatabase.GUIDToAssetPath(languageIds[i]);
            languageNames[i] = Path.GetFileNameWithoutExtension(languagePaths[i]);
            languageDropdownField.choices.Add(languageNames[i]);
        }

    }
    private LanguageBase GetLanguageBaseFromString(string languageName)
    {
        string[] tempArray = AssetDatabase.FindAssets(languageName);
        string tempPath = AssetDatabase.GUIDToAssetPath(tempArray[0]);
        return (LanguageBase)AssetDatabase.LoadAssetAtPath(tempPath, typeof(LanguageBase));
    }



}