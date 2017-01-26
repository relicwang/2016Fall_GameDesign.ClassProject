﻿using UnityEngine;

/// <summary>
/// A target for the player to fly through
/// </summary>
public class Target : MonoBehaviour {
    /// <summary>
    /// How many points the player gets for flying through this
    /// </summary>
    public int ScoreValue = 5;
    /// <summary>
    /// How fast it should tumble
    /// </summary>
    public float SpinSpeed = 25f;

    private static readonly Vector3 SpinVector = new Vector3(1f, 1f, 0f);

    /// <summary>
    /// Spin in place
    /// </summary>
    internal void Update() {
        transform.Rotate(SpinVector * SpinSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Note score and disappear when the player passes through
    /// </summary>
    /// <param name="other">Ignored (it has to be the player)</param>
    internal void OnTriggerEnter(Collider other) {
        ScoreManager.IncreaseScore(other.gameObject, ScoreValue);
        Destroy(gameObject);
    }
}
