using UnityEngine;

/// <summary>
/// Plays the appropriate sound when the score goes up or down.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SoundController : MonoBehaviour {
    /// <summary>
    /// Sound to play when the player scores
    /// </summary>
    public AudioClip ScoreUp;
    /// <summary>
    /// Sound to play when they miss
    /// </summary>
    public AudioClip ScoreDown;
    /// <summary>
    /// Whether to play sounds at all
    /// </summary>
    public bool PlaySound;
    /// <summary>
    /// Connection to the sound card
    /// </summary>
    private AudioSource source;

    /// <summary>
    /// Initialize the component
    /// </summary>
	internal void Start() {
        source = GetComponent<AudioSource>();

        if (PlaySound) {
            var tongue = FindObjectOfType<Tongue>();
            tongue.CaughtYummy += ScoreUpSound;
            var spawn = FindObjectOfType<YummySpawner>();
            spawn.YummyMissed += ScoreDownSound;
        }
    }

    /// <summary>
    /// Play the sound for scoring
    /// </summary>
    private void ScoreUpSound() { source.PlayOneShot (ScoreUp); }

    /// <summary>
    /// Play the sound for losing a point
    /// </summary>
    private void ScoreDownSound() { source.PlayOneShot (ScoreDown); }
}
