using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

// Declaration of the main class for the agent, that inherited from the Agent class of Unity-ML Agents
public class CapsuleAgent : Agent {

	// Robot physics parameters (angular and linear velocity constant)
	public float angularStep;
	public float linearStep;

	// Default paramters for reset
	private Vector3 startPosition;
	private Quaternion startRotation;

	// Collision Flags
	private bool wallCollision = false;
	private bool goalCollision = false;

	// Start Positions
	public int patientNumber = 0;
	private Vector3 startingPos0 = new Vector3( 0, -18.8f, -10.4f);
	private Vector3 startingPos1 = new Vector3( 57.4f, -20.1f, -5.7f);
	private Vector3 startingPos2 = new Vector3( 107.33f, -21.81f, -9.88f);
	private Vector3 startingPos3 = new Vector3( 152.69f, -16.91f, -3.92f);

	//
	public GameObject[] colonsList;

	// Distance Computation
	public Transform[] viewPoints;
	public float distance;
	public int nextPoint;

	// Called at the creation of the enviornment (before the first episode)
	// and only once
    public override void Initialize() {
		startPosition = gameObject.transform.position;
		startRotation = gameObject.transform.rotation;
	}


	// Called at the every new episode of the training loop,
	// after each reset (both from target, crash or timeout)
	public override void OnEpisodeBegin() {
		nextPoint = 0;
		// Reset to the default rotation at the beginning of each episode
		gameObject.transform.rotation = startRotation;
		//gameObject.transform.position = startPosition;
		// Randomize the patient
		// patientNumber = Random.Range(0, 4);
		// Reset to the default position at the beginning of each episode
		if ( patientNumber == 0 ) gameObject.transform.position = startingPos0;
		if ( patientNumber == 1 ) gameObject.transform.position = startingPos1;
		if ( patientNumber == 2 ) gameObject.transform.position = startingPos2;
		if ( patientNumber == 3 ) gameObject.transform.position = startingPos3;
		//
		viewPoints = colonsList[patientNumber].GetComponent<ViewPointsContainer>().viewPoints;
	}


	// Listener for the action received, both from the neural network and the keyboard
	// (if heuristic mode), inside the Python script, the action is passed with the step funciton
	public override void OnActionReceived(ActionBuffers actionBuffers)	{
		// Read the action buffer, in this set-up, discrete
		var actionBuffer = actionBuffers.DiscreteActions;
		
		// Basic setting for the action 0 (CoC)
		// float angularVelocityX = 0f;
		// float angularVelocityY = 0f;
		// float linearVelocity = linearStep;
		float dX = 0.0f; float dY = 0.0f; float dL = 0.0f;
		if ( actionBuffer[0] == 0 ) { dX=  0.0f; dY=  0.0f; dL= 1.0f; }
		if ( actionBuffer[0] == 1 ) { dX=  0.0f; dY=  1.0f; dL= 1.0f; }
		if ( actionBuffer[0] == 2 ) { dX=  0.0f; dY= -1.0f; dL= 1.0f; }
		if ( actionBuffer[0] == 3 ) { dX=  1.0f; dY=  0.0f; dL= 1.0f; }
		if ( actionBuffer[0] == 4 ) { dX= -1.0f; dY=  0.0f; dL= 1.0f; }

		// Apply the movement (rotation and translation) 
		// according with angular and linear velocity
		transform.Rotate(Vector3.left * dY * angularStep);
		transform.Rotate(Vector3.forward * dX * angularStep);
		transform.Translate( -Vector3.up * dL * linearStep );
		
		// Standard reward for a single step, return the distance from the goal
		// the code to recognize the distance is (1000 + distance )
		//SetReward( 1000f + distance );
		SetReward ( distance );
		if( wallCollision ) SetReward ( -distance );
		if( goalCollision ) SetReward ( 1000f );

		wallCollision = false;
		goalCollision = false;
	}


	// Listener for the observations collections.
	// The observations for the LiDAR sensor are inherited from the 
	// editor, in thi function we add the other observations (angle, distance)
	public override void CollectObservations(VectorSensor sensor) {	}


	// Debug function, useful to control the agent with the keyboard in heurisitc mode
	// (must be setted in the editor)
	public override void Heuristic(in ActionBuffers actionsOut) { 
		// Set the basic action and wait or a keyboard key
		int action = 0;
		if (Input.GetKey(KeyCode.W)) action = 1;
		if (Input.GetKey(KeyCode.S)) action = 2;
		if (Input.GetKey(KeyCode.D)) action = 3;
		if (Input.GetKey(KeyCode.A)) action = 4;
		// Add the action to the actionsOut object
		var actions = actionsOut.DiscreteActions;
		actions[0] = action;
	}


	// Listener for the collison with a solid object
	private void OnCollisionStay(Collision collision) { 
		if (collision.collider.CompareTag("Wall")) { wallCollision = true; } //SetReward(-1000f + distance ); }
		if (collision.collider.CompareTag("Goal")) { goalCollision = true; } //SetReward( 1f); }
		
	}

	// Update is called once per frame
    void Update() { 

		// Compute the distance
		if (nextPoint < viewPoints.Length-1) {
			// Update the next view point to visit
			if ( Vector3.Distance( transform.position, viewPoints[nextPoint].position ) < 5 ) nextPoint++;
			// Compute the distance from the goal
			distance = Vector3.Distance( transform.position, viewPoints[nextPoint].position );
			for (int i=nextPoint+1; i<viewPoints.Length; i++) {
				distance += Vector3.Distance( viewPoints[i-1].position, viewPoints[i].position );
			}
		} else {
			// Fort the last step the distance is just the goal
			distance = Vector3.Distance( transform.position, viewPoints[viewPoints.Length-1].position );
		}

	}

}
