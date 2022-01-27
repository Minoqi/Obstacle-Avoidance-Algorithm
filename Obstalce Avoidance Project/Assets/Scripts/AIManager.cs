using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    // Variables
    private Ray2D ray;
    private RaycastHit2D hitLeft, hitRight, hitUp, hitLeftUp, hitRightUp;

    [Header("Player Stats")]
    public float speed;
    public float collisionCounter;

    [Header("Reset Time")]
    public float waitTime;
    public float startTime;

    [Header("Visiblity Lines")]
    public LineRenderer debugLine;
    public Color colorHit, colorNotHit;
    public float visibilityRangeUp, visibilityRangeSides;
    
    [Header("Target Location")]
    public Vector2 targetLoc;
    public bool canGenerateNewLoc;
    public Transform topBound, bottomBound, leftBound, rightBound;

    // Start is called before the first frame update
    void Start()
    {
        collisionCounter = 0;
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

            GenerateNewTargetLoc();
        }
        else 
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.up) *  visibilityRangeUp, Color.green);
            debugLine.startColor = colorNotHit;
            debugLine.endColor = colorNotHit;
        }

        // Raycast hitUp
        if (hitLeft)
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.left) *  visibilityRangeSides, Color.red);

            GenerateNewTargetLoc();
        }
        else 
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.left) *  visibilityRangeSides, Color.green);
        }

        // Raycast hitRight
        if (hitRight)
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.right) *  visibilityRangeSides, Color.red);

            GenerateNewTargetLoc();
        }
        else 
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.right) *  visibilityRangeSides, Color.green);
        }

        // Raycast hitLeftUp
        if (hitLeftUp)
        {
            Debug.DrawRay(transform.position, (transform.TransformDirection(Vector2.left) + transform.TransformDirection(Vector2.up)) *  visibilityRangeSides, Color.red);

            GenerateNewTargetLoc();
        }
        else 
        {
            Debug.DrawRay(transform.position, (transform.TransformDirection(Vector2.left) + transform.TransformDirection(Vector2.up)) *  visibilityRangeSides, Color.green);
        }

        // Raycast hitRightUp
        if (hitRightUp)
        {
            Debug.DrawRay(transform.position, (transform.TransformDirection(Vector2.right) + transform.TransformDirection(Vector2.up)) *  visibilityRangeSides, Color.red);

            GenerateNewTargetLoc();
        }
        else 
        {
            Debug.DrawRay(transform.position, (transform.TransformDirection(Vector2.right) + transform.TransformDirection(Vector2.up)) *  visibilityRangeSides, Color.green);
        }
    }

    void GenerateNewTargetLoc()
    {
        if (canGenerateNewLoc)
        {
            targetLoc = new Vector2(Random.Range(leftBound.position.x, rightBound.position.x), Random.Range(bottomBound.position.y, topBound.position.y));
            waitTime = startTime;
        }
    }

    void WanderAI()
    {
        if (Vector2.Distance(transform.position, targetLoc) > 0.2f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetLoc, speed * Time.deltaTime);
        }
        else
        {
            targetLoc = new Vector2(Random.Range(leftBound.position.x, rightBound.position.x), Random.Range(bottomBound.position.y, topBound.position.y));
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(targetLoc.x, targetLoc.y, 0), .5f);
    }
    private void OnCollisionEnter2D(Collision2D other) {
        collisionCounter++;
    }
}
