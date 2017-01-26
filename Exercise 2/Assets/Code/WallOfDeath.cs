using UnityEngine;

/// <summary>
/// Destroys wayward objects that run into it.
/// </summary>
public class WallOfDeath : MonoBehaviour
{
    internal void OnTriggerEnter2D(Collider2D target)
    {
        if (target.tag == "Tank")
        {
            ScoreManager.DecreaseScore(target.gameObject, 50);
        }
        Destroy(target.gameObject);
    }
}
