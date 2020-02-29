using UnityEngine;
using System.Collections;

public class RotorCollisionController : Singleton<RotorCollisionController> {

    [SerializeField]
    [Tooltip("Bounce force when colliding with an object")]
	private float bounceMultiplier;

    [SerializeField]
    [Tooltip("Drone rotor collider")]
    private Collider rotorCollider;

    [SerializeField]
    [Tooltip("Sound to play when colliding with an object")]
    private AudioSource hitSoundSource;

    [SerializeField]
    [Tooltip("Wait for collision sound to end before playing it again")]
	private bool waitForSoundEnd;

    [SerializeField]
    [Tooltip("Pitch the collision sound by a random value")]
    private bool pitchRandom;

    [SerializeField]
    [Tooltip("Value for max. pitch height")]
	private float maxPitch;

    [SerializeField]
    [Tooltip("value for min. pitch height")]
    private float minPitch;

	void OnCollisionEnter (Collision collision) {
		
		Bounce (collision);
	}

	void OnCollisionStay (Collision collision) {
		
		Bounce (collision);
	}

	void Bounce (Collision collision)
	{
		foreach (ContactPoint contact in collision.contacts) {

			if (contact.thisCollider != rotorCollider)
				continue;

//			Vector3 point = contact.point;
//			Vector3 position = transform.TransformPoint (0, 0, 0);
//			Vector3 bounceDirection = position - point;
//			bounceDirection.Normalize ();

//			contact.

			GetComponent<Rigidbody>().AddForceAtPosition (contact.normal * bounceMultiplier, contact.point, ForceMode.Force);
			Debug.DrawRay(contact.point, contact.normal * 10, Color.white);
			
			PlayHitSound();

			break;
		}
	}

	private void PlayHitSound() {
		
		if(waitForSoundEnd) {
			
			if(!hitSoundSource.isPlaying) {

				PitchHitSound();
				hitSoundSource.Play();
			}
		}
		else {
			
			if(hitSoundSource.isPlaying)
				hitSoundSource.Stop();

			PitchHitSound();
			hitSoundSource.Play();
		}

	}

	private void PitchHitSound() {

		if (!pitchRandom)
			return;

		float pitch = Random.Range (minPitch, maxPitch);
		hitSoundSource.pitch = pitch;
	}
}
