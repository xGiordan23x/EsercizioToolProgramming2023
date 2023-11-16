using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LanguageTextMeshPro))]
public class LanguageTextMeshProEditor : Editor
{
    public LanguageBase selectedLanguage;  
   
   
    public override void OnInspectorGUI()
    {       
        LanguageTextMeshPro component = (LanguageTextMeshPro)target;
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("ID:");
        component.IDsSelected = EditorGUILayout.TextField(component.IDsSelected);
        selectedLanguage = component.SelectedLanguage;
        GUILayout.EndHorizontal();

        GUILayout.Space(10f);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label($"Relative String: ");  
        GUILayout.Label(component.StringSelected, EditorStyles.boldLabel);
        GUILayout.EndHorizontal();

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(target);
        }

    }
  
}
