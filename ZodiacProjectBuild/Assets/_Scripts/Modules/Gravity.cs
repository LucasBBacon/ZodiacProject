using UnityEngine;

public class Gravity : MonoBehaviour
{
    Core core;
    Rigidbody2D Body => core.body;
    // Animator Animator => core.animator;
    // Movement Movement => core.movement;
    EntityData Data => core.data;

    [SerializeField] AirState airState;
    [SerializeField] WallControlState wallControlState;
    [SerializeField] DashState dashState;

    private void Awake()
    {
        core = GetComponent<Core>();
    }

    public void CalculateGravity()
    {
        if(!dashState.IsDashAttacking)
        {
            if (wallControlState.IsWallSliding) Body.gravityScale = 0;

            else if (
                Body.velocity.y < Mathf.Epsilon && 
                UserInput.instance.MoveInput.y < 0
                )
            {
                // higher gravity if holding Down button
                Body.gravityScale = Data.gravityScale * Data.fallGravityMultiplierFast;
            }
            
            else if (
                airState.IsJumpCut
                )
            {
                // higher gravity if jump button is released
                Body.gravityScale = Data.gravityScale * Data.jumpCutGravityMultiplier;
                Body.velocity = new Vector2(Body.velocity.x, Mathf.Max(Body.velocity.y, -Data.fallMaxSpeed));
            }

            else if (
                (airState.IsJumping || wallControlState.IsWallJumping || airState.IsJumpFalling) && 
                Mathf.Abs(Body.velocity.y) < Data.jumpHangTimeThreshold
                )
            {
                Body.gravityScale = Data.gravityScale * Data.jumpHangGravityMultiplier;
            }

            else if(Body.velocity.y < 0)
            {
                // higher gravity if falling
                Body.gravityScale = Data.gravityScale * Data.fallGravityMultiplier;
                // caps maximum fall speed, so when falling over large distances not accelerated to insanely high speeds
                Body.velocity = new Vector2(Body.velocity.x, Mathf.Max(Body.velocity.y, -Data.fallMaxSpeed));
            }

            // Defaut gravity if standing on a platform or moving upwards
            else Body.gravityScale = Data.gravityScale;
        }
        // highger gravity if the jump input is release or is falling
    }
}