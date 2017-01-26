using UnityEngine;

/// <summary>
/// Abstract class for powerups
/// Contains basic pickup/respawn logic w/out the specific code for firing the powerup
/// </summary>
public abstract class PowerUpController : MonoBehaviour
{
    /// <summary>
    /// Fill this in in your powerup class with the behavior of your specific kind of powerup
    /// </summary>
    public abstract void Fire();

    /// <summary>
    /// Notice when we're picked up by a player
    /// - Place self in player tank
    /// - Set their PowerUp field
    /// - Disable further pickup
    /// </summary>
    /// <param name="other"></param>
    internal void OnTriggerEnter2D(Collider2D other)
    {
        var tankControl = other.gameObject.GetComponent<TankControl>();
        if (tankControl != null)
        {
            EnablePickup(false);
            tankControl.PowerUp = this;
            tankControl.GetComponent<AudioSource>().PlayOneShot(tankControl.PickUpPowerUpSound);
            // Reparent to the tank
            transform.parent = other.gameObject.transform;
        }
    }

    /// <summary>
    /// Turn pickup sensing on/off
    /// </summary>
    /// <param name="enabled"></param>
    private void EnablePickup(bool enabled)
    {
        // Make the star appear/disappear
        GetComponent<SpriteRenderer>().enabled = enabled;
        // Turn on/off collision testing
        GetComponent<CircleCollider2D>().enabled = enabled;
    }

    /// <summary>
    /// Called when powerup is done firing.
    /// </summary>
    protected virtual void PowerUpDone()
    {
        Respawn();
    }

    /// <summary>
    /// Replace the powerup elsewhere.
    /// </summary>
    private void Respawn()
    {
        // Move up to root level
        transform.parent = transform.parent.parent;
        transform.rotation = Quaternion.identity;
        transform.position = SpawnController.FindFreeLocation(4);
        EnablePickup(true);
    }
}
