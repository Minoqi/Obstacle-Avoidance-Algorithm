using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ObstalceAvoidance : MonoBehaviour
{
    // Variables
    private RaycastHit obstacleHit;
    private bool obstacleHitBool;

    [Header("Player Stats")]
    public float speed;
    public float collisionCounter;
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

        debugLine.enabled = false;

        collisionCounter = 0;
        collisionText.text = "Collisions: " + collisionCounter;
    }

    // Update is called once per frame
    void Update()
    {
        WanderAI();
    }

    public void AvoidObstacle(GameObject obstacle)
    {
        // Variables
        Vector3 origin = transform.position;
        Vector3 dest = obstacle.transform.position;
        Vector3 direction = dest - origin;

        debugLine.enabled = true;

        obstacleHitBool = Physics.Raycast(transform.position, direction, out obstacleHit, fovObject.GetComponent<FOVTool>().distance);

        if (obstacleHitBool)
        {
            Move(obstacleHit);

            Debug.DrawRay(transform.position, direction, Color.red);

            // Line renderer
            debugLine.startColor = colorHit;
            debugLine.endColor = colorHit;
            debugLine.SetPosition(0, origin);
            debugLine.SetPosition(1, dest);
        }
    }

    void WanderAI()
    {
        if (Vector3.Distance(transform.position, new Vector3(targetLoc.position.x, targetLoc.position.y, targetLoc.position.z)) > 0.2f)
        {
            Move();
        }
    }

    // Move (no obstacles detected)
    void Move()
    {
        // Variables
        Vector3 direction = (new Vector3(targetLoc.position.x, targetLoc.position.y, targetLoc.position.z) - transform.position).normalized;

        // Move
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime);
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    // Move (obstalce(s) detected)
    void Move(RaycastHit hit)
    {
        // Variables
        Vector3 direction = (new Vector3(targetLoc.position.x, targetLoc.position.y, targetLoc.position.z) - transform.position).normalized;

        // Repel object
        direction += hit.normal * repelForce; // Repel object

        // Move
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime);
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision other)
    {
        collisionCounter++;
        collisionText.text = "Collisions: " + collisionCounter;
    }
}
