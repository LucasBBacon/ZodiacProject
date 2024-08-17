using System;
using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] GameObject wayPointsGO;
    [SerializeField] Transform[] wayPoints;
    [SerializeField] float speed = 1.5f;
    [SerializeField] float waitDuration = 0.5f;

    Movement movement;
    Rigidbody2D playerBody;
    Rigidbody2D body;

    Vector3 targetPos;
    Vector3 moveDirection;

    
    int pointIndex;
    int pointCount;
    int direction = 1;

    private void Awake()
    {
        movement = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Movement>();
        
        body = GetComponent<Rigidbody2D>();

        wayPoints = new Transform[wayPointsGO.transform.childCount];
        for (int i = 0; i < wayPointsGO.transform.childCount; i++)
        {
            wayPoints[i] = wayPointsGO.transform.GetChild(i).gameObject.transform;
        }
    }

    private void Start()
    {
        playerBody = movement.Body;
        pointIndex = 1;
        pointCount = wayPoints.Length;
        targetPos = wayPoints[1].transform.position;
        
        DirectionToCalculate();
    }

    private void Update()
    {
        if (Vector2.Distance(transform.position, targetPos) < 0.05f)
        {
            NextPoint();
        }
    }

    private void FixedUpdate()
    {
        body.velocity = moveDirection * speed;
    } 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("On Platform");
            movement.IsOnPlatform = true;
            movement.PlatformBody = body;
            playerBody.gravityScale *= 10;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            movement.IsOnPlatform = false;
            playerBody.gravityScale /= 10;
        }
    }

    private void DirectionToCalculate()
    {
        moveDirection = (targetPos - transform.position).normalized;
    }

    private void NextPoint()
    {
        transform.position = targetPos;
        moveDirection = Vector3.zero;

        if (pointIndex == pointCount - 1)
        {
            direction = -1;
        }

        if (pointIndex == 0)
        {
            direction = 1;
            
        }

        pointIndex += direction;
        targetPos = wayPoints[pointIndex].transform.position;
        
        StartCoroutine(WaitNextPoint());
    }

    IEnumerator WaitNextPoint()
    {
        yield return new WaitForSeconds(waitDuration);
        DirectionToCalculate();
    }
}
