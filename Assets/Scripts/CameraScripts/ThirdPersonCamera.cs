using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour {
    [SerializeField]
    float distanceAway;
    [SerializeField]
    float distanceUp;
    [SerializeField]
    float smooth;

    [SerializeField]
    Transform followTransform;
    
    //height from the top of the character
    [SerializeField]
    private Vector3 offset = new Vector3(0f, 1.5f, 0f);

    private Vector3 velocityCamSmooth = Vector3.zero;
    [SerializeField]
    private float camSmoothDampTime = 0.1f;
    [SerializeField]
    private float wideScreen = 0.2f;
    [SerializeField]
    private float targetingTime = 0.5f;

    public enum CamStates
    {
        Behind,
        FirstPerson,
        Target,
        Free
    }

    private Vector3 lookDirection;
    private Vector3 targetPosition;

    private CamStates camState = CamStates.Behind;
    //private BarsEffect barEffect;

    void Start () {
        followTransform = GameObject.FindWithTag("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void LateUpdate()
    {
        Vector3 characterOffset = followTransform.position + offset;

        if (Input.GetAxis("Target") > 0.01f)
        {
            camState = CamStates.Target;
        }
        else
        {
            camState = CamStates.Behind;
        }

        switch (camState)
        {
            case CamStates.Behind:
                lookDirection = characterOffset - this.transform.position;
                lookDirection.y = 0;
                lookDirection.Normalize();

                Debug.DrawRay(this.transform.position, lookDirection, Color.green);

                //targetPosition = characterOffset + followTransform.up * distanceUp - lookDirection * distanceAway;
                //Debug.DrawRay(followTransform.position, Vector3.up * distanceUp, Color.red);
                //Debug.DrawRay(followTransform.position, -1f * followTransform.forward * distanceAway, Color.blue);
                Debug.DrawLine(followTransform.position, targetPosition, Color.magenta);
                break;

            case CamStates.Target:
                lookDirection = followTransform.forward;
                
                break;

        }

        targetPosition = characterOffset + followTransform.up * distanceUp - lookDirection * distanceAway;
        CompensateForWalls(characterOffset, ref targetPosition);
        //Making a smooth transition from last position to next position
        SmoothPosition(this.transform.position, targetPosition);

        transform.LookAt(characterOffset);
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
            argToTarget = new Vector3(wallHit.point.x - .1f, argToTarget.y + .3f, wallHit.point.z - .1f);
        }
    }
}
