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

    private int _bouncesLeft = 3; // get rid of the swarm of bullets?

    /// <summary>
    /// Do dammage if hitting player
    /// </summary>
    /// <param name="collision"></param>
    internal void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy")
        {
            var points = Creator.tag != collision.gameObject.tag ? 10 : -20;
            ScoreManager.IncreaseScore(Creator, points);
            collision.gameObject.GetComponent<AudioSource>().PlayOneShot(ExplosionSound);

            Destroy(gameObject);
        }

        if (_bouncesLeft-- < 0) {
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
        if (creator.GetComponent<TankControl>() != null) {
            GetComponent<SpriteRenderer>().color = creator.GetComponent<TankControl>().ProjectileColor;
        } else {
            GetComponent<SpriteRenderer>().color = creator.GetComponent<AITankControl>().ProjectileColor;
        }
        transform.position = pos;
        GetComponent<Rigidbody2D>().velocity = Speed*direction;
    }
}
