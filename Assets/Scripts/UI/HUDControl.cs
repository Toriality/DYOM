using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HUDControl : MonoBehaviour
{
    public GameObject dyomMenu;
    public GameObject subtitle;
    public DYOMHud dyomHudScript;
    public Mission mission;
    public bool isMenuActive;
    private List<string> menuStack= new List<string>();
    public string currentMenu;
    private string[] specialMenus = { "Add Object", "Object", "Add Pickup" };
    private int currentEntity = 0;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private float moveSpeed = 0.05f;
    private bool inputMode = false;


    private void Awake()
    {
        dyomMenu.SetActive(false);
        subtitle.SetActive(false);
    }

    private void Update()
    {
        if (System.Array.IndexOf(specialMenus, currentMenu) != -1)
        {
            HandleSpecialMenu();
        }
        else
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
                    dyomHudScript.ResetMenu();
                }
            }

            if ((Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Backspace)) && isMenuActive) 
            {
                if (menuStack.Count > 1)
                {
                    menuStack.RemoveAt(menuStack.Count - 1);
                    dyomHudScript.HandleMenu(menuStack.Last());
                }
                else
                {
                    dyomMenu.SetActive(false);
                    isMenuActive = false;
                    dyomHudScript.ResetMenu();
                }
            }
        }
    }

    private void HandleSpecialMenu()
    {
        dyomMenu.SetActive(false);
        subtitle.SetActive(true);
        InputField inputField = subtitle.GetComponent<InputField>();

        if (!inputMode) inputField.text = currentEntity.ToString();

        // Y and N will select preview/next
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (inputMode)
            {
                inputMode = false;
                inputField.interactable = false;
                int inputInt = int.Parse(inputField.text);
                if (inputInt <= dyomHudScript.arrayOfObjects.Count() - 1 && inputInt >= 0)
                    currentEntity = inputInt;
                else
                    currentEntity = 0;
            }
            else
            {
                if (currentEntity >= dyomHudScript.arrayOfObjects.Count() - 1)
                    currentEntity = 0;
                else
                    currentEntity++;
            }

            inputField.text = currentEntity.ToString();
            Destroy(dyomHudScript.renderedObject);
            dyomHudScript.renderedObject = Instantiate(
                    dyomHudScript.arrayOfObjects.Skip(currentEntity).First(),
                    dyomHudScript.entity.transform.localPosition,
                    Quaternion.identity,
                    dyomHudScript.entity.transform);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            if (inputMode)
            {
                inputMode = false;
                inputField.interactable = false;
                int inputInt = int.Parse(inputField.text);
                if (inputInt <= dyomHudScript.arrayOfObjects.Count() - 1 && inputInt >= 0)
                    currentEntity = inputInt;
                else
                    currentEntity = 0;
            }
            else
            {
                if (currentEntity == 0)
                    currentEntity = dyomHudScript.arrayOfObjects.Count() - 1;
                else
                    currentEntity--;
            }


            inputField.text = currentEntity.ToString();
            Destroy(dyomHudScript.renderedObject);
            dyomHudScript.renderedObject = Instantiate(
                    dyomHudScript.arrayOfObjects.Skip(currentEntity).First(),
                    dyomHudScript.entity.transform.localPosition,
                    Quaternion.identity,
                    dyomHudScript.entity.transform);
        }

        // Pressing TAB allows user to type an ID
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            inputMode = true;
            inputField.interactable = true;
            inputField.Select();
        }

        // Move/Rotate/Scale the object
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // If Mouse1 is being pressed move speed will decrease by x5
        if (Input.GetKey(KeyCode.Mouse1)) moveSpeed = 0.05f / 5f;
        else moveSpeed = 0.05f;

        // If Q/E is being pressed, decrease/increase scale
        if (Input.GetKey(KeyCode.Q))
        {
            if (dyomHudScript.renderedObject.transform.localScale.sqrMagnitude >= 0.03f)
                dyomHudScript.renderedObject.transform.localScale =
                    dyomHudScript.renderedObject.transform.localScale -
                    dyomHudScript.renderedObject.transform.localScale * moveSpeed;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            if (dyomHudScript.renderedObject.transform.localScale.sqrMagnitude <= 300f)
                dyomHudScript.renderedObject.transform.localScale =
                    dyomHudScript.renderedObject.transform.localScale +
                    dyomHudScript.renderedObject.transform.localScale * moveSpeed;
        }

        // If SHIFT is being pressed, it will be rotated and/or translataed in Y axis
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            moveDirection = dyomHudScript.virtualCamera.transform.up * verticalInput;
            dyomHudScript.entity.transform.Translate(moveDirection * moveSpeed * 0.5f);
            dyomHudScript.renderedObject.transform.Rotate(new Vector3(0, 0 + horizontalInput * 10f, 0) * moveSpeed);
        }

        // If CTRL is pressed, rotate object around X and Z axis
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            moveDirection = dyomHudScript.virtualCamera.transform.forward * verticalInput
                + dyomHudScript.virtualCamera.transform.right * horizontalInput;
            dyomHudScript.renderedObject.transform.Rotate(moveDirection);
        }

        // If CAPSLOCK is pressed, move camera around object
        else if (Input.GetKey(KeyCode.CapsLock))
        {
            moveDirection = dyomHudScript.virtualCamera.transform.forward
                * verticalInput
                + dyomHudScript.virtualCamera.transform.right * horizontalInput;
            dyomHudScript.virtualCamera.transform.position =
                dyomHudScript.virtualCamera.transform.position + moveDirection * moveSpeed;
        }

        // Normal movement
        else
        {
            moveDirection =
                new Vector3(
                    dyomHudScript.virtualCamera.transform.forward.x,
                    0,
                    dyomHudScript.virtualCamera.transform.forward.z)
                * verticalInput
                + dyomHudScript.virtualCamera.transform.right * horizontalInput;
            dyomHudScript.entity.transform.Translate(moveDirection * moveSpeed);
        }

        // Accept
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentMenu == "Object")
            {
                mission.AddObjective(
                    mission.GetNumOfObjectives(),
                    "Object",
                    dyomHudScript.renderedObject,
                    dyomHudScript.renderedObject.transform.position,
                    dyomHudScript.renderedObject.transform.rotation);
            }
            else
            {
                 mission.AddObject(
                    mission.GetNumOfObjects(),
                    dyomHudScript.renderedObject,
                    dyomHudScript.renderedObject.transform.position,
                    dyomHudScript.renderedObject.transform.rotation);
            }  
            Destroy(dyomHudScript.renderedObject);
            isMenuActive = false;
            subtitle.SetActive(false);
            dyomHudScript.virtualCamera.gameObject.SetActive(false);
            menuStack.RemoveRange(0, menuStack.Count);
            dyomHudScript.ResetMenu();
            currentMenu = null;
        }

        // Return
        if (Input.GetKey(KeyCode.F))
        {
            Destroy(dyomHudScript.renderedObject);
            isMenuActive = false;
            subtitle.SetActive(false);
            dyomHudScript.virtualCamera.gameObject.SetActive(false);
            menuStack.RemoveRange(0, menuStack.Count);
            dyomHudScript.ResetMenu();
            currentMenu = null;
        }
    }
}
