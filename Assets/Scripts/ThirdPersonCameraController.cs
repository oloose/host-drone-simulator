using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class ThirdPersonCameraController : MonoBehaviour {
	
	public LayerMask layerMask;
	public Transform anchor;
	public float wallOffsetAmount;

	private Vector3 offset;
//	private Vector3

	// Use this for initialization
	void Start () {
	
		offset = transform.position;
		offset = anchor.InverseTransformPoint (offset);
	}
	
	// Update is called once per frame
	void LateUpdate () {
	
//		Camera camera = GetComponent<Camera> ();
//		Vector3.

		Vector3 targetPosition = anchor.TransformPoint (offset);

		transform.position = CameraHelper.CompensateCameraCollision (
			anchor.position,
			targetPosition,
			layerMask,
			wallOffsetAmount);

//		Debug.DrawRay (targetPosition, Vector3.up);
//		Debug.DrawLine (anchor.position, targetPosition);
//
//		RaycastHit hitInfo;
//		if(Physics.Linecast(anchor.position, targetPosition, out hitInfo)) {
//						
//			Debug.DrawRay (hitInfo.point, hitInfo.normal * 10, Color.red);
//			transform.position = hitInfo.point;
//		}
	}
}
