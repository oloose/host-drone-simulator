using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using AssemblyCSharp;

public class DateTimeDisplay : MonoBehaviour {

	private Text txtDay;
    private Text txtMonth;
    private Text txtYear;
    private Text txtHour;
    private Text txtMinute;

	// Use this for initialization
	void Start () {

		Text[] texts = GetComponentsInChildren<Text> ();
		
		foreach (Text text in texts) {
			if(text.gameObject.name == "txtDay")
				txtDay = text;
			else if(text.gameObject.name == "txtMonth")
				txtMonth = text;
			else if(text.gameObject.name == "txtYear")
				txtYear = text;
			else if(text.gameObject.name == "txtHour")
				txtHour = text;
			else if(text.gameObject.name == "txtMinute")
				txtMinute = text;
		}
	
	}
	
	// Update is called once per frame
	void Update () {

		DateTime dateTime = Game.dateTimeProvider.GetCurrent ();
		
		txtDay.text = dateTime.Day.ToString ();
		txtMonth.text = dateTime.Month.ToString ();
		txtYear.text = dateTime.Year.ToString ();
		txtHour.text = dateTime.Hour.ToString ();
		txtMinute.text = dateTime.Minute.ToString ();
	}
}
