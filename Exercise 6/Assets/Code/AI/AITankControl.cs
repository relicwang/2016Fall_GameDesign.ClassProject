using System.Text;
using UnityEngine;

/// <summary>
/// Tank controller for AI-based tanks
/// Ticks behavior tree to control tank.
/// </summary>
public class AITankControl : MonoBehaviour
{
    /// <summary>
    /// The behavior tree to use for this tank.
    /// </summary>
    public GroupDecider BehaviorTree;

    // Initialize BehaviorTree field
    internal void Reset()
    {
        BehaviorTree = (GroupDecider)ScriptableObject.CreateInstance(typeof(GroupDecider));
    }

    public int AILevel = 0; // how "smart" the AI is

    public float ForwardSpeed = 1f; // How fast to drive
    public float Acceleration = 2f; // Gain for velocity control.
    public float RotationSpeed = 80f; // How fast to turn

    public float FireCooldown = 0.5f; // Delay between shooting
    public float Recoil = 10; // Pushback on the tank when it fires

    // Prefab for the bullets we fire.
    public GameObject Projectile;
    // Color to tint the projections fired by this tank
    public Color ProjectileColor = Color.white;

    public PowerUpController PowerUp;

    public AudioClip FireSound;
    public AudioClip DeployPowerUpSound;
    public AudioClip PickUpPowerUpSound;

    private Rigidbody2D _tankRb;
    private float _coolDownTimer; // Time at which we will next be allowed to fire.

    internal void Start() { _tankRb = GetComponent<Rigidbody2D>(); }

    internal void Update()
    {
        ApplyDamping();
        BehaviorTree.Tick(this);
    }

    void ApplyDamping() {
        var right = transform.right;
        var damping = Vector2.Dot(right, _tankRb.velocity);
        _tankRb.AddForce(-damping*right);
    }

    public void MoveTowards(Vector3 goal) {
        float angle = Vector2.Angle(goal - transform.position, transform.up);
        if (angle < 10f) {
            MoveForward();
        } else {
            Turn(goal);
        }
    }

    private void MoveForward() {
        var forwardSpeed = Vector2.Dot(_tankRb.velocity, transform.up);
        var forceMagnitude = Acceleration*(ForwardSpeed - forwardSpeed);
        _tankRb.AddForce(forceMagnitude*transform.up);   
    }

    public void Turn(Vector3 target) {
        if (_tankRb.angularVelocity < RotationSpeed*20)
        {
            Quaternion look = Quaternion.LookRotation(Vector3.forward, target - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, look, RotationSpeed * Time.smoothDeltaTime);
        }
    }

    /// <summary>
    /// Fire the gun, if possible
    /// </summary>
    public void Fire() {
        if (Time.time > _coolDownTimer) {
            FireProjectile();
            _coolDownTimer = Time.time + FireCooldown;
        }
    }

    /// <summary>
    /// Deplot the powerup, if possible
    /// </summary>
    public void FirePowerUp() {
        if (PowerUp)
        {
            PowerUp.Fire();
            PowerUp = null;
            GetComponent<AudioSource>().PlayOneShot(DeployPowerUpSound);
        }
    }
        
    void FireProjectile() {
        var go = Instantiate(Projectile) ;
        var ps = go.GetComponent<Projectile>();
        var up = transform.up;
        ps.Init(gameObject, transform.position + up * 2f, up);
        GetComponent<AudioSource>().PlayOneShot(FireSound);
        _tankRb.AddForce(-Recoil*transform.up,ForceMode2D.Impulse);
    }

    #region Debug graphics
    private readonly StringBuilder path = new StringBuilder(100);
    internal void OnGUI()
    {
        path.Length = 0;
        BehaviorTree.GetCurrentPath(path);

        var screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        var GUIScreenPosition = new Vector2(screenPosition.x, 20+Screen.height-screenPosition.y);
        GUI.Label(new Rect(GUIScreenPosition.x, GUIScreenPosition.y, 200, 50),
            path.ToString());
    }

    internal void OnDrawGizmos()
    {
        BehaviorTree.OnDrawBTGizmos(this);
    }
    #endregion
}
