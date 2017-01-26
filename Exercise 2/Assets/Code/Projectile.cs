using UnityEngine;

public class Projectile : MonoBehaviour {
    /// <summary>
    /// What game object (tank) fired this projectile.
    /// </summary>
    public GameObject Creator;

    /// <summary>
    /// The speed at which projections should move
    /// </summary>
    public float Speed = 5f;
    
    /// <summary>
    /// The distance from the center past which the projectile
    /// has moved off screen and should be destroyed.
    /// </summary>
    public float Border = 20f;

    /// <summary>
    /// Velocity vector at which this projectile is moving
    /// </summary>
    private Vector3 velocity;

    // <summary>
    /// Velocity vector at which this projectile is moving per second
    /// </summary>
    private Vector3 act_velocity;

    // <summary>
    /// Check if the player wants to stick the tongue out yet.
    /// Called once per frame.
    /// </summary>
    internal void Update()
    {
        act_velocity = velocity * Time.deltaTime;
        transform.position += act_velocity;

        if(gameObject.transform.position.x> Border|| gameObject.transform.position.x <(- Border)
            || gameObject.transform.position.y> Border|| gameObject.transform.position.y <(- Border))
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Called by the TankController.
    /// Initializes the state of the Projectile.
    /// </summary>
    /// <param name="creator">The GameObject of the tank firing this projectile</param>
    /// <param name="initialPosition">Where to initially place the projectile</param>
    /// <param name="direction">What direction the projectile should move in</param>
    public void Init(GameObject creator, Vector3 initialPosition, Vector3 direction)
    {
        Creator = creator;
        transform.position = initialPosition;
        velocity = direction * Speed;
    }
}
