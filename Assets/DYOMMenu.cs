using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class DYOMMenu : MonoBehaviour
{
    GameObject eventSystem;

    [System.Serializable]
    public class DYOMButton
    {
        public string buttonName;
        public string text;
        public string hint;
    }

    public string title;
    public string menuName;
    public enum MenuStyles
    {
        Normal,
        AddObject
    }
    public MenuStyles menuStyle;
    public DYOMController controller;
    public GameObject buttonPrefab;
    public GameObject buttonPrefabSelected;
    public DYOMButton[] buttons;

    private void Awake()
    {
        eventSystem = GameObject.Find("EventSystem");
        transform.name = menuName;
        foreach (var button in buttons)
        {
            GameObject buttonObject = Instantiate(buttonPrefab, transform);
            buttonObject.name = button.text;

            buttonObject.GetComponentInChildren<Text>().text = button.text;
            
            Button buttonButton = buttonObject.GetComponent<Button>();
            buttonButton.onClick.AddListener(delegate { controller.LoadMenu(button.buttonName); });
        }
    }

    private void OnEnable()
    {
        StartCoroutine(SelectFirst());
    }

    private IEnumerator SelectFirst()
    {
        yield return new WaitForEndOfFrame();
        GameObject first = transform.GetChild(0).gameObject;
        first.GetComponent<Button>().Select();
    }
}
