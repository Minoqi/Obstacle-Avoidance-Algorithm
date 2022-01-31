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
            waitTime -= Time.deltaTime;


            if (!changeDirection) // move left
            {
                transform.position = new Vector3(transform.position.x * -moveSpeed * Time.deltaTime, transform.position.y, transform.position.z);
            }
            else // move right
            {
                transform.position = new Vector3(transform.position.x * moveSpeed * Time.deltaTime, transform.position.y, transform.position.z);
            }
        }
        else
        {
            changeDirection = true;
            waitTime = moveTime;
        }
    }
}
