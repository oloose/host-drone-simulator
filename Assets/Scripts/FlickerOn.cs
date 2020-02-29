using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class FlickerOn : MonoBehaviour, LightSwitch {

    [SerializeField]
    [Tooltip("Min. flicker time when turning light on/off")]
	private float minFlickerTime = 0.5f;

    [SerializeField]
    [Tooltip("Max. flicker time when turning light on/off")]
    private float maxFlickerTime = 2.5f;

	private float intensity;
	private Light flickerLight;

	private float flickerTimeLeft = 0;

	private bool initialized;

	// Use this for initialization
	void Awake () {

		Initialize ();
	}
	
	// Update is called once per frame
	void Update () {
	
		if (flickerTimeLeft <= 0) {

			if(flickerLight.intensity != intensity)
				flickerLight.intensity = intensity;
			return;
		}

		flickerTimeLeft -= Time.unscaledDeltaTime;

		flickerLight.intensity = Random.Range (0, intensity);
	}

	public void SwitchLight(bool enable) {
		
		Initialize ();

		if(enable)
			flickerTimeLeft = Random.Range (minFlickerTime, maxFlickerTime);
		
		if (flickerLight == null) {
			
			Debug.LogError("Light not found: " + gameObject.name);
			
			return;
		}

		flickerLight.enabled = enable;
	}

	void Initialize() {

		if (initialized)
			return;
		
		flickerLight = GetComponent<Light> ();
		
		intensity = flickerLight.intensity;
		
		flickerLight.enabled = enabled;

		initialized = true;

	}

//	void OnEnable() {
//
//		flickerTimeLeft = Random.Range (minFlickerTime, maxFlickerTime);
//
//		flickerLight.enabled = true;
//	}
//
//	void OnDisable() {
//
//		flickerLight.enabled = false;
//	}
}
