using UnityEngine;

/// <summary>
/// Just a simple component to keep track of the bounds of the arena
/// and propose locations to respawn objects
/// </summary>
public class SpawnController : MonoBehaviour
{
    // Screen bounds
    public static float XMin;
    public static float XMax;
    public static float YMin;
    public static float YMax;

    // Fill in within editor with walls defining the boundaries of the arena
    public GameObject LeftWall;
    public GameObject RightWall;
    public GameObject TopWall;
    public GameObject BottomWall;

    // Note the bounds of the arena.
    internal void Start()
    {
        XMin = LeftWall.transform.position.x;
        XMax = RightWall.transform.position.x;
        YMin = BottomWall.transform.position.y;
        YMax = TopWall.transform.position.y;
    }

    /// <summary>
    /// Return a random location within the arena that is at least radius units from any other objects.
    /// </summary>
    /// <param name="radius">Safety distance for placement</param>
    /// <returns>Free location</returns>
    public static Vector2 FindFreeLocation(float radius)
    {
        Vector2 direction = new Vector2(0, 0);
        Vector2 expectedPos = new Vector2(Random.Range(XMin, XMax), Random.Range(YMin, YMax));
        RaycastHit2D[] hits = Physics2D.CircleCastAll(expectedPos, radius, direction);

        while (hits.Length > 0)
        {
            expectedPos = new Vector2(Random.Range(XMin, XMax), Random.Range(YMin, YMax));
            hits = Physics2D.CircleCastAll(expectedPos, radius, direction);
        }

        return expectedPos;
    }
}
