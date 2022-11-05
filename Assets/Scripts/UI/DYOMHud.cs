using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections;
using Cinemachine;
using System.Collections.Generic;
using System.Linq;

public class DYOMHud : MonoBehaviour
{
    Color primaryColor = new Color(255 / 255f, 205 / 255f, 29 / 255f);
    Color textColor = new Color(206, 206, 206);
    Color disabledColor = new Color(123, 0, 0);

    public GameObject dyom;
    public GameObject buttonPrefab;
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
    string[] addObjective = { "Add Vehicle", "Add Actor", "Add Pickup", "Add Checkpoint", "Add Object", "Add Special Player Objective", "Add Special Env. Objective" };
    string[] addSpecialPlayerObjective = { "Teleport", "Player Animation", "Teleport To Car", "Set Wanted Level", "Remove Weapons", "Talk On Phone", "Add Money", "Subtract Money" };
    string[] addSpecialEnvObjective = { "Cutscene", "Countdown", "Timeout", "WeatherChange", "SetTime", "NPCs Behaviour", "Adjust Timelimit", "Start Timed Section" };
    string[] cutscene = { "Static", "Linear", "Smooth", "Follow Actor", "Actor 1st Person", "Actor 3rd Person", "Follow Player", "Player 1st Person", "Person 3rd Person" };
    string[] npcsBehaviour = { "Normal Peds and Vehicles", "No Vehicles", "No Peds", "No vehicles and peds" };
    //string[] addActor = { "Ready", "Health", "Gang", "Accuracy", "Headshot", "Must Survive", "Health Bar" };
    //string[] addVehicle = { "Ready", "Health", "Must Survive", "Bullet Proof", "Explosion Proof", "Damage Proof", "Tires Invulnerable", "Locked" };
    //string[] addPickup = { "Weapon", "Health", "Armor", "Bribe", "Other" };
    //string[] addObject = { "Ready", "Move When Approached", "Move Along Path" };

    public void ResetMenu()
    {
        currentMenu = null;
    }

    public void HandleMenu(string name = "Main")
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
        arrayOfObjects = Resources.LoadAll("Prefabs/Objects", typeof(GameObject)).Cast<GameObject>();

        entity.transform.Translate(player.position + player.forward * 5f);


        // Load first object
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

        // Create all buttons
        foreach (var button in menu)
        {   
            // Create button game object and get it's button component
            GameObject newButtonObj = (GameObject)Instantiate(buttonPrefab, transform, false);
            Button newButton = newButtonObj.GetComponent<Button>();

            // Configure button
            newButtonObj.name = $"{button} Button";
            newButtonObj.GetComponentInChildren<Text>().text = button;
            newButtonObj.GetComponentInChildren<Text>().resizeTextForBestFit = true;
            var colors = newButton.colors;
            colors.selectedColor = primaryColor;
            newButton.colors = colors;

            // Button onClick event
            newButton.onClick.AddListener(delegate { HandleMenu(button); });

            // Create navigation and navigation settings
            Navigation nav = newButton.GetComponent<Button>().navigation;
            nav.mode = Navigation.Mode.Vertical;
            newButton.GetComponent<Button>().navigation = nav;
        }

        // Select first button
        GameObject first = transform.GetChild(0).gameObject;
        first.GetComponent<Button>().Select();
    }
}
    