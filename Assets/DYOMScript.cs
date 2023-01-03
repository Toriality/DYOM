using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DYOM Script", menuName = "Scriptable Objects/DYOM Script", order = 1)]
public class DYOMScript : ScriptableObject
{
    [System.Serializable]
    public class DYOMMenu : MonoBehaviour
    {
        [System.Serializable]
        public class DYOMButton
        {
            public string text;
            public string hint;
            public bool isSelected;
        }
        public string title;
        public enum MenuStyles
        {
            Normal
        }
        public MenuStyles menuStyle;
        public DYOMButton[] buttons;
    }
    public string prefabName;
    public DYOMMenu[] menus;
}
