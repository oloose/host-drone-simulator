using UnityEngine;
using System.Collections;

public class ForceField : MonoBehaviour {

    [SerializeField]
    [Tooltip("Force direction")]
	private Vector3 forceDirection;

    [SerializeField]
    [Tooltip("Force power")]
    private float forceAmount;

    [SerializeField]
    [Tooltip("Wherever the force power maximum of the forcefield decreases to the border, or is the same at every position in the forcefield")]
    private bool forceFromFieldCenter;

    [SerializeField]
    [Tooltip("Type of forcefield")]
    private ForceMode forceMode;

	void Start() {

		forceDirection.Normalize ();
	}

	void OnTriggerStay(Collider other) {

//		transform.TransformPoint

		if (other.attachedRigidbody) {

			if(forceFromFieldCenter) {

				Vector3 center = transform.TransformPoint(0, 0, 0);
//				Vector3 localCenter = other.transform.InverseTransformPoint(center);

				other.attachedRigidbody.AddForceAtPosition (forceDirection * forceAmount, center, forceMode);
			}
			else
				other.attachedRigidbody.AddForce(forceDirection * forceAmount, forceMode);
		}
	}
}
