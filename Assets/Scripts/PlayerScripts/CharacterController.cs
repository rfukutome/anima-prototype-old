using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {

    public float inputDelay = 0.1f;
    public float forwardVel = 12;
    public float rotateVel = 100;
    [SerializeField]
    private ThirdPersonCamera gameCam;
    [SerializeField]
    private float directionSpeed = 3.0f;
    [SerializeField]
    private float rotationDegreePerSecond = 120f;
    private float speed = 0;
    [SerializeField]
    private float direction = 0;

    [SerializeField]
    private float verticalInput, horizontalInput = 0.0f;

    Quaternion targetRotation;
    Rigidbody rBody;
    

    public Quaternion TargetRotation()
    {
        return targetRotation;
    }
	// Use this for initialization
	void Start () {
        targetRotation = transform.rotation;
        if (GetComponent<Rigidbody>())
        {
            rBody = GetComponent<Rigidbody>();
        }
        else
        {
            Debug.LogError("Character has no RigidBody");
        }
	}
	
    void GetInput()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
        //speed = new Vector2(horizontalInput, verticalInput).sqrMagnitude;
    }
	// Update is called once per frame
	void Update () {
        GetInput();
        Turn();
        StickToWorldspace(this.transform, gameCam.transform, ref direction, ref speed);
	}

    void FixedUpdate()
    {
        Run();
        if (IsMoving() && ((direction >= 0 && horizontalInput >= 0) || (direction < 0 && horizontalInput < 0)))
        {
            Vector3 rotationAmount = Vector3.Lerp(Vector3.zero, new Vector3(0f, rotationDegreePerSecond * (horizontalInput < 0f ? -1f : 1f), 0f), Mathf.Abs(horizontalInput));
            Quaternion deltaRotation = Quaternion.Euler(rotationAmount * Time.deltaTime);
            this.transform.rotation = (this.transform.rotation * deltaRotation);
        }
    }

    void Run()
    {
        if(Mathf.Abs(verticalInput) > inputDelay)
        {
            //move here
            rBody.velocity = transform.forward * verticalInput * forwardVel;
        }
        else
        {
            rBody.velocity = Vector3.zero;
        }
    }

    void Turn()
    {
        if (Mathf.Abs(horizontalInput) > inputDelay)
        {
            targetRotation *= Quaternion.AngleAxis(rotateVel * horizontalInput * Time.deltaTime, Vector3.up);
        }
        
        transform.rotation = targetRotation;
    }

    public void StickToWorldspace(Transform argRoot, Transform argCamera, ref float argDirectionOut, ref float argSpeedOut)
    {
        Vector3 rootDirection = argRoot.forward;
        Vector3 stickDirection = new Vector3(horizontalInput, 0, verticalInput);
        argSpeedOut = stickDirection.sqrMagnitude;

        Vector3 cameraDirection = argCamera.forward;
        cameraDirection.y = 0.0f;
        Quaternion referentialShift = Quaternion.FromToRotation(Vector3.forward, cameraDirection);

        //Convert joystick input into worldspace coords.
        Vector3 moveDirection = referentialShift * stickDirection;
        Vector3 axisSign = Vector3.Cross(moveDirection, rootDirection);

        Debug.DrawRay(new Vector3(argRoot.position.x, argRoot.position.y + 2f, argRoot.position.z), moveDirection, Color.green);
        Debug.DrawRay(new Vector3(argRoot.position.x, argRoot.position.y + 2f, argRoot.position.z), axisSign, Color.red);
        Debug.DrawRay(new Vector3(argRoot.position.x, argRoot.position.y + 2f, argRoot.position.z), rootDirection, Color.magenta);
        //Debug.DrawRay(new Vector3(argRoot.position.x, argRoot.position.y + 2f, argRoot.position.z), stickDirection, Color.blue);
        float angleRootToMove = Vector3.Angle(rootDirection, moveDirection) * (axisSign.y >= 0 ? -1f : 1f);
        angleRootToMove /= 180f;
        argDirectionOut = angleRootToMove * directionSpeed;
        
    }

    public bool IsMoving()
    {
        return (verticalInput != 0 && horizontalInput != 0);
    }
}
