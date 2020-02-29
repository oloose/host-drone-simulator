using UnityEngine;
using System.Collections;
using AssemblyCSharp.Utils;

public class DroneController : Singleton<DroneController> {

    //transform values on spawn
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    // Lights
    [SerializeField]
    [Tooltip("Drone flashlight light source")]
    private Light flashlight;
    [SerializeField]
    [Tooltip("LED light source to indicate that stabilization is active or not")]
    private Light stabilizerLed;

    // Audio
    [SerializeField]
    [Tooltip("Rotor sound asset")]
    private AudioSource rotorSound;
    [SerializeField]
    [Tooltip("Rotor pitch multiplier")]
    private float rotorPitchMultiplier;
    [SerializeField]
    [Tooltip("Rotor pitch minimum")]
    private float rotorPitchMinimum;

    // General config
    [SerializeField]
    [Tooltip("Whether stabilization is active or not ")]
    private bool stabilized = true;
    public bool Stabilized {
        get { return stabilized; }
        set { stabilized = value; }
    }
    [SerializeField]
    [Tooltip("Maximum tilt without stabilization")]
    private bool stabilizeTilt = true;
    public bool StabilizeTilt {
        get { return stabilizeTilt; }
        set { stabilizeTilt = value; }
    }
    [SerializeField]
    [Tooltip("Current stabilization height of the drone")]
    private bool stabilizeHeight = true;
    public bool StabilizeHeight {
        get { return stabilizeHeight; }
        set { stabilizeHeight = value; }
    }

    // Thrust config
    [SerializeField]
    [Tooltip("Maximum upwards (rise) force when flying up")]
    private float maxRiseForce;
    [SerializeField]
    [Tooltip("Maximum downwards (fall) force")]
    private float maxFallForce;
    [SerializeField]
    [Tooltip("Multiplier applied to upwards force")]
    private float liftAccelerationMultiplier;
    //	public float maxRiseSpeed;
    //	public float maxFallSpeed;

    private float idleLiftForce;

    // Tilt config
    [SerializeField]
    [Tooltip("Maximum drone tilt angle")]
    private float maxTiltAngle = 25f;
    public float MaxTiltAngle {
        get { return maxTiltAngle; }
        set { maxTiltAngle = value; }
    }
    [SerializeField]
    [Tooltip("Maximum break tilt angle")]
    private float maxBreakTiltAngle = 30f;
    [SerializeField]
    [Tooltip("Multiplier applied to movement actions when stabilization is disabled")]
    private float unstabilizedTiltMultiplier;
    [SerializeField]
    [Tooltip("Break tilt multiplier")]
    private float breakTiltMultiplier;
    [SerializeField]
    [Tooltip("Maximum tilt speed")]
    private float maxTiltSpeed;
    [SerializeField]
    [Tooltip("Maximum tilt force")]
    private float maxTiltForce;
    [SerializeField]
    [Tooltip("Tilt velocity muliplier")]
    private float tiltVelocityMultiplier;
    [SerializeField]
    [Tooltip("tilt acceleration multiplier")]
    private float tiltAccelerationMultiplier;

    // Yaw config
    [SerializeField]
    [Tooltip("Maximum yaw speed when colliding")]
    private float maxYawSpeed;
    [SerializeField]
    [Tooltip("Maximum yaw torque when colliding")]
    private float maxYawTorque;
    [SerializeField]
    [Tooltip("Acceleration multiplier on yaw speed when colliding")]
    private float yawAccelerationMultiplier;
	
	private bool StabilizeTiltCom{
		get{
			return stabilized && stabilizeTilt;
		}
	}
	
	private bool StabilizeHeightCom{
		get{
			return stabilized && stabilizeHeight;
		}
	}

	private struct DroneState {

		public Vector3 rotation;
		public Vector3 velocityWorld;
		public Vector3 velocityLocal;
		public Vector3 angularVelocityWorld;
		public Vector3 angularVelocityLocal;
	}

	private struct HandlingInput {

		public float roll;
		public float pitch;
		public float yaw;
		public float thrust;
		public bool breaking;
	}

