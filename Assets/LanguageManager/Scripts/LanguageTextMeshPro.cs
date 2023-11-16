using TMPro;
using UnityEngine;

public class LanguageTextMeshPro : MonoBehaviour
{
    private TextMeshProUGUI textMeshProConnected;
    private bool founded;
    
    [SerializeField] private string iDsSelected;
    [SerializeField] private string stringSelected;
    [SerializeField] private LanguageBase selectedLanguage;

    public string IDsSelected
    {
        get { return iDsSelected; }
        set { iDsSelected = value; }
    } 
    public string StringSelected
    {
        get { return stringSelected; }
        set { stringSelected = value; }
    }
    public LanguageBase SelectedLanguage
    {
        get { return selectedLanguage; }
        set { selectedLanguage = value; }
    }
    
    private void Awake()
    {
        textMeshProConnected = GetComponent<TextMeshProUGUI>();
       
    }

    public void ChangeText()
    {
        if (selectedLanguage != null)
        {
            for (int i = 0; i < selectedLanguage.Content.Count; i++)
            {
                if (IDsSelected == selectedLanguage.Content[i].IDs)
                {
                    stringSelected = selectedLanguage.Content[i].String;
                    textMeshProConnected.text = stringSelected;
                  
                    founded = true;
                }

            }
            if (!founded)
            {
                stringSelected = $"??{IDsSelected}??";
                textMeshProConnected.text = stringSelected;               
            }

        }

    }
}
