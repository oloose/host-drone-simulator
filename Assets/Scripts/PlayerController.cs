using UnityEngine;
using AssemblyCSharp.Utils;

/// <summary>
/// NOT USED!!!
/// </summary>
public class PlayerController : Singleton<PlayerController> {

    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

	public bool stabilized;
	public bool stabilizeTilt;
	public bool stabilizeVerticalMovement;

	public AudioSource rotorSound;
	public float rotorPitchMultiplier;
	public float rotorPitchMinimum;

	public Light stabilizerLed;
	public Light flashlight;

	public float maxTilt;

	public float maxAccelerationUp;
	public float maxAccelerationDown;

	public float maxVelocityUp;
	public float maxVelocityDown;

	public float maxVelocityYaw;
	public float yawAccelerationMultiplier;

	public float breakMultiplier;

	public float breakLimitVelocity;

	public float tiltVelocityMultiplier;
	public float tiltAccelerationMultiplier;

	private float defaultUpAcceleration;

	// Use this for initialization
	void Start () {

        //Save position and rotation for possible respawn
        spawnPosition = gameObject.transform.position;
        spawnPosition = new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z);

        spawnRotation = gameObject.transform.rotation;
        spawnRotation = new Quaternion(spawnRotation.x, spawnRotation.y, spawnRotation.z, spawnRotation.w);

