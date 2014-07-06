using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GUIText))]
public class HighScore : MonoBehaviour {

  void Start() {
    int score = PlayerPrefs.HasKey(Scorer.HIGH_SCORE_KEY) ?
      PlayerPrefs.GetInt(Scorer.HIGH_SCORE_KEY) : 0;
    GUIText guiText = GetComponent<GUIText>();
    guiText.text = string.Format("High score: {0:N0}", score);
  }
}
