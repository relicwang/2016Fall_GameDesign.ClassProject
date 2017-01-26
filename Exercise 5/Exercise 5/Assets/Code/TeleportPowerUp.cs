using UnityEngine;

/// <summary>
/// A simple powerup that teleports you to a random free location
/// </summary>
public class TeleportPowerUp : PowerUpController {
    /// <summary>
    /// Teleport the player
    /// </summary>
    public override void Fire()
    {
        transform.parent.GetComponent<Rigidbody2D>().MovePosition(SpawnController.FindFreeLocation(2));
        PowerUpDone();
    }
}
