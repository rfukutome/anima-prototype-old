using UnityEngine;
using System.Collections;

////////////////////////////////////////////////////////////////
//   Class:   CharacterController3D
//   Purpose: Contains all of the logic for the Character Controller
//   for the game. Also handles animation states.   
//   
//   Notes: Attach onto character.
//   Contributors: RSF
////////////////////////////////////////////////////////////////

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class CharacterController3D : MonoBehaviour {

    [SerializeField]
    private Animator animator;
    [SerializeField]
    private float directionDampTime = .25f;
    [SerializeField]
    private ThirdPersonCamera gameCamera;
    [SerializeField]
    private float directionSpeed = 3.0f;
    [SerializeField]
    private float rotationDegreePerSecond = 120f;

    private float speed = 0.0f;
    private float direction = 0.0f;
    private float horizontalInput = 0.0f;
    private float verticalInput = 0.0f;
    private AnimatorStateInfo animStateInfo;

    private int m_LocomotionId = 0;
	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();

        if(animator.layerCount >= 2)
        {
            animator.SetLayerWeight(1, 1);
        }

        m_LocomotionId = Animator.StringToHash("Base Layer.Locomotion");
	}
	
	// Update is called once per frame
	void Update () {
        animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        StickToWorldSpace(this.transform, gameCamera.transform, ref direction, ref speed);

        animator.SetFloat("Speed", speed);
        animator.SetFloat("Direction", direction, directionDampTime, Time.deltaTime);
    }

    void FixedUpdate()
    {
        if(IsMoving() && ((direction >= 0 && horizontalInput >= 0) || (direction < 0 && horizontalInput < 0)))
        {
            Vector3 rotationAmount = Vector3.Lerp(Vector3.zero, new Vector3(0f, rotationDegreePerSecond * (horizontalInput < 0f ? -1f : 1f), 0f), Mathf.Abs(horizontalInput));
            Quaternion deltaRotation = Quaternion.Euler(rotationAmount * Time.deltaTime);
            this.transform.rotation = (this.transform.rotation * deltaRotation);
        }
    }
    public void StickToWorldSpace(Transform argRoot, Transform argCamera, ref float argDirectionOut, ref float argSpeedOut)
    {
        Vector3 rootDirection = argRoot.forward;
        Vector3 stickDirection = new Vector3(horizontalInput, 0, verticalInput);

        argSpeedOut = stickDirection.sqrMagnitude;

        Vector3 cameraDirection = argCamera.forward;
        cameraDirection.y = 0.0f;
        Quaternion referentialShift = Quaternion.FromToRotation(Vector3.forward, cameraDirection);

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
        return (animStateInfo.fullPathHash == m_LocomotionId);
    }
}
