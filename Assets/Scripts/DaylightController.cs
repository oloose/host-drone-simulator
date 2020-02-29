using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using System;
using UnityEngine.UI;
using TH = AssemblyCSharp.TimeHelper;

public class DaylightController : Singleton<DaylightController> {

    [SerializeField]
    [Tooltip("Sun light source")]
    public Light sun;

    [SerializeField]
    [Tooltip("Container for sun and stars")]
    public GameObject sunDayOfYearContainer;

    [SerializeField]
    [Tooltip("Container for sun and stars")]
    public GameObject sunTimeOfDayContainer;

    [SerializeField]
    [Tooltip("Sun intensity fade time")]
    public float sunIntensityFadeTime;

    [SerializeField]
    [Tooltip("Sun color blend time")]
    public float sunColorBlendTime;

    [SerializeField]
    [Tooltip("Sun color at morning time")]
    public Color morningSunColor;

    [SerializeField]
    [Tooltip("Sun color at evening time")]
    public Color eveningSunColor;

    [SerializeField]
    [Tooltip("Lights active at night (lamps, etc.)")]
    public GameObject nightLights;

    [SerializeField]
    [Tooltip("Night light switch offset")]
    public float nightLightSwitchOffset;

    [SerializeField]
    [Tooltip("Ambient night light color")]
    public Color ambientLightNight;

    private Color ambientLightDay;
    private float maxSunIntensity;
    private Color sunColor;

    bool dayInitialized = false;
    bool nightInitialized = false;

	// Use this for initialization
	void Start () {

//		Game.dateTimeProvider = GetComponent<DebugTimeProvider> ();

		maxSunIntensity = sun.intensity;
		sunColor = sun.color;

		ambientLightDay = RenderSettings.ambientLight;
	}
	
	// Update is called once per frame
	void Update () {

		DateTime currentTime = Game.dateTimeProvider.GetCurrent ();

		float relativeSunLatitude = GetCurrentRelativeSunLatitude (currentTime);
		relativeSunLatitude *= 0.5f;
		relativeSunLatitude += 0.5f;

		float currentSunAltitude = Mathf.Lerp (Stralsund.LOWEST_ALTITUDE, Stralsund.HIGHEST_ALTITUDE, relativeSunLatitude);

		sunDayOfYearContainer.transform.rotation = Quaternion.Euler (currentSunAltitude, 180, 0);

		float sunrise = Mathf.Lerp(Stralsund.LATEST_SUNRISE, Stralsund.EARLIEST_SUNRISE, relativeSunLatitude);
		float sunset = Mathf.Lerp (Stralsund.EARLIEST_SUNSET, Stralsund.LATEST_SUNSET, relativeSunLatitude);
		float dayLength = sunset - sunrise;
		
		float milliesToday = TH.TimeOfDayInMillies (currentTime);

		float sunAngle;

		if (milliesToday > sunrise && milliesToday < sunset) {

			// day
			if(!dayInitialized) {

				dayInitialized = true;
				nightInitialized = false;
			}

			float sunriseOffset = milliesToday - sunrise;
			float relativeSunriseOffset = sunriseOffset / dayLength;

			sunAngle = Mathf.Lerp(-90f, 90f, relativeSunriseOffset);

			float sunsetOffset = sunset - milliesToday;

			float offset;
			Color sunBlendColor;

			if(sunriseOffset <= sunsetOffset) {

				offset = sunriseOffset;
				sunBlendColor = morningSunColor;
			} else {

				offset = sunsetOffset;
				sunBlendColor = eveningSunColor;
			}

			sun.intensity = Mathf.Lerp(0, maxSunIntensity, offset / sunIntensityFadeTime);

			sun.color = Color.Lerp(sunBlendColor, sunColor, offset / sunColorBlendTime);

			if(nightLights.activeSelf) {

				if(sunriseOffset > nightLightSwitchOffset &&
				   sunsetOffset > nightLightSwitchOffset)
					nightLights.SetActive(false);
			} else {

				if(sunriseOffset <= nightLightSwitchOffset ||
				   sunsetOffset <= nightLightSwitchOffset)
					nightLights.SetActive(true);
			}

//			if(!nightLights.activeSelf) {
//				if(sunsetOffset <= nightLightSwitchOffset || 
//				   sunriseOffset <= nightLightSwitchOffset)
//					nightLights.SetActive(true);
//			}
//			else {
//				if(sunsetOffset > nightLightSwitchOffset && 
//				   sunriseOffset > nightLightSwitchOffset)
//					nightLights.SetActive(false);
//			}

//			if(morning)
//				RenderSettings.ambientLight = Color.Lerp(ambientLightNight, ambientLightDay, offset / sunColorBlendTime);
//			else
//				RenderSettings.ambientLight = Color.Lerp(ambientLightDay, ambientLightNight, offset / sunColorBlendTime);

		} else {

			// night
			if(!nightInitialized) {
								
				sun.intensity = 0;
				RenderSettings.ambientLight = ambientLightNight;

				nightInitialized = true;
				dayInitialized = false;
			}
			float nightLength = TH.MILLIES_PER_DAY - dayLength;

			if(milliesToday < sunrise)
				milliesToday += TH.MILLIES_PER_DAY;

			float sunsetOffset = milliesToday - sunset;
			float relativeSunsetOffset = sunsetOffset / nightLength;

			sunAngle = Mathf.Lerp(90f, 270f, relativeSunsetOffset);

			if(!nightLights.activeSelf)
				nightLights.SetActive(true);
		}

//		sunAngle = this.sunAngle;
		
		sunTimeOfDayContainer.transform.localRotation = Quaternion.Euler (0, sunAngle, 0);
	}

//	float GetSunLatitudeOffset (DateTime currentTime) {
//
//		float sunLatitude = GetCurrentSunLatitude (currentTime);
//		return Stralsund.LATITUDE - sunLatitude;
//	}

	bool solsticesInitialized = false;

	DateTime lastSummerSolstice;
	DateTime nextSummerSolstice;

	float GetCurrentRelativeSunLatitude (DateTime currentTime)
	{
		// initialize summer solstices
		if (!solsticesInitialized) {

			lastSummerSolstice = new DateTime (currentTime.Year, 6, 22);

			if (lastSummerSolstice > currentTime) {

				nextSummerSolstice = lastSummerSolstice;
				lastSummerSolstice = new DateTime (nextSummerSolstice.Year - 1, 6, 22);
			} else {

				nextSummerSolstice = new DateTime (lastSummerSolstice.Year + 1, 6, 22);
			}
			
			solsticesInitialized = true;
		} else if (nextSummerSolstice < currentTime) {

			lastSummerSolstice = nextSummerSolstice;
			nextSummerSolstice = new DateTime(lastSummerSolstice.Year + 1, 6, 22);
		}

		float yearInTicks = nextSummerSolstice.Ticks - lastSummerSolstice.Ticks;
		float currentOffsetInTicks = currentTime.Ticks - lastSummerSolstice.Ticks;
		
//		float piRelativeOffset = Mathf.PI * currentOffsetInTicks / yearInTicks;
		float piRelativeOffset = 2 * Mathf.PI * currentOffsetInTicks / yearInTicks;

		return Mathf.Cos (piRelativeOffset);
	}
}
