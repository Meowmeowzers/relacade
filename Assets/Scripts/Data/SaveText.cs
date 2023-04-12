using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;


public class SaveText : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputField;
    public List<TextData> text = new();
    public TextData textData;

    public void Save()
    {
        textData = new TextData(inputField.text);
        text.Add(textData);
        DataHandler.SavetoJSON(text, "text.json");
    }
}
