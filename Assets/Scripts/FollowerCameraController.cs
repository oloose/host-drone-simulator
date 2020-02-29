using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class FollowerCameraController : MonoBehaviour {
    
    [SerializeField]
    [Tooltip("Layers this camera shows")]
	private LayerMask layerMask;

    [SerializeField]
    [Tooltip("Transform this camera follows")]
    private Transform anchor;

    [SerializeField]
    [Tooltip("Gameobject to use as reference to position the camera when compensating wall collisions")]
    private GameObject positioningReference;

    [SerializeField]
    [Tooltip("Camera offset when colliding with a wall")]
    private float wallOffsetAmount;

	private Vector3 offset;

	// Use this for initialization
	void Start () {
		
		float yaw = anchor.rotation.eulerAngles.y;

		offset = positioningReference.transform.position -
			anchor.position;
		
		offset = Quaternion.AngleAxis (-yaw, Vector3.up) * offset;
	}
	
	// Update is called once per frame
	void LateUpdate () {
	
		float yaw = anchor.rotation.eulerAngles.y;

		transform.rotation = Quaternion.Euler (0, yaw, 0);

		Vector3 relativePosition = Quaternion.AngleAxis (yaw, Vector3.up) * offset;

		Debug.DrawLine (anchor.position, anchor.position + relativePosition);

		transform.position = CameraHelper.CompensateCameraCollision (
			anchor.position,
			anchor.position + relativePosition,
			layerMask,
			wallOffsetAmount);

//		RaycastHit hitInfo;
//		if(Physics.Linecast(anchor.position, anchor.position + relativePosition, out hitInfo, layerMask)) {
//			
//			Debug.DrawRay (hitInfo.point, hitInfo.normal * 10, Color.red);
//			transform.position = hitInfo.point;
//		}
//		else
//			transform.position = anchor.position + relativePosition;
	}
}
