using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections;
using Cinemachine;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting.Dependencies.NCalc;
using System;

public class DYOMHud : MonoBehaviour
{
    public GameObject dyom;
    public GameObject buttonPrefab;
    public GameObject titlePrefab;
    public string currentMenu;

    string[] main = { "Mission Menu", "Settings", "Player", "Objectives", "Actors", "Vehicles", "Pickups", "Objects & Effects", "Tools" };
    string[] missionMenu = { "Play Mission", "Save Mission", "Load Mission", "Publish Mission", "New Mission", "Show MissionAudio Code", "Import Mission Data", "Export Mission Data" };
    string[] settings = { "Mission Name", "Intro Text", "Time Limit", "Timed Mission", "Time Of The Day", "Weather", "Minimum Wanted Level", "Maximum Wanted Level" };
    string[] objectives = { "Add Objective", "Edit Objective", "Delete Objective", "Select Objective", "Put After Selected" };
    string[] actors = { "Add Actor", "Edit Actor", "Spawn Actor", "Hide Actor", "Delete Actor" };
    string[] vehicles = { "Add Vehicle", "Edit Vehicle", "Spawn Vehicle", "Hide Vehicle", "Delete Vehicle" };
    string[] pickups = { "Add Pickup", "Edit Pickup", "Spawn Pickup", "Hide Pickup", "Delete Pickup" };
    string[] objects = { "Add Object", "Edit Object", "Spawn Object", "Hide Object", "Delete Object" };
    string[] tools = { "Teleport To Marker", "Teleport To Objective", "Fly Mode", "Spawn Test Vehicle", "Browse Interiors", "Play From Selected" };
    string[] addObjective = { "Vehicle", "Actor", "Pickup", "Checkpoint", "Object", "Special Player Objective", "Special Env. Objective" };
    string[] addSpecialPlayerObjective = { "Teleport", "Player Animation", "Teleport To Car", "Set Wanted Level", "Remove Weapons", "Talk On Phone", "Add Money", "Subtract Money" };
    string[] addSpecialEnvObjective = { "Cutscene", "Countdown", "Timeout", "WeatherChange", "SetTime", "NPCs Behaviour", "Adjust Timelimit", "Start Timed Section" };
    string[] cutscene = { "Static", "Linear", "Smooth", "Follow Actor", "Actor 1st Person", "Actor 3rd Person", "Follow Player", "Player 1st Person", "Person 3rd Person" };
    string[] npcsBehaviour = { "Normal Peds and Vehicles", "No Vehicles", "No Peds", "No vehicles and peds" };
    string[] objectProperties = { "Static", "Move Along Path", "Move When Approached" };
    string[] objectObjective = { "Touch Object" };
    //string[] addActor = { "Ready", "Health", "Gang", "Accuracy", "Headshot", "Must Survive", "Health Bar" };
    //string[] addVehicle = { "Ready", "Health", "Must Survive", "Bullet Proof", "Explosion Proof", "Damage Proof", "Tires Invulnerable", "Locked" };
    //string[] addPickup = { "Weapon", "Health", "Armor", "Bribe", "Other" };
    //string[] addObject = { "Ready", "Move When Approached", "Move Along Path" };
    string[] specialButtons = { "Touch Object" };

    public void ResetMenu()
    {
        currentMenu = null;
    }

