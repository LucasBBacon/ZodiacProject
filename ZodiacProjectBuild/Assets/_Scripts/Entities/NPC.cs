using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] LayerMask groundMask;
    public Vector2 cornerLocation;

    public int FacingDirection = 1;

    private void Start() {
        
    }

    private void Update()
    {
        cornerLocation = CornerPosition();

        if (Input.GetMouseButton(2))
        {
            FacingDirection *= -1;
        }

        
    }

    private void FixedUpdate() 
    {
        
    }

    public Vector2 CornerPosition()
    {
        RaycastHit2D xHit = Physics2D.Raycast
            (
                new Vector2
                    (
                        transform.position.x,
                        transform.position.y
                    ),
                Vector2.right * FacingDirection,
                10f,
                groundMask
            );

        Vector2 workspace = Vector2.zero;

        if (xHit.collider != null)
        {
            Debug.Log(xHit.collider.name);
            float xDist = xHit.point.x;
            Debug.Log("x:" + xDist);

            RaycastHit2D yHit = Physics2D.Raycast
                (
                    new Vector2
                        (
                            xDist + (0.1f * FacingDirection),
                            transform.position.y + 2f
                        ),
                    Vector2.down,
                    3f,
                    groundMask
                );

            float yDist = yHit.point.y;

            Debug.Log(yDist);

            workspace.Set
                (
                    xDist,
                    yDist
                );
        }

        return workspace;
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine
            (
                transform.position,
                (Vector2)transform.position + (Vector2.right * FacingDirection * 10f)
            );
    }
}