	// Use this for initialization
	void Start () {
        //Save position and rotation for possible respawn
        spawnPosition = gameObject.transform.position;
        spawnRotation = gameObject.transform.rotation;

        idleLiftForce = -Physics.gravity.y * GetComponent<Rigidbody> ().mass;
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
	
	// Update is called once per frame
	void FixedUpdate () {

		HandlingInput input = GetInput ();
		DroneState currentState = GetCurrentState();

		// apply forces
		ApplyTilt (input, currentState);
		ApplyYaw (input, currentState);
		ApplyThrust (input, currentState);
	
	}

	/// <summary>
	/// Applies the tilt.
	/// </summary>
	/// <param name="inputRoll">roll input</param>
	/// <param name="inputPitch">pitch input</param>
	void ApplyTilt (HandlingInput input, DroneState currentState){

		float offsetRoll, offsetPitch;

		if (StabilizeTiltCom) {

			// stabilized

			float targetRoll, targetPitch;

			if (input.breaking) {

				GetBreakTilt(out targetRoll, out targetPitch, currentState);
			} else {

				GetTargetTilt(out targetRoll, out targetPitch, input);
			}

			offsetRoll = MathHelper.AngularDistance (currentState.rotation.z, targetRoll);
			offsetPitch = MathHelper.AngularDistance (currentState.rotation.x, targetPitch);
		} else {

			// unstabilized

			offsetRoll = input.roll * unstabilizedTiltMultiplier;
			offsetPitch = input.pitch * unstabilizedTiltMultiplier;
		}

		// targeted tilt velocity
		float targetVelocityRoll = MathHelper.Aserp (offsetRoll, tiltVelocityMultiplier, maxTiltSpeed);
		float targetVelocityPitch = MathHelper.Aserp (offsetPitch, tiltVelocityMultiplier, maxTiltSpeed);

		float velocityOffsetRoll = targetVelocityRoll - currentState.angularVelocityLocal.z;
		float velocityOffsetPitch = targetVelocityPitch - currentState.angularVelocityLocal.x;

		// targeted tilt force
		float targetForceRoll = MathHelper.Aserp (velocityOffsetRoll, tiltAccelerationMultiplier, maxTiltForce);
		float targetForcePitch = MathHelper.Aserp (velocityOffsetPitch, tiltAccelerationMultiplier, maxTiltForce);

		// apply force
		GetComponent<Rigidbody> ().AddRelativeTorque (targetForcePitch, 0, targetForceRoll);
	}

	/// <summary>
	/// Applies the yaw.
	/// </summary>
	/// <param name="inputYaw">yaw input</param>
	void ApplyYaw (HandlingInput input, DroneState currentState){
		
		float targetVelocity = maxYawSpeed * input.yaw;
		float velocityOffset = targetVelocity - currentState.angularVelocityLocal.y;
		float acceleration = MathHelper.Aserp (velocityOffset, yawAccelerationMultiplier, maxYawTorque);
		GetComponent<Rigidbody> ().AddRelativeTorque (0, acceleration, 0);
	}

	private float minForce;
	private float maxForce;

//	/// <summary>
//	/// Applies the thrust.
//	/// </summary>
//	/// <param name="inputThrust">thrust input</param>
//	void ApplyThrust (HandlingInput input, DroneState currentState){
//
//		float targetForce;
//		float idleVelocity;
//
//		if (StabilizeHeight) {
//
//			float angle = Vector3.Angle (Vector3.up, transform.up);
//			angle *= Mathf.Deg2Rad;
//
//			targetForce = idleLiftForce / Mathf.Cos(angle);
//
//			targetForce = idleLiftForce;
//
//			idleVelocity = transform.InverseTransformDirection (transform.up * -currentState.velocityWorld.y).y;
//		} else {
//
//			targetForce = idleLiftForce;
//			idleVelocity = -currentState.velocityLocal.y;
//		}
//		
//		float targetVelocity = GetTargetLiftVelocity (idleVelocity, input);
//
//		float velocityOffset = targetVelocity - currentState.velocityLocal.y;
//
//		float additionalForce = GetAdditionalLiftForce (velocityOffset);
//
//		targetForce += additionalForce;
//
//		MathHelper.Limit (ref targetForce, idleLiftForce - maxFallForce, idleLiftForce + maxRiseForce);
//
//		GetComponent<Rigidbody> ().AddRelativeForce (0, targetForce + additionalForce, 0);
//	}

	void ApplyThrust(HandlingInput input, DroneState currentState) {

		float velocityOffset;
		
		if (StabilizeHeightCom) {
			
			velocityOffset = transform.InverseTransformDirection (transform.up * -currentState.velocityWorld.y).y;
		} else {
			
			velocityOffset = -currentState.velocityLocal.y;
		}
				
//		float targetAcceleration = MathHelper.NthRoot (targetVelocity, 7) + idleLiftForce;
//		float targetAcceleration = MathHelper.Aserp (targetVelocity, liftAccelerationMultiplier, idleLiftForce);

		float targetAcceleration;

		if (velocityOffset >= 0)
			targetAcceleration = MathHelper.Aserp (velocityOffset, liftAccelerationMultiplier, maxRiseForce - idleLiftForce) + idleLiftForce;
		else
			targetAcceleration = MathHelper.Aserp (-velocityOffset, liftAccelerationMultiplier, maxFallForce - idleLiftForce) + idleLiftForce;
		
//		MathHelper.Clamp (ref targetAcceleration, maxFallForce, maxRiseForce);
		
		float newAcceleration;
		
		if (input.thrust == 0)
			newAcceleration = targetAcceleration;
		else if (input.thrust > 0) 
			newAcceleration = MathHelper.Interpolate (targetAcceleration, maxRiseForce, input.thrust);
		else 
			newAcceleration = MathHelper.Interpolate (targetAcceleration, maxFallForce, -input.thrust);
		
		GetComponent<Rigidbody>().AddRelativeForce (0, newAcceleration, 0);
		
		rotorSound.pitch = newAcceleration * rotorPitchMultiplier + rotorPitchMinimum;
	}

	float GetAdditionalLiftForce (float velocityOffset) {

		if (velocityOffset < 0)
			return MathHelper.Aserp (velocityOffset, liftAccelerationMultiplier, maxFallForce);

		return MathHelper.Aserp (velocityOffset, liftAccelerationMultiplier, maxRiseForce);
	}

//	float GetTargetLiftVelocity(float idleVelocity, HandlingInput input) {
//
//		if (input.thrust < 0)
//				return Mathf.Lerp (idleVelocity, -maxFallSpeed, -input.thrust);
//
//		return Mathf.Lerp (idleVelocity, maxRiseSpeed, input.thrust);
//	}
	
	void GetTargetTilt(out float targetRoll, 
	                   out float targetPitch,
	                   HandlingInput input) {
		
		Vector2 maxDirectionalTiltInput = new Vector2 (input.roll, input.pitch);
		maxDirectionalTiltInput.Normalize ();
		
		targetRoll = maxDirectionalTiltInput.x * Mathf.Abs (input.roll) * maxTiltAngle;
		targetPitch = maxDirectionalTiltInput.y * Mathf.Abs (input.pitch) * maxTiltAngle;
	}
	
	void GetBreakTilt(out float targetRoll, 
	                  out float targetPitch,
	                  DroneState currentState) {

		Vector3 rotatedVelocityWorld = Quaternion.AngleAxis(-currentState.rotation.y, Vector3.up) * currentState.velocityWorld;
		
		targetRoll = MathHelper.Aserp (rotatedVelocityWorld.x, breakTiltMultiplier, maxBreakTiltAngle);
		targetPitch = MathHelper.Aserp (-rotatedVelocityWorld.z, breakTiltMultiplier, maxBreakTiltAngle);
		
	}

//	/// <summary>
//	/// Gets the input.
//	/// </summary>
//	/// <param name="inputRoll">roll input</param>
//	/// <param name="inputPitch">pitch input</param>
//	/// <param name="inputYaw">yaw input</param>
//	/// <param name="inputThrust">thrust input</param>
//	private void GetInput(out float inputRoll,
//	                      out float inputPitch,
//	                      out float inputYaw,
//	                      out float inputThrust,
//	                      out bool breaking) {
//		
//		inputRoll = Input.GetAxis ("Roll");
//		inputPitch = Input.GetAxis ("Pitch");
//		inputYaw = Input.GetAxis ("Yaw");
//		inputThrust = Input.GetAxis ("Thrust");
//		breaking = Input.GetButton("Break");
//	}

	private HandlingInput GetInput() {

		HandlingInput input = new HandlingInput ();
		
		input.roll = Input.GetAxis ("Roll");
		input.pitch = Input.GetAxis ("Pitch");
		input.yaw = Input.GetAxis ("Yaw");
		input.thrust = Input.GetAxis ("Thrust");
		input.breaking = Input.GetButton("Break");

		return input;
	}

	private DroneState GetCurrentState() {

		Rigidbody rb = GetComponent<Rigidbody> ();

		DroneState state = new DroneState ();

		state.rotation = transform.rotation.eulerAngles;
		state.velocityWorld = rb.velocity;
		state.velocityLocal = transform.InverseTransformDirection (state.velocityWorld);
		state.angularVelocityWorld = rb.angularVelocity;
		state.angularVelocityLocal = transform.InverseTransformDirection (state.angularVelocityWorld);

		return state;
	}
    
    /// <summary>
    /// Respawns the player by resetting its position, rotation and velocity.
    /// </summary>
    public void Respawn() {
        gameObject.transform.position = spawnPosition;
        gameObject.transform.rotation = spawnRotation;
        gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
