using UnityEngine;
using System.Collections;

public class SwitchEmissiveMaterial : MonoBehaviour {

    [SerializeField]
    [Tooltip("Emissive material")]
	private Material emissiveMaterial;

    [SerializeField]
    [Tooltip("Emission color")]
    private Color emissionColor;

	// Use this for initialization
//	void Awake () {
//	
////		emissiveScaleOn = emissiveMaterial.GetFloat ("_EmissionScaleUI");
//		emissionColor = emissiveMaterial.GetColor ("_EmissionColor");
//
//		if (!gameObject.activeInHierarchy)
//			SwitchEmissiveColor (false);
//	}
	
	// Update is called once per frame
//	void Update () {
//	
	//	}
	
	void OnEnable() {
		
		SwitchEmissiveColor (true);
	}
	
	void OnDisable() {
		
		SwitchEmissiveColor (false);
	}

	void SwitchEmissiveColor(bool enable) {

//		float newEmissiveScale = enable ? emissiveScaleOn : 0;
		if(enable)
			emissiveMaterial.SetColor ("_EmissionColor", emissionColor);
		else
			emissiveMaterial.SetColor ("_EmissionColor", Color.black);

	}
}
