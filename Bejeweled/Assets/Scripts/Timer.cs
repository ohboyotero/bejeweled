using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour {

  // TODO: do all of this with animation states/transitions.

  // The amount of time on the clock when this timer starts.
  public float startingTime = 61;

  // When the timer runs out, it will fire the Expired event.
  public delegate void TimerEvent(float secondsRemaining);
  public event TimerEvent Expired;

  // Time remaining in the timer 
  private float secondsRemaining;

  // Whether or not this timer is actively counting down.
  private bool timerActive;

  public void TimerStart() {
    timerActive = true;
  }

  public void Reset() {
    timerActive = false;
    secondsRemaining = startingTime;
  }

  void Awake() {
    Reset();
  }

  void Start() {
  }
	
  void Update() {
    if (timerActive) {
      secondsRemaining -= Time.deltaTime;
      if (secondsRemaining <= 0) {
        secondsRemaining = 0;
        timerActive = false;

        if (Expired != null) {
          Expired(secondsRemaining);
        }
      }
    }
    
    guiText.text = FormatTime(secondsRemaining);
  }

  private string FormatTime(float time) {
    return string.Format("{0}:{1:d2}", (int)(time / 60), (int)(time % 60));
  }
}
