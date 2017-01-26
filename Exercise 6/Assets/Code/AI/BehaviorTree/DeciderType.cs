using System;
using UnityEngine;

public enum DeciderType
{
    /// <summary>
    /// Always willing to run
    /// </summary>
    Always,
    /// <summary>
    /// Only runs when the local region is saturated with bullets
    /// </summary>
    TooManyBullets,
    /// <summary>
    /// Only runs when we have a direct path to the player
    /// </summary>
    LineOfSightToPlayer,
    /// <summary>
    /// Only runs when we have a direct path to the player and can shoot
    /// </summary>
    CanFire
}

public static class DeciderImplementation
{
    const int MaxBullets = 3;
    const float BulletSearchRadius = 20;
    const int MaxAngularDifference = 10; // the number of degrees a target can be off from center before I turn
    const float DistanceThreshold = 20f;

    /// <summary>
    /// Run the specified decider and returns its value
    /// </summary>
    /// <param name="d">Decider to run</param>
    /// <param name="tank">Tank being controlled</param>
    /// <returns>True if decider wants to run</returns>
    public static bool Decide(this DeciderType d, AITankControl tank)
    {
        switch (d)
        {
            case DeciderType.Always:
                return true;

            case DeciderType.TooManyBullets:
                return Physics2D.OverlapCircleAll(tank.transform.position, BulletSearchRadius, (int) Layers.Projectiles).Length
                       > MaxBullets;

            case DeciderType.LineOfSightToPlayer:
                return !BehaviorTreeNode.WallBetween(tank.transform.position, BehaviorTreeNode.Player.position);

            case DeciderType.CanFire:
                float angle = Vector2.Angle(BehaviorTreeNode.Player.position - tank.transform.position,
                                            tank.transform.up);
                return angle < MaxAngularDifference
                       && Vector2.Distance(tank.transform.position, BehaviorTreeNode.Player.position)
                                  < DistanceThreshold;

            default:
                throw new ArgumentException("Unknown Decider: " + d);
        }
    }
}