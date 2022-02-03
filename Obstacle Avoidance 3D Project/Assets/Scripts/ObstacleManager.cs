using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    // Variables
    public bool move, changeDirection;
    public float moveSpeed, moveTime, waitTime;

    // Start is called before the first frame update
    void Start()
    {
        waitTime = moveTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (move)
        {
            Move();
        }
    }
    void Move()
    {
        if (waitTime > 0)
        {
            if (changeDirection)
            {
                transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            }

            waitTime -= Time.deltaTime;
        }
        else
        {
            if (changeDirection)
            {
                changeDirection = false;
            }
            else
            {
                changeDirection = true;
            }

            waitTime = moveTime;
        }
    }
}
