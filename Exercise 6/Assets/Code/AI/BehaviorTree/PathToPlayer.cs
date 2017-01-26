using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Behavior that plans and follows path to player
/// </summary>
public class PathToPlayer : BehaviorTreeNode
{
    /// <summary>
    /// Path we are trying to execute
    /// </summary>
    private List<Waypoint> currentPath;

    /// <summary>
    /// Distance from waypoint at which we move on to the next waypoint
    /// </summary>
    const float ReachedWaypointThreshold = 1;

    bool planned = false;

    /// <summary>
    /// Move toward the next waypoint
    /// </summary>
    /// <param name="tank">Tank being controlled</param>
    /// <returns>True if we aren't there yet</returns>
    public override bool Tick (AITankControl tank)
    {     
        Waypoint cur_wp;
     
        if (currentPath.Count != 0)
        {
            cur_wp = currentPath[0];
            if (getDistance(tank.transform.position, cur_wp.transform.position) <= ReachedWaypointThreshold)
            {
                currentPath.RemoveAt(0);
                if (currentPath.Count != 0)
                {
                    cur_wp = currentPath[0];
                    tank.MoveTowards(cur_wp.transform.position);
                    return true;
                }
            }
            else
            {
                tank.MoveTowards(cur_wp.transform.position);
                return true;
            }
        }
      
        if(currentPath.Count == 0)
        {         
            return false;
        }

        return true;
    }

    float getDistance(Vector3 p1, Vector3 p2)
    {
        return (p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y);
    }
    public override void Activate(AITankControl tank)
    {
        currentPath = Waypoint.FindPath(tank.transform.position, Player.transform.position);
    }
    #region Debug visualization

    public override void OnDrawBTGizmos(AITankControl tank)
    {
        if (currentPath == null || currentPath.Count == 0)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(currentPath[0].transform.position + Vector3.right,
                        tank.transform.position + Vector3.right);
        Gizmos.DrawLine(currentPath[0].transform.position + Vector3.up,
                        tank.transform.position + Vector3.up);
        for (int i = 1; i < currentPath.Count; i++)
        {
            Gizmos.DrawLine(currentPath[i].transform.position + Vector3.right,
                            currentPath[i - 1].transform.position + Vector3.right);
            Gizmos.DrawLine(currentPath[i].transform.position + Vector3.up,
                            currentPath[i - 1].transform.position + Vector3.up);
        }
    }
    #endregion
}
