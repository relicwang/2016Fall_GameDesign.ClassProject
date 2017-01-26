using System;
using UnityEngine;

/// <summary>
/// Creates yummies for the frog to eat.
/// Also detects when the yummy falls off the bottom of the screen.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class YummySpawner : MonoBehaviour {
    /// <summary>
    /// The prefab of the yummy to spawn
    /// </summary>
    public GameObject YummyPrefab;
    /// <summary>
    /// Number of seconds between spawns
    /// </summary>
    public float SpawnRate = 5;
    /// <summary>
    /// Rate at which to decrease the spawn rate.
    /// </summary>
    public float SpawnAcceleration = 0.05f;
    /// <summary>
    /// Time at which the next spawn will occur
    /// </summary>
    private float nextSpawn=5;

    /// <summary>
    /// Event handlers to call if the frog misses the yummy
    /// </summary>
    public event Action YummyMissed;




    /// <summary>
    /// Check if it's time to create a new yummy
    /// </summary>
    internal void Update()
    {    
        if(Time.time > nextSpawn) { 
        SpawnYummy();
        }
    }
    
    /// <summary>
    /// Make a new yummy
    /// </summary>
    void SpawnYummy()
    {
        GameObject yummy = (GameObject)Instantiate(YummyPrefab);;
        nextSpawn= Time.time+ (SpawnRate -= SpawnAcceleration);       
    }

    /// <summary>
    /// Called when a yummy hits the spawner's collider, which should be
    /// placed at the bottom of the screen
    /// </summary>
    /// <param name="yummy">The object collided with, i.e. the yummy</param>
    internal void OnTriggerEnter2D(Collider2D yummy) {
        Destroy(yummy.gameObject);
        YummyMissed();
    }
}
