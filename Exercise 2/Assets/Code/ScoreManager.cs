using System;
using UnityEngine;

/// <summary>
/// Keeps track of the scores of the players.
/// </summary>
public class ScoreManager : MonoBehaviour {
    /// <summary>
    /// This is a singleton class (i.e. there's only supposed to be one instance)
    /// This makes it easy to find the one instance.
    /// </summary>
    private static ScoreManager theScoreScript;
    /// <summary>
    /// GameObjects for the players' tanks.
    /// </summary>
    public GameObject[] Players;
    /// <summary>
    /// UI elements in which to display the respective players' scores.
    /// </summary>
    public UnityEngine.UI.Text[] ScoreFields;

    /// <summary>
    /// Scores for the different players
    /// </summary>
    private int[] Scores;

    /// <summary>
    /// Initialize component
    /// </summary>
    internal void Start(){
        theScoreScript = this;
        Scores = new int[Players.Length];
        UpdateText();
    }

    /// <summary>
    /// Increase the score for the designated player
    /// </summary>
    /// <param name="player">Player</param>
    /// <param name="val">Score</param>
    public static void IncreaseScore(GameObject player, int val)
    {

        int playIndex = Array.IndexOf(ScoreManager.theScoreScript.Players, player);
        theScoreScript.Scores[playIndex] += val;
        theScoreScript.UpdateText();
    }


    /// <summary>
    /// Decrease the score for the designated player
    /// </summary>
    /// <param name="player">Player</param>
    /// <param name="val">Score</param>
    public static void DecreaseScore(GameObject player, int val)
    {
        int playIndex = Array.IndexOf(ScoreManager.theScoreScript.Players, player);
        theScoreScript.Scores[playIndex] -= val;
        theScoreScript.UpdateText();
    }


    /// <summary>
    /// Update all the score fields
    /// </summary>
    private void UpdateText(){
        for (int i = 0; i < Players.Length; i++)
            if (Players[i] != null)   
                ScoreFields[i].text = string.Format("{0}: {1}", Players[i].name, Scores[i]);  
    }
}
