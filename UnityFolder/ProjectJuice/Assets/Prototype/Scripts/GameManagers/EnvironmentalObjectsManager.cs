using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnvironmentalObjectsManager : MonoBehaviour {

    private static EnvironmentalObjectsManager _instance;
    public static EnvironmentalObjectsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<EnvironmentalObjectsManager>();
                if (_instance == null) Debug.LogError("No EnvironmentalObjectsManager found");
            }
            return _instance;
        }
    }

    IEnumerable<ExplosiveObject> EnvironmentalExplosiveObjects { get; set; }

    private List<ExplosiveObject> InstantiatedObjects { get; set; }

    // Use this for initialization
    void Start ()
    {
        InstantiatedObjects = new List<ExplosiveObject>();
        EnvironmentalExplosiveObjects = FindObjectsOfType<ExplosiveObject>();
        foreach(var prefabToKeep in EnvironmentalExplosiveObjects)
        {
            prefabToKeep.gameObject.SetActive(false);
        }
        SpawnEnvironment();
	
	}
	
    public void SpawnEnvironment()
    {
        foreach(var prefab in EnvironmentalExplosiveObjects)
        {
            var currentGameObject = Instantiate(prefab, prefab.transform.position, prefab.transform.rotation) as ExplosiveObject;
            currentGameObject.gameObject.SetActive(true);
            currentGameObject.Destroyed += CurrentGameObject_Destroyed;
            InstantiatedObjects.Add(currentGameObject);
        }
    }

    private void CurrentGameObject_Destroyed(object sender, System.EventArgs e)
    {
        InstantiatedObjects.Remove(sender as ExplosiveObject);
    }

    public void RespawnEnvironment()
    {
        DestroyEnvironment();
        SpawnEnvironment();
    }

    public void DestroyEnvironment()
    {
        foreach(var toDestroy in InstantiatedObjects)
        {
            Destroy(toDestroy.gameObject);
        }
        InstantiatedObjects.Clear();
    }
}
