/// <summary>
/// Beavhior that drives in a straight line toward the player.
/// </summary>

using UnityEngine;
using System.Collections;

public class MoveTowardPlayer : BehaviorTreeNode
{
    public override bool Tick (AITankControl tank) {
        if (getDistance(tank.transform.position, Player.transform.position) > 1) { 
        tank.MoveTowards(Player.transform.position);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Return distance between p1 and p2
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <returns></returns>
    float getDistance(Vector3 p1, Vector3 p2)
    {
        return (p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y);
    }
}
