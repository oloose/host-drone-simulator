using UnityEngine;
using System.Collections;

public class TranslateWithCamera : MonoBehaviour {

	public CameraController cameraController;
	public Vector3 offset;
	
	// Update is called once per frame
	void Update () {
	
		transform.position = cameraController.GetCurrent ().transform.position + offset;
	}
}
