using UnityEngine;
using System.Collections;

public class ThirdPersonOrbitCam : MonoBehaviour 
{
	public Transform player;
	
	public Vector3 pivotOffset = new Vector3(0.0f, 1.0f,  0.0f);
	public Vector3 camOffset   = new Vector3(0.0f, 0.7f, -3.0f);

	public float smooth = 10f;

	//public Vector3 aimPivotOffset = new Vector3(0.0f, 1.7f,  -0.3f);
	//public Vector3 aimCamOffset   = new Vector3(0.8f, 0.0f, -1.0f);

	public float horizontalAimingSpeed = 400f;
	public float verticalAimingSpeed = 400f;
	public float maxVerticalAngle = 30f;
	public float minVerticalAngle = -60f;
	
	public float mouseSensitivity = 0.3f;

	public float sprintFOV = 100f;
	
	private PlayerControl playerControl;
	private float angleH = 0;
	private float angleV = 0;
	private Transform cam;

	private Vector3 relCameraPos;
	private float relCameraPosMag;
	
	private Vector3 smoothPivotOffset;
	private Vector3 smoothCamOffset;
	private Vector3 targetPivotOffset;
	private Vector3 targetCamOffset;

	private float defaultFOV;
	private float targetFOV;
    

    void Awake()
	{
		cam = transform;
		playerControl = player.GetComponent<PlayerControl> ();

        //relCameraPosMag = relCameraPos.magnitude - 0.5f;

        smoothPivotOffset = pivotOffset;
		smoothCamOffset = camOffset;

		defaultFOV = cam.GetComponent<Camera>().fieldOfView;
        
    }

    void Update()
    {
        relCameraPos = transform.position - player.position;
        relCameraPosMag = relCameraPos.magnitude;
    }

	void LateUpdate()
	{
		angleH += Mathf.Clamp(Input.GetAxis("Mouse X"), -1, 1) * horizontalAimingSpeed * Time.deltaTime;
		angleV += Mathf.Clamp(Input.GetAxis("Mouse Y"), -1, 1) * verticalAimingSpeed * Time.deltaTime;

		angleV = Mathf.Clamp(angleV, minVerticalAngle, maxVerticalAngle);


		Quaternion aimRotation = Quaternion.Euler(-angleV, angleH, 0);
		Quaternion camYRotation = Quaternion.Euler(0, angleH, 0);
		cam.rotation = aimRotation;
        
		targetPivotOffset = pivotOffset;
		targetCamOffset = camOffset;

		if(playerControl.isSprinting())
			targetFOV = sprintFOV;

        if (playerControl.timeToNextRoll > 0)
            targetFOV = sprintFOV;
        else
			targetFOV = defaultFOV;

		cam.GetComponent<Camera>().fieldOfView = Mathf.Lerp (cam.GetComponent<Camera>().fieldOfView, targetFOV,  Time.deltaTime);



		// Test for collision
		Vector3 baseTempPosition = player.position + camYRotation * targetPivotOffset;
		Vector3 tempOffset = targetCamOffset;

		for(float zOffset = targetCamOffset.z; zOffset <= 0; zOffset += 0.5f)
		{
			tempOffset.z = zOffset;
			if (DoubleViewingPosCheck (baseTempPosition + aimRotation * tempOffset) || zOffset == 0)
            {
                targetCamOffset.z = tempOffset.z;
				break;
			} 
		}


		smoothPivotOffset = Vector3.Lerp(smoothPivotOffset, targetPivotOffset, smooth * Time.deltaTime);
		smoothCamOffset = Vector3.Lerp(smoothCamOffset, targetCamOffset, smooth * Time.deltaTime);

		cam.position =  player.position + camYRotation * smoothPivotOffset + aimRotation * smoothCamOffset;

	}

	// concave objects doesn't detect hit from outside, so cast in both directions
	bool DoubleViewingPosCheck(Vector3 checkPos)
	{
		float playerFocusHeight = player.GetComponent<CapsuleCollider> ().height *0.5f;
		return ViewingPosCheck (checkPos, playerFocusHeight) && ReverseViewingPosCheck (checkPos, playerFocusHeight);
	}

	bool ViewingPosCheck (Vector3 checkPos, float deltaPlayerHeight)
	{
        RaycastHit hit;
        
        if (Physics.Raycast(checkPos, player.position + (Vector3.up * deltaPlayerHeight) - checkPos, out hit, relCameraPosMag, 1<<8))
        {
			return false;
        }
        return true;
	}

	bool ReverseViewingPosCheck(Vector3 checkPos, float deltaPlayerHeight)
	{
		RaycastHit hit;

        Debug.DrawRay(player.position + (Vector3.up * deltaPlayerHeight), checkPos - player.position, Color.cyan);

        if (Physics.Raycast(player.position+(Vector3.up* deltaPlayerHeight), checkPos - player.position, out hit, relCameraPosMag, 1 << 8))
        {
            return false;
		}
		return true;
	}
}