    public void HandleMenu(string name = "Design Your Own Mission")
    {
        currentMenu = name;

        switch (name)
        {
            case "Mission Menu":
                StartCoroutine(CreateMenu(missionMenu));
                break;
            case "Add Objective":
                StartCoroutine(CreateMenu(addObjective));
                break;
            case "Add Special Player Objective":
                StartCoroutine(CreateMenu(addSpecialPlayerObjective));
                break;
            case "Add Special Env. Objective":
                StartCoroutine(CreateMenu(addSpecialEnvObjective));
                break;
            case "Cutscene":
                StartCoroutine(CreateMenu(cutscene));
                break;
            case "NPCs Behaviour":
                StartCoroutine(CreateMenu(npcsBehaviour));
                break;
            //case "Add Actor":
            //    StartCoroutine(CreateMenu(addActor));
            //    break;
           // case "Add Vehicle":
           //     StartCoroutine(CreateMenu(addVehicle));
           //     break;
           // case "Add Pickup":
           //     StartCoroutine(CreateMenu(addPickup));
           //     break;
            case "Add Object":
                StartCoroutine(SpecialMenu("addObject"));
                break;
            case "Object":
                StartCoroutine(SpecialMenu("addObject"));
                break;
            case "Object Properties":
                StartCoroutine(CreateMenu(objectProperties));
                break;
            case "Object Objective":
                StartCoroutine(CreateMenu(objectObjective));
                break;
            case "Settings":
                StartCoroutine(CreateMenu(settings));
                break;
            case "Objectives":
                StartCoroutine(CreateMenu(objectives));
                break;
            case "Actors":
                StartCoroutine(CreateMenu(actors));
                break;
            case "Vehicles":
                StartCoroutine(CreateMenu(vehicles));
                break;
            case "Pickups":
                StartCoroutine(CreateMenu(pickups));
                break;
            case "Objects & Effects":
                StartCoroutine(CreateMenu(objects));
                break;
            case "Tools":
                StartCoroutine(CreateMenu(tools));
                break;
            case "Main":
            default:
                StartCoroutine(CreateMenu(main));
                break;
        }   
    }

    public HUDControl hudControl;
    public void HandleSpecialButton(string name)
    {
        switch (name)
        {
            case "Touch Object":
                hudControl.HUDInput(name);
                break;
            default:
                break;
        }
    }

    public CinemachineVirtualCamera virtualCamera;
    public GameObject renderedObject;
    public IEnumerable<GameObject> arrayOfObjects;
    public Transform player;
    public GameObject entity;

    private void Start()
    {
        entity = new GameObject("Entity");
    }

    private IEnumerator SpecialMenu(string name)
    {
        // Destroy past menu
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        yield return new WaitForEndOfFrame();

        // Load assets
        arrayOfObjects = Resources.LoadAll("Prefabs/Objects").Cast<GameObject>().ToList();

        entity.transform.position = player.position + player.forward * 5f;

        renderedObject = Instantiate(
            arrayOfObjects.First(),
            player.position + player.forward * 5f,
            player.rotation, entity.transform );
        

        virtualCamera.transform.position = new Vector3(
                    player.position.x + player.right.x * 3f,
                    player.position.y + player.up.y * 1f, 
                    player.position.z + player.forward.z * -3f);
        virtualCamera.gameObject.SetActive(true);
        virtualCamera.Priority = 2;
        virtualCamera.Follow = entity.gameObject.transform;
        virtualCamera.LookAt = entity.gameObject.transform;
    }

    private IEnumerator CreateMenu(string[] menu) 
    {
        // Destroy past menu
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Wait for the GameObjects to be totally destroyed
        yield return new WaitForEndOfFrame();

        // Create title
        GameObject title = (GameObject)Instantiate(titlePrefab, transform, false);
        title.name = "Title Panel";
        title.GetComponentInChildren<Text>().text = currentMenu;

        // Create all buttons
        foreach (var button in menu)
        {   
            // Create button game object and get it's button component
            GameObject newButtonObj = (GameObject)Instantiate(buttonPrefab, transform, false);
            Button newButton = newButtonObj.GetComponent<Button>();

            // Configure button
            newButtonObj.name = $"{button}";
            newButtonObj.AddComponent<MenuButton>();
            newButtonObj.GetComponentInChildren<Text>().text = button;
            newButtonObj.GetComponentInChildren<Text>().resizeTextForBestFit = true;

            // Button onClick event
            if (System.Array.IndexOf(specialButtons, button) != -1)
                newButton.onClick.AddListener(delegate { HandleSpecialButton(button); });
            else
                newButton.onClick.AddListener(delegate { HandleMenu(button); });

            // Create navigation and navigation settings
            Navigation nav = newButton.GetComponent<Button>().navigation;
            nav.mode = Navigation.Mode.Vertical;
            newButton.GetComponent<Button>().navigation = nav;
        }

        // Select first button
        GameObject first = transform.GetChild(1).gameObject;
        first.GetComponent<Button>().Select();
    }
}
    


