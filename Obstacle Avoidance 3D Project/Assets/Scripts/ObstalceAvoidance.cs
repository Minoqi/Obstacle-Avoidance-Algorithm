using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ObstalceAvoidance : MonoBehaviour
{
    // Variables
    private RaycastHit hitLeft, hitRight, hitUp, hitLeftUp, hitRightUp;
    private bool hitLeftBool, hitRightBool, hitUpBool, hitLeftUpBool, hitRightUpBool;

    [Header("Player Stats")]
    public float speed;
    public float collisionCounter;
    public float repelForce;

    [Header("Visiblity Lines")]
    public LineRenderer debugLine;
    public Color colorHit, colorNotHit;
    public float visibilityRangeUp, visibilityRangeSides;

    [Header("Target Location")]
    public Transform targetLoc;
    //public Transform topBound, bottomBound, leftBound, rightBound;
    public List<GameObject> obstacles;

    //[Header("UI")]
    //public TextMeshProUGUI collisionText;

    // Start is called before the first frame update
    void Start()
    {
        collisionCounter = 0;
        //collisionText.text = "Collisions: " + collisionCounter;
        //targetLoc = new Vector2(Random.Range(leftBound.position.x, rightBound.position.x), Random.Range(bottomBound.position.y, topBound.position.y));
        WanderAI();
    }

    // Update is called once per frame
    void Update()
    {
        VisibilityRange();
        WanderAI();
    }

    void VisibilityRange()
    {
        // Raycasts
        hitUpBool = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitUp, visibilityRangeUp);
        hitLeftBool = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hitLeft, visibilityRangeSides);
        hitRightBool = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hitRight, visibilityRangeSides);
        hitLeftUpBool = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left) + transform.TransformDirection(Vector3.forward) / 2, out hitLeftUp, visibilityRangeSides);
        hitRightUpBool = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right) + transform.TransformDirection(Vector3.forward) / 2, out hitRightUp, visibilityRangeSides);

        // Line renderers
        debugLine.SetPosition(0, transform.position);
        debugLine.SetPosition(1, new Vector3(transform.position.x + visibilityRangeUp, transform.position.y, transform.position.z));
        //debugLine.transform.rotation = this.transform.rotation;

        // Check all raycasts
        // Raycast hitUp
        if (hitUpBool)
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * visibilityRangeUp, Color.red);
            debugLine.startColor = colorHit;
            debugLine.endColor = colorHit;

            Move(hitUpBool, hitUp);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * visibilityRangeUp, Color.green);
            debugLine.startColor = colorNotHit;
            debugLine.endColor = colorNotHit;
        }

        // Raycast hitLeft
        if (hitLeftBool)
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left) * visibilityRangeSides, Color.red);

            Move(hitLeftBool, hitLeft);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left) * visibilityRangeSides, Color.green);
        }

        // Raycast hitRight
        if (hitRightBool)
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * visibilityRangeSides, Color.red);

            Move(hitRightBool, hitRight);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * visibilityRangeSides, Color.green);
        }

        // Raycast hitLeftUp
        if (hitLeftUpBool)
        {
            Debug.DrawRay(transform.position, (transform.TransformDirection(Vector3.left) + transform.TransformDirection(Vector3.forward)) * visibilityRangeSides, Color.red);
            Move(hitLeftUpBool, hitLeftUp);
        }
        else
        {
            Debug.DrawRay(transform.position, (transform.TransformDirection(Vector3.left) + transform.TransformDirection(Vector3.forward)) * visibilityRangeSides, Color.green);
        }

        // Raycast hitRightUp
        if (hitRightUpBool)
        {
            Debug.DrawRay(transform.position, (transform.TransformDirection(Vector3.right) + transform.TransformDirection(Vector3.forward)) * visibilityRangeSides, Color.red);

            Move(hitRightUpBool, hitRightUp);
        }
        else
        {
            Debug.DrawRay(transform.position, (transform.TransformDirection(Vector3.right) + transform.TransformDirection(Vector3.forward)) * visibilityRangeSides, Color.green);
        }
    }

    /*void GenerateNewTargetLoc()
    {
        // Variables
        Vector2 newLoc = new Vector2(Random.Range(leftBound.position.x, rightBound.position.x), Random.Range(bottomBound.position.y, topBound.position.y));
        bool safe = false;

        if (obstacles.Count > 0)
        {
            while (!safe)
            {
                for (int i = 0; i < obstacles.Count; i++)
                {
                    if (Vector2.Distance(obstacles[i].transform.position, newLoc) < 1f)
                    {
                        newLoc = new Vector2(Random.Range(leftBound.position.x, rightBound.position.x), Random.Range(bottomBound.position.y, topBound.position.y));
                        safe = false;
                        break;
                    }
                    else
                    {
                        safe = true;
                    }
                }
            }
        }
    }*/

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
    void Move(bool didHit, RaycastHit hit)
    {
        // Variables
        Vector3 direction = (new Vector3(targetLoc.position.x, targetLoc.position.y, targetLoc.position.z) - transform.position).normalized;

        // Check for collision
        if (didHit)
        {
            direction += hit.normal * repelForce;
        }

        // Move
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime);
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        collisionCounter++;
        //collisionText.text = "Collisions: " + collisionCounter;
    }
}
