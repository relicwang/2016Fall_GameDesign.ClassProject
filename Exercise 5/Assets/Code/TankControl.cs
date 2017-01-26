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


    internal void Update() {

        //Section 1: Control the tank moving forward and backward
        //https://docs.unity3d.com/ScriptReference/Input.GetAxis.html
        float desired = DeadZone(Input.GetAxis(VerticalAxis)) * ForwardSpeed;

        //1. Fake act speed
        Vector2 unit_direction_up = new Vector2(transform.up.x, transform.up.y);//Get the unit up direction (y axis)
        float actual_speed_up = (tankRb.velocity.x * unit_direction_up.x+ tankRb.velocity.y * unit_direction_up.y); //Get the speed in up direction

        float force_magnitude = Acceleration * (desired - actual_speed_up);
        tankRb.AddForce(force_magnitude*transform.up);


        //Section 2: Control the steering of the tank
        //https://docs.unity3d.com/ScriptReference/Rigidbody2D.MoveRotation.html

         tankRb.rotation += DeadZone(Input.GetAxis(HorizontalAxis))* RotationSpeed*Time.fixedDeltaTime;
     
        Vector2 unit_direction_right = new Vector2(transform.right.x, transform.right.y);//Get the speed in right direction (x axis)
        float actual_speed_right = (tankRb.velocity.x * unit_direction_right.x + tankRb.velocity.y * unit_direction_right.y);
        float force_magnitude_x = actual_speed_right * 10;
        Vector2  unit_direction_right_reverse = new Vector2(-unit_direction_right.x, -unit_direction_right.y);
       
        //Section 3: Directional damping
        tankRb.AddForce(force_magnitude_x * unit_direction_right_reverse);

        //Section 4:Fire control
        if (Input.GetKey(FireButton)){
            FireProjectileIfPossible();
        }


        // Deploy power up
        if (PowerUp != null && Input.GetKeyDown(PowerUpButton))
        {
            PowerUp.Fire();
            PowerUp = null;
            GetComponent<AudioSource>().PlayOneShot(DeployPowerUpSound);
        }
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
        var up = transform.up.normalized; 
        ps.Init(gameObject, transform.position + up * 2f, up);
        GetComponent<AudioSource>().PlayOneShot(FireSound);
        tankRb.AddForce(-Recoil*transform.up,ForceMode2D.Impulse);
    }
}
