using UnityEngine;
using System.Collections;

public class TranslateWithGameObject : MonoBehaviour {

	public Transform anchor;

	public Vector3 offset;

	void Update() {

		transform.position = anchor.position + offset;
	}

}
