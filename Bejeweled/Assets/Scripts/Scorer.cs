using UnityEngine;
using System.Collections;

public class Scorer {

  // The base score received for each gem cleared.
  private const int BASE_SCORE_PER_GEM = 100;

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

  public Scorer() {
    score = 0;
    multiplier = 1;
  }

  public void AddCombo(int numGems) {
    score += (numGems * BASE_SCORE_PER_GEM) * multiplier;
  }

  public void IncrementMultiplier() {
    ++multiplier;
  }
}
