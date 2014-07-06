using UnityEngine;
using System.Collections;

public class Scorer : MonoBehaviour {

  // The base score received for each gem cleared.
  public int baseScore = 100;

  // The player's starting multiplier.
  public int startingMultiplier = 1;

  // Current player score.
  private int score;

  // Current player multiplier.
  private int multiplier;

  public int Score {
    get { return score; }
  }

  public int Multiplier {
    get { return multiplier; }
  }

  void Start() {
    score = 0;
    multiplier = startingMultiplier;
  }

  void Update() {
    guiText.text = FormatScore(score);
  }

  public void AddCombo(int numGems) {
    score += (numGems * baseScore) * multiplier;
  }

  public void IncrementMultiplier() {
    ++multiplier;
  }

  private string FormatScore(int score) {
    return string.Format("{0:N0}", score);
  }
}
