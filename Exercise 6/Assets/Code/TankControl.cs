using UnityEngine;

/// <summary>
/// Implements player control of tanks, as well as collision detection.
/// </summary>
public class TankControl : MonoBehaviour {
    /// <summary>
    /// How fast to drive
    /// </summary>
    public float ForwardSpeed = 1f;
    /// <summary>
    /// Gain for velocity control.
    /// Force is this times the difference between our target velocity and our actual velocity
    /// </summary>
    public float Acceleration = 2f;

    /// <summary>
    /// How fast to turn
    /// </summary>
    public float RotationSpeed = 80f;
    /// <summary>
    /// Delay between shooting
    /// </summary>
    public float FireCooldown = 0.5f;
    /// <summary>
    /// Pushback on the tank when it fires
    /// </summary>
    public float Recoil = 10;

    /// <summary>
    /// Axis for controlling driving
    /// </summary>
    public string VerticalAxis;
    /// <summary>
    /// Axis for controlling rotation
    /// </summary>
    public string HorizontalAxis;
    /// <summary>
    /// Button to fire projectile
    /// </summary>
    public KeyCode FireButton;
    /// <summary>
    /// Button to deploy powerup
    /// </summary>
    public KeyCode PowerUpButton;

    public AudioClip FireSound;
    public AudioClip DeployPowerUpSound;
    public AudioClip PickUpPowerUpSound;

    /// <summary>
    /// Prefab for the bullets we fire.
    /// </summary>
    public GameObject Projectile;
    /// <summary>
    /// Color to tint the projections fired by this tank
    /// </summary>
    public Color ProjectileColor = Color.white;

    /// <summary>
    /// Time at which we will next be allowed to fire.
    /// </summary>
    private float coolDownTimer;

    /// <summary>
    /// Rigid body component for tank.
    /// </summary>
    private Rigidbody2D tankRb;

    /// <summary>
    /// The powerup we've collected.
    /// Null if we don't have one.
    /// </summary>
    public PowerUpController PowerUp;

    /// <summary>
    /// Initialize
    /// </summary>
    internal void Start() {
        tankRb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Joystick values less than this will be treated as zero
    /// </summary>
    const float DeadZoneSize = 0.2f;

    float DeadZone(float axis)
    {
        if (Mathf.Abs(axis) < DeadZoneSize)
            return 0;
        return axis;
    }

    /// <summary>
    /// Move the tank and fire if necessary.
    /// </summary>
    internal void Update()
    {
        // Accelerate if necessary
        var forward = DeadZone(Input.GetAxis(VerticalAxis));
        var targetSpeed = forward*ForwardSpeed;
        var forwardSpeed = Vector2.Dot(tankRb.velocity, transform.up);
        var forceMagnitude = Acceleration*(targetSpeed - forwardSpeed);
        tankRb.AddRelativeForce(forceMagnitude*Vector2.up);

        // Apply directional damping
        var right = transform.right;
        var damping = Vector2.Dot(right, tankRb.velocity);
        tankRb.AddForce(-damping*right);

        // Apply steering control if we aren't spinning out of control
        if (tankRb.angularVelocity < RotationSpeed*0.5f)
        {
            tankRb.angularVelocity = 0;
            tankRb.rotation += DeadZone(Input.GetAxis(HorizontalAxis))*Time.deltaTime*RotationSpeed;
        }

        // Fire
        if (Input.GetKeyDown(FireButton))
        {
            FireProjectileIfPossible();
        }

        // Deploy power up
        if (PowerUp != null && Input.GetKeyDown(PowerUpButton))
        {
            Debug.Log("Ppowerup");
            PowerUp.Fire();
            PowerUp = null;
            GetComponent<AudioSource>().PlayOneShot(DeployPowerUpSound);
        }
    }

    /// <summary>
    /// The player pushed fire.
    /// Launch if we aren't cooling down.
    /// </summary>
    void FireProjectileIfPossible(){
        if (Time.time > coolDownTimer) {
            FireProjectile();
            coolDownTimer = Time.time + FireCooldown;
        }
    }

    /// <summary>
    /// Really and truly fire the projectile.
    /// </summary>
    void FireProjectile() {
        var go = Instantiate(Projectile) ;
        var ps = go.GetComponent<Projectile>();
        var up = transform.up.normalized;  // shouldn't this be normalized already?  -ian
        ps.Init(gameObject, transform.position + up * 2f, up);
        GetComponent<AudioSource>().PlayOneShot(FireSound);
        tankRb.AddForce(-Recoil*transform.up,ForceMode2D.Impulse);
    }
}
