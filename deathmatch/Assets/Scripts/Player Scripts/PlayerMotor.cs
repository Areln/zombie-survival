using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {

    public bool debugmode = false;

	[SerializeField]
	private Camera cam;
	
	Vector3 velocity = Vector3.zero;
	private Vector3 rotation = Vector3.zero;
	private float cameraRotationX = 0f;
	private float currentCameraRotationX = 0f;
	
	[SerializeField]
	private float cameraRotationLimit = 85f;
	
	private Rigidbody rb;

    //speed
    Vector3 lastPosition = Vector3.zero;
    public float speed;
	void Start ()
	{

        rb = GetComponent<Rigidbody>();
	}
    void Update()
    {
        if (debugmode)
        {
            speed = 0.16f;
        }
    }
	
	// Gets a movement vector
	public void Move (Vector3 _velocity)
	{
        velocity = _velocity;
        
	}
	
	// Gets a rotational vector
	public void Rotate(Vector3 _rotation)
	{
		rotation = _rotation;
	}
	
	// Gets a rotational vector for the camera
	public void RotateCamera(float _cameraRotationX)
	{
		cameraRotationX = _cameraRotationX;
	}
	
	// Run every physics iteration
	void FixedUpdate ()
	{
        PerformMovement();
		PerformRotation();
        //gets speed
        if (!debugmode) {
            speed = (transform.position - lastPosition).magnitude;
        }
        lastPosition = transform.position;
    }
	
	//Perform movement based on velocity variable
	void PerformMovement ()
	{
		if (velocity != Vector3.zero)
		{
            
			rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }		
	}
	
	//Perform rotation
	void PerformRotation ()
	{
		rb.MoveRotation(rb.rotation * Quaternion.Euler (rotation));
		if (cam != null)
		{
			// Set our rotation and clamp it
			currentCameraRotationX -= cameraRotationX;
			currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);
			
			//Apply our rotation to the transform of our camera
			cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
		}
	}
	
}