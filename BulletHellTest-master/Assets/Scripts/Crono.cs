using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crono : MonoBehaviour {
	
	private float StartTime;
	// Use this for initialization
	void Start () {
		StartTime = Time.time;
		
	}
	
	// Update is called once per frame
	void Update () {
		float TimerControl = Time.time;
		string mins = ((int)TimerControl / 60).ToString ("00");
		string segs = (TimerControl % 60).ToString ("00");
		string TimerString = string.Format ("{00}:{01}", mins, segs);

		GetComponent<Text> ().text = TimerString.ToString ();
		
	}
}
