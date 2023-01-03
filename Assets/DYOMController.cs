using Cinemachine;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DYOMController : MonoBehaviour
{
    [Header("Menu config")]
    public List<DYOMMenu> menus;
    public List<DYOMMenu> menuStack;
    public bool disablePlayerMovement;
    public GameObject subtitle;
    public InputField inputField;
    public bool inputMode;
    public int currentEntity = 0;
    public GameObject entity;

    [Header("Virtual Camera")]
    public CinemachineVirtualCamera virtualCamera;
    public float horizontalInput;
    public float verticalInput;
    public float moveSpeed;
    public Vector3 moveDirection;

    [Header("Key mapping")]
    public KeyCode nextKey = KeyCode.Y;
    public KeyCode previewKey = KeyCode.N;
    public KeyCode returnKey = KeyCode.F;
    public KeyCode inputKey = KeyCode.Tab;

    [Header("Player")]
    public GameObject player;

    [Header("Objects")]
    public GameObject renderedObject;
    public IEnumerable<GameObject> arrayOfObjects;

    private void Start()
    {
        // Create Entity gameobj
        entity = new GameObject("Entity");

        // Load subtitle's input field
        inputField = subtitle.GetComponent<InputField>();

        // Load all the game objects
        arrayOfObjects = Resources.LoadAll("Prefabs/Objects").Cast<GameObject>().ToList();

        // Search through all the children to find all existing DYOM Menus
        foreach (Transform child in transform)
        {
            if (child.GetComponent<DYOMMenu>() != null)
                menus.Add(child.GetComponent<DYOMMenu>());
        }
    }

    private void Update()
    {
        MenuInput();
    }

    /// <summary>
    /// Load a new menu
    /// </summary>
    /// <param name="buttonName">Title of the menu</param>
    /// <param name="isPreview">False as default, should only be true when used in LoadPreviousMenu method.</param>
    public void LoadMenu(string buttonName, bool isPreview = false)
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);

        GameObject menu = transform.Find(buttonName).gameObject;
        menu.SetActive(true);

        disablePlayerMovement = true;

        if (!isPreview) menuStack.Add(menu.GetComponent<DYOMMenu>());

        // Configure special menus
        if (menuStack.Last().menuStyle.ToString() == "AddObject")
        {
            entity.transform.position = player.transform.position + player.transform.forward * 5f;

            renderedObject = Instantiate(
                arrayOfObjects.First(),
                entity.transform.position,
                player.transform.rotation,
                entity.transform
                );

            virtualCamera.transform.position = new Vector3(
                player.transform.position.x + player.transform.right.x * 3f,
                player.transform.position.y + player.transform.up.y * 1f,
                player.transform.position.z + player.transform.forward.z * -3f
                );
            virtualCamera.gameObject.SetActive(true);
            virtualCamera.Priority = 2;
            virtualCamera.Follow = entity.gameObject.transform;
            virtualCamera.LookAt = entity.gameObject.transform;

            subtitle.SetActive(true);
            inputField.text = currentEntity.ToString();
        }
    }

    /// <summary>
    /// Load the previous menu from Menu Stack.
    /// </summary>
    private void LoadPreviousMenu()
    {
        if (menuStack.Count > 1)
        {
            menuStack.RemoveAt(menuStack.Count - 1);
            LoadMenu(menuStack.Last().menuName, true);
        }
        else ResetMenu();
    }

    /// <summary>
    /// Resets the Menu Stack and sets all panels as inactive.
    /// </summary>
    private void ResetMenu()
    {
        menuStack.Clear();
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
        disablePlayerMovement = false;
    }

    /// <summary>
    /// Handles the input of menu based on Menu Style enum.
    /// </summary>
    public void MenuInput()
    {
        // If nextKey (default Y) is pressed, start main menu.
        if (menuStack.Count == 0)
        {
            if (Input.GetKeyUp(nextKey))
            {
                LoadMenu("main");
            }
        }
        else
        {
            switch (menuStack.Last().menuStyle.ToString())
            {
                case "Normal": NormalInput(); break;
                case "AddObject": AddObjectInput(); break;
            }
        }
    }

    private void NormalInput()
    {
        if (Input.GetKeyUp(nextKey))
            ResetMenu();
        if (Input.GetKeyUp(returnKey))
            LoadPreviousMenu();
    }

    private void AddObjectInput()
    {
        // Movement Inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Selects next object
        if (Input.GetKeyDown(nextKey))
        {
            if (inputMode) AcceptInput();
            else SelectNextObject();
        }

        // Selects preview object
        if (Input.GetKeyDown(previewKey))
        {
            if (inputMode) AcceptInput();
            else SelectPreviousObject();
        }

        // TAB allows user to type an ID
        if (Input.GetKeyDown(KeyCode.Tab))
            StartInputMode();

        // Slow-mode toggle
        if (Input.GetKey(KeyCode.Mouse1)) moveSpeed = 0.05f / 5f;
        else moveSpeed = 0.05f;

        // Scale object
        if (Input.GetKey(KeyCode.Q))
            ScaleDownObject();
        else if (Input.GetKey(KeyCode.E))
            ScaleUpObject();

        // Rotate or translate in Y axis
        else if (Input.GetKey(KeyCode.LeftShift))
            RotateAndMoveObject();

        // Rotate around X and Z axis
        else if (Input.GetKey(KeyCode.LeftControl))
            RotateZXObject();

        // Move camera around object
        else if (Input.GetKey(KeyCode.CapsLock))
            MoveCameraAroundObject();

        // Normal object movement
        else
            MoveObject();

        // Return
        if (Input.GetKeyDown(returnKey))
            CancelObject();
    }

    private void SelectNextObject()
    {
        if (currentEntity >= arrayOfObjects.Count() - 1)
            currentEntity = 0;
        else
            currentEntity++;
        RenderObject();
    }

    private void SelectPreviousObject()
    {
        if (currentEntity == 0)
            currentEntity = arrayOfObjects.Count() - 1;
        else
            currentEntity--;
        RenderObject();
    }

    private void MoveObject()
    {
        moveDirection =
            new Vector3(
                virtualCamera.transform.forward.x,
                0,
                virtualCamera.transform.forward.z)
            * verticalInput
            + virtualCamera.transform.right
            * horizontalInput;
        entity.transform.Translate(moveDirection * moveSpeed);
    }

    private void StartInputMode()
    {
        inputMode = true;
        inputField.interactable = true;
        inputField.Select();
    }

    private void AcceptInput()
    {
        inputMode = false;
        inputField.interactable = false;
        int inputInt = int.Parse(inputField.text);
        if (inputInt <= arrayOfObjects.Count() - 1 && inputInt >= 0)
            currentEntity = inputInt;
        else
            currentEntity = 0;
        RenderObject();
    }

    private void ScaleDownObject()
    {
        if (renderedObject.transform.localScale.sqrMagnitude >= 0.03f)
            renderedObject.transform.localScale =
                renderedObject.transform.localScale -
                renderedObject.transform.localScale * moveSpeed;
    }

    private void ScaleUpObject()
    {
        if (renderedObject.transform.localScale.sqrMagnitude <= 300f)
            renderedObject.transform.localScale =
                renderedObject.transform.localScale +
                renderedObject.transform.localScale * moveSpeed;
    }

    private void RotateAndMoveObject()
    {
        moveDirection = virtualCamera.transform.up * verticalInput;
        entity.transform.Translate(moveDirection * moveSpeed * 0.5f);
        renderedObject.transform.Rotate(
            new Vector3(0, 0 + horizontalInput * 10f, 0) * moveSpeed);
    }

    private void RotateZXObject()
    {
        moveDirection = 
            virtualCamera.transform.forward * verticalInput + 
            virtualCamera.transform.right * horizontalInput;
        renderedObject.transform.Rotate(moveDirection);
    }

    private void MoveCameraAroundObject()
    {
       moveDirection =
           virtualCamera.transform.forward * verticalInput +
           virtualCamera.transform.right * horizontalInput;
        virtualCamera.transform.position =
            virtualCamera.transform.position + moveDirection * moveSpeed;
    }

    private void RenderObject()
    {
        inputField.text = currentEntity.ToString();
        Destroy(renderedObject);
        renderedObject = Instantiate(
            arrayOfObjects.Skip(currentEntity).First(),
            entity.transform.localPosition,
            Quaternion.identity,
            entity.transform
            );
    }

    private void CancelObject()
    {
        Destroy(renderedObject);
        virtualCamera.gameObject.SetActive(false);
        LoadPreviousMenu();
    }
}
