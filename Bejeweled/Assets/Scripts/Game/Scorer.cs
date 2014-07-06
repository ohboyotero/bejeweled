using UnityEngine;
using System.Collections;

public class Scorer : MonoBehaviour {

  public const string HIGH_SCORE_KEY = "player-high-score";

  // The base score received for each gem cleared.
  public int baseScore = 100;

  // The player's starting multiplier.
  public int startingMultiplier = 1;

  // Current player score.
  private int score;

  // Current player multiplier.
  private int multiplier;

  // The player's high score as of the start of the game.
  private int originalHighScore;

  // The player's high score.
  private int highScore;

  public int Score {
    get { return score; }
  }

  public int HighScore {
    get { return highScore; }
  }

  public int Multiplier {
    get { return multiplier; }
  }

  void Start() {
    originalHighScore = PlayerPrefs.HasKey(HIGH_SCORE_KEY) ?
      PlayerPrefs.GetInt(HIGH_SCORE_KEY) : 0;
    highScore = originalHighScore;
    score = 0;
    multiplier = startingMultiplier;
  }

  void Update() {
    guiText.text = FormatScore(score);
  }

  public void AddCombo(int numGems) {
    score += (numGems * baseScore) * multiplier;

    if (score > highScore) {
      highScore = score;
    }
  }

  public void IncrementMultiplier() {
    ++multiplier;
  }

  // Should be called at the end of the game.
  public void GameOver() {
    // If we have a new high score, save it.
    if (originalHighScore != highScore) {
      PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
    }
  }

  private string FormatScore(int score) {
    return string.Format("{0:N0}", score);
  }
}
