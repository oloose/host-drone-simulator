using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class SwitchChildLights : MonoBehaviour {

	void Awake() {

		SwitchLights (enabled);
	}
	
	void OnEnable() {
		
		SwitchLights (true);
	}
	
	void OnDisable() {
		
		SwitchLights (false);
	}

	private void SwitchLights(bool enable) {

		LightSwitch[] lights = GetComponentsInChildren<LightSwitch> ();
		
		foreach (LightSwitch lightSwitch in lights) {
			
			lightSwitch.SwitchLight(enable);
		}
	}
}
