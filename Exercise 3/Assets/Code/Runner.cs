using UnityEngine;
using System;

/// <summary>
/// Controller for the player
/// </summary>
public class Runner : MonoBehaviour
{
    #region Fields that can be adjusted in the editor
    /// <summary>
    /// Rate at which character accelerates horizontally when running
    /// </summary>
    public float RunningAccelerationRate = 5f;
    /// <summary>
    /// Speed character at which rises when jumping
    /// </summary>
    public float JumpSpeed = 20f;
    /// <summary>
    /// Speed at which character falls
    /// (platformers don't usually implement acceleration due to gravity)
    /// </summary>
    public float Gravity = 0.5f;
    /// <summary>
    /// If the player drops beneath this Y coordinate, they're considered to have fallen into the void.
    /// The right way to do this is to compute the Y coordinate of the bottom of the screen, but we
    /// haven't shown you how to do that yet.
    /// </summary>
    public int BottomOfTheWorld = -60;
    #endregion

    #region Event notifications for other components
    /// <summary>
    /// Called when the character land on a platform
    /// </summary>
    public event Action LandedOnPlatform = delegate { };
    /// <summary>
    /// Called when the character falls into the void
    /// </summary>
    public event Action FellIntoTheVoid = delegate { };
    /// <summary>
    /// True if we've already called FellIntoTheVoid.
    /// </summary>
    private bool notifiedGameEnd;
    #endregion

    #region Internal state variables
    /// <summary>
    /// Kluge factor for aligning character with platform when they land.
    /// Its value is determined by the sizes of the character and the platform and where
    /// the origins of their coordinate systems are relative to their collider edges.
    /// Someday, this should get refactored to something cleaner.
    /// </summary>
    private float magicHeightOffset;

    /// <summary>
    /// Player's current velocity
    /// </summary>
    private Vector3 velocity;

    /// <summary>
    /// True when player is on a platform
    /// </summary>
    private bool isTouchingPlatform;

    /// <summary>
    /// Iniitialization
    /// </summary>
    internal void Awake()
    {
        magicHeightOffset = (GetComponent<BoxCollider2D>().size.y + 1f) / 2f;
        velocity = new Vector3(10f, 0f, 0f);
      //  notifiedGameEnd = false;
        //isTouchingPlatform = true;
    }
    #endregion

    /// <summary>
    /// Update player's status by input
    /// </summary>
    internal void Update()
    {
    
        if (isTouchingPlatform)
        {
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = JumpSpeed;
            }

            if (Input.GetButtonDown("Short Jump"))
            {
                velocity.y = (JumpSpeed / 2f);
            }
            velocity.x = velocity.x + (RunningAccelerationRate * Time.deltaTime);
        }

        else
        {
            velocity.y -= (Gravity * Time.deltaTime);
        }

        transform.Translate(velocity.x * Time.deltaTime, velocity.y * Time.deltaTime,0);

        if (gameObject.transform.position.y <= BottomOfTheWorld)
        {
            if (!notifiedGameEnd)
            {
                FellIntoTheVoid.Invoke();
                notifiedGameEnd = true;
            }
        }
        if (gameObject.transform.position.y <= (BottomOfTheWorld * 2))
        {
            gameObject.SetActive(false);
        }
    }



    /// <summary>
    /// Snap character to the top of the platform at the specified position.
    /// We use this so that if a character hits the platform from the side, they
    /// "climb up" the platform.  It makes the game somewhat easier.
    /// </summary>
    /// <param name="platformPosition"></param>
    private void AlignToPlatform(Vector3 platformPosition)
    {
        var myPos = transform.position;
        myPos.y = platformPosition.y + magicHeightOffset;
        transform.position = myPos;
    }

    /// <summary>
    /// Event handling when player touch the platform
    /// </summary>
    /// <param name="other"></param>
    internal void OnTriggerEnter2D(Collider2D other)
    {
        isTouchingPlatform = true;
        LandedOnPlatform.Invoke();
        AlignToPlatform(other.transform.position);
        velocity.y = 0f;
    }

    /// <summary>
    /// Event handling when player leaves the platform
    /// </summary>
    /// <param name="target"></param>
    internal void OnTriggerExit2D(Collider2D target)
    {
        isTouchingPlatform = false;
    }
}
