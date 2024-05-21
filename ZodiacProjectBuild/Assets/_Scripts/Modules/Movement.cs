using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Core core;
    private EntityData Data => core.data;
    private Rigidbody2D Body => core.body;
    private CollisionSensors Sensors => core.collisionSensors;

    private void Awake()
    {
        core = GetComponent<Core>();
    }

    public void Run(float lerpAmount)
    {
        float _accelRate;

        float _targetSpeed = UserInput.instance.MoveInput.x * Data.runMaxSpeed;
        _targetSpeed = Mathf.Lerp(core.body.velocity.x, _targetSpeed, lerpAmount);

        if (Sensors.IsGrounded)
            _accelRate = (Mathf.Abs(_targetSpeed) > Mathf.Epsilon) ?
            Data.runAccelAmount : Data.runDeccelAmount;

        else
            _accelRate = (Mathf.Abs(_targetSpeed) > Mathf.Epsilon) ?
            Data.runAccelAmount * Data.airAcceleration : Data.runDeccelAmount * Data.airDecceleration; 
    }
}