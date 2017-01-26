using UnityEngine;

/// <summary>
/// Behavior that runs to a random location
/// </summary>
public class Flee : BehaviorTreeNode
{
    /// <summary>
    /// Where we're running to
    /// </summary>
    private Vector3 goal;

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


    bool isMoving = false;

    /// <summary>
    /// Run toward the goal
    /// </summary>
    /// <param name="tank">Tank being controlled</param>
    /// <returns>True if behavior wants to keep running</returns>
    public override bool Tick(AITankControl tank)
    {
        if (getDistance(tank.transform.position, goal) < 2)
        {
            return false;
        }
        tank.MoveTowards(goal);

        return true;

    }

    public override void Activate(AITankControl tank)
    {
        goal = SpawnController.FindFreeLocation(4);
        while (WallBetween(tank.transform.position, goal))
        {
            goal = SpawnController.FindFreeLocation(4);
        }
    }

    /// <summary>
    /// Return distance of two position
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <returns></returns>
    float getDistance(Vector3 p1, Vector3 p2)
    {
        return (p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y);
    }

}
