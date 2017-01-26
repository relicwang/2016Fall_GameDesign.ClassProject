using UnityEngine;

/// <summary>
/// The ordnance shot by the tanks
/// </summary>
public class Projectile : MonoBehaviour {
    /// <summary>
    /// Who shot us
    /// </summary>
    public GameObject Creator;
    /// <summary>
    /// How fast to move
    /// </summary>
    public float Speed = 5f;

    public AudioClip ExplosionSound;

    /// <summary>
    /// Do dammage if hitting player
    /// </summary>
    /// <param name="collision"></param>
    internal void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.GetComponent<TankControl>() != null)
        {
            var points = Creator != collision.gameObject ? 10 : -20;
            ScoreManager.IncreaseScore(Creator, points);
            collision.gameObject.GetComponent<AudioSource>().PlayOneShot(ExplosionSound);

            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Start the projectile moving.
    /// </summary>
    /// <param name="creator">Who's shooting</param>
    /// <param name="pos">Where to place the projectile</param>
    /// <param name="direction">Direction to move in (unit vector)</param>
    public void Init(GameObject creator, Vector3 pos, Vector3 direction)
    {
        Creator = creator;
        GetComponent<SpriteRenderer>().color = creator.GetComponent<TankControl>().ProjectileColor;
        transform.position = pos;
        GetComponent<Rigidbody2D>().velocity = Speed*direction;
    }
}
