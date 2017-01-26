using UnityEngine;
using System.Collections.Generic;   // not just System.Collections
using System;

/// <summary>
/// Maintains a set of platforms and places then as they're needed.
/// 
/// IMPORTANT: this must be placed in the player character,
/// not in its own object.
/// </summary>
[RequireComponent(typeof(Runner))]
public class PlatformManager : MonoBehaviour {
    #region Parameters editable in Unity
    /// <summary>
    /// Prefab to use for creating new platforms
    /// </summary>
    public GameObject PlatformPrefab;

    /// <summary>
    /// Spawn a new platform when the last platform is this far ahead of the player.
    /// </summary>
    public float SpawnAtDistance = 50f;
    
    // Platforms are created with random width in the range [MinPlatformWidth, MaxPlatformWidth]
    public float MinPlatformWidth = 20f;
    public float MaxPlatformWidth = 30f;

    // Platforms are spawned with a horizontal separation in the range [MinXSpacing, MaxXSpacing]
    public float MinXSpacing = 35f;
    public float MaxXSpacing = 45f;

    // Platforms are spawned with a vertical spacing in the range [MinYSpacing, MaxYSpacing]
    public float MinYSpacing = -8f;
    public float MaxYSpacing = 8f;

    /// <summary>
    /// Platforms will be recycled when the player is this distance past them.
    /// </summary>
    public float RecycleDistance = 50f;//50
    #endregion

    #region Internal state of the platform pool
    /// <summary>
    /// Location at which the next platform will be spawned
    /// </summary>
    private Vector3 nextSpawnPoint = new Vector3 (0f, -6.5f, 0f);

    /// <summary>
    /// The platforms that are currently on screen.
    /// </summary>
    private readonly Queue<GameObject> platformsInUse = new Queue<GameObject> ();
    /// <summary>
    /// A pool of platforms that have been instantiated in the past but that aren't
    /// currently needed.  We save these, rather than destroying them so that we can
    /// avoid destorying and recreating platforms, which is somewhat expensive.
    /// </summary>
    private readonly Queue<GameObject> unusedPlatforms = new Queue<GameObject> ();
    #endregion

    /// <summary>
    /// True when the player hasn't lost yet, i.e. when the game isn't over.
    /// </summary>
    private bool playerNotDead = true;

    /// <summary>
    /// Initialize
    /// </summary>
    internal void Start(){
        FindObjectOfType<Runner>().FellIntoTheVoid += OnGameOver;
        SpawnNewPlatform();
    }

    /// <summary>
    /// Handling platform generating and recycling.
    /// </summary>
    internal void Update()
    {
        if (playerNotDead) {
            if (ShouldRecyclePlatform(OldestPlatformInUse, gameObject))
            {
                RecycleOldestPlatform();
            }

            if (ShouldSpawnPlatform(nextSpawnPoint, gameObject))
            {
                SpawnNewPlatform();
            }
        }    
    }

    /// <summary>
    /// Check whether the platform should be spawned.
    /// </summary>
    /// <param name="spawnPoint"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    private bool ShouldSpawnPlatform(Vector3 spawnPoint, GameObject player)
    {
        float playPosX = player.transform.position.x;
        float spawnPointX = spawnPoint.x;

        if ((spawnPointX - playPosX) <= SpawnAtDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    ///  Check whether the platform should be recycled.
    /// </summary>
    /// <param name="oldestPlatformInUse"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    private bool ShouldRecyclePlatform(GameObject oldestPlatformInUse, GameObject player)
    {
        float playPosX = player.transform.position.x;
        float olatformPosX = oldestPlatformInUse.transform.position.x;

        if ((playPosX- olatformPosX) >= RecycleDistance) {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Returns the platform that's been on screen for the longest time,
    /// aka the rearmost platform that's on screen.
    /// </summary>
    private GameObject OldestPlatformInUse
    {
        get { return platformsInUse.Peek(); }
    }

    /// <summary>
    /// Recycle oldest platform out of scene.
    /// </summary>
    private void RecycleOldestPlatform()
    {
        GameObject platform;
        if (platformsInUse.Count > 0)
        {
            platform = platformsInUse.Dequeue();
            platform.SetActive(false);

            unusedPlatforms.Enqueue(platform);
        }
 
    }
    /// <summary>
    /// Remove an unused platform from the pool, place it, and activate it.
    /// </summary>
    private void SpawnNewPlatform() {
        GameObject platform;
        if (unusedPlatforms.Count > 0) {
            platform = unusedPlatforms.Dequeue();
            platform.SetActive(true);
        } else {
            platform = Instantiate(PlatformPrefab);
        }

        platformsInUse.Enqueue(platform);
        MovePlatformToSpawnPoint(platform);
    }

    /// <summary>
    /// Move platform to SpawnPoint
    /// </summary>
    /// <param name="platform"></param>
    private void MovePlatformToSpawnPoint(GameObject platform)
    {
        platform.transform.position = nextSpawnPoint;
        platform.transform.localScale =
            new Vector3(
                UnityEngine.Random.Range(MinPlatformWidth, MaxPlatformWidth),
                1f,
                1f);

        nextSpawnPoint.x += UnityEngine.Random.Range
            (MinXSpacing, MaxXSpacing);

        nextSpawnPoint.y += UnityEngine.Random.Range
     (MinYSpacing, MaxYSpacing);
    }

    /// <summary>
    /// Called when the game is over.
    /// </summary>
    private void OnGameOver(){
        // Clear playerNotDead
        playerNotDead = false;
    }
}
