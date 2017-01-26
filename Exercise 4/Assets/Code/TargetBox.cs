using UnityEngine;

public class TargetBox : MonoBehaviour
{
    /// <summary>
    /// Targets that move past this point score automatically.
    /// </summary>
    public static float OffScreen;

    /// <summary>
    /// Ensure AddToScore() only fired once
    /// </summary>
    bool isScored = false;

    internal void Start() {
        OffScreen = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width-100, 0, 0)).x;
    }

    internal void Update()
    {
        if (transform.position.x > OffScreen)
            Scored();
    }

    /// <summary>
    /// Turn the target box green, and passing the mass of the targetbox to ScoreKeeper
    /// </summary>
    private void Scored()
    {      
        gameObject.GetComponent<SpriteRenderer>().color = Color.green;
        if (!isScored)
        {
            ScoreKeeper.AddToScore((int)gameObject.GetComponent<Rigidbody2D>().mass);
            isScored = true;    
        }
    }

    /// <summary>
    /// When the projectile hits the ground, call the Scored() procedure 
    /// </summary>
    /// <param name="obj"></param>
    void OnCollisionEnter2D(Collision2D obj)
    {
        if (obj.gameObject.tag == "Ground")
        {
            Scored();
        }
    }
}
