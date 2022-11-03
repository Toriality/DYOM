using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HUDControl : MonoBehaviour
{
    public GameObject dyomMenu;
    public DYOMHud dyomHudScript;
    public bool isMenuActive;
    private List<string> menuStack= new List<string>();
    public string currentMenu;

    private void Awake()
    {
        dyomMenu.SetActive(false);
    }

    private void Update()
    {
        // Check if menus have changed
        if (currentMenu != dyomHudScript.currentMenu)
        {
            currentMenu = dyomHudScript.currentMenu;
            if (!menuStack.Contains(currentMenu) && currentMenu != null) menuStack.Add(currentMenu);
        }

        // DYOM Menu Button
        if (Input.GetKeyUp(KeyCode.Y))
        {
            dyomMenu.SetActive(!dyomMenu.activeInHierarchy);
            isMenuActive = dyomMenu.activeInHierarchy;
            if (isMenuActive) dyomHudScript.HandleMenu();
            else
            {
                menuStack.RemoveRange(0, menuStack.Count);
                Debug.Log(string.Join(", ", menuStack));
                dyomHudScript.ResetMenu();
            }
        }

        // Return buttons
        if ((Input.GetKeyUp(KeyCode.F) || Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Return)) && isMenuActive)
        {
            if (menuStack.Count == 1)
            {
                dyomMenu.SetActive(!dyomMenu.activeInHierarchy);
                isMenuActive = dyomMenu.activeInHierarchy;
            }
            else
            {
                menuStack.RemoveAt(menuStack.Count - 1);
                dyomHudScript.HandleMenu(menuStack[menuStack.Count - 1]);
            }
        }

        // Forward button (Space not included because it is default)
        if (Input.GetKeyUp(KeyCode.LeftShift) && isMenuActive)
        {

        }
    }
}
