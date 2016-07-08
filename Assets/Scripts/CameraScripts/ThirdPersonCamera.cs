using UnityEngine;
using System.Collections;
////////////////////////////////////////////////////////////////
//   Class:   ThirdPersonCamera
//   Purpose:    
//   
//   Notes:
//   Contributors: RSF
////////////////////////////////////////////////////////////////

public class ThirdPersonCamera : MonoBehaviour {

    [SerializeField]
    private float distanceAway;
    [SerializeField]
    private float distanceUp;
    [SerializeField]
    private float smooth;
    [SerializeField]
    private Transform followTransform;

    [SerializeField]
    private float targetingTime = 0.5f;

    private Vector3 lookDirection;
    private Vector3 targetPosition;
    private CameraStates camState = CameraStates.Behind;

    private Vector3 velocityCamSmooth = Vector3.zero;
    [SerializeField]
    private float camSmoothDampTime = 0.1f;

    public enum CameraStates
    {
        Behind,
        FirstPerson,
        Target,
        Free
    }

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void LateUpdate()
    {
        Vector3 characterOffset = followTransform.position + new Vector3(0f, distanceUp, 0f);

        //Find what state the camera is in
        if(Input.GetAxis("Target") > 0.01f)
        {
            camState = CameraStates.Target;
        }
        else
        {
            camState = CameraStates.Behind;
        }

        switch (camState)
        {
            case CameraStates.Behind:
                lookDirection = characterOffset - this.transform.position;
                lookDirection.y = 0;
                lookDirection.Normalize();

                Debug.DrawRay(this.transform.position, lookDirection, Color.green);
                
                //targetPosition = characterOffset + followTransform.up * distanceUp - lookDirection * distanceAway;

                //Debug.DrawRay(follow.position, Vector3.up * distanceUp, Color.red);
                //Debug.DrawRay(follow.position, -1f * follow.forward * distanceAway, Color.blue);
                //Debug.DrawLine(follow.position, targetPosition, Color.magenta);
                break;

            case CameraStates.Target:
                lookDirection = followTransform.forward;
                break;
        }



        //Always want to run these in all states
        targetPosition = characterOffset + followTransform.up * distanceUp - lookDirection * distanceAway;

        CompensateForWalls(characterOffset, ref targetPosition);
        SmoothPosition(this.transform.position, targetPosition);

        transform.LookAt(followTransform);
    }

    private void SmoothPosition(Vector3 argFromPosition, Vector3 argToPosition)
    {
        this.transform.position = Vector3.SmoothDamp(argFromPosition, argToPosition, ref velocityCamSmooth, camSmoothDampTime);
    }

    private void CompensateForWalls(Vector3 argFromObject, ref Vector3 argToTarget)
    {
        RaycastHit wallHit = new RaycastHit();
        if(Physics.Linecast(argFromObject, argToTarget, out wallHit))
        {
            argToTarget = new Vector3(wallHit.point.x, argToTarget.y, wallHit.point.z);
        }
    }

}
