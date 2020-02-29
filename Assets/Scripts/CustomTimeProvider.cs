using UnityEngine;
using System.Collections;
using System;

public class CustomTimeProvider : MonoBehaviour, DateTimeProvider {

	private DateTime dateTime;

    [SerializeField]
    [Tooltip("Time scale")]
    private float timeScale;

	void Start () {

		dateTime = DateTime.Now;
	}
	
	// Update is called once per frame
	void Update () {
	
		dateTime = dateTime.AddSeconds (Time.deltaTime * timeScale);
	}

	public DateTime GetCurrent () {

		return dateTime;
	}

	public void SetCurrent(DateTime newDateTime) {

		dateTime = newDateTime;
	}

	public void SetTimeScale (float scale) {

		timeScale = scale;
	}
}