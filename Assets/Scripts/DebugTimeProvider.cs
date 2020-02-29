using UnityEngine;
using System.Collections;
using System;

public class DebugTimeProvider : MonoBehaviour, DateTimeProvider {

    [SerializeField]
    private bool running;

    [SerializeField]
    private float timeScale;

    [SerializeField]
    private int year;
    [SerializeField]
    private int month;
    [SerializeField]
    private int day;
    [SerializeField]
    private int hour;
    [SerializeField]
    private int minute;
    [SerializeField]
    private int second;
    [SerializeField]
    private int millieSecond;
		
	private DateTime dateTime;
	
	void Start () {
		
		dateTime = DateTime.Now;
	}
	
	// Update is called once per frame
	void Update () {

		if (!running) {

			dateTime = new DateTime(year, month, day, hour, minute, second, millieSecond);
			return;
		}

		dateTime = dateTime.AddSeconds (Time.deltaTime * timeScale);
	}
	
	public DateTime GetCurrent () {
		
		return dateTime;
	}
	
	public void SetCurrent(DateTime dateTime) {
		
		this.dateTime = dateTime;
	}
	
	public void SetTimeScale (float scale) {
		
		timeScale = scale;
	}
}
