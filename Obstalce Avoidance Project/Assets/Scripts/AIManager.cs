using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AIManager : MonoBehaviour
{
    // Variables
    private RaycastHit2D hitLeft, hitRight, hitUp, hitLeftUp, hitRightUp;

    [Header("Player Stats")]
    public float speed;
    public float collisionCounter;
    public float repelForce;

    [Header("Reset Time")]
    public float waitTime;
    public float startTime;

    [Header("Visiblity Lines")]
    public LineRenderer debugLine;
    public Color colorHit, colorNotHit;
    public float visibilityRangeUp, visibilityRangeSides;
    
    [Header("Target Location")]
    public Vector3 targetLoc;
    public bool canGenerateNewLoc;
    public Transform topBound, bottomBound, leftBound, rightBound;
    public List<GameObject> obstacles;

    //[Header("UI")]
    //public TextMeshProUGUI collisionText;

    // Start is called before the first frame update
    void Start()
    {
        collisionCounter = 0;
        //collisionText.text = "Collisions: " + collisionCounter;
        canGenerateNewLoc = true;
        targetLoc = new Vector2(Random.Range(leftBound.position.x, rightBound.position.x), Random.Range(bottomBound.position.y, topBound.position.y));
        WanderAI();
    }

    // Update is called once per frame
    void Update()
    {
        if(waitTime > 0)
        {
            canGenerateNewLoc = false;
            waitTime -= Time.deltaTime;
        }
        else
        {
            canGenerateNewLoc = true;
        }

        VisibilityRange();
        WanderAI();
    }

    void VisibilityRange()
    {
        // Raycasts
        hitUp = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.up), visibilityRangeUp);
        hitLeft = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.left), visibilityRangeSides);
        hitRight = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.right), visibilityRangeSides);
        hitLeftUp = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.left) + transform.TransformDirection(Vector2.up) / 2, visibilityRangeSides);
        hitRightUp = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.right) + transform.TransformDirection(Vector2.up) / 2, visibilityRangeSides);

        // Line Renderers
        debugLine.SetPosition(0, transform.position);
        debugLine.SetPosition(1, new Vector2(transform.position.x, transform.position.y + visibilityRangeUp));
        //debugLine.transform.rotation = this.transform.rotation;

        // Raycast hitUp
        if (hitUp)
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.up) *  visibilityRangeUp, Color.red);
            debugLine.startColor = colorHit;
            debugLine.endColor = colorHit;

            Move(hitUp);
        }
        else 
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.up) *  visibilityRangeUp, Color.green);
            debugLine.startColor = colorNotHit;
            debugLine.endColor = colorNotHit;
        }

        // Raycast hitLeft
        if (hitLeft)
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.left) *  visibilityRangeSides, Color.red);

            Move(hitLeft);
        }
        else 
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.left) *  visibilityRangeSides, Color.green);
        }

        // Raycast hitRight
        if (hitRight)
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.right) *  visibilityRangeSides, Color.red);

            Move(hitRight);
        }
        else 
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.right) *  visibilityRangeSides, Color.green);
        }

        // Raycast hitLeftUp
        if (hitLeftUp)
        {
            Debug.DrawRay(transform.position, (transform.TransformDirection(Vector2.left) + transform.TransformDirection(Vector2.up)) *  visibilityRangeSides, Color.red);
            Move(hitLeftUp);
        }
        else 
        {
            Debug.DrawRay(transform.position, (transform.TransformDirection(Vector2.left) + transform.TransformDirection(Vector2.up)) *  visibilityRangeSides, Color.green);
        }

        // Raycast hitRightUp
        if (hitRightUp)
        {
            Debug.DrawRay(transform.position, (transform.TransformDirection(Vector2.right) + transform.TransformDirection(Vector2.up)) *  visibilityRangeSides, Color.red);

            Move(hitRightUp);
        }
        else 
        {
            Debug.DrawRay(transform.position, (transform.TransformDirection(Vector2.right) + transform.TransformDirection(Vector2.up)) *  visibilityRangeSides, Color.green);
        }
    }

    void GenerateNewTargetLoc()
    {
        // Variables
        Vector2 newLoc = new Vector2(Random.Range(leftBound.position.x, rightBound.position.x), Random.Range(bottomBound.position.y, topBound.position.y));
        bool safe = false;

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

    void WanderAI()
    {
        if (Vector2.Distance(transform.position, targetLoc) > 0.2f)
        {
            Move();
        }
        else
        {
            GenerateNewTargetLoc();
        }
    }

    // Move (no obstacles detected)
    void Move()
    {
        // Variables
        Vector2 direction = (targetLoc - transform.position).normalized;

        // Move
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime);
        Quaternion q = transform.rotation;
        q.eulerAngles = new Vector3(q.eulerAngles.x, 0, q.eulerAngles.z);
        transform.rotation = q;
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    // Move (obstalce(s) detected)
    void Move(RaycastHit2D hit)
    {
        // Variables
        Vector2 direction = (targetLoc - transform.position).normalized;

        // Check for collision
        if (hit)
        {
            direction += hit.normal * repelForce;
        }

        // Move
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime);
        Quaternion q = transform.rotation;
        q.eulerAngles = new Vector3(q.eulerAngles.x, 0, q.eulerAngles.z);
        transform.rotation = q;
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(targetLoc.x, targetLoc.y, 0), .5f);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        collisionCounter++;
        //collisionText.text = "Collisions: " + collisionCounter;
    }
}
