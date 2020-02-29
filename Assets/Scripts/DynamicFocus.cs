using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class DynamicFocus : MonoBehaviour {

//	public float smooth = 0.5f;

	private DepthOfField dof;
//	public DepthOfFieldDeprecated dofd;

	void Start() {

		dof = GetComponent<DepthOfField> ();
	}

	void LateUpdate() {

		RaycastHit hitInfo;

		if(Physics.Raycast(transform.position, transform.forward, out hitInfo)) {
			
//			dof.focalLength = Mathf.Lerp(dof.focalLength, hitInfo.distance, smooth);
			dof.focalLength = hitInfo.distance;
			Debug.DrawRay(hitInfo.point, hitInfo.normal, Color.magenta);

//			dofd.focalPoint = hitInfo.distance;
		}
	}
}
