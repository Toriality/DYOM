using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public IEnumerable<GameObject> arrayOfModels;
    public bool spawned;
    public LayerMask ground;

    private IEnumerator WaitSomeTime()
    {
        yield return new WaitForSeconds(20f);
    }

    private IEnumerator SpawnModels()
    {
        int LayerToBeIgnored = LayerMask.NameToLayer("whatIsGround");
        spawned = true;
        foreach (var go in arrayOfModels)
        {
            GameObject _go = Instantiate(go, transform.position, Quaternion.identity, transform) as GameObject;
            if (_go.GetComponentInChildren<MeshCollider>() == null)
            {
                Rigidbody rb = _go.AddComponent<Rigidbody>();
            }
            _go.transform.GetChild(0).gameObject.layer = LayerToBeIgnored;
            //_go.gameObject.layer = LayerToBeIgnored;
            yield return new WaitForSeconds(10f);
        }
    }

    private void Start()
    {
        try
        {
            Debug.Log("Loading with Proper Method...");

            // This is the short hand version and requires that you include the "using System.Linq;" at the top of the file.
            var loadedObjects = Resources.LoadAll("Prefabs", typeof(GameObject)).Cast<GameObject>();
            arrayOfModels = loadedObjects;

            // Casts each individual UnityEngine.Object into UnityEngine.GameObject and adds it to an actual list of GameObject type. 
            // This one works too but is the long version.

            //var loadedObjects = Resources.LoadAll("GameObjects");
            //List<GameObject> gameObjects = new List<GameObject>();
            //foreach (var loadedObject in loadedObjects)
            //{
            //    gameObjects.Add(loadedObject as GameObject);
            //}

            //foreach (GameObject go in gameObjects)
            //{
            //    Debug.Log(go.name);
            //}
        }
        catch (Exception e)
        {
            Debug.Log("Proper Method failed with the following exception: ");
            Debug.Log(e);
        }
    }

    private void Update()
    {
        if (!spawned) StartCoroutine(SpawnModels());
    }
}
