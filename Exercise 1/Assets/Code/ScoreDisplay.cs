using UnityEngine;

/// <summary>
/// Displays the score on the screen
/// </summary>
public class ScoreDisplay : MonoBehaviour {
    private static int score;
    private UnityEngine.UI.Text myText;

    /// <summary>
    /// Initialize the component
    /// </summary>
    internal void Start() {
        myText = GetComponent<UnityEngine.UI.Text>();
        UpdateScoreDisplay(0);

        var tongue = FindObjectOfType<Tongue>();
        tongue.CaughtYummy += IncrementScore;
        var spawn = FindObjectOfType<YummySpawner>();
        spawn.YummyMissed += DecrementScore;
    }

    /// <summary>
    /// Update the text display in the UI.Text component
    /// </summary>
    private void UpdateScoreDisplay (int newScore)
    {
        score = newScore > 0 ? newScore : 0;
        myText.text = string.Format("Score: {0}", score);
	}

    /// <summary>
    /// Called from Tongue when the score is raised
    /// </summary>
    private void IncrementScore(){
        UpdateScoreDisplay(score+1);
    }

    /// <summary>
    /// Called from spawner when the score is lowered.
    /// </summary>
    private void DecrementScore()
    {
        UpdateScoreDisplay(score-1);
    }
}