        defaultUpAcceleration = -1 * GetComponent<Rigidbody>().mass * Physics.gravity.y;
		maxAccelerationUp *= GetComponent<Rigidbody>().mass;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetButtonDown ("Stabilizer")) {
			
			stabilized ^= true;

			if(stabilized)
				stabilizerLed.color = Color.green;
			else
				stabilizerLed.color = Color.red;
		}

		if (Input.GetButtonDown ("Light")) {

			flashlight.enabled = !flashlight.enabled;
		}
	}

	void FixedUpdate() {


		Vector3 currentRotation = transform.rotation.eulerAngles;

		float currentRoll = currentRotation.z;
		float currentPitch = currentRotation.x;
		float currentYaw = currentRotation.y;
		
		Vector3 velocity = GetComponent<Rigidbody>().velocity;
		Vector3 localVelocity = transform.InverseTransformDirection (velocity);

		float inputRoll = Input.GetAxis ("Roll");
		float inputPitch = Input.GetAxis ("Pitch");
		float inputYaw = Input.GetAxis ("Yaw");

		float inputThrust = Input.GetAxis ("Thrust");
		
		bool breaking = Input.GetButton("Break");

		float targetRoll;
		float targetPitch;

		if (!stabilized || !stabilizeTilt || !breaking) {
			GetTargetTilt (out targetRoll, out targetPitch,
		               inputRoll, inputPitch,
		               currentRoll, currentPitch);
		} else {

			Vector3 yawDependantVelocity = Quaternion.AngleAxis(-currentRotation.y, Vector3.up) * velocity;

			GetBreakTilt (out targetRoll, out targetPitch,
			              yawDependantVelocity.x, yawDependantVelocity.z);
		}

		float offsetRoll = MathHelper.AngularDistance(currentRoll, targetRoll);
		MathHelper.Flatten (ref offsetRoll, 0.05f);

		float offsetPitch = MathHelper.AngularDistance(currentPitch, targetPitch);
		MathHelper.Flatten (ref offsetPitch, 0.05f);


		float targetVelocityRoll = MathHelper.NthRoot (offsetRoll, 1) * tiltVelocityMultiplier;
//		MathHelper.Flatten (ref targetVelocityRoll, 0.5f);

		float targetVelocityPitch = MathHelper.NthRoot (offsetPitch, 1) * tiltVelocityMultiplier;
//		MathHelper.Flatten (ref targetVelocityPitch, 0.5f);


		Vector3 localAngularVelocity = GetComponent<Rigidbody>().transform.InverseTransformDirection (GetComponent<Rigidbody>().angularVelocity);

		localAngularVelocity.z = targetVelocityRoll;
		localAngularVelocity.x = targetVelocityPitch;

		GetComponent<Rigidbody>().angularVelocity = transform.TransformDirection (localAngularVelocity);


//		float velocityOffsetRoll = targetVelocityRoll - localAngularVelocity.z;
//		MathHelper.Flatten (ref velocityOffsetRoll, 0.01f);
//
//		float velocityOffsetPitch = targetVelocityPitch - localAngularVelocity.x;
//		MathHelper.Flatten (ref velocityOffsetPitch, 0.01f);
//
//
//		float rollAcceleration = MathHelper.NthRoot (velocityOffsetRoll, 1) * tiltAccelerationMultiplier;
////		MathHelper.Flatten (ref rollAcceleration, 1f);
//
//		float pitchAcceleration = MathHelper.NthRoot (velocityOffsetPitch, 1) * tiltAccelerationMultiplier;
////		MathHelper.Flatten (ref pitchAcceleration, 1f);
//
//
//		rigidbody.AddRelativeTorque (pitchAcceleration, 0, rollAcceleration);

//		GetTiltOffset (out offsetRoll, out offsetPitch, targetRoll, targetPitch, currentRoll, currentPitch);
		
//		transform.rotation = Quaternion.Euler (targetPitch, currentYaw, targetRoll);

		float targetVelocity;

		if (stabilized && stabilizeVerticalMovement) {

			Vector3 worldUp = transform.TransformDirection (Vector3.up);
			worldUp *= -velocity.y;
			Vector3 localUp = transform.InverseTransformDirection (worldUp);

			targetVelocity = localUp.y;
		} else {

			targetVelocity = -localVelocity.y;
		}

		float idleAcceleration = MathHelper.NthRoot (targetVelocity, 7) + defaultUpAcceleration;

		MathHelper.Clamp (ref idleAcceleration, maxAccelerationDown, maxAccelerationUp);

		float newAcceleration;

		if (inputThrust == 0)
			newAcceleration = idleAcceleration;
		else if (inputThrust > 0) 
			newAcceleration = MathHelper.Interpolate (idleAcceleration, maxAccelerationUp, inputThrust);
		else 
			newAcceleration = MathHelper.Interpolate (idleAcceleration, maxAccelerationDown, -inputThrust);

		GetComponent<Rigidbody>().AddRelativeForce (0, newAcceleration, 0);

		rotorSound.pitch = newAcceleration * rotorPitchMultiplier + rotorPitchMinimum;

		ApplyYawForce (inputYaw);

	}
	
	private void ApplyYawForce (float inputYaw)
	{
		float targetVelocity = maxVelocityYaw * inputYaw;
		
		Vector3 globalAngularVelocity = GetComponent<Rigidbody>().angularVelocity;
		Vector3 localAngularVelocity = GetComponent<Rigidbody>().transform.InverseTransformDirection (globalAngularVelocity);
		
		float acceleration = MathHelper.NthRoot (targetVelocity - localAngularVelocity.y, 7) * yawAccelerationMultiplier;
		GetComponent<Rigidbody>().AddRelativeTorque (0, acceleration, 0);
	}
	
	void GetTargetTilt(out float targetRoll, 
	                   out float targetPitch,
	                   float inputRoll,
	                   float inputPitch,
	                   float currentRoll,
	                   float currentPitch) {
		
		Vector2 maxDirectionalTiltInput = new Vector2 (inputRoll, inputPitch);
		maxDirectionalTiltInput.Normalize ();
		
		targetRoll = maxDirectionalTiltInput.x * Mathf.Abs (inputRoll) * maxTilt;
		targetPitch = maxDirectionalTiltInput.y * Mathf.Abs (inputPitch) * maxTilt;
		
		if (!stabilized || !stabilizeTilt) {
			targetRoll += currentRoll;
			targetPitch += currentPitch;
		}
		
	}
	
	void GetBreakTilt(out float targetRoll, 
	                  out float targetPitch,
	                  float currentSpeedX,
	                  float currentSpeedY) {

		MathHelper.Flatten(ref currentSpeedX, breakLimitVelocity);
		
		targetRoll = MathHelper.NthRoot (currentSpeedX, 3) * maxTilt * breakMultiplier;
		MathHelper.Clamp (ref targetRoll, -maxTilt, maxTilt);
		MathHelper.Flatten (ref targetRoll, 1f);


		MathHelper.Flatten(ref currentSpeedY, breakLimitVelocity);

		targetPitch = MathHelper.NthRoot (-currentSpeedY, 3) * maxTilt * breakMultiplier;
		MathHelper.Clamp (ref targetPitch, -maxTilt, maxTilt);
		MathHelper.Flatten (ref targetPitch, 1f);
		
	}
	
	float GetCurrentAngle () {
		
		Vector3 currentDirection = transform.TransformDirection (Vector3.up);
		float currentAngle = Vector3.Angle (Vector3.up, currentDirection);
		return currentAngle;
	}

    /// <summary>
    /// Respawns the player by resetting its position, rotatiion and velocity.
    /// </summary>
    public void Respawn() {
        gameObject.transform.position = spawnPosition;
        gameObject.transform.rotation = spawnRotation;
        gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

}
