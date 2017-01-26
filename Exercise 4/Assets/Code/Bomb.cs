using UnityEngine;

public class Bomb : MonoBehaviour {

    /// <summary>
    /// Threshold to trigger explosion
    /// </summary>
    public float ThresholdForce = 2;

    public GameObject ExplosionPrefab;

    /// <summary>
    /// Destroys the box
    /// </summary>
    private void Destruct()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// 1. Turn on the PointEffector2D
    /// 2. Turning off the box’s SpriteRenderer
    /// </summary>
    private void Boom()
    {
        gameObject.GetComponent<PointEffector2D>().enabled = true;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;

        Instantiate(ExplosionPrefab, transform.position, Quaternion.identity, transform.parent);

        //Destruct() occur in 0.1 seconds
        Invoke("Destruct", 0.1f);
    }

    void OnCollisionEnter2D(Collision2D obj)
    {
        //https://docs.unity3d.com/ScriptReference/Collision-relativeVelocity.html
        if (obj.relativeVelocity.magnitude>= ThresholdForce)
        {
            Boom();
        }
    }


}
