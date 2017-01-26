using UnityEngine;

/// <summary>
/// Implements player control of tanks, as well as collision detection.
/// </summary>
public class TankControl : MonoBehaviour
{
    /// <summary>
    /// How fast to drive
    /// </summary>
    public float ForwardSpeed = 1f;

    /// <summary>
    /// How fast to turn
    /// </summary>
    public float RotationSpeed = 80f;

    /// <summary>
    /// Delay between shooting
    /// </summary>
    public float FireCooldown = 0.5f;

    /// <summary>
    /// Keyboard controls for the player.
    /// </summary>
    public KeyCode ForwardKey, LeftKey, RightKey, FireKey;

    /// <summary>
    /// Prefab for the bullets we fire.
    /// </summary>
    public GameObject Projectile;

    /// <summary>
    /// Forward distance per Time.deltaTime.
    /// </summary>
    float forward_distance;

    /// <summary>
    /// Next available shooting time.
    /// </summary>
    float timeStamp = 0f;

    void Start()
    {
        forward_distance = Time.deltaTime * ForwardSpeed;
    }


    // <summary>
    /// Check if the player wants to stick the tongue out yet.
    /// Called once per frame.
    /// </summary>
    internal void Update()
    {
        if (Input.GetKey(ForwardKey))
        {
            transform.position += transform.up * forward_distance;
        }
        if (Input.GetKey(LeftKey))
        {
            Rotation += RotationSpeed * Time.deltaTime;
        }
        if (Input.GetKey(RightKey))
        {
            Rotation -= RotationSpeed * Time.deltaTime;
        }
        if (Input.GetKeyDown(FireKey))
        {
            if (Time.time > timeStamp)
            {
                timeStamp = Time.time + FireCooldown;
                Vector3 pjt_pos = transform.position + transform.up * (float)1.5;
                GameObject tank_Pjt = (GameObject)Instantiate(Projectile);
                Projectile elemenet = tank_Pjt.GetComponent<Projectile>();

                elemenet.Init(gameObject, pjt_pos, transform.up);
            }
        }
    }

    /// <summary>
    /// Current rotation of the tank (in degrees).
    /// We need this because Unity's 2D system is built on top of its 3D system and so they don't
    /// give you a method for finding the rotation that doesn't require you to know what a quaternion
    /// is and what Euler angles are.  We haven't talked about those yet.
    /// </summary>
    private float Rotation
    {
        get
        {
            return transform.rotation.eulerAngles.z;
        }
        set
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, value)); // don't worry about this just yet
        }
    }

    /// <summary>
    /// Handling events when other objects "entered" into tank
    /// </summary>
    /// <param name="target"></param>
    internal void OnTriggerEnter2D(Collider2D target)
    {
        //Other object is "Projectile"
        //Destory the "Projectile", and increase the opponent tank score
        if (target.tag == "Projectile")
        {
            Projectile elemenet = target.GetComponent<Projectile>();
            ScoreManager.IncreaseScore(elemenet.Creator, 10);
            Destroy(target.gameObject);
        }
        //Other object is "Mine"
        //Decrease the current tank score
        else if (target.tag == "Mine")
        {
            var obj = FindObjectOfType<ScoreManager>();
            ScoreManager.DecreaseScore(gameObject, 20);
            Destroy(target.gameObject);

        }
        //Other object is "WallOfDeath"
        //It is already handled in "WallOfDeath"
        //avoid duplicate deleting
        else if (target.tag == "WallOfDeath")
        {
            //Destroy(gameObject);
        }
        //Other object is "Tank"
        //Do nothing
        else if (target.tag == "Tank")
        {
            //Destroy(gameObject);
        }
        //Destory any other objects
        else
        {
            Destroy(target.gameObject);
        }
    }

}
