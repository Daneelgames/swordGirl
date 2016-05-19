using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CameraController : MonoBehaviour
{
    Camera myCamera;
    Transform camTransform;
    Transform pivot;
    
    Transform character;

    public float maxTargetRadius = 50f;
    [SerializeField]
    private Transform target;
    [SerializeField]
    private bool lockOn = false;
    [SerializeField]
    List<AngelKingBodyColliderController> listOfTargets = new List<AngelKingBodyColliderController>();

    public float camLookAtSpeed = 20;
    public int camRotateSpeed = 10;
    public int vertSpeed = 3;
    public bool reverseVertical;

    public float sprintFOV = 30f;
    public float rollFOV = 70f;

    float offset = -6;
    float camFollow = .75f;
    
    [SerializeField]
    LayerMask mask;

    private float defaultFOV;
    private float targetFOV;

    private PlayerControl playerControl;
    
    [SerializeField]
    GameObject[] enemyColliders;

    private CrosshairController crosshair;

    private bool canSwitchTarget = true;

    void Awake()
    {
        pivot = transform.Find("CamTarget").transform;
        target = pivot;
        GetEnemyColliders();
        character = transform.parent.transform;
        crosshair = GameObject.Find("Crosshair").GetComponent<CrosshairController>();
    }

    void OnEnable()
    {
        //pivot = transform;
        myCamera = Camera.main;
        camTransform = myCamera.transform;
        playerControl = character.gameObject.GetComponent<PlayerControl>();
        camTransform.position = pivot.TransformPoint(Vector3.forward * offset);
        defaultFOV = myCamera.fieldOfView;

        transform.parent = null;
    }

    void Update()
    {
        ControlFov();

        transform.position = new Vector3(character.position.x, character.position.y + 1.5f, character.position.z);

        if (Input.GetButtonDown("LockOn"))
            LockOnController();

        if (lockOn)
            SwitchTarget();
    }

    void ControlFov()
    {
        //control camers FOV
        if (playerControl.isSprinting())
            targetFOV = sprintFOV;

        if (playerControl.timeToNextRoll > 0)
            targetFOV = rollFOV;
        else if (playerControl.isSprinting())
            targetFOV = sprintFOV;
        else
            targetFOV = defaultFOV;

        myCamera.fieldOfView = Mathf.Lerp(myCamera.fieldOfView, targetFOV, 2 * Time.deltaTime);
    }

    void GetEnemyColliders()
    {
        //get list of targets
        enemyColliders = GameObject.FindGameObjectsWithTag("EnemyActionColl");
        foreach (GameObject coll in enemyColliders)
        {
            AngelKingBodyColliderController colliderScript = coll.GetComponent<AngelKingBodyColliderController>();
            if (colliderScript.localHealth > 0 && colliderScript.isTarget)
            {
                listOfTargets.Add(colliderScript);
            }

        }
    }

    public void BrokeTarget(AngelKingBodyColliderController collider)
    {
        if (collider.transform == target)
            LockOnController();
    }

    void LockOnController()
    {
        if (!lockOn)
        {
            //get closest target
            Transform closestTarget = null;
            float closestDistance = maxTargetRadius;
            
            foreach (AngelKingBodyColliderController j in listOfTargets)
            {
                if (j.localHealth <= 0)
                {
                    listOfTargets.Remove(j);
                    break;
                }

                float distance = Vector3.Distance(j.transform.position, pivot.position);
                if (distance <= closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = j.transform;
                }
            }

            if (closestTarget != null)
            {
                print("target - " + closestTarget.name);
                target = closestTarget;
                lockOn = true;
                crosshair.ShowCrosshair(target);
            }
        }
        else
        {
            //clear list of targets
            lockOn = false;
            target = pivot;
            crosshair.HideCrosshair();
        }

    }

    void SwitchTarget()
    {
        float hor = Input.GetAxis("Mouse X");
        float ver = Input.GetAxis("Mouse Y");
        
        if (canSwitchTarget)
        {

            if (hor != 0 || ver != 0)
            {
                FindNewTarget(hor, ver);
                canSwitchTarget = false;
            }
        }
        else
        {
            if (hor == 0 && ver == 0)
            {
                canSwitchTarget = true;
            }
        }
    }

    void FindNewTarget(float hor, float ver)
    {
        Vector3 curTargetScreenPos = Camera.main.WorldToScreenPoint(target.position);
        float minimumDistanceToCurTarget = maxTargetRadius;
        Transform newTarget = null;

        print(hor + ", " + ver);

        foreach (AngelKingBodyColliderController i in listOfTargets)
        {
            float distanceToPivot = Vector3.Distance(pivot.position, i.transform.position);
            if (distanceToPivot < maxTargetRadius )
            {
                Vector3 iScreenPos = Camera.main.WorldToScreenPoint(i.transform.position);
                float distanceToCurTarget = Vector2.Distance((Vector2)curTargetScreenPos, (Vector2)iScreenPos);
                if (distanceToCurTarget < minimumDistanceToCurTarget)
                {
                    //minimumDistanceToCurTarget = distanceToCurTarget;

                    if (hor > 0 && iScreenPos.x > curTargetScreenPos.x && Mathf.Abs(hor) > Mathf.Abs(ver))
                    {
                        newTarget = i.transform;
                        minimumDistanceToCurTarget = distanceToCurTarget;
                    }
                    else if (hor < 0 && iScreenPos.x < curTargetScreenPos.x && Mathf.Abs(hor) > Mathf.Abs(ver))
                    {
                        newTarget = i.transform;
                        minimumDistanceToCurTarget = distanceToCurTarget;
                    }
                    else if (ver > 0 && iScreenPos.y > curTargetScreenPos.y && Mathf.Abs(hor) < Mathf.Abs(ver))
                    {
                        newTarget = i.transform;
                        minimumDistanceToCurTarget = distanceToCurTarget;
                    }
                    else if (ver < 0 && iScreenPos.y < curTargetScreenPos.y && Mathf.Abs(hor) < Mathf.Abs(ver))
                    {
                        newTarget = i.transform;
                        minimumDistanceToCurTarget = distanceToCurTarget;
                    }
                }

            }
        }
        if (newTarget != null)
            target = newTarget;
    }

    void LateUpdate()
    {
        //Camera Orbits the charcter
        float hor = Input.GetAxis("Mouse X");
        float vert = Input.GetAxis("Mouse Y");

        if (!reverseVertical)
            vert *= -vertSpeed;
        else
            vert *= vertSpeed;

        hor *= camRotateSpeed;

        //CLAMP Vertical Axis
        float x = pivot.eulerAngles.x;
        if (vert > 0 && x > 61 && x < 270) vert = 0;
        else if (vert < 0 && x < 300 && x > 180) vert = 0;

        // сохранять направление камеры при отключении локОна
        if (!lockOn)
            pivot.localEulerAngles += new Vector3(vert, hor, 0);
        else
            pivot.LookAt(target);

        //Central Ray
        float unobstructed = offset;
        Vector3 idealPostion = pivot.TransformPoint(Vector3.forward * offset);

        RaycastHit hit;
        if (Physics.Linecast(pivot.position, idealPostion, out hit, mask.value))
        {
            unobstructed = -hit.distance + .01f;
        }

        //smooth
        Vector3 desiredPos;
        if (!lockOn)
            desiredPos = pivot.TransformPoint(Vector3.forward * unobstructed);
        else
            desiredPos = pivot.position + (target.position - pivot.position).normalized * unobstructed;

        Vector3 currentPos = camTransform.position;

        Vector3 goToPos = new Vector3(Mathf.Lerp(currentPos.x, desiredPos.x, camFollow), Mathf.Lerp(currentPos.y, desiredPos.y, camFollow), Mathf.Lerp(currentPos.z, desiredPos.z, camFollow));

        //camTransform.LookAt(pivot.position);
        //camTransform.localPosition = goToPos;
        //camTransform.LookAt(target.position);
        
        Quaternion rotation = Quaternion.LookRotation(target.position - camTransform.position);
        camTransform.rotation = Quaternion.Slerp(camTransform.rotation, rotation, Time.deltaTime * camLookAtSpeed);

        camTransform.localPosition = Vector3.Lerp(camTransform.localPosition, goToPos, Time.deltaTime * 5f);


        //Viewport Bleed prevention
        float c = myCamera.nearClipPlane;
        bool clip = true;
        while (clip)
        {
            Vector3 pos1 = myCamera.ViewportToWorldPoint(new Vector3(0, 0, c));
            Vector3 pos2 = myCamera.ViewportToWorldPoint(new Vector3(.5f, 0, c));
            Vector3 pos3 = myCamera.ViewportToWorldPoint(new Vector3(1, 0, c));
            Vector3 pos4 = myCamera.ViewportToWorldPoint(new Vector3(0, .5f, c));
            Vector3 pos5 = myCamera.ViewportToWorldPoint(new Vector3(1, .5f, c));
            Vector3 pos6 = myCamera.ViewportToWorldPoint(new Vector3(0, 1, c));
            Vector3 pos7 = myCamera.ViewportToWorldPoint(new Vector3(.5f, 1, c));
            Vector3 pos8 = myCamera.ViewportToWorldPoint(new Vector3(1, 1, c));

            Debug.DrawLine(camTransform.position, pos1, Color.yellow);
            Debug.DrawLine(camTransform.position, pos2, Color.yellow);
            Debug.DrawLine(camTransform.position, pos3, Color.yellow);
            Debug.DrawLine(camTransform.position, pos4, Color.yellow);
            Debug.DrawLine(camTransform.position, pos5, Color.yellow);
            Debug.DrawLine(camTransform.position, pos6, Color.yellow);
            Debug.DrawLine(camTransform.position, pos7, Color.yellow);
            Debug.DrawLine(camTransform.position, pos8, Color.yellow);

            if (Physics.Linecast(camTransform.position, pos1, out hit, mask))
            {
                // clip
            }
            else if (Physics.Linecast(camTransform.position, pos2, out hit, mask))
            {
                // clip
            }
            else if (Physics.Linecast(camTransform.position, pos3, out hit, mask))
            {
                // clip
            }
            else if (Physics.Linecast(camTransform.position, pos4, out hit, mask))
            {
                // clip
            }
            else if (Physics.Linecast(camTransform.position, pos5, out hit, mask))
            {
                // clip
            }
            else if (Physics.Linecast(camTransform.position, pos6, out hit, mask))
            {
                // clip
            }
            else if (Physics.Linecast(camTransform.position, pos7, out hit, mask))
            {
                // clip
            }
            else if (Physics.Linecast(camTransform.position, pos8, out hit, mask))
            {
                // clip
            }
            else clip = false;

            if (clip) camTransform.localPosition += camTransform.forward * c;
        }
    }
}