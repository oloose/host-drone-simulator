using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : Singleton<CameraController> {

    [SerializeField]
    [Tooltip("List of all possible cameras")]
	private List<Camera> Cameras;

	private int currentCameraIndex;

	private Camera currentCamera;

	// Use this for initialization
	void Start () {
	
		currentCameraIndex = 0;
//		currentCameraIndex = Cameras.Count - 1;
//		NextCamera ();
	}
	
	// Update is called late once per frame
	void LateUpdate () {
	
		if (Input.GetButtonDown ("Camera"))
			NextCamera ();
	}

	void NextCamera () {

        Cameras[currentCameraIndex].gameObject.SetActive(false);

		currentCameraIndex++;

		if (currentCameraIndex >= Cameras.Count)
			currentCameraIndex = 0;

		ActivateCameraByCurrentIndex ();
	}

	void ActivateCameraByCurrentIndex() {
		
		currentCamera = Cameras [currentCameraIndex];
		
		currentCamera.gameObject.SetActive(true);
	}

	public void SetSpecialCamera(Camera camera) {

		if (camera == null) {
            currentCamera.gameObject.SetActive(false);
			ActivateCameraByCurrentIndex();
			return;
		}

		if(currentCamera != null)
			currentCamera.gameObject.SetActive (false);

		currentCamera = camera;
        currentCamera.gameObject.SetActive(true);
	}

	public Camera GetCurrent() {

		return currentCamera;
	}

}
