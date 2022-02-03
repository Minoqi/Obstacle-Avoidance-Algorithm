using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ObstalceAvoidance : MonoBehaviour
{
    // Variables
    private RaycastHit obstacleHit;
    public bool obstacleHitBool, alreadyMoving;
    Vector3 origin, dest, direction;

    [Header("Player Stats")]
    public float speed;
    public float rotateSpeed, collisionCounter;
    public float repelForce;

    [Header("Raycast")]
    public LineRenderer debugLine;
    public Color colorHit;

    [Header("Target Location")]
    public Transform targetLoc;

    [Header("FOV")]
    public GameObject fovObject;

    [Header("UI")]
    public TextMeshProUGUI collisionText;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        targetLoc.GetComponent<Renderer>().material.color = GetComponent<Renderer>().material.color;

        obstacleHitBool = false;
        alreadyMoving = false;

        debugLine.enabled = false;

        collisionCounter = 0;
        collisionText.text = "Collisions: " + collisionCounter;

        targetLoc.GetComponent<TargetManager>().GenerateNewLocation();
    }

    // Update is called once per frame
    void Update()
    {
        WanderAI();
    }

    public void AvoidObstacle(GameObject obstacle)
    {
        // Variables
        origin = transform.position;
        dest = obstacle.transform.position;
        direction = (dest - origin).normalized;

        // Repel object
        obstacleHitBool = Physics.Raycast(transform.position, direction, out obstacleHit, fovObject.GetComponent<FOVTool>().distance);
        direction += obstacleHit.normal * repelForce;

        // Debug
        debugLine.enabled = true;
        Debug.DrawRay(transform.position, direction, Color.red);

        // Line renderer
        debugLine.startColor = colorHit;
        debugLine.endColor = colorHit;
        debugLine.SetPosition(0, origin);
        debugLine.SetPosition(1, dest);
    }

    void WanderAI()
    {
        if (Vector3.Distance(transform.position, new Vector3(targetLoc.position.x, targetLoc.position.y, targetLoc.position.z)) > 2f)
        {
            Move();
        }
        else
        {
            targetLoc.GetComponent<TargetManager>().GenerateNewLocation();
        }
    }

    // Move (no obstacles detected)
    void Move()
    {
        // Variables
        if (!obstacleHitBool) {
            origin = transform.position;
            dest = targetLoc.transform.position;
            direction = (dest - origin).normalized;
        }

        // Move
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotateSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        transform.position += transform.forward * speed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    private void OnCollisionEnter(Collision other)
    {
        collisionCounter++;
        collisionText.text = "Collisions: " + collisionCounter;
    }
}
