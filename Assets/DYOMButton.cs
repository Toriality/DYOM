using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DYOMButton : MonoBehaviour
{
    public string textDisplay;
    
    // Start is called before the first frame update
    void Start()
    {
        Button button = transform.GetComponent<Button>();
        string text = transform.GetComponentInChildren<Text>().text;
        text = transform.name;
        button.onClick.AddListener(delegate { LoadMenu(text); });
    }

    void LoadMenu(string menu)
    {

    }
}
