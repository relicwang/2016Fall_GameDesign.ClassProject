using UnityEngine;

/// <summary>
/// Mine that explodes when the player rides over it
/// </summary>
public class Mine : MonoBehaviour
{
    /// <summary>
    /// Prefab for the explosion animation + force controller
    /// Fill in within editor
    /// </summary>
    public GameObject ExplosionPrefab;

    /// <summary>
    /// Go boom and respawn
    /// </summary>
    /// <param name="other">Object that entered the area</param>
    internal void OnTriggerEnter2D(Collider2D other)
    {
        GetComponent<AudioSource>().Play();
        ScoreManager.IncreaseScore(other.gameObject, -20);
        Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);

        // Respawn
        transform.position = SpawnController.FindFreeLocation(2);
    }
}
