using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {
	[SerializeField]
	public float speed = 10;
    [SerializeField]
    public float bonusSpeed = 0;
    [SerializeField]
	public float lookSensitivity = 1f;
    [SerializeField]
    public float gravity = -60f;

    Vector3 _velocity;

    CursorLockMode lockmode;

    // Component caching
    private PlayerMotor motor;
	private ConfigurableJoint joint;
	private Animator animator;

	public bool canmove;
	public bool cameraMove;
    public bool paused = false;
	void Start ()
	{

        Cursor.lockState = CursorLockMode.None;

        motor = GetComponent<PlayerMotor>();
		joint = GetComponent<ConfigurableJoint>();
		animator = GetComponent<Animator>();
		cameraMove = true;
        //if there is an option object then get the sensitivity from it
        if (GameObject.FindGameObjectWithTag("OptionManager"))
        {
            Debug.Log("found options");
            lookSensitivity = GameObject.FindGameObjectWithTag("OptionManager").GetComponent<OptionManager>().sensitivity;
        }
	}
	
	void Update ()
	{
        
        //SPEEEED
        //speed = GetComponent<PlayerFunctions>().speed + GetComponent<PlayerFunctions>().bonusSpeed;
        speed = speed + bonusSpeed;
        //Calculate movement velocity as a 3D vector
        float _xMov = Input.GetAxisRaw ("Horizontal");
		float _zMov = Input.GetAxisRaw ("Vertical");
		
		Vector3 _movHorizontal = transform.right * _xMov;
		Vector3 _movVertical = transform.forward * _zMov;
		
		// Final movement vector **********
		_velocity = (_movHorizontal + _movVertical).normalized * speed;

        //gravity
        Physics.gravity = new Vector3(0, gravity, 0);

        if (canmove) {
            //Apply movement
			motor.Move (_velocity);
		} 
		//Calculate rotation as a 3D vector (turning around)
		float _yRot = Input.GetAxisRaw ("Mouse X");
		
		Vector3 _rotation = new Vector3 (0f, _yRot, 0f) * (lookSensitivity/2);
		
		//Calculate camera rotation as a 3D vector (turning around)
		float _xRot = Input.GetAxisRaw ("Mouse Y");
		
		float _cameraRotationX = _xRot * (lookSensitivity/2);

		if (cameraMove) {
			//Apply camera rotation
			motor.RotateCamera (_cameraRotationX);
			//Apply rotation
			motor.Rotate (_rotation);
		}
	}
    public void pauseToggle() {
        paused = !paused;
        if (paused)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else {
            Cursor.lockState = CursorLockMode.Locked;
        }
        Cursor.visible = paused;
        cameraMove = !paused;
        canmove = !paused;
    }
	
}
