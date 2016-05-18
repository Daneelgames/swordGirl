using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    Camera myCamera;
    Transform camTransform;
    Transform pivot;
    
    Transform character;

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
    

    // Use this for initialization
    void OnEnable()
    {
        character = transform.parent.transform;
        //pivot = transform;
        pivot = transform.Find("CamTarget").transform;
        myCamera = Camera.main;
        camTransform = myCamera.transform;
        playerControl = character.gameObject.GetComponent<PlayerControl>();
        camTransform.position = pivot.TransformPoint(Vector3.forward * offset);
        defaultFOV = myCamera.fieldOfView;

        transform.parent = null;
    }

    void Update()
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

        transform.position = new Vector3(character.position.x, character.position.y + 1.5f, character.position.z);
    }

    void LateUpdate()
    {
        //Camera Orbits the charcter
        float hor = Input.GetAxis("Mouse X");
        float vert = Input.GetAxis("Mouse Y");

        if (!reverseVertical) vert *= -vertSpeed;
        else vert *= vertSpeed;

        hor *= camRotateSpeed;

        //CLAMP Vertical Axis
        float x = pivot.eulerAngles.x;
        if (vert > 0 && x > 61 && x < 270) vert = 0;
        else if (vert < 0 && x < 300 && x > 180) vert = 0;

        pivot.localEulerAngles += new Vector3(vert, hor, 0);
        
        

        //Central Ray
        float unobstructed = offset;
        Vector3 idealPostion = pivot.TransformPoint(Vector3.forward * offset);

        RaycastHit hit;
        if (Physics.Linecast(pivot.position, idealPostion, out hit, mask.value))
        {
            unobstructed = -hit.distance + .01f;
        }


        //smooth
        Vector3 desiredPos = pivot.TransformPoint(Vector3.forward * unobstructed);
        Vector3 currentPos = camTransform.position;

        Vector3 goToPos = new Vector3(Mathf.Lerp(currentPos.x, desiredPos.x, camFollow), Mathf.Lerp(currentPos.y, desiredPos.y, camFollow), Mathf.Lerp(currentPos.z, desiredPos.z, camFollow));

        camTransform.localPosition = goToPos;
        camTransform.LookAt(pivot.position);



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