using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A simple powerup that reverses the direction of all projectiles
/// </summary>
/// 
public class ReversePowerup : PowerUpController{
    /// <summary>
    /// reverses the direction of all projectiles
    /// </summary>
    /// 
    ///    /// <summary>
    /// Prefab for the bullets we fire.
    /// </summary>
    public GameObject Projectile;

    /// <summary>
    /// Reverse all porjectiles moving direction.
    /// </summary>
    public override void Fire()
    {
        Projectile[] porjectile_list = FindObjectsOfType(typeof(Projectile)) as Projectile[];

        foreach (Projectile projectile in porjectile_list) {
            projectile.GetComponent<Rigidbody2D>().velocity = -projectile.GetComponent<Rigidbody2D>().velocity;
        }

        PowerUpDone();
    }

}
