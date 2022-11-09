using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission : MonoBehaviour
{
    [Header("Mission Information")]
    [SerializeField] private string missionName;
    [SerializeField] private string missionAuthor;

    [Header("Entries Quantity")]
    [SerializeField] private int numOfObjectives;
    [SerializeField] private int numOfTriggers;
    [SerializeField] private int numOfActors;
    [SerializeField] private int numOfVehicles;
    [SerializeField] private int numOfPickups;
    [SerializeField] private int numOfObjects;

    [Header("Entries Lists")]
    [SerializeField] private List<Objective> objectives;
    [SerializeField] private List<Trigger> triggers;
    [SerializeField] private List<ActorEntity> actors;
    [SerializeField] private List<VehicleEntity> vehicles;
    [SerializeField] private List<PickupEntity> pickups;
    [SerializeField] private List<ObjectEntity> objects;

    [Header("Mission Settings")]
    [SerializeField] private int weather;
    [SerializeField] private int dayTime;

    public void Start()
    {
        // Add the first objective (the player)
        AddObjective(0, "Player", transform);
    }                   

    public int GetNumOfTriggers() { return numOfTriggers; }
    public int GetNumOfVehicles() { return numOfVehicles; }
    public int GetNumOfPickups() { return numOfPickups; }
    public int GetNumOfActors() { return numOfActors; }
    public int GetNumOfObjects() { return numOfObjects; }
    public int GetNumOfObjectives() { return numOfObjectives; }

    /// <summary>
    /// Default objective spawn used for: <c>objects</c>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="type"></param>
    /// <param name="prefab"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    public void AddObjective(
        int id, 
        string type, 
        GameObject prefab, 
        Vector3 position, 
        Quaternion rotation)
    {
        // Add object prefab to the scene
        GameObject gameObject = Instantiate(prefab, position, rotation);
        Objective objective = gameObject.AddComponent<Objective>();

        // ObjectEntity info
        gameObject.name = "Objective" + id;
        objective.id = id;
        objective.type = type;

        // Add to the list
        objectives.Add(objective);
        numOfObjectives = objectives.Count;
    }

    /// <summary>
    /// Default objective spawn used for: <c>Player</c>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="type"></param>
    /// <param name="transform"></param>
    public void AddObjective(
        int id,
        string type,
        Transform transform)
    {
        GameObject gameObject = new GameObject("Objective" + id);
        gameObject.transform.position = transform.position;
        gameObject.transform.rotation = transform.rotation;
        Objective objective = gameObject.AddComponent<Objective>();
        objective.id = id;
        objective.type = type;
        objectives.Add(objective);
        numOfObjectives = objectives.Count;
    }
        
   /// <summary>
   /// Default object spawn. This method is called by default when called by the HUD menu.
   /// </summary>
   /// <param name="id">Object ID</param>
   /// <param name="prefab">Prefab model to spawn</param>
   /// <param name="position">Position in the world</param>
   /// <param name="rotation">Object rotation</param>
    public void AddObject(int id, GameObject prefab, Vector3 position, Quaternion rotation)
    {
        // Add object prefab to the scene
        GameObject gameObject = Instantiate(prefab, position, rotation);
        ObjectEntity objectEntity = gameObject.AddComponent<ObjectEntity>();
 
        // ObjectEntity info
        gameObject.name = "Object" + id;
        objectEntity.id = id;
        objectEntity.spawnAt = GetNumOfObjectives() - 1;

        // Add to the list
        objects.Add(objectEntity);
        numOfObjects = objects.Count;
    }
}
